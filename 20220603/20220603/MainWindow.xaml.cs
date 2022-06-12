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
        //private PolyBezierCanvas MyPolyBezierCanvas;
        private PolyBezierCanvas2 MyPolyBezierCanvas2 { get; set; }

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


            //MyPolyBezierCanvas = new(Brushes.Red, 10);
            //MyCanvas.Children.Add(MyPolyBezierCanvas);
            //MyPolyBezierCanvas.AddPoint(new Point(100, 100));
            //MyPolyBezierCanvas.AddPoint(new Point(150, 100));

            MyPolyBezierCanvas2 = new(new Point(100, 100), new Point(200, 200));
            MyCanvas.Children.Add(MyPolyBezierCanvas2);
            MyPolyBezierCanvas2.AddAnchorPoint(new Point(300, 150));
            DataContext = MyPolyBezierCanvas2;

        }


        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            //MyPolyLineCanvas.AddPoint(new Point(300, 200));
            MyPolyBezierCanvas2.AddAnchorPoint(new Point(200, 30));
            MyPolyBezierCanvas2.AddAnchorPoint(new Point(300, 300));
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            //MyPolyLineCanvas.RemovePoint();
            //MyThumbsAndPoints.RemovePoint();
            //MyPolyBezierCanvas2.RemoveAnchorPoint(1);
            MyPolyBezierCanvas2.RemoveAnchorPoint(MyListBox.SelectedIndex);
        }


    }

    public class PolyBezierCanvas2 : Canvas
    {
        
        private readonly Path MyBezierPath;//ベジェ曲線表示用
        //BezierSegmentのPoints
        public PointCollection MyBezierPoints { get; } = new();
        //StartPointを持つFigure
        public PathFigure MyBezierFigure { get; }
        //Path                  MyBezierPath    ベジェ曲線表示用
        //┗PathGeometry
        // ┗PathFigures
        //  ┗PathFigure         MyBezierFigure  StartPointの指定で使う
        //   ┗PointCollection   MyBezierPoints  StartPoint以外の全てのPoint
        
        //アンカー点管理用Collection
        public ObservableCollection<Point> MyAnchorPoints { get; } = new();
        //ベジェ曲線の頂点はアンカー点と制御点の2種類で構成されている
        //頂点の追加や削除はアンカー点を基準に行い、制御点だけを追加削除することはしない
        //アンカー点追加時は同時に制御点2つを追加
        //アンカー点削除時は同時に制御点2つを削除

        public PolyBezierCanvas2(Point anchor0, Point anchor1)
        {
            MyAnchorPoints.CollectionChanged += MyAnchorPoints_CollectionChanged;
            MyBezierPath = new() { Stroke = Brushes.Red, StrokeThickness = 10 };
            Children.Add(MyBezierPath);

            PolyBezierSegment seg = new(); seg.Points = MyBezierPoints;
            MyBezierFigure = new(); MyBezierFigure.Segments.Add(seg);
            PathGeometry geo = new(); geo.Figures.Add(MyBezierFigure);
            MyBezierPath.Data = geo;

            MyBezierFigure.StartPoint = anchor0;
            MyAnchorPoints.Add(anchor0);
            AddAnchorPoint(anchor1);
        }

        //アンカー点の追加、削除時
        private void MyAnchorPoints_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //追加時は制御点も2点追加する
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?[0] is not Point p) { return; }
                if (MyAnchorPoints.Count > 1)
                {
                    MyBezierPoints.Add(p);//制御点
                    MyBezierPoints.Add(p);//制御点
                    MyBezierPoints.Add(p);//アンカー点
                }

            }
            //削除時、アンカー点に付随する制御点2点削除する
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //削除されたアンカー点のIndex
                int pi = e.OldStartingIndex;
                //BezierSegmentのPointsから削除する点のIndex
                int api = pi * 3 - 2;
                //削除したアンカー点が始点だった場合
                if (pi == 0)
                {
                    //FigureのstartPointの入れ替え(前に詰める)
                    MyBezierFigure.StartPoint = MyAnchorPoints[0];
                    api = 0;
                }
                //削除したアンカー点が終点だった場合
                else if (pi == MyAnchorPoints.Count)
                {
                    api = pi * 3 - 3;
                }
                //削除
                //Collectionからの削除は削除はだるま落としなので同じIndex指定
                MyBezierPoints.RemoveAt(api);
                MyBezierPoints.RemoveAt(api);
                MyBezierPoints.RemoveAt(api);
            }
        }


        public void AddAnchorPoint(Point p)
        {
            MyAnchorPoints.Add(p);
        }

        public void RemoveAnchorPoint(int pi)
        {
            //無効なIndexなら削除しない
            if (0 > pi || pi > MyAnchorPoints.Count) { return; }
            //アンカー点が2個なら削除しない(2個以上を保つ)
            if (MyAnchorPoints.Count == 2) { return; }

            MyAnchorPoints.RemoveAt(pi);
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

        public readonly Path MyDirectionLinePath;//方向線表示用Path
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


            MyDirectionLinePath = new();
            MyDirectionLinePath.Stroke = Brushes.RoyalBlue;
            MyDirectionLinePath.StrokeThickness = 1;
            this.Children.Add(MyDirectionLinePath);
            MyDirectionFigureCollection = new PathFigureCollection();
            geo = new(); geo.Figures = MyDirectionFigureCollection;
            MyDirectionLinePath.Data = geo;

        }


        //アンカー点を追加
        public void AddPoint(Point p)
        {
            //追加アンカー点が始点だった場合
            if (MyStartPointThumb == null)
            {
                //スタートポイントだけ追加、Thumbは専用ドラッグ移動イベント
                MyBezierFigure.StartPoint = p;
                Thumb t = MakeThumb(p);
                t.DragDelta += StartPointThumb_DragDelta;//専用
                MyStartPointThumb = t;
                this.Children.Add(t);
            }
            //始点以外の場合は追加と同時に制御点、方向線も追加とThumbも追加
            else
            {
                //制御点2つとアンカー点の合計3点追加とそのThumb
                for (int i = 0; i < 3; i++)
                {
                    Thumb t = MakeThumb(p);
                    t.DragDelta += Thumb_DragDelta;
                    MyPoints.Add(p);
                    MyThumbs.Add(t);
                    this.Children.Add(t);
                }

                //方向線用LineSegments2個追加
                //方向線のスタートポイントを決定
                //1個め
                Point start;
                //一個前のアンカー点が始点だった場合はBezier自体のスタートポイントと同じ
                if (MyPoints.Count == 3)
                {
                    start = MyBezierFigure.StartPoint;
                }
                //それ以外の場合は、一個前のアンカー点をスタートポイントに指定
                else
                {
                    //同じ↓start = MyPoints[MyPoints.Count - 4];
                    start = MyPoints[^4];
                }
                MyDirectionFigureCollection.Add(MakeLineFigure(start, p));

                //2個め
                MyDirectionFigureCollection.Add(MakeLineFigure(p, p));
            }

        }
        private PathFigure MakeLineFigure(Point start, Point p)
        {
            PathFigure fig = new();
            fig.StartPoint = start;
            fig.Segments.Add(new LineSegment(p, true));
            return fig;

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
            //始点を削除する場合は次のアンカーを始点にする
            if (MyCurrentThumb == MyStartPointThumb)
            {
                MyBezierFigure.StartPoint = MyPoints[0];
                MyPoints.RemoveAt(0);
                MyBezierFigure.Segments.RemoveAt(0);

            }
            else
            {
                //Thumbとベジェ曲線のPoint削除
                int i = MyThumbs.IndexOf(MyCurrentThumb);
                MyPoints.RemoveAt(i);
                MyThumbs.Remove(MyCurrentThumb);
                this.Children.Remove(MyCurrentThumb);

            }
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

