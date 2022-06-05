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

//対象の要素とそのDataにバインド
//これは前回の要素に直接版インドする方法とほぼ同じになった

//Thumbの配置番号
//0 1 2
//3   4
//5 6 7
namespace _20220605_SizeChangeThumb3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data1 MyRectangleData;
        private Data1 MyEllipseData;
        public MainWindow()
        {
            InitializeComponent();

            MyRectangleData = new Data1(150, 110, 100, 70);
            Test1(MyRectangleData, MyRectangle);
            MyEllipseData = new Data1(250, 100, 150, 150);
        }
        private void Test1(Data1 data, FrameworkElement element)
        {
            Binding bx = MakeBinding(nameof(data.X), data);
            Binding by = MakeBinding(nameof(data.Y), data);
            Binding bw = MakeBinding(nameof(data.W), data);
            Binding bh = MakeBinding(nameof(data.H), data);
            element.SetBinding(LeftProperty, bx);
            element.SetBinding(TopProperty, by);
            element.SetBinding(WidthProperty, bw);
            element.SetBinding(HeightProperty, bh);

            MultiBinding mb0 = MakeMultiBinding(data, new DC0(), bx, bw);
            MultiBinding mb1 = MakeMultiBinding(data, new DC1(), by, bh);
            MultiBinding mb2 = MakeMultiBinding(data, new MMM2(), bx, bw);
            MultiBinding mb3 = MakeMultiBinding(data, new DC2(), bx, bw);
            MultiBinding mb4 = MakeMultiBinding(data, new DC3(), by, bh);
            MultiBinding mb5 = MakeMultiBinding(data, new MMM2(), by, bh);

            MyThumb0.SetBinding(LeftProperty, mb0);
            MyThumb0.SetBinding(TopProperty, mb1);
            MyThumb1.SetBinding(LeftProperty, mb2);
            MyThumb1.SetBinding(TopProperty, mb1);
            MyThumb2.SetBinding(LeftProperty, mb3);
            MyThumb2.SetBinding(TopProperty, mb1);

            MyThumb3.SetBinding(LeftProperty, mb0);
            MyThumb3.SetBinding(TopProperty, mb5);
            MyThumb4.SetBinding(LeftProperty, mb3);
            MyThumb4.SetBinding(TopProperty, mb5);

            MyThumb5.SetBinding(LeftProperty, mb0);
            MyThumb5.SetBinding(TopProperty, mb4);
            MyThumb6.SetBinding(LeftProperty, mb2);
            MyThumb6.SetBinding(TopProperty, mb4);
            MyThumb7.SetBinding(LeftProperty, mb3);
            MyThumb7.SetBinding(TopProperty, mb4);

        }
        private Binding MakeBinding(string path, object source)
        {
            return new(path) { Source = source, Mode = BindingMode.TwoWay };
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
            MyRectangle.Width += 10;
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(MyRectangle, Canvas.GetLeft(MyRectangle) + 10);
        }


        private void MyRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Test1(MyRectangleData, (FrameworkElement)sender);
        }

        private void MyEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Test1(MyEllipseData, (FrameworkElement)sender);
        }
        #endregion チェック用

    }


    public class DC0 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Data1 data = (Data1)parameter;
            double left = (double)value;
            double total = data.X + data.W;
            double width = total - left;
            object[] result = new object[2];
            result[0] = left;
            result[1] = width;
            //サイズが1未満にならないように調整
            if (width < 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            return result;
        }
    }
    public class DC1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Data1 data = (Data1)parameter;
            double top = (double)value;
            double total = data.Y + data.H;
            double height = total - top;
            object[] result = new object[2];
            result[0] = top;
            result[1] = height;
            //サイズが1未満にならないように調整
            if (height < 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            return result;
        }
    }
    public class DC2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] + (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Data1 data = (Data1)parameter;
            double left = data.X;
            double width = (double)value - left;

            object[] result = new object[2];
            result[0] = left;
            result[1] = width;
            //サイズが1未満にならないように調整
            if (width < 1.0)
            {
                result[1] = 1.0;
            }
            return result;
        }
    }
    public class DC3 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] + (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Data1 data = (Data1)parameter;
            double top = data.Y;
            double height = (double)value - top;

            object[] result = new object[2];
            result[0] = top;
            result[1] = height;
            //サイズが1未満にならないように調整
            if (height < 1.0)
            {
                result[1] = 1.0;
            }
            return result;
        }
    }

    public class MMM2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] + (double)values[1] / 2.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Data1 : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _x;
        private double _y;
        private double _w;
        private double _h;
        public double X { get => _x; set { if (value == _x) { return; } _x = value; OnPropertyChanged(); } }
        public double Y { get => _y; set { if (value == _y) { return; } _y = value; OnPropertyChanged(); } }
        public double W { get => _w; set { if (value == _w) { return; } _w = value; OnPropertyChanged(); } }
        public double H { get => _h; set { if (value == _h) { return; } _h = value; OnPropertyChanged(); } }
        public Data1() { }
        public Data1(double x, double y, double w, double h)
        {
            X = x; Y = y; W = w; H = h;
        }
    }

}
