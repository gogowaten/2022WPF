using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Globalization;

//WPF、マウスドラッグ移動で要素のサイズ変更、サイズ変更ハンドル8個はThumbで作成して対象にバインド - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2022/06/06/154322

//対象の要素に直接Thumbをバインド
//Thumbの配置番号
//0 1 2
//3   4
//5 6 7
namespace _20220605_SizeChangeThumb
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
            //Test1(MyButton2);
        }
        //対象の要素にThumbをバインド
        private void Test1(FrameworkElement element)
        {
            Binding b0 = MakeBinding(element, LeftProperty);
            Binding b1 = MakeBinding(element, WidthProperty);
            Binding b2 = MakeBinding(element, TopProperty);
            Binding b3 = MakeBinding(element, HeightProperty);

            MultiBinding m0 = MakeMultiBinding(new MMM0(), element, b0, b1);
            MultiBinding m1 = MakeMultiBinding(new MMM1(), element, b2, b3);
            MultiBinding m2 = MakeMultiBinding(new MMM2(), element, b0, b1);
            MultiBinding m3 = MakeMultiBinding(new MMM3(), element, b0, b1);
            MultiBinding m4 = MakeMultiBinding(new MMM2(), element, b2, b3);
            MultiBinding m5 = MakeMultiBinding(new MMM4(), element, b2, b3);

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
        private Binding MakeBinding(FrameworkElement source, DependencyProperty dp)
        {
            Binding b = new()
            {
                Source = source,
                Path = new PropertyPath(dp),
                Mode = BindingMode.TwoWay
            };
            return b;
        }
        private MultiBinding MakeMultiBinding(
            IMultiValueConverter converter, object? param = null, params Binding[] bindings)
        {
            MultiBinding m = new()
            {
                ConverterParameter = param,
                Converter = converter,
                Mode = BindingMode.TwoWay
            };
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
        #region 動作チェック
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
        #endregion 動作チェック

    }

    #region コンバーター
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
            //サイズが1未満にならないように調整
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
    #endregion コンバーター



}
