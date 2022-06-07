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

namespace _20220603
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private PointCollection _myPoints = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PointCollection MyPoints
        {
            get => _myPoints;
            set { if (value == _myPoints) { return; } _myPoints = value; OnPropertyChanged(); }
        }
       
        private Point _pp;
        public Point PP { get => _pp;
            set { if (value == _pp) { return; }_pp = value;OnPropertyChanged(); }
        }
        public PointCollection MyPointC { get; set; }
        public PointCollection MyPointC1 { get; set; }
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
            MyPointC1 = new() { new(10, 110), new(100, 120), new(200, 110) };
            MyPolyLine1.Points = MyPointC1;
            //MyObPoints.CollectionChanged += MyObPoints_CollectionChanged;
            Points = new() { new(10, 10), new(200, 100) };
            MyPoints = new() { PP, new (10, 130), new(100, 140), new(200, 130) };
            
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
            MyPointC1.Add(new(500, 360));
            MyPoints.Add(new(500, 370));
            

            PP = new(23, 440);
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLine1.Points.Add(new(100, 200));
        }



        #endregion チェック用


    }

    public class OOO0 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Point> points = (ObservableCollection<Point>)value;
            return new PointCollection(points);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

