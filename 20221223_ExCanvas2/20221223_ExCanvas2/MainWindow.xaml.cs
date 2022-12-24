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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _20221223_ExCanvas2
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
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is Thumb thumb && thumb.Parent is ExCanvas parent)
            {
                //double minX = double.MaxValue;
                //double minY = double.MaxValue;
                //foreach (var item in parent.Children.OfType<Thumb>())
                //{
                //    double x = Canvas.GetLeft(item);
                //    double y = Canvas.GetTop(item);
                //    if (minX > x) minX = x;
                //    if (minY > y) minY = y;
                //}
                //foreach (var item in parent.Children.OfType<Thumb>())
                //{
                //    Canvas.SetLeft(item, Canvas.GetLeft(item) - minX);
                //    Canvas.SetTop(item, Canvas.GetTop(item) - minY);
                //}
                //Canvas.SetLeft(parent, Canvas.GetLeft(parent) + minX);
                //Canvas.SetTop(parent, Canvas.GetTop(parent) + minY);

                //parent.Measure(new Size(double.NaN,double.NaN));//エラー
                //parent.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));//実行されない？
                //parent.InvalidateMeasure();//微妙に違う結果になる
                parent.Measure(new Size(0, 0));//これがいい
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MyThumb11.Width = 500;
        }

    }

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

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //arrangeSzie、子要素群表示に必要なサイズ
            return base.ArrangeOverride(arrangeSize);
        }
        protected override Size MeasureOverride(Size constraint)
        {
            //constraint、上限サイズ
            if (double.IsNaN(Width) && double.IsNaN(Height))
            {
                base.MeasureOverride(constraint);
                Size size = new();
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                foreach (var item in InternalChildren.OfType<FrameworkElement>())
                {
                    double x = GetLeft(item);
                    double y = GetTop(item);
                    if (minX > x) minX = x;
                    if (minY > y) minY = y;
                    double w = GetLeft(item) + item.DesiredSize.Width;
                    double h = GetTop(item) + item.DesiredSize.Height;
                    if (size.Width < w) size.Width = w;
                    if (size.Height < h) size.Height = h;
                }
                foreach (var item in InternalChildren.OfType<FrameworkElement>())
                {
                    SetLeft(item, GetLeft(item) - minX);
                    SetTop(item, GetTop(item) - minY);
                }
                SetLeft(this, GetLeft(this) + minX);
                SetTop(this, GetTop(this) + minY);

                return size;
            }
            else
            {
                return base.MeasureOverride(constraint);
            }

            //return base.MeasureOverride(constraint);
        }

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    if (double.IsNaN(Width) && double.IsNaN(Height))
        //    {
        //        base.MeasureOverride(constraint);
        //        Size size = new();
        //        foreach (var item in InternalChildren.OfType<FrameworkElement>())
        //        {
        //            double w = GetLeft(item) + item.DesiredSize.Width;
        //            double h = GetTop(item) + item.DesiredSize.Height;
        //            if (size.Width < w) size.Width = w;
        //            if (size.Height < h) size.Height = h;
        //        }
        //        return size;
        //    }
        //    else
        //    {
        //        return base.MeasureOverride(constraint);
        //    }
        //}

    }


}
