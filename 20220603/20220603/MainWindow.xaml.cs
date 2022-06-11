using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

//PointCollectionの要素はPoint、これが頂点座標、座標を変更したいときはPoint自体を入れ替える
//xだけを変更したいときも新たにPointを作成して、元のPointと入れ替える必要があって
//Point.X=新しい値ってのはできない、エラーになる
//
namespace _20220603
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region notifyProperty
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion notifyProperty

        //public PointCollection MyPointC { get; set; } = new() { new(100, 100), new(200, 100), new(200, 200) };
        //private PolyLineCanvas MyPolyLineCanvas;
        private PolyBezierCanvas MyPolyBezierCanvas;

        public MainWindow()
        {
#if DEBUG
            Left = 100; Top = 100;
#endif
            InitializeComponent();


            PointCollection points = new() { new(10, 100), new(150, 100), new(200, 200), new(250, 100), new(300, 30), new(400, 100) };

            //MyPolyBezierCanvas = new(Brushes.Red, 10);
            //MyPolyBezierCanvas.AddPoint(new(100, 100));
            //MyPolyBezierCanvas.AddPoint(new(150, 100));
            //MyPolyBezierCanvas.AddPoint(new(200, 200));
            //MyCanvas.Children.Add(MyPolyBezierCanvas);

            //MyPolyBezierCanvas = new(Brushes.Red, 10, points, new Point(30, 30));
            MyPolyBezierCanvas = new(Brushes.Red, 10);
            MyCanvas.Children.Add(MyPolyBezierCanvas);
            MyPolyBezierCanvas.AddPoint(new Point(100, 100));
            MyPolyBezierCanvas.AddPoint(new Point(150, 100));

            //Path pt = new() { Stroke = Brushes.Gray, StrokeThickness = 3 };
            //PathGeometry geo = new();
            //PathFigure fig = new();
            //LineSegment seg = new();
            //seg.Point = new Point(100, 100);
            //fig.Segments.Add(seg); fig.StartPoint = new(10, 10);
            //geo.Figures.Add(fig);
            //seg = new(new Point(200, 20), true);
            //fig.Segments.Add(seg);
            //pt.Data = geo;
            //MyCanvas.Children.Add(pt);

            //LineSegment seg = new()
            //{
            //    Point = new Point(100, 100)
            //};
            //PathFigure fig = new(); fig.Segments.Add(seg); fig.StartPoint = new(10, 10);
            //PathFigureCollection figs = new()
            //{
            //    fig
            //}; 
            //PathGeometry geo = new()
            //{
            //    Figures = figs
            //};
            //seg = new(new Point(200, 20), true);
            //fig = new(); fig.Segments.Add(seg); fig.StartPoint = new(10, 50);
            //figs.Add(fig);

            //Path pt = new() { Stroke = Brushes.Gray, StrokeThickness = 3 };
            //pt.Data = geo;
            //MyCanvas.Children.Add(pt);


        }


        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            //MyPolyLineCanvas.AddPoint(new Point(300, 200));
            MyPolyBezierCanvas.AddPoint(new Point(200, 30));
            var neko = MyPolyBezierCanvas.MyBezierFigure.StartPoint;

        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            //MyPolyLineCanvas.RemovePoint();
            //MyThumbsAndPoints.RemovePoint();
        }


    }

    public class PolyBezierCanvas : Canvas
    {
        public readonly List<Thumb> MyThumbs = new();
        public readonly PointCollection MyPoints = new();//PolyBezierSegmentのPoint
        public readonly Path MyPolyBezierPath;//ベジェ曲線表示用Path
        //public Point MyStartPoint;
        public Thumb? MyStartPointThumb;//スタートポイント専用Thumb
        public PathFigure MyBezierFigure;//

        public readonly Path MyDirectionLine;//方向線表示用Path
        private PathFigureCollection MyDirectionFigureCollection;//方向線表示用PathのFigure
        //クリックしたThumb
        public Thumb? MyCurrentThumb { get; private set; }

        public PolyBezierCanvas(Brush stroke, double thickness)
        {
            PolyBezierSegment seg = new();
            seg.Points = MyPoints;
            MyBezierFigure = new();
            MyBezierFigure.Segments.Add(seg);
            //MyFigure.StartPoint = MyStartPoint;
            PathGeometry geo = new();
            geo.Figures.Add(MyBezierFigure);
            MyPolyBezierPath = new()
            {
                Stroke = stroke,
                StrokeThickness = thickness,
                Data = geo
            };
            this.Children.Add(MyPolyBezierPath);


            MyDirectionLine = new();
            MyDirectionLine.Stroke = Brushes.RoyalBlue;
            MyDirectionLine.StrokeThickness = 1;
            this.Children.Add(MyDirectionLine);
            MyDirectionFigureCollection = new PathFigureCollection();
            geo = new(); geo.Figures = MyDirectionFigureCollection;
            MyDirectionLine.Data = geo;

        }

        public PolyBezierCanvas(Brush stroke, double thickness, Point startPoint)
        {
            PolyBezierSegment seg = new();
            seg.Points = MyPoints;
            PathFigure fig = new(); fig.Segments.Add(seg); fig.StartPoint = startPoint;
            PathGeometry geo = new(); geo.Figures.Add(fig);
            MyPolyBezierPath = new()
            {
                Stroke = stroke,
                StrokeThickness = thickness,
                Data = geo
            };
            this.Children.Add(MyPolyBezierPath);
            MyDirectionLine = new();
            this.Children.Add(MyDirectionLine);

            MyDirectionLine.Data = geo;
        }
        public PolyBezierCanvas(Brush stroke, double thickness, PointCollection ps, Point startPoint) : this(stroke, thickness, startPoint)
        {
            for (int i = 0; i < ps.Count; i++)
            {
                AddPoint(ps[i]);
            }
        }

        public void AddPoint(Point p)
        {
            if (MyStartPointThumb != null)
            {
                //アンカー点を末尾に追加と同時に制御点も追加
                for (int i = 0; i < 3; i++)
                {
                    Thumb t = MakeThumb(p);
                    t.DragDelta += Thumb_DragDelta;
                    MyPoints.Add(p);
                    MyThumbs.Add(t);
                    this.Children.Add(t);

                }
                //方向線用Segments2個追加
                //1個め
                PathFigure fig = new();
                //方向線のスタートポイント
                //一個前のアンカー点が始点だった場合はBezier自体のスタートポイントと同じ
                if (MyPoints.Count == 3)
                {
                    fig.StartPoint = MyBezierFigure.StartPoint;
                }
                //それ以外の場合は、一個前のアンカー点をスタートポイントに指定
                else
                {
                    int id = MyPoints.Count - 4;
                    fig.StartPoint=MyPoints[id];
                }
                fig.Segments.Add(new LineSegment(p, true));
                MyDirectionFigureCollection.Add(fig);

                //2個め
                fig = new();
                fig.StartPoint = p;
                fig.Segments.Add(new LineSegment(p, true));
                MyDirectionFigureCollection.Add(fig);
            }
            //始点だった場合
            else
            {
                //スタートポイントだけ追加、専用ドラッグ移動イベント
                //MyStartPoint = p;
                MyBezierFigure.StartPoint = p;
                Thumb t = MakeThumb(p);
                t.DragDelta += StartPointThumb_DragDelta;//専用
                MyStartPointThumb = t;
                this.Children.Add(t);
            }

        }
        private Thumb MakeThumb(Point p)
        {
            Thumb t = new() { Width = 20, Height = 20 };
            t.PreviewMouseDown += Thumb_PreviewMouseDown;
            SetLeft(t, p.X); SetTop(t, p.Y);
            return t;
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

            //制御点の更新
            //制御点(終点側)だった場合
            int a = i % 3;
            if (a == 0)
            {
                int di = i / 3 * 2;
                LineSegment seg = (LineSegment)MyDirectionFigureCollection[di].Segments[0];
                seg.Point = p;
            }
            //制御点(始点側)だった場合
            else if (a == 1)
            {
                int di = i / 3 * 2 + 1;
                LineSegment seg = (LineSegment)MyDirectionFigureCollection[di].Segments[0];
                seg.Point = p;
            }
            //アンカー点だった場合、制御線の始点更新
            else if (a == 2)
            {
                //figureのIndex
                int di1 = i / 3 * 2 + 1;//始点側index
                int di2 = i / 3 * 2 + 2;//終点側
                //終点だった場合
                if (i == MyThumbs.Count - 1)
                {
                    MyDirectionFigureCollection[di1].StartPoint = p;
                }
                //中間点だった場合
                else
                {
                    MyDirectionFigureCollection[di1].StartPoint = p;
                    MyDirectionFigureCollection[di2].StartPoint = p;
                }

            }
        }
        //始点だった場合
        private void StartPointThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            double x = GetLeft(t) + e.HorizontalChange;
            double y = GetTop(t) + e.VerticalChange;
            Point p = new(x, y);
            //MyStartPoint = p;
            MyBezierFigure.StartPoint = p;
            SetLeft(t, x); SetTop(t, y);
            //方向線のstartPoint
            MyDirectionFigureCollection[0].StartPoint = p;
        }

    }



    public class PolyLineCanvas : Canvas
    {
        private List<Thumb> MyThumbs = new();
        private PointCollection MyPoints = new();
        public Thumb? MyCurrentThumb { get; private set; }
        public Polyline MyPolyline;
        private Polyline TempPolyline;
        private PointCollection TempPoints = new();
        public PolyLineCanvas(Brush stroke, double thickness)
        {
            MyPolyline = new() { Stroke = stroke, StrokeThickness = thickness, Points = MyPoints };
            this.Children.Add(MyPolyline);
            TempPolyline = new() { Stroke = stroke, StrokeThickness = thickness, Points = TempPoints };
            TempPolyline.Visibility = Visibility.Collapsed;
            this.Children.Add(TempPolyline);
        }

        public void AddPoint(Point p)
        {
            Thumb t = new() { Width = 20, Height = 20 };
            t.DragStarted += Thumb_DragStarted;
            t.DragDelta += Thumb_DragDelta;
            t.DragCompleted += Thumb_DragCompleted;
            t.PreviewMouseDown += Thumb_PreviewMouseDown;
            Canvas.SetLeft(t, p.X); Canvas.SetTop(t, p.Y);
            MyPoints.Add(p);
            MyThumbs.Add(t);
            this.Children.Add(t);
        }


        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MyCurrentThumb = sender as Thumb;
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
        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            if (MyThumbs.Count > 2)
            {
                foreach (var item in MyThumbs)
                {
                    item.Visibility = Visibility.Collapsed;
                }
                t.Visibility = Visibility.Visible;
                MyPolyline.Visibility = Visibility.Collapsed;
                MovePolyLine(MyThumbs.IndexOf(t));
                TempPolyline.Visibility = Visibility.Visible;
            }
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (MyThumbs.Count > 2)
            {
                foreach (var item in MyThumbs)
                {
                    item.Visibility = Visibility.Visible;
                }
                MyPolyline.Visibility = Visibility.Visible;
                TempPolyline.Visibility = Visibility.Collapsed;
                TempPolyline.Points.Clear();
            }
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            double x = Canvas.GetLeft(t) + e.HorizontalChange;
            double y = Canvas.GetTop(t) + e.VerticalChange;
            Point p = new(x, y);
            int i = MyThumbs.IndexOf(t);
            MyPoints[i] = p;
            Canvas.SetLeft(t, x); Canvas.SetTop(t, y);
            if (MyThumbs.Count > 2) { TempPoints[1] = p; }
            //else { TempPoints[0] = p; }
        }
        private void MovePolyLine(int tIndex)
        {
            TempPoints.Clear();
            int end = tIndex + 2;
            if (end > MyThumbs.Count) { end = MyThumbs.Count; }
            int begin = tIndex - 1;
            if (begin < 0) { begin = 0; }
            for (int i = begin; i < end; i++)
            {
                TempPoints.Add(MyPoints[i]);
            }

        }
    }



    public class TThumb : Thumb
    {
        public string MyText { get; set; }
        public int MyIndex { get; set; }
        //public NotifyXY MyNotifyXY { get; set; }


        public TThumb(double x, double y, int index)
        {
            //MyNotifyXY = new NotifyXY(x, y);
            MyText = "text";
            SetTemplate();
            DataContext = this;
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);
            MyIndex = index;
            //var neko = Canvas.GetLeft(this);
            //Binding b = new(nameof(MyNotifyXY.X));
            //b.Source = MyNotifyXY;
            //b.Mode = BindingMode.TwoWay;
            //this.SetBinding(Canvas.LeftProperty, b);
            //neko = Canvas.GetLeft(this);
            //b = new(nameof(MyNotifyXY.Y));
            //b.Source = MyNotifyXY;
            //b.Mode = BindingMode.TwoWay;
            //this.SetBinding(Canvas.TopProperty, b);
            //DragDelta += TThumb_DragDelta;
            //neko = Canvas.GetLeft(this);
        }

        //private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    if (sender is not TThumb tt) { return; }
        //    MyNotifyXY.X = Canvas.GetLeft(tt) + e.HorizontalChange;
        //    MyNotifyXY.Y = Canvas.GetTop(tt) + e.VerticalChange;
        //}

        private void SetTemplate()
        {
            FrameworkElementFactory panelFactory = new(typeof(Grid));
            FrameworkElementFactory textblockFactory = new(typeof(TextBlock));
            textblockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            textblockFactory.SetValue(TextBlock.PaddingProperty, new Thickness(4, 2, 4, 2));
            Binding b = new(nameof(MyIndex));
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textblockFactory.SetBinding(TextBlock.TextProperty, b);
            panelFactory.AppendChild(textblockFactory);
            panelFactory.SetValue(Panel.BackgroundProperty, Brushes.Blue);
            panelFactory.SetValue(OpacityProperty, 0.2);
            ControlTemplate template = new();
            template.VisualTree = panelFactory;
            this.Template = template;
            //ApplyTemplate();
        }
        public override string ToString()
        {
            return $"Name index{MyIndex}";
        }
    }


}

