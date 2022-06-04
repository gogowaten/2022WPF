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

namespace _20220604_SizeChangeThumb
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
            Test2();

        }


        #region Test1, 2

        private Thumb MakeAndSetThumb(Panel panel, double size, double opa, Brush brush)
        {
            Thumb t = new() { Width = size, Height = size, Opacity = opa, Background = brush };
            panel.Children.Add(t);
            return t;
        }
        private Rectangle MakeAndSetRectangle(Panel panel, double size, double opa, Brush brush)
        {
            Rectangle r = new() { Width = size, Height = size, Opacity = opa, Fill = brush };
            panel.Children.Add(r);
            return r;
        }

        //通知プロパティを介した方法
        private void Test1()
        {
            Rectangle element = MakeAndSetRectangle(MyCanvas, 100, 1.0, Brushes.MediumAquamarine);
            MyData.X = 120; MyData.Y = 150;
            MyData.W = 100; MyData.H = 50;
            //DataとRectangleのバインド
            element.SetBinding(LeftProperty,
                new Binding(nameof(MyData.X))
                { Source = MyData, Mode = BindingMode.TwoWay });
            element.SetBinding(TopProperty,
                new Binding(nameof(MyData.Y))
                { Source = MyData, Mode = BindingMode.TwoWay });
            element.SetBinding(WidthProperty,
                new Binding(nameof(MyData.W))
                { Source = MyData, Mode = BindingMode.TwoWay });
            element.SetBinding(HeightProperty,
                new Binding(nameof(MyData.H))
                { Source = MyData, Mode = BindingMode.TwoWay });

            //Dataとthumbのバインド、水平方向
            Binding b1 = new(nameof(MyData.X))
            {
                Source = MyData,
                Mode = BindingMode.TwoWay
            };
            Binding b2 = new(nameof(MyData.W))
            {
                Source = MyData,
                Mode = BindingMode.TwoWay
            };
            MultiBinding mb = new()
            {
                ConverterParameter = MyData,
                Mode = BindingMode.TwoWay,
                Converter = new NNNHorizontal()
            };
            mb.Bindings.Add(b1);
            mb.Bindings.Add(b2);

            Thumb t = MakeAndSetThumb(MyCanvas, 20, 0.2, Brushes.Black);
            t.SetBinding(LeftProperty, mb);
            t.DragDelta += MyThumb_DragDelta;

            //Dataとthumbのバインド、垂直方向
            b1 = new(nameof(MyData.Y))
            {
                Source = MyData,
                Mode = BindingMode.TwoWay
            };
            b2 = new(nameof(MyData.H))
            {
                Source = MyData,
                Mode = BindingMode.TwoWay
            };
            mb = new()
            {
                ConverterParameter = MyData,
                Mode = BindingMode.TwoWay,
                Converter = new NNNVertical()
            };
            mb.Bindings.Add(b1);
            mb.Bindings.Add(b2);
            t.SetBinding(TopProperty, mb);
        }

        //Rectangleに直接バインド
        private void Test2()
        {

            Rectangle element = MakeAndSetRectangle(MyCanvas, 100, 1.0, Brushes.MediumOrchid);
            Canvas.SetLeft(element, 20);
            Canvas.SetTop(element, 220);

            Binding b1 = new()
            {
                Source = element,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(LeftProperty)
            };
            Binding b2 = new()
            {
                Source = element,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(WidthProperty)
            };
            MultiBinding mb = new()
            {
                ConverterParameter = element,
                Converter = new NNNRectHorizontal(),
                Mode = BindingMode.TwoWay
            };
            mb.Bindings.Add(b1);
            mb.Bindings.Add(b2);
            Thumb t = MakeAndSetThumb(MyCanvas, 20, 0.2, Brushes.Black);
            t.SetBinding(LeftProperty, mb);
            t.DragDelta += MyThumb_DragDelta;

            b1 = new()
            {
                Source = element,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(TopProperty)
            };
            b2 = new()
            {
                Source = element,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(HeightProperty)
            };
            mb = new()
            {
                ConverterParameter = element,
                Converter = new NNNRectVertical(),
                Mode = BindingMode.TwoWay
            };
            mb.Bindings.Add(b1);
            mb.Bindings.Add(b2);
            t.SetBinding(TopProperty, mb);

        }
        #endregion Test1, 2

        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb t)
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
                Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            }
        }


    }

    //Rectangleに直接バインド、水平方向用
    public class NNNRectHorizontal : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Rectangle element = (Rectangle)parameter;
            double total = Canvas.GetLeft(element) + element.Width;
            double v = (double)value;
            object[] result = new object[2];
            
            //widthが1.0以下になる場合は1.0に修正、x座標も修正
            if (total - v <= 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            else
            {
                result[0] = v;
                result[1] = total - v;
            }
            return result;
        }
    }
    //Rectangleに直接バインド、垂直方向用
    public class NNNRectVertical : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Rectangle element = (Rectangle)parameter;
            double total = Canvas.GetTop(element) + element.Height;//ここだけ違う
            double v = (double)value;
            object[] result = new object[2];
            //heightが1.0以下になる場合は1.0に修正、x座標も修正
            if (total - v <= 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            else
            {
                result[0] = v;
                result[1] = total - v;
            }
            return result;
        }
    }

    //通知プロパティを介してバインド用
    public class NNNHorizontal : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Data1 data = (Data1)parameter;
            double total = data.X + data.W;
            double v = (double)value;
            object[] result = new object[2];
            //widthが1.0以下になる場合は1.0に修正、x座標も修正
            if (total - v <= 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            else
            {
                result[0] = v;
                result[1] = total - v;
            }
            return result;
        }
    }
    public class NNNVertical : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Data1 data = (Data1)parameter;
            double total = data.Y + data.H;//horizontalと違うのはここだけ
            double v = (double)value;
            object[] result = new object[2];
            //heightが1.0以下になる場合は1.0に修正、x座標も修正
            if (total - v <= 1.0)
            {
                result[0] = total - 1.0;
                result[1] = 1.0;
            }
            else
            {
                result[0] = v;
                result[1] = total - v;
            }
            return result;
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
    }
}