using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace _20221121
{
    /// <summary>
    /// LLine.xaml の相互作用ロジック
    /// </summary>
    public partial class LLine : UserControl
    {
        public LLine()
        {
            InitializeComponent();
        }
        #region dp
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(LLine), new PropertyMetadata(Brushes.Black));

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(nameof(X1), typeof(double), typeof(LLine), new PropertyMetadata(0.0));

        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(nameof(Y1), typeof(double), typeof(LLine), new PropertyMetadata(0.0));

        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(nameof(X2), typeof(double), typeof(LLine), new PropertyMetadata(0.0));

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double), typeof(LLine), new PropertyMetadata(0.0));

        public double HeadSize
        {
            get { return (double)GetValue(HeadSizeProperty); }
            set { SetValue(HeadSizeProperty, value); }
        }
        public static readonly DependencyProperty HeadSizeProperty =
            DependencyProperty.Register(nameof(HeadSize), typeof(double), typeof(LLine), new PropertyMetadata(0.0));
        #endregion

        public void Test1()
        {
            var height = MyLine.ActualHeight;
            var lineSize = MyLine.RenderSize;
            var rectSize = MyRectangle.RenderSize;
            var lineGeo = MyLine.RenderedGeometry;
            var rectGeo = MyRectangle.RenderedGeometry;
            var neko = Geometry.Combine(lineGeo, rectGeo, GeometryCombineMode.Union, null);
            var tftv = MyRectangle.TransformToVisual(this);
        }

        private void MyLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = MyLine.ActualWidth;
            this.Height = MyLine.ActualHeight;
        }
    }

    public class PointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {   
            var locate = values[0] as double?;
            var size = values[1] as double?;

            if (locate != null && size != null) { return locate - (size / 2.0); }
            else return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PP : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var ll = (LLine)parameter;
            var v = (double)values[0];
            return 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PCon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var locate = (double)value;
            var ll = (LLine)parameter;
            return locate - (ll.HeadSize / 2.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
