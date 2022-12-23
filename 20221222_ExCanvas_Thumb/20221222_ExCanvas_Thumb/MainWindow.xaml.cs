//using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace _20221222_ExCanvas_Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //MyExCanvas.Children.Add(AddTest(2000));//多少カクつく
            //MyExCanvas.Children.Add(AddTest(200));//全く問題ない
            //MyExCanvas1.SizeChanged += MyExCanvas1_SizeChanged;
            //for (int i = 0; i < 4; i++)
            //{
            //    var ex = MakeExCanvasWithThumb(30);
            //    Canvas.SetLeft(ex, i * 100); Canvas.SetTop(ex, i * 100);
            //    MyExCanvas1.Children.Add(ex);
            //}

        }
        private Thumb MakeThumb(double left, double top)
        {
            Thumb t = new() { Width = 100, Height = 30 };
            Canvas.SetLeft(t, left); Canvas.SetTop(t, top);
            t.DragDelta += Thumb_DragDelta;
            return t;
        }


        private ExCanvas MakeExCanvasWithThumb(int childrenCount)
        {
            ExCanvas ex = new() { Background = Brushes.MediumAquamarine, Opacity = 0.5 };
            for (int i = 0; i < childrenCount; i++)
            {
                ex.Children.Add(MakeThumb(i, i));
            }
            return ex;
        }
        private ExCanvas AddTest(int count)
        {
            ExCanvas ex = new() { Background = Brushes.Gold };
            for (int i = 0; i < count; i++)
            {
                ex.Children.Add(MakeThumb(i, i));
            }
            return ex;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb t)
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
                Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //MyExCanvas2.Measure(new System.Windows.Size(300, 200));
            var left = Canvas.GetLeft(MyExCanvas2);
            MyThumb3.Width = 500;
        }

    }

    /// <summary>
    /// オートサイズのCanvas、子要素のサイズや位置の変更時にサイズ更新
    /// 入れ子になったExCanvasもオートサイズ
    /// </summary>
    public class ExCanvas : Canvas
    {
        public ExCanvas() : base()
        {

        }

        //protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        //{
        //    base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        //}
        public override string ToString()
        {
            //return base.ToString();
            return Name;
        }
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (double.IsNaN(Width) && double.IsNaN(Height))
            {
                base.ArrangeOverride(arrangeSize);
                Size size = new();
                foreach (var item in InternalChildren.OfType<FrameworkElement>())
                {
                    double x = GetLeft(item) + item.ActualWidth;
                    double y = GetTop(item) + item.ActualHeight;
                    if (size.Width < x) size.Width = x;
                    if (size.Height < y) size.Height = y;
                }
                //親ExCanvasのMeasureを実行、これで入れ子状態でもすべてのExCanvasのサイズが更新される
                if(Parent is ExCanvas ex) { ex.Measure(size); }
                return size;
            }
            else
            {
                return base.ArrangeOverride(arrangeSize);
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
            //if (double.IsNaN(Width) && double.IsNaN(Height))
            //{
            //    base.MeasureOverride(constraint);
            //    System.Windows.Size size = new();
            //    foreach (var item in Children.OfType<FrameworkElement>())
            //    {
            //        var neko = item.DesiredSize.Width;
            //        var left = GetLeft(item);
            //        var top = GetTop(item);
            //        double x = GetLeft(item) + item.ActualWidth;
            //        double y = GetTop(item) + item.ActualHeight;
            //        if (size.Width < x) size.Width = x;
            //        if (size.Height < y) size.Height = y;
            //    }
            //    return size;
            //}
            //else { return base.MeasureOverride(constraint); }

        }
        //protected override void OnChildDesiredSizeChanged(UIElement child)
        //{
        //    base.OnChildDesiredSizeChanged(child);
        //}
        //protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        //{
        //    base.OnRenderSizeChanged(sizeInfo);
        //    var neko = sizeInfo.NewSize;
        //    var wc = sizeInfo.WidthChanged;
        //    var hc = sizeInfo.HeightChanged;
        //    //Measure(sizeInfo.NewSize);
        //    //UpdateLayout();
        //    //Arrange(new(sizeInfo.NewSize));
        //    if (Parent is FrameworkElement parent)
        //    {
        //        parent.Measure(sizeInfo.NewSize);//left、topがNaN以外ならおk、NaNの場合は変化なし
        //        //parent.Arrange(new(sizeInfo.NewSize));//left、topがNaN以外ならおk、NaNの場合は子要素につられて移動してしまう
        //        //parent.UpdateLayout();//変化なし
        //    }
        //}
     
    }
}
