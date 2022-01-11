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

namespace _20220107
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
        //Rect MyRect;
        //Rectangle MyRectangle;
        //RectangleGeometry MyRectangleGeometry;
        RectThumb MyRectThumb = new();
        //Path MyPath = new();
        private readonly Thumb TTopLeft = new() { Width = 10, Height = 10 };
        private readonly Thumb TTopRight = new() { Width = 10, Height = 10 };
        private readonly Thumb TBottomRight = new() { Width = 10, Height = 10 };
        private readonly Thumb TBottomLeft = new() { Width = 10, Height = 10 };

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

            MyCanvas.Children.Add(MyRectThumb);
            MyStackPnel.DataContext = MyRectThumb;
            MyRectThumb.MyFill = Brushes.MediumOrchid;
            MyRectThumb.MyHandleSize = 20;
            MyRectThumb.MyStrokeThickness = 5;
            MyRectThumb.MyStroke = Brushes.Orange;



        }

        private void MySThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.OriginalSource != e.Source) { return; }
            RectThumb st = sender as RectThumb;
            Canvas.SetLeft(st, Canvas.GetLeft(st) + e.HorizontalChange);
            Canvas.SetTop(st, Canvas.GetTop(st) + e.VerticalChange);
        }

        private void Test1()
        {

            //MyStackPnel.DataContext = this;
            //MyCanvas.DataContext = this;
            //MyCanvas.Children.Add(MyPath);
            //MyPath.Fill = Brushes.MediumAquamarine;
            //Canvas.SetLeft(MyPath, 0);
            //Canvas.SetTop(MyPath, 0);


            //Binding bLeft = new(nameof(MyLeft));
            //bLeft.Source = this;
            //Binding bTop = new(nameof(MyTop));
            //bTop.Source = this;
            //Binding bRight = new(nameof(MyRight));
            //bRight.Source = this;
            //Binding bBottom = new(nameof(MyBottom));
            //bBottom.Source = this;
            //MultiBinding mb = new();
            //mb.Bindings.Add(bLeft);
            //mb.Bindings.Add(bTop);
            //mb.Bindings.Add(bRight);
            //mb.Bindings.Add(bBottom);
            //mb.Converter = new MyRectConverter();
            //MyPath.SetBinding(Path.DataProperty, mb);


            //TTopLeft.DragDelta += TTopLeft_DragDelta;
            //TTopRight.DragDelta += TTopRight_DragDelta;
            //TBottomRight.DragDelta += TBottomRight_DragDelta;
            //TBottomLeft.DragDelta += TBottomLeft_DragDelta;

            //MyCanvas.Children.Add(TTopLeft);
            //MyCanvas.Children.Add(TTopRight);
            //MyCanvas.Children.Add(TBottomLeft);
            //MyCanvas.Children.Add(TBottomRight);


            ////左上
            //TTopLeft.SetBinding(Canvas.LeftProperty, bLeft);
            //TTopLeft.SetBinding(Canvas.TopProperty, bTop);
            ////右上
            //TTopRight.SetBinding(Canvas.LeftProperty, bRight);
            //TTopRight.SetBinding(Canvas.TopProperty, bTop);
            ////左下
            //TBottomLeft.SetBinding(Canvas.LeftProperty, bLeft);
            //TBottomLeft.SetBinding(Canvas.TopProperty, bBottom);
            ////右下
            //TBottomRight.SetBinding(Canvas.LeftProperty, bRight);
            //TBottomRight.SetBinding(Canvas.TopProperty, bBottom);

            //MyPath.SetBinding(Canvas.LeftProperty, bLeft);
            //MyPath.SetBinding(Canvas.TopProperty, bTop);

        }

        private void TBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //MyLeft += e.HorizontalChange;
            //MyBottom += e.VerticalChange;
            double left = myLeft + e.HorizontalChange;
            if (left < myRight) { MyLeft = left; }
            double bottom = MyBottom + e.VerticalChange;
            if (bottom > myTop) { MyBottom = bottom; }
        }

        private void TBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //MyRight += e.HorizontalChange;
            //MyBottom += e.VerticalChange;
            double right = MyRight + e.HorizontalChange;
            if (right > myLeft) { MyRight = right; }
            double bottom = MyBottom + e.VerticalChange;
            if (bottom > myTop) { MyBottom = bottom; }
        }

        private void TTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //MyRight += e.HorizontalChange;
            //MyTop += e.VerticalChange;
            double right = MyRight + e.HorizontalChange;
            if (right > myLeft) { MyRight = right; }
            double top = MyTop + e.VerticalChange;
            if (top < myBottom) { MyTop = top; }
        }

        private void TTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double left = myLeft + e.HorizontalChange;
            if (left < myRight) { MyLeft = left; }
            double top = MyTop + e.VerticalChange;
            if (top < myBottom) { MyTop = top; }

            //MyLeft += e.HorizontalChange;
            //MyTop += e.VerticalChange;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(MyRectThumb.GetRect().ToString());
          
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
            return new RectangleGeometry(new Rect(new Point(0, 0), new Point(right - left, bottom - top)));
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
    public class MyMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double v1 = (double)values[0];
            double v2 = (double)values[1];
            return v1 + v2;
            //throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            double w = (double)value;
            double left = (double)parameter;
            object[] o = new object[1];
            o[0] = left;
            o[1] = w + left;
            return o;
            //throw new NotImplementedException();
        }
    }
}
