using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Runtime.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Media.Animation;

namespace _20221208
{

    public abstract class TThumb : Thumb//, INotifyPropertyChanged
    {
        #region 依存プロパティ


        //public double X
        //{
        //    get { return (double)GetValue(XProperty); }
        //    set { SetValue(XProperty, value); }
        //}
        //public static readonly DependencyProperty XProperty =
        //    DependencyProperty.Register(nameof(X), typeof(double), typeof(TTTextBlock), new PropertyMetadata(0.0));

        //デザイン画面での値変更時に描画更新対応した、依存プロパティ
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(nameof(X), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(nameof(Y), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public int Z
        {
            get { return (int)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }
        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register(nameof(Z), typeof(int), typeof(TThumb),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double TTWidth
        {
            get { return (double)GetValue(TTWidthProperty); }
            set { SetValue(TTWidthProperty, value); }
        }
        public static readonly DependencyProperty TTWidthProperty =
            DependencyProperty.Register(nameof(TTWidth), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double TTHeight
        {
            get { return (double)GetValue(TTHeightProperty); }
            set { SetValue(TTHeightProperty, value); }
        }
        public static readonly DependencyProperty TTHeightProperty =
            DependencyProperty.Register(nameof(TTHeight), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        //public string Name
        //{
        //    get { return (string)GetValue(NameProperty); }
        //    set { SetValue(NameProperty, value); }
        //}
        //public static readonly DependencyProperty NameProperty =
        //    DependencyProperty.Register(nameof(Name), typeof(string), typeof(TThumb), new PropertyMetadata(""));

        #endregion

        #region プロパティ
        public double Right { get; protected set; }//いらないかも
        public double Bottom { get; protected set; }//いらないかも
        //public TTRoot? MyTTRootThumb { get; internal set; }

        #endregion プロパティ
        public TTGroup? ParentThumb { get; internal set; }

        #region コンストラクタ

        public TThumb()
        {
            DataContext = this;
            SetBinding(Canvas.LeftProperty, new Binding(nameof(X)) { Mode = BindingMode.TwoWay });
            SetBinding(Canvas.TopProperty, new Binding(nameof(Y)) { Mode = BindingMode.TwoWay });
            SetBinding(Panel.ZIndexProperty, new Binding(nameof(Z)) { Mode = BindingMode.TwoWay });
            //SetBinding(NameProperty, new Binding(nameof(MyName)) { Mode = BindingMode.TwoWay });
            SetBinding(FrameworkElement.NameProperty, new Binding() { Path = new PropertyPath(Thumb.NameProperty) });

            //Loaded += TThumb_Loaded;
            SizeChanged += TThumb_SizeChanged;

            //SetBinding(WidthProperty, new Binding(nameof(Width)));
            //SetBinding(HeightProperty, new Binding(nameof(Height)));
        }
        protected FrameworkElementFactory MakeBaseTemplate()
        {
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Shape.StrokeProperty, Brushes.Cyan);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            waku.SetValue(Panel.ZIndexProperty, 1);
            //waku.SetValue(WidthProperty, new Binding(nameof(Width)));
            //waku.SetValue(HeightProperty, new Binding(nameof(Height)));
            //waku.SetValue(Rectangle.WidthProperty, new Binding(nameof(ActualWidth)) { Source = this });
            //waku.SetValue(Rectangle.HeightProperty, new Binding(nameof(ActualHeight)) { Source = this });
            FrameworkElementFactory panel = new(typeof(Grid));
            panel.AppendChild(waku);

            return panel;
        }


        private void TThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Right = X + ActualWidth;
            //Bottom = Y + ActualHeight;

            //重要：実際のサイズをサイズに指定する
            //TextBlockなどはサイズがNaNになるので実際のサイズを入れる
            Width = ActualWidth;
            Height = ActualHeight;

            //親要素のサイズと位置の更新
            ParentThumb?.UpdateRect(ParentThumb);
        }

        private void TThumb_Loaded(object sender, RoutedEventArgs e)
        {
            Width = ActualWidth; Height = ActualHeight;
        }

        public TThumb(Data data) : this()
        {
            SetData(data);
        }

        #endregion コンストラクタ

        protected virtual void SetData(Data data)
        {
            X = data.X; Y = data.Y; Z = data.Z;
            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(data.Name) == false)
                Name = data.Name;
        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class TTTextBlock : TThumb
    {
        #region 依存プロパティ
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TTTextBlock), new PropertyMetadata(""));


        public Brush FontColor
        {
            get { return (Brush)GetValue(FontColorProperty); }
            set { SetValue(FontColorProperty, value); }
        }
        public static readonly DependencyProperty FontColorProperty =
            DependencyProperty.Register(nameof(FontColor), typeof(Brush), typeof(TTTextBlock), new PropertyMetadata(Brushes.Black));


        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register(nameof(BackColor), typeof(Brush), typeof(TTTextBlock), new PropertyMetadata(Brushes.Transparent));


        #endregion

        public TTTextBlock()
        {
            var panel = MakeBaseTemplate();
            FrameworkElementFactory elem = new(typeof(TextBlock));
            elem.SetValue(TextBlock.TextProperty, new Binding(nameof(Text)));
            elem.SetValue(TextBlock.ForegroundProperty, new Binding(nameof(FontColor)));
            elem.SetValue(TextBlock.BackgroundProperty, new Binding(nameof(BackColor)));
            //フォントサイズプロパティはThumbにもあるし、名前も変える必要ないのでそのままBinding
            elem.SetValue(TextBlock.FontSizeProperty, new Binding() { Path = new PropertyPath(Thumb.FontSizeProperty) });
            panel.AppendChild(elem);

            this.Template = new() { VisualTree = panel };
        }

        public TTTextBlock(Data data) : this() { SetData(data); }
        protected override void SetData(Data data)
        {
            Text = data.Text;
            FontColor = data.ForeColor ?? Brushes.Black;
            BackColor = data.BackColor ?? Brushes.Transparent;
            FontSize = data.FontSize;
            base.SetData(data);
        }

    }

    public class TTRectangle : TThumb
    {

        #region 依存プロパティ
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(TTRectangle), new PropertyMetadata(Brushes.Red));


        #endregion 依存プロパティ

        public TTRectangle()
        {
            FrameworkElementFactory elem = new(typeof(Rectangle));
            elem.SetValue(Shape.FillProperty, new Binding(nameof(Fill)));
            elem.SetValue(Rectangle.WidthProperty, new Binding(nameof(Width)));
            elem.SetValue(Rectangle.HeightProperty, new Binding(nameof(Height)));
            FrameworkElementFactory pp = MakeBaseTemplate();
            pp.AppendChild(elem);
            this.Template = new() { VisualTree = pp };

        }
        public TTRectangle(Data data) : this()
        {
            SetData(data);
        }
        protected override void SetData(Data data)
        {
            Fill = data.BackColor ?? Brushes.Red;
            Width = data.Width;
            Height = data.Height;
            base.SetData(data);
        }
    }

    //WPFカスタムコントロールとダイレクトコンテンツのサポート
    //    https://stackoverflow.com/questions/4633568/wpf-custom-control-and-direct-content-support

    //WPF4.5入門 その10 「コンテンツ構文」 - かずきのBlog@hatena
    //  https://blog.okazuki.jp/entry/20130103/1357202019
    //これでXAMLでも子要素を追加できるようになる、ContentProperty(コレクション変数名)
    //もっとも、ダイレクトコンテンツをサポートしていない場合は<local:TTGroup.Children>で追加できる
    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        public ObservableCollection<TThumb> Children { get; protected set; } = new();

        //#region 通知プロパティ
        //public event PropertyChangedEventHandler? PropertyChanged;
        //protected void SetProperty<T>(ref T field, T value,
        //    [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, value)) return;
        //    field = value;
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}

