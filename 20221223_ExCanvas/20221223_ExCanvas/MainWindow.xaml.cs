using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


//exCanvasの位置は固定でThumb移動で一番上のExCanvasまでサイズ更新
//DragCompletedでMeasureするようにした
namespace _20221223_ExCanvas
{
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
            if (sender is Thumb thumb && thumb.Parent is ExCanvas canvas)
            {
                Size size = new();
                canvas.Measure(size);
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //MyExCanvas2.Measure(new System.Windows.Size(300, 200));
            //var left = Canvas.GetLeft(MyExCanvas2);
            MyThumb11.Width = 500;
        }

    }

    //Thumbの移動でマイナス座標になったのを修正したり、Canvasの座標を常に0に保ちたいときは
    //ArrangeOverrideだと移動中も更新されるのが災いする
    //なのでサイズ変更時のみ更新されるMeasureOverrideを使うようにした
    //座標修正はDragCompletedで行うのがいい？
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
            if (double.IsNaN(Width) && double.IsNaN(Height))
            {
                base.MeasureOverride(constraint);
                Size size = new();
                foreach (var item in InternalChildren.OfType<FrameworkElement>())
                {
                    double w = GetLeft(item) + item.DesiredSize.Width;
                    double h = GetTop(item) + item.DesiredSize.Height;
                    if (size.Width < w) size.Width = w;
                    if (size.Height < h) size.Height = h;
                }
                return size;
            }
            else
            {
                return base.MeasureOverride(constraint);
            }

        }
    }
}
