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

//失敗
//PointコレクションをPathにバインドしてみたけど、できないみたい？
namespace _20220607_PathとBindingは失敗
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private PointCollection _myPoints = new();

        //public event PropertyChangedEventHandler? PropertyChanged;
        //protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}

        //public PointCollection MyPoints
        //{
        //    get => _myPoints;
        //    set { if (value == _myPoints) { return; } _myPoints = value; OnPropertyChanged(); }
        //}
        public PointCollection MyPointC { get; set; }
        public ObservableCollection<Point> MyObPoints { get; set; } = new();
        public ObservableCollection<Point> MyObPoints2 { get; set; } = new();
        public ObservableCollection<Point> MyObPoints3 { get; set; } = new();
        public ObservableCollection<Point> MyObPoints4 { get; set; } = new();
        public PointCollection Points { get; set; }
        //public DataPoints MyDataPoints { get; set; }
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            Left = 10; Top = 10;
#endif
            DataContext = this;
            MyPointC = new() { new(10, 10), new(100, 20), new(200, 10) };
            MyObPoints = new() { new(10, 30), new(100, 40), new(200, 30) };
            MyObPoints2 = new() { new(10, 50), new(100, 60), new(200, 50) };
            MyObPoints3 = new() { new(10, 70), new(100, 80), new(200, 70) };
            MyObPoints4 = new() { new(10, 90), new(100, 100), new(200, 90) };
            //MyObPoints.CollectionChanged += MyObPoints_CollectionChanged;
            Points = new() { new(10, 10), new(200, 100) };
            //MyDataPoints = new(new PointCollection() { new(10, 50), new(10, 100), new(100, 200) });

        }

        private void MyObPoints_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //MyObPoints = new ObservableCollection<Point>(MyObPoints);
        }

        private Thumb MakeThumb(double x, double y)
        {
            Thumb t = new()
            {
                Width = 20,
                Height = 20,
                Background = Brushes.Black,
                Opacity = 0.2
            };
            Canvas.SetLeft(t, x);
            Canvas.SetTop(t, y);
            t.DragDelta += MyThumb_DragDelta;
            return t;
        }
        private Binding MakeBinding(string path, object source)
        {
            return new(path) { Source = source, Mode = BindingMode.TwoWay };
        }
        private Binding MakeBinding(DependencyProperty dp, object source)
        {
            return new() { Path = new PropertyPath(dp), Source = source, Mode = BindingMode.TwoWay };
        }


        private MultiBinding MakeMultiBinding(object param, IMultiValueConverter converter, params Binding[] bindings)
        {
            MultiBinding m = new();
            m.ConverterParameter = param;
            m.Converter = converter;
            m.Mode = BindingMode.TwoWay;
            foreach (var item in bindings)
            {
                m.Bindings.Add(item);
            }
            return m;
        }


        #region ドラッグ移動
        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
            Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
        }
        private void MyThumb_DragDeltaOnlyVertical(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
        }
        private void MyThumb_DragDeltaOnlyHorizontal(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
        }
        #endregion ドラッグ移動

        #region チェック用
        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            //MyPoints.Add(new(100, 300));
            //MyPoints = new() { new(100, 100), new(0, 0)};
            //MyObPoints = new() { new(0, 0), new(100, 100) };
            //MyPoints = new PointCollection(MyPoints);
            //MyObPoints = new ObservableCollection<Point>(MyObPoints);
            //MyDataPoints.Points.Add(new Point(100, 300));
            MyPointC.Add(new Point(500, 310));
            MyObPoints.Add(new(500, 320));
            MyObPoints2.Add(new(500, 330));
            MyObPoints3.Add(new(500, 340));
            MyObPoints4.Add(new(500, 350));
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            //MyPointC = new() { new(92, 2) };
            //MyObPoints = new() { new(2,2) };
            //MyObPoints2 = new() { new(3,3) };
            //MyObPoints3 = new() { new(3,3) };
            //MyObPoints4 = new() { new(3,3) };
            var neko = (PathGeometry)MyPath1.Data;
            PathFigure? inu = neko.Figures[0];
            var uma = (PolyLineSegment)inu.Segments[0];
            var tako = uma.Points;
            uma.Points.Add(new Point(300, 300));

            PolyLineSegment plseg = new(tako, true);
            PathFigure pf = new(); pf.Segments.Add(plseg);
            PathFigureCollection pfc = new() { pf };
            PathGeometry pgeo = new(pfc);
            //MyPath0.Data = pgeo;//おｋ

            MyPath1.Data = new PathGeometry(neko.Figures);//おｋ
            //MyPath1.Data = neko;//あかん
            //Pointsを変更するとData(PathGeometry)も更新されるけど、なぜか描画は変わらない
            //だけど、DataのFiguresからPathGeometryを作成して、それをDataに指定すると更新された！！！！！！！！！

            //ってことは
            //Path要素とPointコレクションをバインドするのはあんまり意味がないかも？
            //バインドしたポイントが反映されるのは起動時のみ
            //その後にPointの増減や値変更をしても反映されなくて、反映させるには
            //PathのDataを取得したものから新たにDataを作成して、それを指定する必要がある
            //Pointのコレクションの種類は普通のPointCollectionでもObsevableCollectionでも同じだった

            //直線ならPath要素じゃなくてPolyLine要素を使ったほうが良さそう

            //順番にバインドすればできる？
        }



        #endregion チェック用


    }

    public class OOODataPoints : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataPoints data = (DataPoints)value;
            return new PointCollection(data.Points);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OOO : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Point> ps = (ObservableCollection<Point>)value;
            return new PointCollection(ps);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //public class OOO1 : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var ps=(poin)
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class OOO2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //ObservableCollection<Point> ps = (ObservableCollection<Point>)value;
            IEnumerable<Point> ps = (IEnumerable<Point>)value;
            PolyLineSegment seg = new(ps, true);
            PathFigure figure = new() { StartPoint = new() };
            figure.Segments.Add(seg);
            PathGeometry geo = new();
            geo.Figures.Add(figure);
            return geo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OOO4 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Point> ps = (ObservableCollection<Point>)value;
            //IEnumerable<Point> ps = (IEnumerable<Point>)value;
            PolyLineSegment seg = new(ps, true);
            PathSegmentCollection pseg = new();
            pseg.Add(seg);
            //PathFigure figure = new() { StartPoint = new() };
            //figure.Segments.Add(seg);
            return pseg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OOO5 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Point> ps = (ObservableCollection<Point>)value;
            PolyLineSegment seg = new(ps, true);
            PathSegmentCollection pseg = new();
            pseg.Add(seg);
            PathGeometry pgeo = new();
            PathFigureCollection pfc = new();
            PathFigure pf = new();
            pf.Segments = pseg;
            pfc.Add(pf);
            return pfc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DDD : DependencyObject
    {

        public DDD()
        { }


        //public PointCollection MyPc
        //{
        //    get { return (PointCollection)GetValue(MyPcProperty); }
        //    set { SetValue(MyPcProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for MyPc.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty MyPcProperty =
        //    DependencyProperty.Register("MyPc", typeof(PointCollection), typeof(DDD), new PropertyMetadata(0));


        public Point Point
        {
            get { return (Point)GetValue(PointProperty); }
            set { SetValue(PointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Point.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register("Point", typeof(Point), typeof(DDD), new PropertyMetadata(new Point(0, 0)));



    }

    public class OOO3 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<Point> ps = (IEnumerable<Point>)value;
            return $"{ps.Count()}個";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DataPoints : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private PointCollection _points;
        public PointCollection Points
        {
            get => _points;
            set { if (value == _points) { return; } _points = value; OnPropertyChanged(nameof(Points)); }
        }
        public DataPoints()
        {
            _points = new();
        }
        public DataPoints(PointCollection points) : this()
        {
            Points = points;
        }
    }

}