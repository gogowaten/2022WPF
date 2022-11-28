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

public enum ArrowHeadType { Type0, Type1, Type2, Type3 }

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
                    switch (HeadType)
                    {
                        case ArrowHeadType.Type0:
                            InternalDraw0(context);
                            StrokeStartLineCap = PenLineCap.Flat;
                            StrokeEndLineCap = PenLineCap.Flat;
                            Fill = null;
                            break;
                        case ArrowHeadType.Type1:
                            InternalDraw1(context);
                            Fill = null;
                            StrokeStartLineCap = PenLineCap.Round;
                            StrokeEndLineCap = PenLineCap.Round;
                            break;
                        case ArrowHeadType.Type2:
                            InternalDraw2(context);
                            StrokeStartLineCap = PenLineCap.Flat;
                            StrokeEndLineCap = PenLineCap.Flat;
                            Fill = Stroke;
                            break;
                        case ArrowHeadType.Type3:
                            InternalDraw3(context);
                            StrokeStartLineCap = PenLineCap.Round;
                            StrokeEndLineCap = PenLineCap.Round;
                            Fill = Stroke;
                            break;
                        default:
                            InternalDraw0(context);
                            break;
                    }
                }
                geometry.Freeze();
                return geometry;
            }
        }

        //矢じりが直線＆フラット
        private void InternalDraw0(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            Point p2 = new(X2, Y2);

            double beseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double headRadian = AngleToRadian(Angle);

            double radian = beseRadian + headRadian;//矢じりの角度
            Point p3 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            radian = beseRadian - headRadian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            context.BeginFigure(p1, true, false);
            context.LineTo(p2, true, true);
            context.BeginFigure(p3, true, false);
            context.LineTo(p2, true, false);
            context.LineTo(p4, true, true);
        }
        //矢じりが直線、端丸め
        private void InternalDraw1(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            Point p2 = new(X2, Y2);

            double beseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double headRadian = AngleToRadian(Angle);

            double radian = beseRadian + headRadian;//矢じりの角度
            Point p3 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            radian = beseRadian - headRadian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            context.BeginFigure(p1, true, false);
            context.LineTo(p2, true, true);
            context.BeginFigure(p3, true, false);
            context.LineTo(p2, true, true);
            context.LineTo(p4, true, true);
        }

        //矢じりが三角形＆端フラット鋭角
        private void InternalDraw2(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            Point p2 = new(X2, Y2);

            double beseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double headRadian = AngleToRadian(Angle);

            double radian = beseRadian + headRadian;//矢じりの角度
            Point p3 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            radian = beseRadian - headRadian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            context.BeginFigure(p1, true, false);
            context.LineTo(p2, true, false);
            context.BeginFigure(p2, true, false);
            context.LineTo(p3, true, false);
            context.LineTo(p4, true, false);
            context.LineTo(p2, true, false);
            context.LineTo(p3, true, false);
        }

        //矢じりが三角線＆端丸め
        private void InternalDraw3(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            Point p2 = new(X2, Y2);

            double beseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double headRadian = AngleToRadian(Angle);

            double radian = beseRadian + headRadian;//矢じりの角度
            Point p3 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            radian = beseRadian - headRadian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            context.BeginFigure(p1, true, false);
            context.LineTo(p2, true, true);
            context.LineTo(p3, true, true);
            context.LineTo(p4, true, true);
            context.LineTo(p2, true, true);
        }

        //未使用
        private void InternalDraw4(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            Point p2 = new(X2, Y2);

            double beseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double headRadian = AngleToRadian(Angle);

            double radian = beseRadian + headRadian;//矢じりの角度
            Point p3 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            radian = beseRadian - headRadian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(radian),
                Y2 + HeadSize * Math.Sin(radian));

            context.BeginFigure(p1, false, false);
            context.LineTo(p2, true, false);
            context.BeginFigure(p2, true, false);
            context.LineTo(p3, false, false);
            context.LineTo(p4, false, false);
            context.LineTo(p2, false, false);
            context.LineTo(p3, false, false);
        }

        private static double AngleToRadian(double angle)
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



        public ArrowHeadType HeadType
        {
            get { return (ArrowHeadType)GetValue(HeadTypeProperty); }
            set { SetValue(HeadTypeProperty, value); }
        }
        public static readonly DependencyProperty HeadTypeProperty =
            DependencyProperty.Register(nameof(HeadType), typeof(ArrowHeadType), typeof(Arrow),
                new FrameworkPropertyMetadata(ArrowHeadType.Type1,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

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
