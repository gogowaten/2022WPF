using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(LLine), new PropertyMetadata(Brushes.Black));

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
            var rectSize = MyBegin.RenderSize;
            var lineGeo = MyLine.RenderedGeometry;
            var rectGeo = MyBegin.RenderedGeometry;
            var neko = Geometry.Combine(lineGeo, rectGeo, GeometryCombineMode.Union, null);
            var tftv = MyBegin.TransformToVisual(this);
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



    //    WPF Arrow and Custom Shapes - CodeProject
    //https://www.codeproject.com/Articles/23116/WPF-Arrow-and-Custom-Shapes

    public class Arrow : Shape
    {
        #region dp
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(nameof(X1), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));
//        図形コントロール
//http://www.kanazawa-net.ne.jp/~pmansato/wpf/wpf_graph_drawtool.htm#arrow

        //affectsを付けるとデザイン画面で数値変更したときに即表示が更新されるようになる
        //Renderは表示更新
        //Measureはサイズ変更
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(nameof(Y1), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(nameof(X2), typeof(double), typeof(Arrow), new PropertyMetadata(0.0));

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double), typeof(Arrow), new PropertyMetadata(0.0));

        [TypeConverter(typeof(LengthConverter))]
        public double HeadSize
        {
            get { return (double)GetValue(HeadSizeProperty); }
            set { SetValue(HeadSizeProperty, value); }
        }
        public static readonly DependencyProperty HeadSizeProperty =
            DependencyProperty.Register(nameof(HeadSize), typeof(double), typeof(Arrow), new PropertyMetadata(0.0));
        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                StreamGeometry geometry = new();
                geometry.FillRule = FillRule.EvenOdd;
                using (var context = geometry.Open())
                {
                    InternalDraw(context);
                }
                geometry.Freeze();
                return geometry;
            }
        }
        private void InternalDraw(StreamGeometryContext context)
        {
            double theta = Math.Atan2(Y1 - Y2, X1 - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pt1 = new Point(X1, this.Y1);
            Point pt2 = new Point(X2, this.Y2);

            Point pt3 = new Point(
                X2 + (HeadSize * cost - HeadSize * sint),
                Y2 + (HeadSize * sint + HeadSize * cost));

            Point pt4 = new Point(
                X2 + (HeadSize * cost + HeadSize * sint),
                Y2 - (HeadSize * cost - HeadSize * sint));

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);
        }
    }
}
