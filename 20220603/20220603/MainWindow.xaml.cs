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

        public MainWindow()
        {
            InitializeComponent();

            Test1();
            //Test2();

        }
        private void Test1()
        {
            Binding rx = new();
            rx.Source = MyRectangle;
            rx.Path = new PropertyPath(LeftProperty);
            MyThumb0.SetBinding(LeftProperty, rx);
            Binding ry = new();
            ry.Source = MyRectangle;
            ry.Path = new PropertyPath(TopProperty);
            MyThumb0.SetBinding(TopProperty, ry);

            Binding rx2 = new();
            rx2.Source = MyRectangle;
            rx2.Path = new PropertyPath(WidthProperty);
            rx2.Converter = new MMM();
            rx2.ConverterParameter = MyRectangle;
            MyThumb1.SetBinding(LeftProperty, rx2);
            MyThumb1.SetBinding(TopProperty, ry);

            Binding rx3 = new();
            rx3.Source = MyRectangle;
            rx3.Path = new PropertyPath(WidthProperty);
            rx3.ConverterParameter = MyRectangle;
            rx3.Converter = new MMM2();
            MyThumb2.SetBinding(LeftProperty, rx3);
            MyThumb2.SetBinding(TopProperty, ry);
        }
        //private void SetLocate(FrameworkElement element, double x, double y)
        //{
        //    Canvas.SetLeft(element, x); Canvas.SetTop(element, y);
        //}



        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            if (t.Name == nameof(MyThumb0))
            {
                double rx = Canvas.GetLeft(MyRectangle);
                double ry = Canvas.GetTop(MyRectangle);
                double rw = MyRectangle.Width;
                double rh = MyRectangle.Height;
                rx += e.HorizontalChange;
                ry += e.VerticalChange;
                rw -= e.HorizontalChange;
                rh -= e.VerticalChange;
                MyRectangle.Width = rw;
                MyRectangle.Height = rh;
                Canvas.SetLeft(MyRectangle, rx);
                Canvas.SetTop(MyRectangle, ry);
            }
            else if (t.Name == nameof(MyThumb1))
            {
                double ry = Canvas.GetTop(MyRectangle);
                double rh = MyRectangle.Height;
                ry += e.VerticalChange;
                rh -= e.VerticalChange;
                MyRectangle.Height = rh;
                Canvas.SetTop(MyRectangle, ry);

            }
            else if (t.Name == nameof(MyThumb2))
            {
                double ry = Canvas.GetTop(MyRectangle);
                double rw = MyRectangle.Width;
                double rh = MyRectangle.Height;
                ry += e.VerticalChange;
                rw += e.HorizontalChange;
                rh -= e.VerticalChange;
                MyRectangle.Width = rw;
                MyRectangle.Height = rh;
                Canvas.SetTop(MyRectangle, ry);
            }

        }

        private void MyThumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }

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
            double v = (double)value;
            v += Canvas.GetLeft(r);
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
