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
using System.Windows.Controls.Primitives;

namespace _20220108_RectangleBinding2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
        Path MyPath = new();
        Thumb TTopLeft = new() { Width = 10, Height = 10 };
        Thumb TTopRight = new() { Width = 10, Height = 10 };
        Thumb TBottomRight = new() { Width = 10, Height = 10 };
        Thumb TBottomLeft = new() { Width = 10, Height = 10 };

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


            TTopLeft.DragDelta += TTopLeft_DragDelta;
            TTopRight.DragDelta += TTopRight_DragDelta;
            TBottomRight.DragDelta += TBottomRight_DragDelta;
            TBottomLeft.DragDelta += TBottomLeft_DragDelta;
            MyCanvas.Children.Add(TTopLeft);
            MyCanvas.Children.Add(TTopRight);
            MyCanvas.Children.Add(TBottomLeft);
            MyCanvas.Children.Add(TBottomRight);

            Binding tb;
            //左上
            tb = new(nameof(MyLeft));
            tb.Source = this;
            TTopLeft.SetBinding(Canvas.LeftProperty, tb);
            tb = new(nameof(MyTop));
            tb.Source = this;
            TTopLeft.SetBinding(Canvas.TopProperty, tb);
            //右上
            tb = new(nameof(MyRight));
            tb.Source = this;
            TTopRight.SetBinding(Canvas.LeftProperty, tb);
            tb = new(nameof(MyTop));
            tb.Source = this;
            TTopRight.SetBinding(Canvas.TopProperty, tb);
            //左下
            tb = new(nameof(MyLeft));
            tb.Source = this;
            TBottomLeft.SetBinding(Canvas.LeftProperty, tb);
            tb = new(nameof(MyBottom));
            tb.Source = this;
            TBottomLeft.SetBinding(Canvas.TopProperty, tb);
            //右下
            tb = new(nameof(MyRight));
            tb.Source = this;
            TBottomRight.SetBinding(Canvas.LeftProperty, tb);
            tb = new(nameof(MyBottom));
            tb.Source = this;
            TBottomRight.SetBinding(Canvas.TopProperty, tb);


        }

        private void TBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyLeft += e.HorizontalChange;
            MyBottom += e.VerticalChange;
        }

        private void TBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyRight += e.HorizontalChange;
            MyBottom += e.VerticalChange;
        }

        private void TTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyRight += e.HorizontalChange;
            MyTop += e.VerticalChange;
        }

        private void TTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;
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
        }
    }
}
