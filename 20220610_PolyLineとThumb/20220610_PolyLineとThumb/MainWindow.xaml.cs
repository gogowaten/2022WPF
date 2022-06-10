using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace _20220610_PolyLineとThumb
{
    public partial class MainWindow : Window
    {
        private PolyLineCanvas MyPolyLineCanvas;

        public MainWindow()
        {
#if DEBUG
            Left = 100; Top = 100;
#endif
            InitializeComponent();

            MyPolyLineCanvas = new(Brushes.ForestGreen, 20);
            MyPolyLineCanvas.AddPoint(new Point(20, 20));
            MyPolyLineCanvas.AddPoint(new Point(120, 20));
            MyCanvas.Children.Add(MyPolyLineCanvas);
            //MyGrid.Children.Add(MyPolyLineCanvas);
        }


        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.AddPoint(new Point(300, 200));
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.RemovePoint();
        }


    }

    /// <summary>
    /// PolyLineとその各頂点にドラッグ移動できるThumbの表示するCanvas
    /// 頂点とThumbは連動している、連動動作は追加、削除、移動
    /// 削除処理の対象は最後にクリックしたThumbに対応する頂点
    /// </summary>
    public class PolyLineCanvas : Canvas
    {
        public readonly List<Thumb> MyThumbs = new();
        public readonly PointCollection MyPoints = new();
        public readonly Polyline MyPolyline;
        //クリックしたThumb
        public Thumb? MyCurrentThumb { get; private set; }
        public PolyLineCanvas(Brush stroke, double thickness)
        {
            MyPolyline = new()
            {
                Stroke = stroke,
                StrokeThickness = thickness,
                Points = MyPoints
            };
            this.Children.Add(MyPolyline);
        }

        public void AddPoint(Point p)
        {
            Thumb t = new() { Width = 20, Height = 20 };
            t.DragDelta += Thumb_DragDelta;
            t.PreviewMouseDown += Thumb_PreviewMouseDown;
            SetLeft(t, p.X); SetTop(t, p.Y);
            MyPoints.Add(p);
            MyThumbs.Add(t);
            this.Children.Add(t);
        }

        public void RemovePoint()
        {
            if (MyCurrentThumb is null) { return; }
            int i = MyThumbs.IndexOf(MyCurrentThumb);
            MyPoints.RemoveAt(i);
            MyThumbs.Remove(MyCurrentThumb);
            this.Children.Remove(MyCurrentThumb);
            MyCurrentThumb = null;
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MyCurrentThumb = sender as Thumb;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            double x = GetLeft(t) + e.HorizontalChange;
            double y = GetTop(t) + e.VerticalChange;
            Point p = new(x, y);
            int i = MyThumbs.IndexOf(t);
            MyPoints[i] = p;
            SetLeft(t, x); SetTop(t, y);
        }
    }


}