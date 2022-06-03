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

namespace _20220603
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LineSegment MySegment;
        public MainWindow()
        {
            InitializeComponent();
            MySegment = new();
            Test1();
        }
        private void Test1()
        {
            MySegment.Point = new Point(100, 50);
            MySegment.IsStroked = true;

            PathFigure f = new PathFigure(); f.Segments.Add(MySegment);
            PathGeometry pg = new(); pg.Figures.Add(f);
            MyPath.Data = pg;
            


            Canvas.SetLeft(MyThumb1, MySegment.Point.X);
            Canvas.SetTop(MyThumb1, MySegment.Point.Y);
            Binding b1 = new();
            b1.Source = MyThumb1;
            b1.Path = new PropertyPath(Canvas.LeftProperty);
            Binding b2 = new();
            b2.Source = MyThumb1;
            b2.Path = new PropertyPath(Canvas.TopProperty);
            MultiBinding mb = new();
            mb.Bindings.Add(b1);mb.Bindings.Add(b2);
            mb.Converter = new MMM();
            BindingOperations.SetBinding(MySegment, LineSegment.PointProperty, mb);
        }

        private void MyThumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb t)
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
                Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            }

        }
    }
    public class MMM : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Point p = new((double)values[0], (double)values[1]);
            return p;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MM : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
