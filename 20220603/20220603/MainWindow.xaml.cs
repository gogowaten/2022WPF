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

//Thumbの配置は
//0 1 2
//3   4
//5 6 7
//ほぼできた
//対象(Rectangle)の位置変更したときに一部のThumbが追従しないけど
//どれかのThumbを移動させれば正常になる
//これはバインドがLeftとTopのバインドがないThumb

namespace _20220603
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Test1();
            
        }
        private void Test1()
        {
            Binding rx0 = new();
            rx0.Source = MyRectangle;
            rx0.Path = new PropertyPath(LeftProperty);
            rx0.Mode = BindingMode.TwoWay;
            Binding ry0 = new();
            ry0.Source = MyRectangle;
            ry0.Path = new PropertyPath(TopProperty);
            ry0.Mode = BindingMode.TwoWay;
            MyThumb0.SetBinding(LeftProperty, rx0);
            MyThumb0.SetBinding(TopProperty, ry0);

            Binding rx1 = new();
            rx1.Source = MyRectangle;
            rx1.Path = new PropertyPath(WidthProperty);
            rx1.Mode = BindingMode.TwoWay;
            rx1.Converter = new MMM();
            rx1.ConverterParameter = MyRectangle;
            MyThumb1.SetBinding(LeftProperty, rx1);
            MyThumb1.SetBinding(TopProperty, ry0);

            Binding rx2 = new();
            rx2.Source = MyRectangle;
            rx2.Path = new PropertyPath(WidthProperty);
            rx2.Mode = BindingMode.TwoWay;
            rx2.ConverterParameter = MyRectangle;
            rx2.Converter = new MMM2();
            MyThumb2.SetBinding(LeftProperty, rx2);
            MyThumb2.SetBinding(TopProperty, ry0);

            Binding ry1 = new();
            ry1.Source = MyRectangle;
            ry1.Path = new PropertyPath(HeightProperty);
            ry1.Mode = BindingMode.TwoWay;
            ry1.ConverterParameter = MyRectangle;
            ry1.Converter = new MMM3();
            MyThumb3.SetBinding(LeftProperty, rx0);
            MyThumb3.SetBinding(TopProperty, ry1);

            MyThumb4.SetBinding(LeftProperty, rx2);
            MyThumb4.SetBinding(TopProperty, ry1);

            Binding ry2 = new();
            ry2.Source = MyRectangle;
            ry2.Path = new PropertyPath(HeightProperty);
            ry2.Mode = BindingMode.TwoWay;
            ry2.ConverterParameter = MyRectangle;
            ry2.Converter = new MMM4();
            MyThumb5.SetBinding(LeftProperty, rx0);
            MyThumb5.SetBinding(TopProperty, ry2);

            MyThumb6.SetBinding(LeftProperty, rx1);
            MyThumb6.SetBinding(TopProperty, ry2);

            MyThumb7.SetBinding(LeftProperty, rx2);
            MyThumb7.SetBinding(TopProperty, ry2);
        }




        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            double rx = Canvas.GetLeft(MyRectangle);
            double ry = Canvas.GetTop(MyRectangle);
            double rw = MyRectangle.Width;
            double rh = MyRectangle.Height;

            if (t.Name == nameof(MyThumb0))
            {
                rx += e.HorizontalChange;
                ry += e.VerticalChange;
                rw -= e.HorizontalChange;
                rh -= e.VerticalChange;
                //サイズが1.0未満にならないように調整                
                if (rw < 1.0) { rx += rw - 1.0; rw = 1.0; }
                if (rh < 1.0) { ry += rh - 1.0; rh = 1.0; }
                //順番大事、位置変更してからサイズ変更、
                //もし順序逆だとThumbの位置が微妙にずれる
                Canvas.SetLeft(MyRectangle, rx);
                Canvas.SetTop(MyRectangle, ry);
                MyRectangle.Width = rw;
                MyRectangle.Height = rh;
            }
            else if (t.Name == nameof(MyThumb1))
            {
                ry += e.VerticalChange;
                rh -= e.VerticalChange;
                if (rh < 1.0) { ry += rh - 1.0; rh = 1.0; }
                Canvas.SetTop(MyRectangle, ry);
                MyRectangle.Height = rh;

            }
            else if (t.Name == nameof(MyThumb2))
            {
                ry += e.VerticalChange;
                rw += e.HorizontalChange;
                rh -= e.VerticalChange;
                if (rw < 1.0) { rw = 1.0; }
                if (rh < 1.0) { ry += rh - 1.0; rh = 1.0; }
                Canvas.SetTop(MyRectangle, ry);
                MyRectangle.Width = rw;
                MyRectangle.Height = rh;
            }
            else if (t.Name == nameof(MyThumb3))
            {
                rx += e.HorizontalChange;
                rw -= e.HorizontalChange;
                if (rw < 1.0) { rx += rw - 1.0; rw = 1.0; }
                Canvas.SetLeft(MyRectangle, rx);
                MyRectangle.Width = rw;
            }
            else if (t.Name == nameof(MyThumb4))
            {
                rw += e.HorizontalChange;
                if (rw < 1.0) { rw = 1.0; }
                MyRectangle.Width = rw;
            }
            else if (t.Name == nameof(MyThumb5))
            {
                rx += e.HorizontalChange;
                rw -= e.HorizontalChange;
                rh += e.VerticalChange;
                if (rw < 1.0) { rx += rw - 1.0; rw = 1.0; }
                if (rh < 1.0) { rh = 1.0; }
                Canvas.SetLeft(MyRectangle, rx);
                MyRectangle.Width = rw;
                MyRectangle.Height = rh;
            }
            else if (t.Name == nameof(MyThumb6))
            {
                rh += e.VerticalChange;
                if (rh < 1.0) { rh = 1.0; }
                MyRectangle.Height = rh;
            }
            else if (t.Name == nameof(MyThumb7))
            {
                rw += e.HorizontalChange;
                rh += e.VerticalChange;
                if (rw < 1.0) { rw = 1.0; }
                if (rh < 1.0) { rh = 1.0; }
                MyRectangle.Width = rw;
                MyRectangle.Height = rh;
            }
        }

      
        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyRectangle.Width += 10;
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(MyRectangle, Canvas.GetLeft(MyRectangle) + 10);
        }
    }

    public class MMM : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rectangle r = (Rectangle)parameter;
            double v = (double)value;
            return v / 2.0 + Canvas.GetLeft(r);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MMM2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rectangle r = (Rectangle)parameter;
            double w = (double)value;
            double x = Canvas.GetLeft(r);
            return x + w;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MMM3 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rectangle r = (Rectangle)parameter;
            double h = (double)value;
            double y = Canvas.GetTop(r);

            return y + h / 2.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MMM4 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rectangle r = (Rectangle)parameter;
            double h = (double)value;
            double y = Canvas.GetTop(r);
            return y + h;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
