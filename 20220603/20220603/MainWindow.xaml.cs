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

        public PointCollection MyPointC { get; set; } = new() { new(100, 100), new(200, 100), new(200, 200) };
        //public ObservableCollection<Thumb> MyThumbs { get; set; } = new();
        private Polyline MyPolyLine;
        private ThumbsAndPoints MyThumbsAndPoints;
        private PolyLineCanvas MyPolyLineCanvas;

        public MainWindow()
        {
#if DEBUG
            Left = 10; Top = 10;
#endif
            InitializeComponent();

            //MyThumbsAndPoints = new(MyCanvas);
            //MyPolyLine = new() { Stroke = Brushes.Magenta, StrokeThickness = 4, Points = MyThumbsAndPoints.MyPoints };
            //MyCanvas.Children.Add(MyPolyLine);
            //MyThumbsAndPoints.AddPoint(new Point(20, 20));
            //MyThumbsAndPoints.AddPoint(new Point(100, 20));

            MyPolyLineCanvas = new(Brushes.OrangeRed, 4);
            MyPolyLineCanvas.AddPoint(new Point(20, 20));
            MyPolyLineCanvas.AddPoint(new Point(120, 20));
            //MyGrid.Children.Add(MyPolyLineCanvas);
            MyCanvas.Children.Add(MyPolyLineCanvas);
            DataContext = MyPolyLineCanvas.MyCurrentThumb;

        }


        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.AddPoint(new Point(300, 200));
            //MyThumbsAndPoints.AddPoint(new Point(300, 200));

        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.RemovePoint();
            //MyThumbsAndPoints.RemovePoint();
        }


    }

    public class PolyLineCanvas : Canvas
    {
        private List<Thumb> MyThumbs = new();
        private PointCollection MyPoints = new();
        public Thumb? MyCurrentThumb { get; private set; }
        public Polyline MyPolyline;
        public PolyLineCanvas(Brush stroke, double thickness)
        {
            MyPolyline = new() { Stroke = stroke, StrokeThickness = thickness, Points = MyPoints };
            this.Children.Add(MyPolyline);
        }

        public void AddPoint(Point p)
        {
            Thumb t = new() { Width = 20, Height = 20 };
            t.DragDelta += Thumb_DragDelta;
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
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            double x = Canvas.GetLeft(t) + e.HorizontalChange;
            double y = Canvas.GetTop(t) + e.VerticalChange;
            Point p = new(x, y);
            int i = MyThumbs.IndexOf(t);
            MyPoints[i] = p;
            Canvas.SetLeft(t, x); Canvas.SetTop(t, y);

        }
    }

    public class ThumbsAndPoints
    {
        public List<TThumb> MyThumbs = new();
        public PointCollection MyPoints = new();
        public Canvas MyCanvas;
        public TThumb? MyCurrentThumb;
        public ThumbsAndPoints(Canvas canvas)
        {
            MyCanvas = canvas;
        }
        public void AddPoint(Point p)
        {
            TThumb t = new(p.X, p.Y, MyPoints.Count);
            t.DragDelta += Thumb_DragDelta;
            t.PreviewMouseDown += Thumb_PreviewMouseDown;
            MyPoints.Add(p);
            MyThumbs.Add(t);
            MyCanvas.Children.Add(t);
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MyCurrentThumb = sender as TThumb;
        }

        public void RemovePoint()
        {
            if (MyCurrentThumb is null) { return; }
            int i = MyThumbs.IndexOf(MyCurrentThumb);
            MyPoints.RemoveAt(i);
            MyThumbs.Remove(MyCurrentThumb);
            MyCanvas.Children.Remove(MyCurrentThumb);
            MyCurrentThumb = null;
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not TThumb t) { return; }
            double x = Canvas.GetLeft(t) + e.HorizontalChange;
            double y = Canvas.GetTop(t) + e.VerticalChange;
            Point p = new(x, y);
            int i = MyThumbs.IndexOf(t);
            MyPoints[i] = p;
            Canvas.SetLeft(t, x); Canvas.SetTop(t, y);

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
    public class Matome
    {
        public List<TThumb> ThumbList;
        public PointCollection PointCollection;
        public List<int> IntList;
        public List<Point> PointList;

        public Matome(List<TThumb> thumbList, PointCollection pointCollection, List<int> intList, List<Point> pointList)
        {
            ThumbList = thumbList;
            PointCollection = pointCollection;
            IntList = intList;
            PointList = pointList;
            PointCollection.Changed += PointCollection_Changed;
        }

        private void PointCollection_Changed(object? sender, EventArgs e)
        {
            var neko = 0;
        }
    }
    public class NotifyXY : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private Point _point;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double X { get => _x; set { if (_x == value) return; _x = value; Point = new Point(_x, _y); OnPropertyChanged(); } }
        public double Y { get => _y; set { if (_y == value) return; _y = value; Point = new Point(_x, _y); OnPropertyChanged(); } }
        public Point Point
        {
            get => _point; set
            {
                if (_point == value) { return; }
                _point = value;
                //_x = value.X;_y= value.Y;
                OnPropertyChanged();
            }
        }
        public NotifyXY(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public class ObservablePoints : INotifyPropertyChanged
    {
        public ObservableCollection<NotifyXY> Vertices { get; set; } = new();
        public ObservablePoints()
        {
            Vertices.CollectionChanged += Vertices_CollectionChanged;
        }

        private void Vertices_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
                var neko = 0;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;


        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Vertices));
        }


        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
    public class Data
    {
        public List<NotifyXY> NPoints { get; set; }
        public PointCollection PC { get; set; }
        public Data(List<NotifyXY> nPoints, PointCollection pC)
        {
            NPoints = nPoints;
            PC = pC;
        }
    }


}

