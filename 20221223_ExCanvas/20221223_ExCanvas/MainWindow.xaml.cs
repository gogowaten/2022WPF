using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _20221223_ExCanvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb t)
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
                Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            }
        }
        private void Thumb_DragComplated(object sender, DragCompletedEventArgs e)
        {
            if (sender is Thumb thumb && thumb.Parent is ExCanvas canvas)
            {
                Size size = new();
                double xMin = double.MaxValue, yMin = double.MaxValue;
                foreach (var item in canvas.Children.OfType<FrameworkElement>())
                {
                    double x = Canvas.GetLeft(item);
                    double y = Canvas.GetTop(item);
                    double w = x + item.ActualWidth;
                    double h = y + item.ActualHeight;
                    if (xMin > x) xMin = x;
                    if (yMin > y) yMin = y;
                    if (size.Width < w) size.Width = w;
                    if (size.Height < h) size.Height = h;
                }
                size.Width -= xMin; size.Height -= yMin;
                double left = Canvas.GetLeft(canvas);
                double top = Canvas.GetTop(canvas);
                if (left < 0) { left = 0; }
                if (top < 0) { top = 0; }
                Canvas.SetLeft(canvas, left);
                Canvas.SetTop(canvas, top);

                foreach (var item in canvas.Children.OfType<FrameworkElement>())
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) - xMin);
                    Canvas.SetTop(item, Canvas.GetTop(item) - yMin);

                }
                canvas.Measure(size);//変化なし
                //canvas.Arrange(new(size));
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //MyExCanvas2.Measure(new System.Windows.Size(300, 200));
            //var left = Canvas.GetLeft(MyExCanvas2);
            MyThumb11.Width = 500;
        }

    }


    /// <summary>
    /// オートサイズのCanvas、子要素のサイズや位置の変更時にサイズ更新
    /// 入れ子になったExCanvasもオートサイズ
    /// </summary>
    public class ExCanvas : Canvas
    {
        public ExCanvas() : base() { }

        public override string ToString()
        {
            //return base.ToString();
            return Name;
        }
        //protected override Size ArrangeOverride(Size arrangeSize)
        //{
        //    if (double.IsNaN(Width) && double.IsNaN(Height))
        //    {
        //        base.ArrangeOverride(arrangeSize);
        //        Size size = new();
        //        double xMin = double.MaxValue, yMin = double.MaxValue;
        //        foreach (var item in InternalChildren.OfType<FrameworkElement>())
        //        {
        //            double x = GetLeft(item);
        //            double y = GetTop(item);
        //            double w = x + item.ActualWidth;
        //            double h = y + item.ActualHeight;
        //            if (xMin > x) xMin = x;
        //            if (yMin > y) yMin = y;
        //            if (size.Width < w) size.Width = w;
        //            if (size.Height < h) size.Height = h;
        //        }
        //        //foreach (var item in InternalChildren.OfType<FrameworkElement>())
        //        //{
        //        //    SetLeft(item, GetLeft(item) - xMin);
        //        //    SetTop(item, GetTop(item) - yMin);
        //        //}
        //        size.Width -= xMin; size.Height -= yMin;
        //        this.Measure(size);
        //        //SetLeft(this, GetLeft(this) - xMin);
        //        //SetTop(this, GetTop(this) - xMin);

        //        //親ExCanvasのMeasureを実行、これで入れ子状態でもすべてのExCanvasのサイズが更新される
        //        //if (Parent is ExCanvas ex) { ex.Measure(size); }
        //        return size;
        //    }
        //    else
        //    {
        //        return base.ArrangeOverride(arrangeSize);
        //    }
        //}

        protected override Size MeasureOverride(Size constraint)
        {
            //return base.MeasureOverride(constraint);
            if (double.IsNaN(Width) && double.IsNaN(Height))
            {
                base.MeasureOverride(constraint);
                Size size = new();
                //double xMin = double.MaxValue, yMin = double.MaxValue;
                foreach (var item in InternalChildren.OfType<FrameworkElement>())
                {
                    double w = GetLeft(item) + item.DesiredSize.Width;
                    double h = GetTop(item) + item.DesiredSize.Height;
                    if (size.Width < w) size.Width = w;
                    if (size.Height < h) size.Height = h;
                }
                //size.Width -= xMin; size.Height -= yMin;
                //if(Parent is ExCanvas ex) { ex.Measure(size); } 
                return size;
            }
            else
            {
                return base.MeasureOverride(constraint);
            }

        }
    }
}
