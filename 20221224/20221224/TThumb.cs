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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Imaging;
using System.IO;


//前面移動、背面移動できた
//対象は選択状態のThumbすべて
//ZIndexは使わずに、要素のIndexを変更することで移動
//Indexの変更は削除と追加を繰り返して行ったけど、
//moveメソッドを使ったほうが良かったかも
namespace _20221224
{
    public class Data : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private double _x;
        public double X { get => _x; set => SetProperty(ref _x, value); }

        private double _y;
        public double Y { get => _y; set => SetProperty(ref _y, value); }

    }
    public class DataText : Data
    {
        private string? _myText;
        public string? MyText { get => _myText; set => SetProperty(ref _myText, value); }
    }
    public class DataGroup : Data
    {
        private Collection<Data> _childrenData = new();
        public Collection<Data> ChildrenData { get => _childrenData; set => SetProperty(ref _childrenData, value); }
    }

    public abstract class TThumb : Thumb, INotifyPropertyChanged
    {
        #region 依存プロパティ、通知プロパティ

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //依存プロパティじゃなくても、通知プロパティでおｋだったけど変えた
        //private double _myLeft;
        //public double MyLeft { get => _myLeft; set => SetProperty(ref _myLeft, value); }


        public double MyLeft
        {
            get { return (double)GetValue(MyLeftProperty); }
            set
            {
                SetValue(MyLeftProperty, value);
            }
        }
        public static readonly DependencyProperty MyLeftProperty =
            DependencyProperty.Register(nameof(MyLeft), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));

        public double MyTop
        {
            get { return (double)GetValue(MyTopProperty); }
            set { SetValue(MyTopProperty, value); }
        }
        public static readonly DependencyProperty MyTopProperty =
            DependencyProperty.Register(nameof(MyTop), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));


        #endregion 依存プロパティ、通知プロパティ

        public TTGroup? TTParent { get; set; }
        public Data MyData { get; set; }
        public TThumb()
        {
            MyData ??= new Data();
            DataContext = this;

            Binding b;
            b = new(nameof(MyData.X)) { Source = MyData, Mode = BindingMode.TwoWay };
            SetBinding(Canvas.LeftProperty, b);
            SetBinding(MyLeftProperty, b);

            b = new(nameof(MyData.Y)) { Source = MyData, Mode = BindingMode.TwoWay };
            SetBinding(Canvas.TopProperty, b);
            SetBinding(MyTopProperty, b);



            //重要！！！！エッジモードをエイリアスに指定
            //これでアンチエイリアスがなくなって枠線とかがくっきりで保存できる,
            //けどこれでもまだ微妙にずれる
            //あと、なぜか表示自体は変化ない？
            //RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            //Enabledでテキストの保存時にクリアタイプが適用される。既定値はAutoでこれだと適用されない
            //RenderOptions.SetClearTypeHint(this, ClearTypeHint.Enabled);
        }
        public TThumb(Data data) : this()
        {
            MyData = data;
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

        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(TTTextBlock), new PropertyMetadata(""));

        public new DataText MyData { get; set; }
        public TTTextBlock()
        {
            MyData ??= new DataText();
            DataContext = this;
            //Template構築、適用
            SetTemplate3();

        }
        private void SetTemplate0()
        {
            //this.VisualTextRenderingMode = TextRenderingMode.Aliased;

            //FrameworkElementFactory text = new(typeof(TextBlock));
            //FrameworkElementFactory waku = new(typeof(Rectangle));
            //FrameworkElementFactory panel = new(typeof(Grid));
            //waku.SetValue(Shape.StrokeProperty, Brushes.Red);
            //waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            ////waku.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);//エッジモード
            //text.SetValue(TextBlock.TextProperty, new Binding(nameof(MyText)));
            ////text.SetValue(TextBlock.ForegroundProperty, Brushes.Red);
            ////画像保存時のクリアタイプOFF、既定値
            ////text.SetValue(RenderOptions.ClearTypeHintProperty, ClearTypeHint.Auto);
            ////画像保存時のクリアタイプON
            ////text.SetValue(RenderOptions.ClearTypeHintProperty, ClearTypeHint.Enabled);
            //panel.SetValue(BackgroundProperty, Brushes.AliceBlue);
            //panel.AppendChild(text);
            //panel.AppendChild(waku);
            //this.Template = new() { VisualTree = panel };
            //this.ApplyTemplate();

        }

        private void SetTemplate1()
        {
            FrameworkElementFactory text = new(typeof(TextBlock));
            FrameworkElementFactory waku = new(typeof(Border));
            waku.SetValue(Border.BorderThicknessProperty, new Thickness(4.0));
            waku.SetValue(Border.BorderBrushProperty, Brushes.Red);
            text.SetValue(TextBlock.TextProperty, new Binding(nameof(MyText)));
            waku.AppendChild(text);
            this.Template = new() { VisualTree = waku };
        }
        private void SetTemplate3()
        {
            FrameworkElementFactory text = new(typeof(TextBlock));
            FrameworkElementFactory waku = new(typeof(Border));
            waku.SetValue(Border.BorderThicknessProperty, new Thickness(4.0));
            waku.SetValue(Border.BorderBrushProperty, Brushes.Red);
            Binding b;
            b = new(nameof(MyData.MyText)) { Source = MyData, Mode = BindingMode.TwoWay };
            text.SetValue(TextBlock.TextProperty, b);
            //text.SetValue(TextBlock.TextProperty, new Binding(nameof(MyText)));
            this.SetBinding(MyTextProperty, b);
            waku.AppendChild(text);
            this.Template = new() { VisualTree = waku };
            //this.BorderThickness = new Thickness(4.0);
        }
        private void SetTemplate2()
        {
            FrameworkElementFactory text = new(typeof(TextBlock));
            text.SetValue(TextBlock.TextProperty, new Binding(nameof(MyText)));
            this.Template = new() { VisualTree = text };
            this.BorderBrush = Brushes.Red;
            this.BorderThickness = new Thickness(4.0);
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

            SetTemplate3();

            ////Template構造
            ////Thumb
            ////┗ Template
            ////   ┗ Grid
            ////      ┣ ItemsControl   コレクションをBinding
            ////      ┃  ┗ Canvas      PanelTemplate
            ////      ┗ Rectangle      枠
            //FrameworkElementFactory waku = new(typeof(Rectangle));//サイズ確認用枠
            //waku.SetValue(Rectangle.StrokeProperty, Brushes.Blue);
            //waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);
            //FrameworkElementFactory grid = new(typeof(Grid));
            //FrameworkElementFactory ic = new(typeof(ItemsControl));
            ////ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            //FrameworkElementFactory icPanel = new(typeof(Canvas));
            //ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(icPanel));
            //ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            //grid.AppendChild(ic);
            //grid.AppendChild(waku);
            //this.Template = new() { VisualTree = grid };

        }
        private void SetTemplate1()
        {
            //FrameworkElementFactory waku = new(typeof(Border));
            //waku.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
            //waku.SetValue(Border.BorderBrushProperty, Brushes.Red);
            FrameworkElementFactory fItems = new(typeof(ItemsControl));
            FrameworkElementFactory fCanvas = new(typeof(Canvas));
            fItems.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(fCanvas));
            fItems.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            fItems.SetValue(ItemsControl.BorderThicknessProperty, new Thickness(4.0));
            fItems.SetValue(ItemsControl.BorderBrushProperty, Brushes.Blue);
            //this.BorderThickness = new(4.0);
            //this.BorderBrush = Brushes.Blue;

            this.Template = new() { VisualTree = fItems };
        }
        private void SetTemplate2()
        {
            FrameworkElementFactory waku = new(typeof(Border));
            waku.SetValue(Border.BorderThicknessProperty, new Thickness(4.0));
            waku.SetValue(Border.BorderBrushProperty, Brushes.Blue);
            FrameworkElementFactory fItems = new(typeof(ItemsControl));
            FrameworkElementFactory fCanvas = new(typeof(Canvas));
            fItems.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(fCanvas));
            fItems.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            waku.AppendChild(fItems);
            this.Template = new() { VisualTree = waku };
        }
        private void SetTemplate3()
        {
            FrameworkElementFactory fGrid = new(typeof(Grid));
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);
            waku.SetValue(Rectangle.StrokeProperty, Brushes.Blue);
            FrameworkElementFactory fItems = new(typeof(ItemsControl));
            FrameworkElementFactory fCanvas = new(typeof(Canvas));
            fItems.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(fCanvas));
            fItems.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            fGrid.AppendChild(fItems);
            fGrid.AppendChild(waku);
            this.Template = new() { VisualTree = fGrid };
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

        #region オーバーライド関連

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

        #endregion オーバーライド関連

        #region その他関数

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
        /// <summary>
        /// SelectedThumbsを並べ替えたList作成、基準はActiveGroupのChildren
        /// </summary>
        /// <param name="selected">SelectedThumbs</param>
        /// <param name="group">並べ替えの基準にするGroup</param>
        /// <returns></returns>
        private List<TThumb> MakeSortedList(IEnumerable<TThumb> selected, TTGroup group)
        {
            List<TThumb> tempList = new();
            foreach (var item in group.InternalChildren)
            {
                if (selected.Contains(item)) { tempList.Add(item); }
            }
            return tempList;
        }
        /// <summary>
        /// 要素すべてがGroupのChildrenに存在するか判定
        /// </summary>
        /// <param name="thums">要素群</param>
        /// <param name="group">ParentGroup</param>
        /// <returns></returns>
        private bool IsAllContains(IEnumerable<TThumb> thums, TTGroup group)
        {
            if (!thums.Any()) { return false; }//要素が一つもなければfalse
            foreach (var item in thums)
            {
                if (group.InternalChildren.Contains(item) == false)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion その他関数

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
        /// <summary>
        /// 指定Thumbだけを指定Groupから削除
        /// </summary>
        /// <param name="thumb"></param>
        /// <param name="group"></param>
        /// <returns></returns>
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
            //SelectedThumbs並べ替え
            var tempList = MakeSortedList(SelectedThumbs, ActiveGroup);
            TTGroup? group = MakeAndAddGroup(tempList, ActiveGroup);
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
            TTGroup group = new() { Name = "new_group" };
            group.MyData.X = x; group.MyData.Y = y;
            //TTGroup group = new() { Name = "new_group", MyLeft = x, MyTop = y };

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

        #region InOut
        //ActiveThumbを内側(ActiveThumbの親)へ切り替える
        public void ActiveGroupInside()
        {
            if (ActiveThumb is TTGroup group)
            {
                ActiveGroup = group;
                ActiveThumb = GetActiveThumb(ClickedThumb);
                SelectedThumbs.Clear();
            }
        }

        //ActiveThumbを外側(親)へ切り替える
        public void ActiveGroupOutside()
        {
            if (ActiveGroup.TTParent is TTGroup parent)
            {
                ActiveGroup = parent;
                ActiveThumb = GetActiveThumb(ClickedThumb);
                SelectedThumbs.Clear();
            }
        }
        #endregion InOut

        #region ZIndex
        //ZIndexが同じ場合はChildrenIndexが前後関係になるのを利用して
        //Children要素の入れ替えによって前面、背面移動
        #region 背面に移動

        //再背面に移動
        public bool ZDownBackMost()
        {
            return ZDownBackMost(SelectedThumbs, ActiveGroup);
        }
        public bool ZDownBackMost(IEnumerable<TThumb> thumbs, TTGroup group)
        {
            if (IsAllContains(thumbs, group) == false) { return false; }
            //下側にある要素から処理したいので、並べ替えたListを作成
            List<TThumb> tempList = MakeSortedList(thumbs, group);
            //削除してから先頭から挿入
            for (int i = 0; i < tempList.Count; i++)
            {
                if (group.InternalChildren.Remove(tempList[i]))
                {
                    group.InternalChildren.Insert(i, tempList[i]);
                }
                else { return false; }
            }
            return true;
        }
        public bool ZDown()
        {
            return ZDown(SelectedThumbs, ActiveGroup);
        }
        public bool ZDown(IEnumerable<TThumb> thumbs, TTGroup group)
        {
            if (IsAllContains(thumbs, group) == false) { return false; }
            //順番を揃えたリスト作成
            List<TThumb> tempList = MakeSortedList(thumbs, group);

            //一番下の要素がもともと一番下だった場合は処理しない
            if (group.InternalChildren[0] == tempList[0])
            {
                return false;
            }
            //順番に処理、削除してから挿入、挿入箇所は元のインデックス-1
            foreach (var item in tempList)
            {
                int ii = group.InternalChildren.IndexOf(item);
                ii--;
                if (group.InternalChildren.Remove(item))
                {
                    group.InternalChildren.Insert(ii, item);
                }
                else { return false; }
            }

            return true;
        }
        #endregion 背面に移動

        #region 前面に移動

        //最前面へ移動
        public bool ZUpFrontMost(IEnumerable<TThumb> thumbs, TTGroup group)
        {
            //要素すべてがGroupのChildrenに存在するか判定、存在しない要素があれば処理しない            
            if (IsAllContains(thumbs, group) == false) { return false; }

            //下側にある要素から処理したいので、並べ替えたListを作成
            List<TThumb> tempList = MakeSortedList(thumbs, group);
            //削除してから追加(末尾に追加)
            foreach (var item in tempList)
            {
                if (group.InternalChildren.Remove(item))
                {
                    group.InternalChildren.Add(item);
                }
                else { return false; }
            }
            return true;
        }

        public bool ZUpFrontMost()
        {
            return ZUpFrontMost(SelectedThumbs, ActiveGroup);
        }

        //一つ前に移動
        public bool ZUp(IEnumerable<TThumb> thumbs, TTGroup group)
        {
            if (IsAllContains(thumbs, group) == false) { return false; }
            //順番を揃えてから削除して追加
            List<TThumb> tempList = MakeSortedList(thumbs, group);

            //一番上の要素がもともと一番上だった場合は処理しない
            if (group.InternalChildren[^1] == tempList[^1])
            {
                return false;
            }
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                int ii = group.InternalChildren.IndexOf(tempList[i]);
                ii++;
                if (group.InternalChildren.Remove(tempList[i]))
                {
                    group.InternalChildren.Insert(ii, tempList[i]);
                }
                else { return false; }
            }
            return true;

        }
        public bool ZUp()
        {
            return ZUp(SelectedThumbs, ActiveGroup);
        }
        #endregion 前面に移動


        //backmost 最背面 back to back最背面にする
        //frontmsot 一番前
        //front 最前面
        //put one back  一つ後ろにする
        #endregion ZIndex

        #region 画像として保存
        /// <summary>
        /// BitmapSourceをpngファイルにする
        /// </summary>
        /// <param name="bitmap">保存する画像</param>
        /// <param name="filePath">保存先パス(拡張子も付けたフルパス)</param>
        private void SaveBitmapToPng(BitmapSource bitmap, string filePath)
        {
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = File.Open(filePath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        public void SaveImage()
        {
            SaveImage2(this, this);
            //SaveImage(this);
            //SaveImage(ActiveThumb);
        }
        public void SaveImage(FrameworkElement el, FrameworkElement parentPanel)
        {
            GeneralTransform gt = el.TransformToVisual(parentPanel);
            Rect bounds = gt.TransformBounds(new Rect(0, 0, el.ActualWidth, el.ActualHeight));
            DrawingVisual dVisual = new();
            //var neko = VisualTreeHelper.GetDrawing(ActiveThumb);
            using (DrawingContext context = dVisual.RenderOpen())
            {
                VisualBrush vBrush = new(el) { Stretch = Stretch.None };
                context.DrawRectangle(vBrush,
                    null,
                    new Rect(0.0, 0.0, (int)bounds.Width, (int)bounds.Height));

                //context.DrawRectangle(vBrush, null, new Rect(bounds.Size));
            }
            RenderTargetBitmap bitmap
                = new((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(dVisual);

            SaveBitmapToPng(bitmap, "E:result.png");
        }
        public void SaveImage2(FrameworkElement el, FrameworkElement parentPanel)
        {
            GeneralTransform gt = el.TransformToVisual(parentPanel);
            Rect bounds = gt.TransformBounds(new Rect(0, 0, el.ActualWidth, el.ActualHeight));
            DrawingVisual dVisual = new();
            //var debounds = VisualTreeHelper.GetDescendantBounds(parentPanel);
            bounds.Width = (int)(bounds.Width + 0.5);
            bounds.Height = (int)(bounds.Height + 0.5);

            using (DrawingContext context = dVisual.RenderOpen())
            {
                VisualBrush vBrush = new(el) { Stretch = Stretch.None };
                //context.DrawRectangle(vBrush, null, new Rect(0, 0, bounds.Width, bounds.Height));
                //context.DrawRectangle(vBrush, null, bounds);

                context.DrawRectangle(vBrush, null, new Rect(bounds.Size));
            }
            RenderTargetBitmap bitmap
                = new((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(dVisual);

            SaveBitmapToPng(bitmap, "E:result.png");
        }

        public void SaveImage(TThumb target)
        {
            if (target.TTParent is TTGroup parent)
            {
                SaveImage2(target, parent);
            }
        }
        public void SaveImageEz(FrameworkElement el)
        {
            RenderTargetBitmap bitmap
                = new((int)el.ActualWidth, (int)el.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(el);
            SaveBitmapToPng(bitmap, "E:result.png");
        }


        #endregion 画像として保存

        #region データ保存
        public void SaveData(string fileName)
        {

        }
        #endregion データ保存



    }

    //C# ObservableCollection<T>で大量の要素を追加したいとき - Qiita
    //    https://qiita.com/Yuki4/fItems/0e73297db632376804dd

    public class TTObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            foreach (var item in collection)
            {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }
}
