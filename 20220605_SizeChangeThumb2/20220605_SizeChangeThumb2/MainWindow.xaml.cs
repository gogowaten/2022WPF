using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Globalization;

//_20220605_SizeChangeThumbの
//コンバーター部分をまとめることでコードを短くできた、240行から210行
//けど、わかりにくく見づらくなったかも、なので必要ない

//対象の要素に直接Thumbをバインド
//Thumbの配置番号
//0 1 2
//3   4
//5 6 7
namespace _20220605_SizeChangeThumb2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Test2(MyRectangle);
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
        private void Test2(FrameworkElement element)
        {
            object[] param0 = new object[3];
            param0[0] = element;
            param0[1] = new Func<FrameworkElement, double>(Canvas.GetLeft);
            param0[2] = (FrameworkElement e) => e.Width;
            object[] param1 = new object[3];
            param1[0] = element;
            param1[1] = new Func<FrameworkElement, double>(Canvas.GetTop);
            param1[2] = (FrameworkElement e) => e.Height;

            Binding b_left = MakeBinding(element, LeftProperty);
            Binding b_width = MakeBinding(element, WidthProperty);
            Binding b_top = MakeBinding(element, TopProperty);
            Binding b_height = MakeBinding(element, HeightProperty);

            MultiBinding m0 = MakeMultiBinding(param0, new DDD0(), b_left, b_width);
            MultiBinding m1 = MakeMultiBinding(param1, new DDD0(), b_top, b_height);
            MultiBinding m2 = MakeMultiBinding(element, new MMM2(), b_left, b_width);
            MultiBinding m3 = MakeMultiBinding(param0, new DDD1(), b_left, b_width);
            MultiBinding m4 = MakeMultiBinding(element, new MMM2(), b_top, b_height);
            MultiBinding m5 = MakeMultiBinding(param1, new DDD1(), b_top, b_height);

            MyThumb0.SetBinding(LeftProperty, m0);
            MyThumb0.SetBinding(TopProperty, m1);

            MyThumb1.SetBinding(LeftProperty, m2);
            MyThumb1.SetBinding(TopProperty, m1);

            MyThumb2.SetBinding(LeftProperty, m3);
            MyThumb2.SetBinding(TopProperty, m1);

            MyThumb3.SetBinding(LeftProperty, m0);
            MyThumb3.SetBinding(TopProperty, m4);

            MyThumb4.SetBinding(LeftProperty, m3);
            MyThumb4.SetBinding(TopProperty, m4);

            MyThumb5.SetBinding(LeftProperty, m0);
            MyThumb5.SetBinding(TopProperty, m5);

            MyThumb6.SetBinding(LeftProperty, m2);
            MyThumb6.SetBinding(TopProperty, m5);

            MyThumb7.SetBinding(LeftProperty, m3);
            MyThumb7.SetBinding(TopProperty, m5);
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
            Test2((FrameworkElement)sender);
        }

        private void MyEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Test2((FrameworkElement)sender);
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


    public class DDD0 : IMultiValueConverter
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

            double locate = (double)value;
            double total = f0(element) + f1(element);
            double size = total - locate;
            object[] result = new object[2];
            result[0] = locate;
            result[1] = size;
            if (size < 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            return result;
        }
    }
    public class DDD1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] + (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] ooo = (object[])parameter;
            FrameworkElement element = (FrameworkElement)ooo[0];
            Func<FrameworkElement, double> f0 = (Func<FrameworkElement, double>)ooo[1];

            double locate = f0(element);
            double size = (double)value - locate;
            object[] result = new object[2];
            result[0] = locate;
            result[1] = size;
            if (size < 1.0)
            {
                result[1] = 1.0;
            }
            return result;
        }
    }

}