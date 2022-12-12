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

namespace _20221208
{

    public abstract class TThumb : Thumb, INotifyPropertyChanged
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


        public string MyName
        {
            get { return (string)GetValue(ZNameProperty); }
            set { SetValue(ZNameProperty, value); }
        }
        public static readonly DependencyProperty ZNameProperty =
            DependencyProperty.Register(nameof(MyName), typeof(string), typeof(TThumb), new PropertyMetadata(""));

        #endregion

        #region 通知プロパティ
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private TThumb? _active;
        public TThumb? ActiveThumb
        {
            get => _active; set
            {
                //自身がGroupなら子要素のActiveも変更
                if (this is TTGroup tt)
                {
                    foreach (var item in tt.Children)
                    {
                        item.ActiveThumb = value;
                    }
                }

                SetProperty(ref _active, value);
            }
        }

        #endregion 通知プロパティ

        public TTGroup? ParentThumb { get; internal set; }
        public TTGroup? RootThumb { get; internal set; }
        public TThumb()
        {
            DataContext = this;
            SetBinding(Canvas.LeftProperty, new Binding(nameof(X)) { Mode = BindingMode.TwoWay });
            SetBinding(Canvas.TopProperty, new Binding(nameof(Y)) { Mode = BindingMode.TwoWay });
            SetBinding(Panel.ZIndexProperty, new Binding(nameof(Z)) { Mode = BindingMode.TwoWay });
            SetBinding(NameProperty, new Binding(nameof(MyName)) { Mode = BindingMode.TwoWay });

        }



        protected virtual void SetData(Data data)
        {
            X = data.X; Y = data.Y; Z = data.Z;
            if (string.IsNullOrEmpty(MyName) && string.IsNullOrEmpty(data.MyName) == false)
                MyName = data.MyName;
        }

        public TThumb(Data data) : this()
        {
            SetData(data);
        }
        public override string ToString()
        {
            return MyName;
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
            FrameworkElementFactory elem = new(typeof(TextBlock));
            elem.SetValue(TextBlock.TextProperty, new Binding(nameof(Text)));
            elem.SetValue(TextBlock.ForegroundProperty, new Binding(nameof(FontColor)));
            elem.SetValue(TextBlock.BackgroundProperty, new Binding(nameof(BackColor)));
            this.Template = new() { VisualTree = elem };

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
            //Canvas.SetLeft(this,100);Canvas.SetTop(this,0);
            FrameworkElementFactory elem = new(typeof(Rectangle));
            elem.SetValue(Shape.FillProperty, new Binding(nameof(Fill)));
            elem.SetValue(Rectangle.WidthProperty, new Binding(nameof(Width)));
            elem.SetValue(Rectangle.HeightProperty, new Binding(nameof(Height)));
            //elem.SetValue(Rectangle.FillProperty, new Binding(nameof(Fill)));

            this.Template = new() { VisualTree = elem };

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

        //public TThumb? ClickedItem { get; set; }

        public TTGroup()
        {
            FrameworkElementFactory panel = new(typeof(Canvas));
            FrameworkElementFactory ic = new(typeof(ItemsControl));
            ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(panel));
            this.Template = new() { VisualTree = ic };
            Children.CollectionChanged += Children_CollectionChanged;

        }


        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems?[0] is TThumb tt)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        tt.ParentThumb = this;
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        tt.ParentThumb = null;
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

        public TTGroup(Data data) : this()
        {
            base.SetData(data);
            SetData(data);
        }
        protected override void SetData(Data data)
        {
            if (data.Datas != null)
            {
                foreach (var item in data.Datas)
                {
                    Add(item);
                }
            }
        }
        private void Add(Data data)
        {
            switch (data.Type)
            {
                case TType.TextBlock:
                    Children.Add(new TTTextBlock(data));
                    break;
                case TType.Rectangle:
                    Children.Add(new TTRectangle(data));
                    break;
                case TType.Group:
                    Children.Add(new TTGroup(data));
                    break;
                default:
                    break;
            }
        }
        public void Add(TThumb thumb) { Children.Add(thumb); }
    }


    public class TTRoot : TTGroup
    {
        //public event PropertyChangedEventHandler? PropertyChanged;
        //protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, value)) return;
        //    field = value;
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}

        //private TThumb? _active;
        //public TThumb? ActiveThumb { get => _active; set => SetProperty(ref _active, value); }

        private TThumb? _enable;
        //入れ替え時に子要素のDragDeltaの付け外しをする＋ActiveThumbも更新する
        public TThumb? EnableThumb
        {
            get => _enable; set
            {
                if (_enable != value)
                {
                    if (_enable as TTGroup != null)
                    {
                        foreach (var item in ((TTGroup)_enable).Children)
                        {
                            item.DragDelta -= Item_DragDelta;
                            item.ActiveThumb = null;
                        }
                    }
                    if (value as TTGroup != null)
                    {
                        foreach (var item in ((TTGroup)value).Children)
                        {
                            item.DragDelta += Item_DragDelta;
                            item.ActiveThumb = item;//
                        }
                    }
                }
                SetProperty(ref _enable, value);
            }
        }

        private TThumb? _clicked;
        public TThumb? ClickedThumb { get => _clicked; set => SetProperty(ref _clicked, value); }
        public TTRoot()
        {
            Loaded += TTRoot_Loaded;

        }

        private void TTRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //
            EnableThumb ??= this;
        }

        private void Item_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.X += e.HorizontalChange;
                tt.Y += e.VerticalChange;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var source = e.Source;
            var origin = e.OriginalSource;
            //クリックされたThumbを登録
            if (((FrameworkElement)e.OriginalSource).TemplatedParent is TThumb tt)
            {
                ClickedThumb = tt;
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }
    }
}
