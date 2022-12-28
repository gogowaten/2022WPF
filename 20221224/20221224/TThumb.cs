using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using System;
using System.ComponentModel.Design;


//RootThumbに選択Thumbを保持するSelectedThumbsを追加
//Ctrl+クリックしたときにこれに追加していく、同じThumbがあった場合は削除のトグル式
//追加できるのはEnableThumbのChildren要素だけ

//グループ化とグループ解除できた
//グループ化はSelectedThumbsからTTGroup作成して、それをEnableThumbに追加
//解除はMovableThumbがTTGroupならこれを削除して、中の子要素をEnableThumbに追加
namespace _20221224
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

        #region ドラッグ移動系イベント

        //ドラッグ移動終了時に親要素のサイズと位置の更新
        private void TT_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.TTParent?.TTGroupUpdateLayout();
            }
        }
        //マウスドラッグ移動
        private void TT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;
        }
        //public void AddDragEvent()
        //{
        //    DragDelta += TT_DragDelta;
        //    DragCompleted += TT_DragCompleted;
        //}
        public void RemoveDragEvent()
        {
            DragDelta -= TT_DragDelta;
            DragCompleted -= TT_DragCompleted;
        }
        #endregion ドラッグ移動系イベント

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
        private TThumb? _movableThumb;
        public TThumb? MovableThumb { get => _movableThumb; set => SetProperty(ref _movableThumb, value); }

        //子要素が移動対象になっているグループThumb
        //子要素の追加対象
        private TTGroup _enableGroup;
        public TTGroup EnableGroup
        {
            get => _enableGroup;
            set
            {
                ChildrenDragEventDesoption(_enableGroup, value);
                SetProperty(ref _enableGroup, value);
            }
        }
        //EnableGroup用、ドラッグ移動イベント脱着
        private void ChildrenDragEventDesoption(TTGroup removeTarget, TTGroup addTarget)
        {
            foreach (var item in removeTarget.Children)
            {
                item.RemoveDragEvent();
            }
            foreach (var item in addTarget.Children)
            {
                //item.AddDragEvent();
                //AddDragEvent2();
                item.DragDelta += Thumb_DragDelta2;
            }
        }
        //public ReadOnlyObservableCollection<TThumb> InternalSelectedThumbs { get; set; }
        public ObservableCollection<TThumb> SelectedThumbs { get; set; } = new();
        private TThumb? LastAddSelectedThumb { get; set; }

        #region コンストラクタ
        public TTRoot()
        {
            _enableGroup ??= this;
            PreviewMouseLeftButtonDown += TTRoot_PreviewMouseLeftButtonDown;
        }
        #endregion コンストラクタ

        //起動直後、自身がEnableGroupならChildrenにドラッグ移動登録
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (EnableGroup == this)
            {
                foreach (var item in Children)
                {
                    //item.AddDragEvent();
                    //AddDragEvent2();
                    item.DragDelta += Thumb_DragDelta2;
                    item.DragCompleted += Thumb_DragCompleted2;
                }
            }
        }

        //クリックしたとき、ClickedThumbの更新とMovableThumbの更新、SelectedThumbsの更新
        private void TTRoot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            //OriginalSourceにテンプレートに使っている要素が入っているので、
            //そのTemplateParentプロパティから目的のThumbが取得できる
            if (e.OriginalSource is FrameworkElement el)
            {
                if (el.TemplatedParent is TThumb clicked)
                {
                    ClickedThumb = clicked;
                    TThumb? movable = GetMovableThumb(clicked);
                    MovableThumb = movable;
                    //選択Thumb群の更新
                    if (movable != null) { ClickedChanged(movable); }
                }
            }

            //選択Thumb群の更新
            //追加できるのはEnableThumbのChildren要素だけ、つまりMovableThumb
            //クリックしたThumbからMovableThumbを取得、これを判定していく
            //Ctrlキーが押されている    MovableがSelectedにあればSelectedから削除、なければ追加
            //Ctrlキーが押されていない    Selectedをクリアして、追加
            void ClickedChanged(TThumb movable)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (SelectedThumbs.Contains(movable))
                    {
                        //SelectedThumbs.Remove(movable);
                    }
                    else
                    {
                        SelectedThumbs.Add(movable);
                        LastAddSelectedThumb = movable;
                    }
                }
                else
                {
                    SelectedThumbs.Clear();
                    SelectedThumbs.Add(movable);
                    LastAddSelectedThumb = movable;
                }
            }
        }

        //protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    //base.OnPreviewMouseLeftButtonUp(e);
        //    if (e.OriginalSource is FrameworkElement el)
        //    {
        //        if (el is TThumb clicked)
        //        {
        //            if (clicked == ClickedThumb)
        //            {

        //            }
        //            else
        //            {

        //            }
        //            if (Keyboard.Modifiers == ModifierKeys.Control)
        //            {
        //                if (SelectedThumbs.Contains(clicked))
        //                {
        //                    var movable = GetMovableThumb(clicked);
        //                    if (movable != null)
        //                    {
        //                        SelectedThumbs.Remove(movable);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        private bool IsMovable(TThumb thumb)
        {
            if (thumb.TTParent is TTGroup ttg && ttg == EnableGroup)
            {
                return true;
            }
            return false;

        }
        //起点からMovableThumbをサーチ
        //MovableはEnableThumbのChildrenの中で起点に連なるもの
        private TThumb? GetMovableThumb(TThumb? start)
        {
            if (start == null) { return null; }
            if (IsMovable(start))
            {
                return start;
            }
            else if (start.TTParent is TTGroup ttg)
            {
                return GetMovableThumb(ttg);
            }
            return null;
        }

        #region 追加と削除
        //基本的にEnableThumbのChildrenに対して行う
        //削除対象はMovableThumbになる
        //ドラッグ移動イベントの着脱も行う
        public void AddThumb(TThumb thumb)
        {
            //EnableGroup.InternalChildren.Add(thumb);
            //if (thumb is TTItemThumb item) { item.AddDragEvent(); }
            AddThumb(thumb, EnableGroup);
        }
        /// <summary>
        /// 追加先Groupを指定して追加
        /// </summary>
        /// <param name="thumb">追加する子要素</param>
        /// <param name="destGroup">追加先Group</param>
        public void AddThumb(TThumb thumb, TTGroup destGroup)
        {
            destGroup.InternalChildren.Add(thumb);
            //Item系Thumbだけにドラッグ移動イベント付加
            //if (thumb is TTItemThumb item) { item.AddDragEvent(); }

            //こっちのほうが正しい？
            //thumb.AddDragEvent();
            //AddDragEvent2();
            thumb.DragDelta += Thumb_DragDelta2;
            thumb.DragCompleted += Thumb_DragCompleted2;
        }

        //public void AddDragEvent2()
        //{
        //    DragDelta += Thumb_DragDelta2;
        //    DragCompleted += Thumb_DragCompleted2;
        //}
        private void Thumb_DragCompleted2(object sender, DragCompletedEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.TTParent?.TTGroupUpdateLayout();
                if (e.HorizontalChange == 0 && e.VerticalChange == 0)
                {
                    if (GetMovableThumb(tt) is TThumb move)
                    {
                        if (move != LastAddSelectedThumb)
                        {
                            SelectedThumbs.Remove(move);
                            LastAddSelectedThumb = null;
                        }
                    }
                }
            }
        }

        private void Thumb_DragDelta2(object sender, DragDeltaEventArgs e)
        {

            foreach (var item in SelectedThumbs)
            {
                item.MyLeft += e.HorizontalChange;
                item.MyTop += e.VerticalChange;
            }
        }

        public void RemoveThumb(TThumb thumb)
        {
            RemoveThumb(thumb, EnableGroup);
        }
        public void RemoveThumb()
        {
            if (MovableThumb != null) { RemoveThumb(MovableThumb); }
        }
        public void RemoveThumb(TThumb thumb, TTGroup group)
        {
            if (group.InternalChildren.Remove(thumb))
            {
                thumb.RemoveDragEvent();
                group.TTGroupUpdateLayout();
                //SelectedThumbs.Remove(thumb);
                //MovableThumb = null;
            }
        }

        #endregion 追加と削除

        #region グループ化

        //基本的にSelectedThumbsをグループ化して、それをEnableGroupに追加する
        public void AddGroup()
        {
            TTGroup? group = MakeAndAddGroup(SelectedThumbs, EnableGroup);
            if (group != null)
            {
                SelectedThumbs.Clear();
                SelectedThumbs.Add(group);
                MovableThumb = group;
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
                item.RemoveDragEvent();

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
            if (MovableThumb is TTGroup group)
            {
                UnGroup(group, EnableGroup);
                SelectedThumbs.Clear();
                MovableThumb = GetMovableThumb(ClickedThumb);
            }
        }
        public void UnGroup(TTGroup group, TTGroup destGroup)
        {
            foreach (var item in group.InternalChildren.ToArray())
            {
                group.InternalChildren.Remove(item);
                item.RemoveDragEvent();
                destGroup.InternalChildren.Add(item);
                //item.AddDragEvent();
                //AddDragEvent2();
                item.DragDelta += Thumb_DragDelta2;
                item.MyLeft += group.MyLeft;
                item.MyTop += group.MyTop;
            }
            //元のグループ要素削除
            destGroup.InternalChildren.Remove(group);
            group.RemoveDragEvent();//いる？
            //destGroup.TTGroupUpdateLayout();
        }
        #endregion グループ解除

        //EnableThumbを内側(MovableThumbの親)へ切り替える
        public void EnableInside()
        {
            if (MovableThumb is TTGroup group)
            {
                EnableGroup = group;
                MovableThumb = GetMovableThumb(ClickedThumb);
            }
        }

        //EnableThumbを外側(親)へ切り替える
        public void EnableOutside()
        {
            if (EnableGroup.TTParent is TTGroup parent)
            {
                EnableGroup = parent;
                MovableThumb = GetMovableThumb(ClickedThumb);
            }
        }

    }
}
