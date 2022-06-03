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

namespace _20220603
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Data1 MyData = new();
        public MainWindow()
        {
            InitializeComponent();

            Test1();
        }
        private void Test1()
        {
            MyData.X = 200;
            Binding b0 = new(nameof(MyData.X));
            b0.Source = MyData;
            MyRectangle.SetBinding(LeftProperty, b0);
            //右ThumbのBinding
            Binding b1 = new();
            b1.Source = MyRectangle;
            b1.Path = new PropertyPath(WidthProperty);
            b1.ConverterParameter = Canvas.GetLeft(MyRectangle);
            b1.Converter = new MM();
            b1.Mode = BindingMode.TwoWay;
            //MyThumb1.SetBinding(LeftProperty, b1);
            Binding b2 = new();
            b2.Source = MyRectangle;
            b2.Path = new PropertyPath(HeightProperty);
            b2.Converter = new MMHalf();
            b2.ConverterParameter = Canvas.GetTop(MyRectangle);
            //MyThumb1.SetBinding(TopProperty, b2);

            //右下Thumb
            b2 = new();
            b2.Source = MyRectangle;
            b2.Path = new PropertyPath(HeightProperty);
            b2.ConverterParameter = Canvas.GetTop(MyRectangle);
            b2.Converter = new MM();
            b2.Mode = BindingMode.TwoWay;
            //MyThumb2.SetBinding(TopProperty, b2);
            //MyThumb2.SetBinding(LeftProperty, b1);

            //上
            //Binding b3 = new();
            //b3.Source = MyRectangle;
            //b3.Path = new PropertyPath(HeightProperty);
            //Binding b4 = new();
            //b4.Source = MyRectangle;
            //b4.Path = new PropertyPath(TopProperty);
            //MultiBinding mb = new();
            //mb.Bindings.Add(b3); mb.Bindings.Add(b4);
            //mb.ConverterParameter = MyRectangle.Height + Canvas.GetTop(MyRectangle);
            //mb.Converter = new MMM();
            //mb.Mode = BindingMode.TwoWay;
            //MyThumb3.SetBinding(TopProperty, mb);


            Binding b3 = new();
            b3.Source = MyThumb3;
            b3.Path = new PropertyPath(TopProperty);
            b3.Mode = BindingMode.TwoWay;
            MyRectangle.SetBinding(TopProperty, b3);

            Binding b4 = new();
            b4.Source = MyThumb3;
            b4.Path = new PropertyPath(TopProperty);
            b4.ConverterParameter = MyRectangle.Height + Canvas.GetTop(MyRectangle);
            b4.Converter = new MMH();
            b4.Mode = BindingMode.TwoWay;
            MyRectangle.SetBinding(HeightProperty, b4);


        }

        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb t)
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
                Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            }
        }

        private void MyThumb_DragDeltaHorizontal(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb t)
                Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
        }
        private void MyThumb_DragDeltaVertical(object sender, DragDeltaEventArgs e)
        {
            double d = MyRectangle.Height - e.VerticalChange;
            if (sender is Thumb t && d >= 0)
            {
                Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            }
        }
    }


    public class MM : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double rectLeft = (double)parameter;
            double rectW = (double)value;
            return rectW + rectLeft;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double thumbLeft = (double)value;
            double rectLeft = (double)parameter;
            return thumbLeft - rectLeft;
        }
    }
    public class MMHalf : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double rectHeight = (double)value;
            double recttop = (double)parameter;
            return recttop + (rectHeight / 2.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MMH : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double thumbTop = (double)value;
            double total = (double)parameter;
            return total - thumbTop;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //public class MMM : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        double rHeight = (double)values[0];
    //        double rTop = (double)values[1];
    //        double total = (double)parameter;
    //        return rTop;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        double thumbY = (double)value;

    //        double total = (double)parameter;
    //        object[] values = new object[2];
    //        values[0] = total - thumbY;//height
    //        values[1] = thumbY;//top
    //        return values;
    //    }
    //}

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
    }
}
