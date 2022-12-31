using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Xml.Linq;
//RootThumbに複数選択Thumbを保持するSelectedThumbsを追加
//Ctrl+クリックしたときにこれに追加していく、同じThumbがあった場合は削除のトグル式
//追加できるのはActiveThumbのChildren要素だけ

//マウスダウンで追加、マウスアップで削除、ただし削除はクリック前が選択状態だったものだけにする
//これの判定用にBool型プロパティIsSelectedWhenPreviewDownを追加

//削除やドラッグ移動は選択状態のThumb全てに対して実行するようにした

//ドラッグ移動系のイベントはTThumbからRootThumbに移動

namespace _20221231_SelectedThumbs
{
    public class TThumb : Thumb, INotifyPropertyChanged
    {
        #region 依存プロパティ、通知プロパティ

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private double _myLeft;
        public double MyLeft { get => _myLeft; set => SetProperty(ref _myLeft, value); }


        //依存プロパティじゃなくても、通知プロパティでおｋ
        //public double MyLeft
        //{
        //    get { return (double)GetValue(MyLeftProperty); }
        //    set
        //    {
        //        SetValue(MyLeftProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty MyLeftProperty =
        //    DependencyProperty.Register(nameof(MyLeft), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));

        public double MyTop
        {
            get { return (double)GetValue(MyTopProperty); }
            set { SetValue(MyTopProperty, value); }
        }
        public static readonly DependencyProperty MyTopProperty =
            DependencyProperty.Register(nameof(MyTop), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));


        #endregion 依存プロパティ、通知プロパティ

        public TTGroup? TTParent { get; set; }
        public TThumb()
        {
            SetBinding(Canvas.LeftProperty, nameof(MyLeft));
            SetBinding(Canvas.TopProperty, nameof(MyTop));
        }

