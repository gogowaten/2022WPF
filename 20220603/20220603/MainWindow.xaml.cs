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

//対象の要素に直接Thumbをバインド
//Thumbの配置番号
//0 1 2
//3   4
//5 6 7

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

            Test1(MyRectangle);
            //Test2();
        }
        private void Test1(FrameworkElement element)
        {
            Binding b0 = MakeBinding(element, LeftProperty);
            Binding b1 = MakeBinding(element, WidthProperty);
            Binding b2 = MakeBinding(element, TopProperty);
            Binding b3 = MakeBinding(element, HeightProperty);

            MultiBinding m0 = MakeMultiBinding(element, new MMM0(), b0, b1);
            MyThumb0.SetBinding(LeftProperty, m0);
            MultiBinding m1 = MakeMultiBinding(element, new MMM1(), b2, b3);
            MyThumb0.SetBinding(TopProperty, m1);

            MultiBinding m2 = MakeMultiBinding(element, new MMM2(), b0, b1);
            MyThumb1.SetBinding(LeftProperty, m2);
            MyThumb1.SetBinding(TopProperty, m1);

            MultiBinding m3 = MakeMultiBinding(element, new MMM3(), b0, b1);
            MyThumb2.SetBinding(LeftProperty, m3);
            MyThumb2.SetBinding(TopProperty, m1);

            MultiBinding m4 = MakeMultiBinding(element, new MMM2(), b2, b3);
            MyThumb3.SetBinding(LeftProperty, m0);
            MyThumb3.SetBinding(TopProperty, m4);

            MyThumb4.SetBinding(LeftProperty, m3);
            MyThumb4.SetBinding(TopProperty, m4);

            MultiBinding m5 = MakeMultiBinding(element, new MMM4(), b2, b3);
            MyThumb5.SetBinding(LeftProperty, m0);
            MyThumb5.SetBinding(TopProperty, m5);

            MyThumb6.SetBinding(LeftProperty, m2);
            MyThumb6.SetBinding(TopProperty, m5);

            MyThumb7.SetBinding(LeftProperty, m3);
            MyThumb7.SetBinding(TopProperty, m5);
        }
        private Binding MakeBinding(FrameworkElement source, DependencyProperty dp)
        {
            Binding b = new();
            b.Source = source;
            b.Path = new PropertyPath(dp);
            b.Mode = BindingMode.TwoWay;
            return b;
        }
        private MultiBinding MakeMultiBinding(object param, IMultiValueConverter converter, params Binding[] bindings)
        {
            MultiBinding m = new MultiBinding();
            m.ConverterParameter = param;
            m.Converter = converter;
            m.Mode = BindingMode.TwoWay;
            foreach (var item in bindings)
            {
                m.Bindings.Add(item);
            }
            return m;
        }
        private void Test2()
        {
            Func<FrameworkElement, double> f0 = (FrameworkElement e) => { return Canvas.GetLeft(e); };
            Func<FrameworkElement, double> f1 = (FrameworkElement e) => { return Canvas.GetTop(e); };
            Func<FrameworkElement, double> f2 = (FrameworkElement e) => { return e.Width; };
            Func<FrameworkElement, double> f3 = (FrameworkElement e) => { return e.Height; };
            object[] param0 = new object[3];
            param0[0] = MyRectangle;
            param0[1] = f0;
            param0[2] = f2;
            object[] param1 = new object[3];
            param1[0] = MyRectangle;
            param1[1] = f1;
            param1[2] = f3;

            Binding b0 = new();
            b0.Source = MyRectangle;
            b0.Path = new PropertyPath(LeftProperty);
            b0.Mode = BindingMode.TwoWay;
            Binding b1 = new();
            b1.Source = MyRectangle;
            b1.Path = new PropertyPath(WidthProperty);
            b1.Mode = BindingMode.TwoWay;
            MultiBinding m0 = new();
            m0.Bindings.Add(b0); m0.Bindings.Add(b1);
            m0.ConverterParameter = param0;
            m0.Converter = new DDD();
            m0.Mode = BindingMode.TwoWay;
            MyThumb0.SetBinding(LeftProperty, m0);

            Binding b2 = new();
            b2.Source = MyRectangle;
            b2.Path = new PropertyPath(TopProperty);
            b2.Mode = BindingMode.TwoWay;
            Binding b3 = new();
            b3.Source = MyRectangle;
            b3.Path = new PropertyPath(HeightProperty);
            b3.Mode = BindingMode.TwoWay;
            MultiBinding m1 = new();
            m1.Bindings.Add(b2); m1.Bindings.Add(b3);
            m1.ConverterParameter = param1;
            m1.Mode = BindingMode.TwoWay;
            m1.Converter = new DDD();
            MyThumb0.SetBinding(TopProperty, m1);

        }


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
            Test1((FrameworkElement)sender);
        }

        private void MyEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Test1((FrameworkElement)sender);
        }
    }

    public class MMM0 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            FrameworkElement element = (FrameworkElement)parameter;
            double total = element.Width + Canvas.GetLeft(element);
            double left = (double)value;
            double width = total - left;

            object[] result = new object[2];
            result[0] = left;
            result[1] = width;
            //サイズが1以下にならないように調整
            if (width < 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            return result;
        }
    }
    public class MMM1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            FrameworkElement element = (FrameworkElement)parameter;
            double total = element.Height + Canvas.GetTop(element);
            double top = (double)value;
            double height = total - top;

            object[] result = new object[2];
            result[0] = top;
            result[1] = height;
            if (height < 1.0)
            {
                result[0] = total - 1.0;
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
    public class MMM3 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] + (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            FrameworkElement element = (FrameworkElement)parameter;
            double left = (double)Canvas.GetLeft(element);
            double width = (double)value - left;

            object[] result = new object[2];
            result[0] = left;
            result[1] = width;
            if (width < 1.0) { result[1] = 1.0; }
            return result;
        }
    }
    public class MMM4 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] + (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            FrameworkElement element = (FrameworkElement)parameter;
            double top = (double)Canvas.GetTop(element);
            double height = (double)value - top;

            object[] result = new object[2];
            result[0] = top;
            result[1] = height;
            if (height < 1.0) { result[1] = 1.0; }
            return result;
        }
    }


    public class DDD : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] ooo = (object[])parameter;
            FrameworkElement element = (FrameworkElement)ooo[0];
            Func<FrameworkElement, double> f0 = (Func<FrameworkElement, double>)ooo[1];
            Func<FrameworkElement, double> f1 = (Func<FrameworkElement, double>)ooo[2];

            double t = (double)value;
            double locate = f0(element);
            double length = f1(element);
            double total = locate + length;
            object[] result = new object[2];
            if (total - t >= 1.0)
            {
                result[0] = t;
                result[1] = total - t;
            }
            else
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            return result;
        }
    }
}
