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
using System.ComponentModel;
using System.Globalization;


//XAMLメインで書いてみたけど、MultiBindingのところをXAMLで書いたらデザイン画面が表示されない
//それでも動く
namespace _20220109_RectangleBinding2_XAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
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

            TTopLeft.DragDelta += TTopLeft_DragDelta;
            TTopRight.DragDelta += TTopRight_DragDelta;
            TBottomRight.DragDelta += TBottomRight_DragDelta;
            TBottomLeft.DragDelta += TBottomLeft_DragDelta;


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

