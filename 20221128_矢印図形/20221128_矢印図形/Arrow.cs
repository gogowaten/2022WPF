using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

//Shapeを継承して矢印クラス

//参照したところ
//    WPF Arrow and Custom Shapes - CodeProject
//https://www.codeproject.com/Articles/23116/WPF-Arrow-and-Custom-Shapes
//DefiningGeometry | 2,000 Things You Should Know About WPF
//https://wpf.2000things.com/tag/defininggeometry/


//        図形コントロール
//http://www.kanazawa-net.ne.jp/~pmansato/wpf/wpf_graph_drawtool.htm#arrow

//affectsを付けるとデザイン画面で数値変更したときに即表示が更新されるようになる
//Renderは表示更新
//Measureはサイズ変更

namespace _20221128_矢印図形
{
    internal class Arrow : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                StreamGeometry geometry = new();
                //geometry.FillRule = FillRule.EvenOdd;//いる？
                geometry.FillRule = FillRule.Nonzero;//こっちのほうがいいかも
                using (var context = geometry.Open())
                {
                    InternalDraw2(context);
                    //InternalDraw(context);
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

            Point pt1 = new(X1, Y1);
            Point pt2 = new(X2, Y2);

            Point pt3 = new Point(
                X2 + (HeadSize * cost - HeadSize * sint),
                Y2 + (HeadSize * sint + HeadSize * cost));

            Point pt4 = new Point(
                X2 + (HeadSize * cost + HeadSize * sint),
                Y2 - (HeadSize * cost - HeadSize * sint));

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            //context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);
            context.LineTo(pt2, true, true);


            //Point pt11 = new(pt1.X + 10, pt1.Y + 10);
            //Point pt12 = new(pt2.X + 10, pt2.Y + 10);
            //context.BeginFigure(pt11, true, false);
            //context.LineTo(pt12, true, true);
        }
        private void InternalDraw2(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1); Point p2 = new(X2, Y2);
            double beseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double arrowRadian = AngleToRadian(Angle);
            double radian = beseRadian + arrowRadian;

            double sin = Math.Sin(radian);
            double cos = Math.Cos(radian);
            double x = HeadSize * cos;
            double y = HeadSize * sin;
            Point p3 = new Point(X2 + x, Y2 + y);

            radian = beseRadian - arrowRadian;
            sin = Math.Sin(radian);
            cos = Math.Cos(radian);
            x = HeadSize * cos;
            y = HeadSize * sin;
            Point p4 = new Point(X2 + x, Y2 + y);

            context.BeginFigure(p1, true, false);
            context.LineTo(p2, true, true);
            context.LineTo(p3, true, true);
            context.LineTo(p4, true, true);


        }
        private double AngleToRadian(double angle)
        {
            return angle / 360 * (Math.PI * 2.0);
        }

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(nameof(X1), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender));


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
            DependencyProperty.Register(nameof(X2), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double HeadSize
        {
            get { return (double)GetValue(HeadSizeProperty); }
            set { SetValue(HeadSizeProperty, value); }
        }
        public static readonly DependencyProperty HeadSizeProperty =
            DependencyProperty.Register(nameof(HeadSize), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

    }
}
