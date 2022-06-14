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

        private TThumb MyTThumb;
        private TThumb2 MyTThumb2;
        private Dictionary<string, Brush> MyBrushes = new();
        public MainWindow()
        {
#if DEBUG
            Left = 100; Top = 100;
#endif
            InitializeComponent();
            InitializeComboBox();

            MyTThumb = new("test");
            MyCanvas.Children.Add(MyTThumb);
            DataContext = MyTThumb;
            MyTThumb.DragDelta += MyThumb_DragDelta;

            MyTThumb.FontSize = 50;
            MyTThumb.MyTextBox.BorderThickness = new Thickness(1);
            MyTThumb.MyTextBox.AcceptsReturn = true;
            MyTThumb.MyTextBox.BorderBrush = Brushes.Gold;
            MyTextBox.DataContext = MyTThumb;

            MyTThumb2 = new TThumb2();
            MyCanvas.Children.Add(MyTThumb2);
            MyGroupBox.DataContext = MyTThumb2.MyTextBox;
            MyTThumb2.MyTextBox.Text = "LEVEL UP";
            MyTThumb2.MyTextBox.FontSize = 30;
            MyTThumb2.MyTextBox.Foreground = Brushes.Khaki;
            MyTThumb2.MyTextBox.Background = Brushes.Olive;
            MyTThumb2.MyTextBox.BorderBrush = Brushes.Crimson;
            MyTThumb2.MyTextBox.BorderThickness = new Thickness(2);
            MyTThumb2.MyTextBox.AcceptsReturn = true;
            MyTThumb2.MyTextBox.TextAlignment = TextAlignment.Center;
            MyTThumb2.DragDelta += MyTThumb2_DragDelta;



        }
        private static Dictionary<string, Brush> MakeBrushesDictionary()
        {
            var brushInfos = typeof(Brushes).GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static);

            Dictionary<string, Brush> dict = new();
            foreach (var item in brushInfos)
            {
                if (item.GetValue(null) is not Brush bu)
                {
                    continue;
                }
                dict.Add(item.Name, bu);
            }
            return dict;
        }
        private void InitializeComboBox()
        {
            Dictionary<string, Brush> dict = MakeBrushesDictionary();
            MyComboBoxFore.ItemsSource = dict;
            MyComboBoxBack.ItemsSource = dict;
        }
        private void MyTThumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not FrameworkElement element) { return; }
            Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
            Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
        }


        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not TThumb t) { return; }
            Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
            Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(MyTThumb, 50);
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {

        }


    }

    public class TThumb : Thumb, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string? _text;
        public string? MyText
        {
            get => _text;
            set { if (value == _text) { return; } _text = value; OnPropertyChanged(); }
        }

        private const string COVER_NAME = "cover";
        public Grid MyCoverGrid;

        private const string TEXTBOX_NAME = "textbox";
        public TextBox MyTextBox { get; private set; }

        public TThumb(string text)
        {
            this.Template = MakeTemplate();
            MyText = text;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            ApplyTemplate();
            MyCoverGrid = (Grid)Template.FindName(COVER_NAME, this);
            MyTextBox = (TextBox)Template.FindName(TEXTBOX_NAME, this);
            this.MouseDoubleClick += TThumb_MouseDoubleClick;
        }
        public void ChangeMoveEnabled()
        {
            if (MyCoverGrid.Background == null)
            {
                MyCoverGrid.Background = Brushes.Transparent;
                Keyboard.ClearFocus();
            }
            else
            {
                MyCoverGrid.Background = null;
            }
        }
        private void TThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChangeMoveEnabled();
        }

        private ControlTemplate MakeTemplate()
        {
            FrameworkElementFactory coverF = new(typeof(Grid), COVER_NAME);//蓋
            FrameworkElementFactory textF = new(typeof(TextBox), TEXTBOX_NAME);
            FrameworkElementFactory underF = new(typeof(Grid));//ベース
            //TextBoxのTextとのBinding設定
            Binding b = new(nameof(MyText));
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textF.SetBinding(TextBox.TextProperty, b);
            textF.SetValue(TextBox.ForegroundProperty, Brushes.Crimson);
            textF.SetValue(TextBox.BackgroundProperty, Brushes.Lavender);
            //textF.SetValue(TextBox.AcceptsReturnProperty, true);
            //透明色の蓋
            coverF.SetValue(Panel.BackgroundProperty, Brushes.Transparent);
            //ベースになるGridにTextBox、蓋になるGridを順番に追加
            underF.AppendChild(textF);
            underF.AppendChild(coverF);
            //テンプレート作成、VisualTreeにベースを指定
            ControlTemplate template = new();
            template.VisualTree = underF;
            return template;
        }
    }

    public class TThumb2 : Thumb
    {
        private const string COVER_NAME = "cover";
        public Grid MyCoverGrid;

        private const string TEXTBOX_NAME = "textbox";
        public TextBox MyTextBox { get; private set; }

        public TThumb2()
        {
            this.Template = MakeTemplate();
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            ApplyTemplate();
            MyCoverGrid = (Grid)Template.FindName(COVER_NAME, this);
            MyTextBox = (TextBox)Template.FindName(TEXTBOX_NAME, this);
            this.MouseDoubleClick += TThumb_MouseDoubleClick;
        }
        public void ChangeMoveEnabled()
        {
            if (MyCoverGrid.Background == null)
            {
                MyCoverGrid.Background = Brushes.Transparent;
                Keyboard.ClearFocus();
            }
            else
            {
                MyCoverGrid.Background = null;
            }
        }
        private void TThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChangeMoveEnabled();
        }

        private ControlTemplate MakeTemplate()
        {
            FrameworkElementFactory coverF = new(typeof(Grid), COVER_NAME);//蓋
            FrameworkElementFactory textF = new(typeof(TextBox), TEXTBOX_NAME);
            FrameworkElementFactory underF = new(typeof(Grid));//ベース

            //透明色の蓋
            coverF.SetValue(Panel.BackgroundProperty, Brushes.Transparent);
            //ベースになるGridにTextBox、蓋になるGridを順番に追加
            underF.AppendChild(textF);
            underF.AppendChild(coverF);
            //テンプレート作成、VisualTreeにベースを指定
            ControlTemplate template = new();
            template.VisualTree = underF;
            return template;
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





}

