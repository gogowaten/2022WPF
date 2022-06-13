using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
//WPF、PolyLineの頂点にThumb表示、マウスドラッグで頂点移動、その2 - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2022/06/13/115158

namespace _20220610_PolyLineとThumb
{
    public partial class MainWindow : Window
    {
        private PolyLineCanvas MyPolyLineCanvas;
        private PolyLineCanvas MyPolyLineCanvas2;
        private PolyLineCanvas? MyKatori;

        public MainWindow()
        {
#if DEBUG
            Left = 100; Top = 100;
#endif
            InitializeComponent();

            MyPolyLineCanvas = new(Brushes.Crimson, 10);
            MyPolyLineCanvas.AddPoint(new Point(20, 20));
            MyPolyLineCanvas.AddPoint(new Point(120, 20));
            MyCanvas.Children.Add(MyPolyLineCanvas);

            MyPolyLineCanvas2 = new(Brushes.DarkMagenta, 10);
            MyPolyLineCanvas2.AddPoint(new Point(120, 120));
            MyPolyLineCanvas2.AddPoint(new Point(220, 20));
            MyCanvas.Children.Add(MyPolyLineCanvas2);

        }


        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.AddPoint(new Point(300, 200));
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.RemovePoint();
        }

        private void MyButton3_Click(object sender, RoutedEventArgs e)
        {
            AddPoint蚊取り線香(10000, Brushes.DarkGreen, 5, 20);
        }
        private void AddPoint蚊取り線香(int count, Brush brush, int syuukai, double width)
        {
            MyKatori = new(brush, width);
            double r = 200; double s = 360.0 * syuukai / count;
            double rr = r / (count * 1.0); double rrr = r;
            double x; double y;
            for (double i = 0.0; i < 360.0 * syuukai; i += s)
            {
                double rad = Radian(i);
                x = (Math.Cos(rad) * rrr) + r;
                y = (Math.Sin(rad) * rrr) + r;
                MyKatori.AddPoint(new Point(x, y));
                rrr -= rr;
            }
            MyCanvas.Children.Add(MyKatori);
        }

        public static double Radian(double degrees)
        {
            return Math.PI / 180.0 * degrees;
        }

        private void MyButton4_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.ChangeVisibleThumb();
            MyPolyLineCanvas2.ChangeVisibleThumb();
            MyKatori?.ChangeVisibleThumb();
        }

    }

    /// <summary>
    /// PolyLineとその各頂点にドラッグ移動できるThumbの表示するCanvas
    /// 頂点とThumbは連動している、連動動作は追加、削除、移動
    /// 削除処理の対象は最後にクリックしたThumbに対応する頂点
    /// </summary>
    public class PolyLineCanvas : Canvas
    {
        public readonly List<TThumb> MyThumbs = new();
        public readonly PointCollection MyPoints = new();
        public readonly Polyline MyPolyline;
        //クリックしたThumb
        public TThumb? MyCurrentThumb { get; private set; }
        public bool IsThumbVisible = true;
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
            TThumb t = new() { Width = 20, Height = 20 };
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
        public void ChangeVisibleThumb()
        {
            if (IsThumbVisible)
            {
                for (int i = 1; i < MyThumbs.Count; i++)
                {
                    MyThumbs[i].Visibility = Visibility.Collapsed;
                }
                IsThumbVisible = false;
            }
            else
            {
                for (int i = 1; i < MyThumbs.Count; i++)
                {
                    MyThumbs[i].Visibility = Visibility.Visible;
                }
                IsThumbVisible = true;
            }
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MyCurrentThumb = sender as TThumb;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not TThumb t) { return; }
            double x = GetLeft(t) + e.HorizontalChange;
            double y = GetTop(t) + e.VerticalChange;
            Point p = new(x, y);
            int i = MyThumbs.IndexOf(t);
            MyPoints[i] = p;
            SetLeft(t, x); SetTop(t, y);
        }

    }

    public class TThumb : Thumb
    {
        public TThumb()
        {
            this.Template = MakeTemplate();
        }
        private ControlTemplate MakeTemplate()
        {
            FrameworkElementFactory elementF = new(typeof(Rectangle));
            elementF.SetValue(Rectangle.FillProperty, Brushes.Transparent);
            elementF.SetValue(Rectangle.StrokeProperty, Brushes.Black);
            elementF.SetValue(Rectangle.StrokeDashArrayProperty,
                new DoubleCollection() { 2.0 });
            ControlTemplate template = new(typeof(Thumb));
            template.VisualTree = elementF;
            return template;
        }
    }

}