        public override string ToString()
        {
            //return base.ToString();
            return Name;
        }

    }

    /// <summary>
    /// Item系Thumbのベース、表示する要素はこれを継承して作成する
    /// </summary>
    public abstract class TTItemThumb : TThumb
    {
        public TTItemThumb()
        {
            SizeChanged += TThumb_SizeChanged;
        }
        private void TThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TTParent?.TTGroupUpdateLayout();
        }
    }

    /// <summary>
    /// TextBlockのThumb
    /// </summary>
    public class TTTextBlock : TTItemThumb
    {
        private string? _myText;
        public string? MyText { get => _myText; set => SetProperty(ref _myText, value); }

        public TTTextBlock()
        {
            DataContext = this;
            //Template構築、適用
            FrameworkElementFactory text = new(typeof(TextBlock));
            FrameworkElementFactory waku = new(typeof(Rectangle));
            FrameworkElementFactory panel = new(typeof(Grid));
            waku.SetValue(Shape.StrokeProperty, Brushes.Red);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            text.SetValue(TextBlock.TextProperty, new Binding(nameof(MyText)));
            panel.AppendChild(text);
            panel.AppendChild(waku);
            this.Template = new() { VisualTree = panel };
            this.ApplyTemplate();

        }


    }


    /// <summary>
    /// グループ用Thumb、TemplateにItemsControlを使っている
    /// </summary>
    [ContentProperty(nameof(InternalChildren))]
    public class TTGroup : TThumb
    {
        //本当はprivateかprotectedにしたい
        internal ObservableCollection<TThumb> InternalChildren { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb> Children { get; }


        public TTGroup()
        {
            DataContext = this;
            InternalChildren.CollectionChanged += Children_CollectionChanged;
            Children = new(InternalChildren);

            //Template構造
            //Thumb
            //┗ Template
            //   ┗ Grid
            //      ┣ ItemsControl   コレクションをBinding
            //      ┃  ┗ Canvas      PanelTemplate
            //      ┗ Rectangle      枠
            FrameworkElementFactory waku = new(typeof(Rectangle));//サイズ確認用枠
            waku.SetValue(Rectangle.StrokeProperty, Brushes.Blue);
            waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);
            FrameworkElementFactory grid = new(typeof(Grid));
            FrameworkElementFactory ic = new(typeof(ItemsControl));
            //ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            FrameworkElementFactory icPanel = new(typeof(Canvas));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(icPanel));
            ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            grid.AppendChild(ic);
            grid.AppendChild(waku);
            this.Template = new() { VisualTree = grid };

        }

        //TTGroupのRect取得
        public static (double x, double y, double w, double h) GetRect(TTGroup? group)
        {
            if (group == null) { return (0, 0, 0, 0); }
            return GetRect(group.Children);
        }
        public static (double x, double y, double w, double h) GetRect(IEnumerable<TThumb> thumbs)
        {
            double x = double.MaxValue, y = double.MaxValue;
            double w = 0, h = 0;
            if (thumbs != null)
            {
                foreach (var item in thumbs)
                {
                    var left = item.MyLeft; if (x > left) x = left;
                    var top = item.MyTop; if (y > top) y = top;
                    var width = left + item.ActualWidth;
                    var height = top + item.ActualHeight;
                    if (w < width) w = width;
                    if (h < height) h = height;
                }
            }
            return (x, y, w, h);
        }

        //サイズと位置の更新
        public void TTGroupUpdateLayout()
        {
            //Rect取得
            (double x, double y, double w, double h) = GetRect(this);

            //子要素位置修正
            foreach (var item in Children)
            {
                item.MyLeft -= x;
                item.MyTop -= y;
            }
            //自身がRoot以外なら自身の位置を更新
            if (this.GetType() != typeof(TTRoot))
            {
                MyLeft += x;
                MyTop += y;
            }

            //自身のサイズ更新
            w -= x; h -= y;
            if (w < 0) w = 0;
            if (h < 0) h = 0;
            if (w >= 0) Width = w;
            if (h >= 0) Height = h;

            //必要、これがないと見た目が変化しない、実行直後にSizeChangedが発生
            UpdateLayout();

            //親要素Groupがあれば遡って更新
            if (TTParent is TTGroup parent)
            {
                parent.TTGroupUpdateLayout();
            }
        }



        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                //子要素追加時、
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is TThumb thumb)
                    {
                        //子要素のTTParentプロパティに自身を登録
                        thumb.TTParent = this;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    //削除時はサイズと位置の更新
                    //→ここではしない方がいい、グループ化の削除時に面倒なことになる
                    //TTGroupUpdateLayout();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// すべてのThumbを管理する、追加や削除もここで行う
    /// </summary>
    public class TTRoot : TTGroup
    {

        //クリックされたThumb
        private TThumb? _clickedThumb;
        public TThumb? ClickedThumb { get => _clickedThumb; set => SetProperty(ref _clickedThumb, value); }

        //移動可能なThumb、フォーカスされているThumb、カーソルキーで移動させるThumb
        private TThumb? _activeThumb;
        public TThumb? ActiveThumb { get => _activeThumb; set => SetProperty(ref _activeThumb, value); }

        //子要素が移動対象になっているグループThumb
        //子要素の追加対象
        private TTGroup _activeGroup;
        public TTGroup ActiveGroup
        {
            get => _activeGroup;
            set
            {
                ChildrenDragEventDesoption(_activeGroup, value);
                SetProperty(ref _activeGroup, value);
            }
        }
        //ActiveGroup用、ドラッグ移動イベント脱着
        private void ChildrenDragEventDesoption(TTGroup removeTarget, TTGroup addTarget)
        {
            foreach (var item in removeTarget.Children)
            {
                item.DragDelta -= Thumb_DragDelta2;
                item.DragCompleted -= Thumb_DragCompleted2;
            }
            foreach (var item in addTarget.Children)
            {
                item.DragDelta += Thumb_DragDelta2;
                item.DragCompleted += Thumb_DragCompleted2;
            }
        }



        //public ReadOnlyObservableCollection<TThumb> InternalSelectedThumbs { get; set; }
        public ObservableCollection<TThumb> SelectedThumbs { get; set; } = new();
        private bool IsSelectedWhenPreviewDown { get; set; }//クリック前の選択状態、クリックUp時の削除に使う


        #region コンストラクタ
        public TTRoot()
        {
            _activeGroup ??= this;
            //PreviewMouseLeftButtonDown += TTRoot_PreviewMouseLeftButtonDown;
        }
        #endregion コンストラクタ

        //起動直後、自身がActiveGroupならChildrenにドラッグ移動登録
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (ActiveGroup == this)
            {
                foreach (var item in Children)
                {
                    item.DragDelta += Thumb_DragDelta2;
                    item.DragCompleted += Thumb_DragCompleted2;
                }
            }
        }

        //クリックしたとき、ClickedThumbの更新とActiveThumbの更新、SelectedThumbsの更新
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);//要る？

            //OriginalSourceにテンプレートに使っている要素が入っているので、
            //そのTemplateParentプロパティから目的のThumbが取得できる
            if (e.OriginalSource is FrameworkElement el && el.TemplatedParent is TThumb clicked)
            {
                ClickedThumb = clicked;
                TThumb? active = GetActiveThumb(clicked);
                if (active != ActiveThumb)
                {
                    ActiveThumb = active;
                }
                //SelectedThumbsの更新
                if (active != null)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (SelectedThumbs.Contains(active) == false)
                        {
                            SelectedThumbs.Add(active);
                            IsSelectedWhenPreviewDown = false;
                        }
                        else
                        {
                            //フラグ
                            //ctrl+クリックされたものがもともと選択状態だったら
                            //マウスアップ時に削除するためのフラグ
                            IsSelectedWhenPreviewDown = true;
                        }
                    }
                    else
                    {
                        if (SelectedThumbs.Contains(active) == false)
                        {
                            SelectedThumbs.Clear();
                            SelectedThumbs.Add(active);
                            IsSelectedWhenPreviewDown = false;
                        }
                    }
                }
                else { IsSelectedWhenPreviewDown = false; }
            }
        }

        //マウスレフトアップ、フラグがあればSelectedThumbsから削除する
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //
            if (SelectedThumbs.Count > 1 && IsSelectedWhenPreviewDown && ActiveThumb != null)
            {
                SelectedThumbs.Remove(ActiveThumb);//削除
                IsSelectedWhenPreviewDown = false;//フラグ切り替え
                ActiveThumb = null;
                ClickedThumb = null;
            }

        }

        private bool CheckIsActive(TThumb thumb)
        {
            if (thumb.TTParent is TTGroup ttg && ttg == ActiveGroup)
            {
                return true;
            }
            return false;

        }
        //起点からActiveThumbをサーチ
        //ActiveはActiveThumbのChildrenの中で起点に連なるもの
        private TThumb? GetActiveThumb(TThumb? start)
        {
            if (start == null) { return null; }
            if (CheckIsActive(start))
            {
                return start;
            }
            else if (start.TTParent is TTGroup ttg)
            {
                return GetActiveThumb(ttg);
            }
            return null;
        }
        #region ドラッグ移動
        private void Thumb_DragCompleted2(object sender, DragCompletedEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.TTParent?.TTGroupUpdateLayout();
            }
        }

        private void Thumb_DragDelta2(object sender, DragDeltaEventArgs e)
        {
            //複数選択時は全てを移動
            foreach (var item in SelectedThumbs)
            {
                item.MyLeft += e.HorizontalChange;
                item.MyTop += e.VerticalChange;
            }
        }

        #endregion ドラッグ移動

        #region 追加と削除
        //基本的にActiveThumbのChildrenに対して行う
        //削除対象はActiveThumbになる
        //ドラッグ移動イベントの着脱も行う
        public void AddThumb(TThumb thumb)
        {
            AddThumb(thumb, ActiveGroup);
        }
        /// <summary>
        /// 追加先Groupを指定して追加
        /// </summary>
        /// <param name="thumb">追加する子要素</param>
        /// <param name="destGroup">追加先Group</param>
        public void AddThumb(TThumb thumb, TTGroup destGroup)
        {
            destGroup.InternalChildren.Add(thumb);
            //ドラッグ移動イベント付加
            thumb.DragDelta += Thumb_DragDelta2;
            thumb.DragCompleted += Thumb_DragCompleted2;
        }


        /// <summary>
        /// 選択Thumbすべてを削除
        /// </summary>
        /// <returns></returns>
        public bool RemoveThumb()
        {
            if (SelectedThumbs == null) return false;
            bool flag = true;
            foreach (var item in SelectedThumbs.ToArray())
            {
                if (RemoveThumb(item, ActiveGroup) == false)
                {
                    flag = false;
                }
                else
                {
                    SelectedThumbs.Remove(item);
                }
            }
            ClickedThumb = null;
            ActiveThumb = null;
            return flag;
        }
        public bool RemoveThumb(TThumb thumb, TTGroup group)
        {
            if (group.InternalChildren.Remove(thumb))
            {
                thumb.DragCompleted -= Thumb_DragCompleted2;
                thumb.DragDelta -= Thumb_DragDelta2;
                group.TTGroupUpdateLayout();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion 追加と削除

        #region グループ化

        //基本的にSelectedThumbsをグループ化して、それをActiveGroupに追加する
        public void AddGroup()
        {
            TTGroup? group = MakeAndAddGroup(SelectedThumbs, ActiveGroup);
            if (group != null)
            {
                SelectedThumbs.Clear();
                SelectedThumbs.Add(group);
                ActiveThumb = group;
            }
        }
        /// <summary>
        /// グループ化
        /// </summary>
        /// <param name="thumbs">グループ化する要素群</param>
        /// <param name="destGroup">新グループの追加先</param>
        private TTGroup? MakeAndAddGroup(IEnumerable<TThumb> thumbs, TTGroup destGroup)
        {
            if (CheckAddGroup(thumbs, destGroup) == false) { return null; }
            var (x, y, w, h) = GetRect(thumbs);
            TTGroup group = new() { Name = "new_group", MyLeft = x, MyTop = y };

            foreach (var item in thumbs)
            {

                destGroup.InternalChildren.Remove(item);
                item.DragDelta -= Thumb_DragDelta2;
                item.DragCompleted -= Thumb_DragCompleted2;

                //AddThumb(item, group);
                group.InternalChildren.Add(item);
                item.MyLeft -= x;
                item.MyTop -= y;
            }
            AddThumb(group, destGroup);

            group.Arrange(new(0, 0, w, h));//このタイミングで必須、Actualサイズに値が入る
            group.TTGroupUpdateLayout();//必須

            return group;
        }
        private bool CheckAddGroup(IEnumerable<TThumb> thumbs, TTGroup destGroup)
        {
            if (thumbs.Count() < 2) { return false; }
            if (thumbs.Count() == destGroup.InternalChildren.Count) { return false; }
            foreach (TThumb thumb in thumbs)
            {
                if (destGroup.InternalChildren.Contains(thumb) == false) { return false; }
            }
            return true;
        }
        #endregion グループ化

        #region グループ解除

        public void UnGroup()
        {
            if (ActiveThumb is TTGroup group)
            {
                UnGroup(group, ActiveGroup);
                SelectedThumbs.Clear();
                ActiveThumb = GetActiveThumb(ClickedThumb);
            }
        }
        public void UnGroup(TTGroup group, TTGroup destGroup)
        {
            foreach (var item in group.InternalChildren.ToArray())
            {
                group.InternalChildren.Remove(item);
                item.DragDelta -= Thumb_DragDelta2;
                item.DragCompleted -= Thumb_DragCompleted2;
                destGroup.InternalChildren.Add(item);
                item.DragDelta += Thumb_DragDelta2;
                item.DragCompleted += Thumb_DragCompleted2;
                item.MyLeft += group.MyLeft;
                item.MyTop += group.MyTop;
            }
            //元のグループ要素削除
            destGroup.InternalChildren.Remove(group);
            group.DragCompleted -= Thumb_DragCompleted2;//いる？
            group.DragDelta -= Thumb_DragDelta2;
            //destGroup.TTGroupUpdateLayout();
        }
        #endregion グループ解除

        //ActiveThumbを内側(ActiveThumbの親)へ切り替える
        public void ActiveGroupInside()
        {
            if (ActiveThumb is TTGroup group)
            {
                ActiveGroup = group;
                ActiveThumb = GetActiveThumb(ClickedThumb);
            }
        }

        //ActiveThumbを外側(親)へ切り替える
        public void ActiveGroupOutside()
        {
            if (ActiveGroup.TTParent is TTGroup parent)
            {
                ActiveGroup = parent;
                ActiveThumb = GetActiveThumb(ClickedThumb);
            }
        }

    }
}
