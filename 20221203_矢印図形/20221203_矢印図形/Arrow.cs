using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace _20221203_矢印図形
{
    public enum ArrowHeadType { Type0, Type1, Type2 }
    class Arrow : Shape
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
                            //StrokeStartLineCap = PenLineCap.Flat;
                            //StrokeEndLineCap = PenLineCap.Flat;
                            Fill = Stroke;
                            break;
                        case ArrowHeadType.Type1:
                            InternalDraw1(context);
                            Fill = Stroke;
                            break;
                        case ArrowHeadType.Type2:
                            InternalDraw2(context);
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


        //矢印図形描画、ヘッドサイズ自動調整版
        private void InternalDraw0(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            Point p2 = new(X2, Y2);

            //直線の角度
            double baseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double bCos = Math.Cos(baseRadian);
            double bSin = Math.Sin(baseRadian);

            //指定された角度
            double radian = AngleToRadian(Angle);
            double rCos = Math.Cos(radian);
            //ヘッドサイズ自動調整、線の太さと矢じり角度によって変化させる            
            HeadSize = StrokeThickness * 1.0 / Math.Sin(radian);
            //線の太さが1とか細かったり、矢じり指定角度が大きい(60度以上)ときは下の方がよさそう
            //HeadSize = StrokeThickness * 2.0 / Math.Sin(radian);

            //直線と矢じりは別々に描画している
            //直線と矢じりの接点座標が必要
            //接点は矢じり側に多少シフトさせて、重なる部分ができるようにしている、
            //もしぴったり座標の場合は隙間ができてしまう
            //座標シフト、矢じりを短く見せかければ、その分直線が伸びる
            double sideLength = HeadSize * rCos - 1.5;
            Point pContact = new(X2 + (bCos * sideLength), Y2 + bSin * sideLength);

            double arrowHeadRadian = baseRadian + radian;//矢じりの角度
            //矢じりの翼端座標
            Point p3 = new(
                X2 + HeadSize * Math.Cos(arrowHeadRadian),
                Y2 + HeadSize * Math.Sin(arrowHeadRadian));

            arrowHeadRadian = baseRadian - radian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(arrowHeadRadian),
                Y2 + HeadSize * Math.Sin(arrowHeadRadian));

            //直線描画はStrokeで描画
            context.BeginFigure(p1, true, false);
            context.LineTo(pContact, true, true);
            //矢じり描画はFillで描画。もしStrokeで描画すると先端部分が伸びてしまい、見た目が指定座標とずれる
            context.BeginFigure(p2, true, true);//point, isFill, isClose
            context.LineTo(p3, false, false);//point, isStroke, isSmoothJoin
            context.LineTo(p4, false, false);
            context.LineTo(p2, false, false);
        }

        //先端が四角形直線、自動調整版
        private void InternalDraw1(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);

            //直線の角度
            double baseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double bCos = Math.Cos(baseRadian);
            double bSin = Math.Sin(baseRadian);

            //ヘッドサイズ自動調整、線の太さによって変化させる
            HeadSize = StrokeThickness;

            //直線と先端図形は別々に描画している
            //直線と図形の接点座標が必要
            double sideLength = HeadSize - 1.5;
            Point pContact = new(X2 + (bCos * sideLength), Y2 + bSin * sideLength);

            //角度は直線に対して直角
            double verticalRadian = baseRadian - Math.PI / 2.0;
            double xDiff = HeadSize * Math.Cos(verticalRadian);
            double yDiff = HeadSize * Math.Sin(verticalRadian);
            //四角形の四隅の座標
            Point p3 = new(X2 + xDiff, Y2 + yDiff);
            Point p4 = new(X2 - xDiff, Y2 - yDiff);
            xDiff = HeadSize * 2 * Math.Cos(baseRadian);
            yDiff = HeadSize * 2 * Math.Sin(baseRadian);
            Point p5 = new(p3.X + xDiff, p3.Y + yDiff);
            Point p6 = new(p4.X + xDiff, p4.Y + yDiff);

            //直線描画はStrokeで描画
            context.BeginFigure(p1, true, false);
            context.LineTo(pContact, true, true);
            //図形描画はFillで描画。もしStrokeで描画すると先端部分が伸びてしまい、見た目が指定座標とずれる
            context.BeginFigure(p3, true, true);//point, isFill, isClose
            //context.LineTo(p3, false, false);//point, isStroke, isSmoothJoin
            context.LineTo(p4, false, false);
            context.LineTo(p6, false, false);
            context.LineTo(p5, false, false);
            context.LineTo(p3, false, false);
        }

        //先端図形が円、自動調整版
        private void InternalDraw2(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);

            //直線の角度
            double baseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double bCos = Math.Cos(baseRadian);
            double bSin = Math.Sin(baseRadian);

            //先端図形サイズ自動調整、線の太さによって変化させる
            //線の太さを円の半径にしてみた
            double radius = StrokeThickness / 2.0;

            //直線と先端図形は別々に描画している
            //円の中心座標
            double sideLength = radius * 2.0;
            Point pContact = new(X2 + (bCos * sideLength), Y2 + bSin * sideLength);

            //直線描画はStrokeで描画
            context.BeginFigure(p1, true, false);
            context.LineTo(pContact, true, true);
            //円はArcよりBezierのほうが効率がいいらしい、近似だけど全く問題ない
            //How to draw a full ellipse in a StreamGeometry in WPF? - Stack Overflow
            //https://stackoverflow.com/questions/2979834/how-to-draw-a-full-ellipse-in-a-streamgeometry-in-wpf
            //Fillで描画。もしStrokeで描画するとStrokeの幅分が伸びてしまい、見た目が指定座標とずれる
            double controlPointRatio = (Math.Sqrt(2) - 1) * 4 / 3;//制御点
            double cx = pContact.X;
            double c0 = cx - radius;
            double c1 = cx - radius * controlPointRatio;
            double c2 = cx;
            double c3 = cx + radius * controlPointRatio;
            double c4 = cx + radius;

            double cy = pContact.Y;
            double s0 = cy - radius;
            double s1 = cy - radius * controlPointRatio;
            double s2 = cy;
            double s3 = cy + radius * controlPointRatio;
            double s4 = cy + radius;

            context.BeginFigure(new Point(c2, s0), true, false);
            context.BezierTo(new(c3, s0), new(c4, s1), new(c4, s2), true, true);
            context.BezierTo(new(c4, s3), new(c3, s4), new(c2, s4), true, true);
            context.BezierTo(new(c1, s4), new(c0, s3), new(c0, s2), true, true);
            context.BezierTo(new(c0, s1), new(c1, s0), new(c2, s0), true, true);

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
