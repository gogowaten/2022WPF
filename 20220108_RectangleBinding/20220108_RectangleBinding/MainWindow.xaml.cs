using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

//矩形範囲
//PathのDataにRectangleGeometryをBinding、上下左右の値とBinding
namespace _20220108_RectangleBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
        Path MyPath = new();
        private double myLeft = 20;
        private double myTop = 20;
        private double myRight = 100;
        private double myBottom = 100;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double MyLeft { get => myLeft; set { myLeft = value; OnPropertyChanged(); } }
        public double MyTop { get => myTop; set { myTop = value; OnPropertyChanged(); } }
        public double MyRight { get => myRight; set { myRight = value; OnPropertyChanged(); } }
        public double MyBottom { get => myBottom; set { myBottom = value; OnPropertyChanged(); } }
        public MainWindow()
        {
            InitializeComponent();

            MyStackPnel.DataContext = this;

            MyCanvas.Children.Add(MyPath);
            MyPath.Fill = Brushes.Red;
            MyLeft = 20;
            MyTop = 20;
            MyRight = 100;
            MyBottom = 100;

            Binding b1 = new(nameof(MyLeft));
            b1.Source = this;
            Binding b2 = new(nameof(MyTop));
            b2.Source = this;
            Binding b3 = new(nameof(MyRight));
            b3.Source = this;
            Binding b4 = new(nameof(MyBottom));
            b4.Source = this;
            MultiBinding mb = new();
            mb.Bindings.Add(b1);
            mb.Bindings.Add(b2);
            mb.Bindings.Add(b3);
            mb.Bindings.Add(b4);
            mb.Converter = new MyRectConverter();
            MyPath.SetBinding(Path.DataProperty, mb);
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            RectangleGeometry pdata = (RectangleGeometry)MyPath.Data;
            var right = MyRight;
            var left = MyLeft;
            MyLeft = 10;
            //MyPath.Data = new RectangleGeometry(new Rect(0, 0, 200, 99));
            //MyRight = 400;
        }
    }
    public class MyRectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double left = (double)values[0];
            double top = (double)values[1];
            double right = (double)values[2];
            double bottom = (double)values[3];
            return new RectangleGeometry(new Rect(new Point(left, top), new Point(right, bottom)));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            //RectangleGeometry r = (RectangleGeometry)value;
            //object[] vs = new object[3];
            //vs[0] = r.Rect.Left;
            //vs[1] = r.Rect.Top;
            //vs[2] = r.Rect.Right;
            //vs[3] = r.Rect.Bottom;
            //return vs;
        }
    }
}
