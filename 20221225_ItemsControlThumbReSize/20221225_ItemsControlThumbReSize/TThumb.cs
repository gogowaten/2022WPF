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

namespace _20221225_ItemsControlThumbReSize
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


        #endregion 依存プロパティ

        public TTGroup? TTParentGroup { get; set; }
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


    public class TTItemThumb : TThumb
    {
        public TTItemThumb()
        {
            SizeChanged += TThumb_SizeChanged;            
        }
        private void TThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TTParentGroup?.UpdateTTGroupLayout();
        }
    }


    public class TTTextBlock : TTItemThumb
    {
        private readonly string TTNAME = "content";
        public TextBlock? Content { get; set; }
        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(TTTextBlock), new PropertyMetadata(""));

        public TTTextBlock()
        {
            DataContext = this;
            FrameworkElementFactory text = new(typeof(TextBlock), TTNAME);
            FrameworkElementFactory waku = new(typeof(Rectangle));
            FrameworkElementFactory panel = new(typeof(Grid));
            waku.SetValue(Shape.StrokeProperty, Brushes.Red);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            panel.AppendChild(text);
            panel.AppendChild(waku);
            this.Template = new() { VisualTree = panel };
            this.ApplyTemplate();
            Content = (TextBlock)this.Template.FindName(TTNAME, this);
            Content.SetBinding(TextBlock.TextProperty, nameof(MyText));

            DragDelta += TTTextBlock_DragDelta;
            DragCompleted += TTTextBlock_DragCompleted;
        }

        //ドラッグ移動終了時に親要素のサイズと位置の更新
        private void TTTextBlock_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.TTParentGroup?.UpdateTTGroupLayout();
                //ReSize(tt.TTParentGroup);
            }
        }

        private void TTTextBlock_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;
        }

    }

    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        private readonly string TTNAME = "content";

        public ItemsControl? Content { get; set; }

        public ObservableCollection<TThumb> Children { get; set; } = new();
        public TTGroup()
        {
            DataContext = this;
            Children.CollectionChanged += Children_CollectionChanged;
            SizeChanged += TTGroup_SizeChanged;

            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Rectangle.StrokeProperty, Brushes.Blue);
            waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);
            FrameworkElementFactory grid = new(typeof(Grid));
            FrameworkElementFactory ic = new(typeof(ItemsControl), TTNAME);
            //ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            //FrameworkElementFactory icPanel = new(typeof(ExCanvas));
            FrameworkElementFactory icPanel = new(typeof(Canvas));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(icPanel));
            grid.AppendChild(ic);
            grid.AppendChild(waku);
            this.Template = new() { VisualTree = grid };
            this.ApplyTemplate();
            Content = (ItemsControl)this.Template.FindName(TTNAME, this);
            Content.SetBinding(ItemsControl.ItemsSourceProperty, nameof(Children));
            Content.ApplyTemplate();

            Loaded += TTGroup_Loaded;
        }

        public void UpdateTTGroupLayout()
        {
            TTGroup target = this;
            (double x, double y, double w, double h) = GetRect(target);

            //子要素位置修正
            foreach (var item in target.Children)
            {
                item.MyLeft -= x;
                item.MyTop -= y;
            }
            //親要素のサイズ更新、Root以外は位置修正
            if (target.Name != "RootTTG")
            {
                target.MyLeft += x;
                target.MyTop += y;
            }
            target.Width = w - x;
            target.Height = h - y;
            //必要、これで実際にサイズ更新される、SizeChangedで確認できる
            target.UpdateLayout();

            //祖先があれば遡って更新
            if (target.TTParentGroup is TTGroup parent)
            {
                parent.UpdateTTGroupLayout();
            }
        }

        //TTGroupのRect取得
        private static (double x, double y, double w, double h) GetRect(TTGroup? group)
        {
            double x = double.MaxValue, y = double.MaxValue;
            double w = 0, h = 0;
            if (group != null)
            {
                foreach (var item in group.Children)
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

        //起動直後にサイズ更新実行、
        //これでXamlで子要素が置かれていても親要素のサイズが決定される
        //デザイン画面でも反映される
        private void TTGroup_Loaded(object sender, RoutedEventArgs e)
        {
            TTParentGroup?.UpdateTTGroupLayout();
            //ReSize(TTParentGroup);
        }

        private void TTGroup_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is TThumb tt)
            {
                var neko = tt.DesiredSize;
                var inu = tt.ActualHeight;
                var uma = tt.ActualWidth;
                var origin = e.OriginalSource;
                var source = e.Source;
            }
        }


        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    //子要素追加時、子要素のParentプロパティに自身を入力しておく
                    if (e.NewItems?[0] is TThumb thumb)
                    {
                        thumb.TTParentGroup = this;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
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

    //public class ExCanvas : Canvas
    //{
    //    //protected override Size MeasureOverride(Size constraint)
    //    //{
    //    //    base.MeasureOverride(constraint);
    //    //    double x = double.MaxValue, y = double.MaxValue;
    //    //    double w = 0, h = 0;
    //    //    foreach (var item in InternalChildren.OfType<TThumb>())
    //    //    {
    //    //        var left = item.MyLeft; if (x > left) x = left;
    //    //        var top = item.MyTop; if (y > top) y = top;
    //    //        var width = left + item.DesiredSize.Width;
    //    //        var height = top + item.DesiredSize.Height;
    //    //        if (w < width) w = width;
    //    //        if (h < height) h = height;
    //    //    }

    //    //    //if (InternalChildren[0] is TThumb tt && tt.TTParentGroup is TTGroup ttg)
    //    //    //{
    //    //    //    tt.ReSize(ttg);
    //    //    //    //ttg.Measure(new());
    //    //    //}
    //    //    return new Size(w - x, h - y);
    //    //    //return base.MeasureOverride(constraint);
    //    //}
    //}
}