        //private TThumb? _active;
        //public TThumb? ActiveThumb { get => _active; set => SetProperty(ref _active, value); }

        //#endregion 通知プロパティ


        #region コンストラクタ

        public TTGroup()
        {
            FrameworkElementFactory panel = new(typeof(Canvas));
            FrameworkElementFactory ic = new(typeof(ItemsControl));
            ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(panel));

            FrameworkElementFactory baseGridPanel = MakeBaseTemplate();
            baseGridPanel.AppendChild(ic);
            this.Template = new() { VisualTree = baseGridPanel };

            Children.CollectionChanged += Children_CollectionChanged;
        }
        public TTGroup(Data data) : this()
        {
            base.SetData(data);
            SetData(data);
        }
        //public TTGroup(ObservableCollection<TThumb> children)
        //{
        //    Children = children;
        //}

        #endregion コンストラクタ


        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is TThumb tth) { tth.ParentThumb = this; }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems?[0] is TThumb ttOld) { ttOld.ParentThumb = null; }//必要ないかも                    
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


        //protected override void SetData(Data data)
        //{
        //    if (data.Datas != null)
        //    {
        //        foreach (var item in data.Datas)
        //        {
        //            AddItem(item);
        //        }
        //    }
        //}

        //private void AddItem(Data data)
        //{
        //    switch (data.Type)
        //    {
        //        case TType.TextBlock:
        //            Children.Add(new TTTextBlock(data));
        //            break;
        //        case TType.Rectangle:
        //            Children.Add(new TTRectangle(data));
        //            break;
        //        case TType.Group:
        //            Children.Add(new TTGroup(data));
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //public void AddItem(TThumb thumb)
        //{
        //    Children.Add(thumb);
        //}


        /// <summary>
        /// 対象GroupThumbのRectを取得、UnionRect
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public (double x, double y, double w, double h) GetGroupRect(TTGroup group, bool actualSize)
        {
            if (group.Children.Count == 0) { return (0, 0, 0, 0); }
            double x = double.MaxValue, y = double.MaxValue;
            double w = 0.0, h = 0.0;
            foreach (var item in group.Children)
            {
                x = Math.Min(x, item.X);
                y = Math.Min(y, item.Y);
                if (actualSize)
                {

                    w = Math.Max(w, item.X + item.ActualWidth);
                    h = Math.Max(h, item.Y + item.ActualHeight);
                }
                else
                {
                    w = Math.Max(w, item.X + item.Width);
                    h = Math.Max(h, item.Y + item.Height);
                }
            }
            w -= x; h -= y;
            return (x, y, w, h);
        }
        /// <summary>
        /// 2個以上のThumbのRectを返す、グループ化などに使用
        /// </summary>
        /// <param name="thumbs"></param>
        /// <returns></returns>
        public (double x, double y, double w, double h) GetGroupRect(ObservableCollection<TThumb> thumbs)
        {
            if (thumbs.Count < 2) return (0, 0, 0, 0);
            double x = double.MaxValue, y = double.MaxValue;
            double w = 0.0, h = 0.0;
            foreach (var item in thumbs)
            {
                if (x > item.X) x = item.X;
                if (y > item.Y) y = item.Y;
                if (w < item.X + item.Width) w = item.X + item.Width;
                if (h < item.Y + item.Height) h = item.Y + item.Height;
            }
            w -= x; h -= y;
            return (x, y, w, h);
        }
        /// <summary>
        /// Thumb群の左上座標を返す、グループ化などに使用
        /// </summary>
        /// <param name="thumbs"></param>
        /// <returns></returns>
        public (double x, double y) GetGroupTopLeft(ObservableCollection<TThumb> thumbs)
        {
            if (thumbs.Count < 2) return (0, 0);
            double x = double.MaxValue, y = double.MaxValue;
            foreach (var item in thumbs)
            {
                if (x > item.X) x = item.X;
                if (y > item.Y) y = item.Y;
            }
            return (x, y);
        }

        /// <summary>
        /// GroupThumbのサイズと位置の更新、Thumb移動後などに使用。指定Groupから親要素に向かって全て更新する
        /// </summary>
        /// <param name="group">更新対象のGroupThumb</param>
        /// <param name="actualSize">false指定でWidhtHeightで計算</param>

        internal void UpdateRect(TTGroup? group, bool actualSize = true)
        {
            if (group == null) return;
            (double x, double y, double w, double h) = GetGroupRect(group, actualSize);
            //if (w == 0 && h == 0) { return; }
            //if (X == x && Y == y && w == ActualWidth && h == ActualHeight) { return; }
            //if (X == x && Y == y && w == group.ActualWidth && h == group.ActualHeight) { return; }

            group.Width = w; group.Height = h;
            if (x != 0 || y != 0)
            {
                foreach (var item in group.Children)
                {
                    item.X -= x; item.Y -= y;
                }
                if (group is not TTRoot) { group.X += x; group.Y += y; }
            }

            //連なる親要素も更新する
            if (group.ParentThumb is TTGroup parentt)
            {
                UpdateRect(parentt, false);
            }
        }



    }


    public class TTRoot : TTGroup, INotifyPropertyChanged
    {
        #region 通知プロパティ
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value,
            [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private TThumb? _active;
        public TThumb? ActiveThumb { get => _active; set => SetProperty(ref _active, value); }

        #endregion 通知プロパティ



        private TTGroup? _enable;
        //入れ替え時に子要素のDragDeltaの付け外しをする＋ActiveThumbも更新(nullに)する        
        public TTGroup? EnableThumb
        {
            get => _enable;
            set
            {
                if (_enable != value)
                {
                    ActiveThumb = null;
                    if (_enable != null)
                    {
                        foreach (var item in _enable.Children)
                        {
                            item.DragDelta -= Item_DragDelta;
                            item.DragCompleted -= Item_DragCompleted;
                        }
                    }
                    if (value != null)
                    {
                        foreach (var item in value.Children)
                        {
                            item.DragDelta += Item_DragDelta;
                            item.DragCompleted += Item_DragCompleted;
                        }
                    }
                }
                SetProperty(ref _enable, value);
            }
        }
        //  public TTGroup? EnableThumb
        //{
        //    get => _enable; set
        //    {
        //        if (_enable != value)
        //        {
        //            if (_enable != null)
        //            {
        //                _enable.ActiveThumb = null;
        //                //_enable.IsEnabledThumb = false;
        //                foreach (var item in _enable.Children)
        //                {
        //                    item.DragDelta -= _enable.Item_DragDelta;
        //                    item.DragCompleted -= _enable.Item_DragCompleted;
        //                }
        //            }
        //            if (value != null)
        //            {
        //                value.ActiveThumb = null;
        //                //value.IsEnabledThumb = true;
        //                foreach (var item in value.Children)
        //                {
        //                    item.DragDelta += value.Item_DragDelta;
        //                    item.DragCompleted += value.Item_DragCompleted;
        //                }
        //            }
        //        }
        //        SetProperty(ref _enable, value);
        //    }
        //}


        private TThumb? _clicked;
        public TThumb? ClickedThumb
        {
            get => _clicked;
            set
            {
                //ActiveThumbの更新
                if (value != _clicked)
                {
                    ActiveThumb = GetActiveThumb(value);
                }
                SetProperty(ref _clicked, value);
            }
        }
        //ActiveThumbはEnableThumbのChildrenのどれかになる
        //
        /// <summary>
        /// ActiveThumbを探す、指定Thumbから親要素を辿ってEnableがあれば、引数がActiveってことになる
        /// </summary>
        /// <param name="itemThumb"></param>
        /// <returns></returns>
        private TThumb? GetActiveThumb(TThumb? itemThumb)
        {
            if (itemThumb == null) return null;
            var parent = itemThumb.ParentThumb;
            if (EnableThumb == parent) return itemThumb;
            else return GetActiveThumb(parent);
        }
        ////関連の有無を返す、自分自身か関連があればtrueを返す
        //private bool IsRelated(TThumb? target, TThumb? me)
        //{
        //    if (target == null || me == null) return false;
        //    if (target == me) return true;
        //    if (me.ParentThumb == target) return true;
        //    else { return IsRelated(target, me.ParentThumb); }
        //}

        //グループ化に使う
        public ObservableCollection<TThumb> SelectedThumbs { get; set; } = new();


        #region コンストラクタ
        public TTRoot()
        {
            Loaded += TTRoot_Loaded;
            //MyTTRootThumb = this;
        }
        #endregion コンストラクタ

        //起動直後
        private void TTRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //
            EnableThumb ??= this;
            //すべてのGroupのサイズ更新
            //下のGroupから実行する
            UpdateAllRect(this, false);
        }
        /// <summary>
        /// Root専用、すべてのGroupのサイズ更新、下側から実行、最後にRootのサイズ更新になる
        /// </summary>
        /// <param name="group">Root</param>
        /// <param name="isActualSize">false指定でWidhtHeightで計算</param>
        private void UpdateAllRect(TTGroup group, bool isActualSize)
        {
            foreach (var item in group.Children)
            {
                if (item is TTGroup ttg)
                {
                    UpdateAllRect(ttg, true);
                }
            }
            if (group == null) return;
            (double x, double y, double w, double h) = GetGroupRect(group, isActualSize);
            if (w == 0 && h == 0) { return; }
            if (X == x && Y == y && w == ActualWidth && h == ActualHeight) { return; }

            //Actualは変更できないので、通常のWidhtHeightを変更
            group.Width = w; group.Height = h;
            if (x != 0 || y != 0)
            {
                foreach (var item in group.Children)
                {
                    item.X -= x; item.Y -= y;
                }
                if (group is not TTRoot) { group.X += x; group.Y += y; }
            }

        }


        //マウス左クリック時
        //クリックされたThumbをClickedThumbに登録
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //SourceはRootThumb、OriginalSourceはItemThumbのTemplateの中の要素になる
            //TextBlockThumbならTextBlock、RectangleThumbならRectangle
            base.OnPreviewMouseLeftButtonDown(e);
            if (e.OriginalSource is FrameworkElement fwElement)
            {
                //ItemThumbがクリックされた場合はClickedThumbを更新する
                if (fwElement.TemplatedParent is TThumb tt)
                {
                    if (tt is TTGroup) { return; }//GroupはClickedにはしない
                    ClickedThumb = tt;
                }
                ////必要ない？
                //if (fwElement is TThumb th)
                //{
                //    if (th is TTGroup) { return; }//GroupはClickedにはしない
                //    ClickedThumb = th;
                //}

                //SelectedThumbsにActiveThumbを追加
                //通常クリックならSelectedをクリアしてから追加(常に1個)
                //Ctrl＋クリックならSelectedThumbsをクリアしないで追加(複数選択用)
                if (ActiveThumb != null)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control && SelectedThumbs.Contains(ActiveThumb) == false)
                    {
                        SelectedThumbs.Add(ActiveThumb);
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.None)
                    {
                        SelectedThumbs.Clear();
                        SelectedThumbs.Add(ActiveThumb);
                    }

                }

            }
        }
        //マウス左クリック離したとき
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            ////修飾キー押されていなかったらSelectedをクリアする
            //if (Keyboard.Modifiers == ModifierKeys.None)
            //{
            //    SelectedThumbs.Clear();

            //}
        }


        #region グループ化

        #endregion グループ化

        #region 追加と削除

        /// <summary>
        /// Item追加
        /// </summary>
        /// <param name="thumb">追加するThumb</param>
        /// <param name="group">追加先Group、指定なしならEnableThumbに追加する</param>
        public void AddItem(TThumb thumb, TTGroup? group = null)
        {
            group ??= EnableThumb;
            if (group != null)
            {
                group.Children.Add(thumb);
                AddDragEvent(thumb);
                ActiveThumb = thumb;
            }
        }
        /// <summary>
        /// Item削除
        /// </summary>
        /// <param name="thumb">削除するThumb、未指定ならActiveThumbを削除する</param>
        /// <param name="group">削除対象を保有するGroup、未指定ならEnableThumb</param>
        public void RemoveItem(TThumb? thumb = null, TTGroup? group = null, bool updateRect = true)
        {
            group ??= EnableThumb;
            thumb ??= ActiveThumb;
            if (group != null && thumb != null)
            {
                group.Children.Remove(thumb);
                RemoveDragEvent(thumb);
                ActiveThumb = null;
                //if (ClickedThumb == thumb) { ClickedThumb = null; }
                ClickedThumb = null;
                //Childrenが1個になったらGroupを削除して、残りの1個を上に上げる(移動)
                //ただし、GroupがRootだった場合を除く
                if (group.Children.Count == 1)
                {
                    if (group.GetType() == typeof(TTRoot))
                    {
                        if (updateRect) UpdateRect(group, false);
                        return;
                    }
                    TTGroup? parent = group.ParentThumb;
                    EnableThumb = parent;//Enable更新
                    RemoveItem(group);//Group削除
                    //残りの1個を確保してひとつ上のGroupに追加、座標修正はGroupの座標+になる
                    TThumb? nokori = group.Children[0];
                    nokori.X += group.X; nokori.Y += group.Y;
                    group.Children.Clear();
                    AddItem(nokori);

                }

                //サイズと位置の更新
                if (updateRect)
                {
                    UpdateRect(group, false);
                }

                ////Childrenが0個になったらGroupも削除
                //if (group.Children.Count == 0)
                //{
                //    RemoveItem(group, group.ParentThumb);
                //    EnableThumb = group.ParentThumb;
                //}
            }
        }

     
        public void MoveItemNewGroup(ObservableCollection<TThumb> thumbs, TTGroup origin, string name)
        {
            //元のグループから外す
            foreach (var item in thumbs)
            {
                RemoveItem(item, origin, true);
            }
            UpdateRect(origin);

            //新しいグループ作成
            var (x, y) = GetGroupTopLeft(thumbs);
            Data data = new(TType.Group)
            {
                X = x + origin.X,
                Y = y + origin.Y,
                Name = name
            };
            TTGroup group = new(data);
            //新しいグループに追加
            foreach (var item in thumbs)
            {
                group.Children.Add(item);
                //AddItem(item, group);
            }
            UpdateRect(group, false);
            AddItem(group);
        }

        #endregion 追加と削除

        private void AddDragEvent(TThumb thumb)
        {
            thumb.DragDelta += Item_DragDelta;
            thumb.DragCompleted += Item_DragCompleted;
        }
        private void RemoveDragEvent(TThumb thumb)
        {
            thumb.DragDelta -= Item_DragDelta;
            thumb.DragCompleted -= Item_DragCompleted;
        }
        private void Item_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (e.OriginalSource is TThumb t)
            {
                if (t.ParentThumb is TTGroup group)
                {
                    //UpdateRect(group, false);//これだとおかしい
                    UpdateRect(group, true);//これが正解、Actualで計算する
                }
            }

            //if (sender is TThumb t)
            //{
            //    if (t.X < 0 || Y < 0)
            //    {
            //        double xDiff = t.X; double yDiff = t.Y;
            //        if (t.ParentThumb != null)
            //        {
            //            foreach (var item in t.ParentThumb.Children)
            //            {
            //                item.X -= xDiff; item.Y -= yDiff;
            //            }
            //        }
            //    }
            //}
        }

        private void Item_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.X += e.HorizontalChange;
                tt.Y += e.VerticalChange;
            }
        }

    }


    public class ConverterThumbBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //未使用
    public class ExCanvas : Canvas
    {
        //c# - WPF: Sizing Canvas to contain its children - Stack Overflow
        //https://stackoverflow.com/questions/55101074/wpf-sizing-canvas-to-contain-its-children
        //protected override Size MeasureOverride(Size constraint)
        //{
        //    base.MeasureOverride(constraint);
        //    Size size = new();
        //    foreach (var item in Children.OfType<FrameworkElement>())
        //    {
        //        double x = GetLeft(item) + item.Width;
        //        if (x > size.Width && !double.IsNaN(x))
        //        {
        //            size.Width = x;
        //        }
        //        double y = GetTop(item) + item.Height;
        //        if (y > size.Height && !double.IsNaN(y))
        //        {
        //            size.Height = y;
        //        }
        //    }
        //    return size;
        //}

        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            Size size = new();
            foreach (var item in Children.OfType<TThumb>())
            {
                double x = item.X;
                double w = item.DesiredSize.Width;
                if (w == double.PositiveInfinity) { w = 0.0; }
                x += w;
                if (x > size.Width && !double.IsNaN(x))
                {
                    size.Width = x;
                }
                double y = item.Y;

                double h = item.DesiredSize.Height;
                if (h == double.PositiveInfinity) { h = 0.0; }
                y += h;
                if (y > size.Height && !double.IsNaN(y))
                {
                    size.Height = y;
                }
            }
            return size;
        }

    }
}
