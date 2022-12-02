using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _20221128
{
    public enum ArrowHeadType { Type0, Type1, Type2, Type3, Type4, Type5 }
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
                            StrokeStartLineCap = PenLineCap.Flat;
                            StrokeEndLineCap = PenLineCap.Flat;
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
                        case ArrowHeadType.Type3:
                            InternalDraw3(context);
                            //StrokeStartLineCap = PenLineCap.Round;
                            //StrokeEndLineCap = PenLineCap.Round;
                            Fill = Stroke;
                            break;
                        case ArrowHeadType.Type4:
                            InternalDraw4(context);
                            Fill = Stroke;
                            break;
                        case ArrowHeadType.Type5:
                            InternalDraw5(context);
                            Fill = Stroke;
                            break;
                        default:
                            InternalDraw0(context);
                            break;
                    }
                }
                geometry.Freeze();
                //UseLayoutRounding = true;
                return geometry;
            }
        }

        //
        private void InternalDraw0(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            //座標を0.5シフト、ぼやけ防止
            //Point p1 = new(X1 - 0.5, Y1 - 0.5);
            Point p2 = new(X2, Y2);

            //直線の角度
            double baseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double bCos = Math.Cos(baseRadian);
            double bSin = Math.Sin(baseRadian);

            //指定された角度
            double radian = AngleToRadian(Angle);
            double rCos = Math.Cos(radian);

            //
            double arrowHeadRadian = baseRadian + radian;//矢じりの角度

            //直線と矢じりの接点
            double zentai = HeadSize * rCos;
            Point pJoint = new(X2 + (bCos * zentai), Y2 + bSin * zentai);
            //座標を0.5シフト、ぼやけ防止
            //Point pJoint = new(X2 - 0.5 + (bCos * zentai), Y2 - 0.5 + bSin * zentai);


            Point p3 = new(
                X2 + HeadSize * Math.Cos(arrowHeadRadian),
                Y2 + HeadSize * Math.Sin(arrowHeadRadian));

            arrowHeadRadian = baseRadian - radian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(arrowHeadRadian),
                Y2 + HeadSize * Math.Sin(arrowHeadRadian));

            //直線描画、Strokeで描画
            context.BeginFigure(p1, true, false);
            context.LineTo(pJoint, true, true);
            //矢じり描画、StrokeじゃなくてFillで描画
            context.BeginFigure(p2, true, true);//point, isFill, isClose
            context.LineTo(p3, false, false);//point, isStroke, isSmoothJoin
            context.LineTo(p4, false, true);
            context.LineTo(p2, false, true);
        }
        //Fillで直線部分だけ描画
        private void InternalDraw1(StreamGeometryContext context)
        {
            double baseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double verticalRadian = baseRadian + Math.PI / 2.0;
            double vCos = Math.Cos(verticalRadian);
            double vSin = Math.Sin(verticalRadian);

            double halfWidth = StrokeThickness / 2.0;
            double dCos = halfWidth * vCos;
            double dSin = halfWidth * vSin;

            Point p1 = new(X1 + dCos, Y1 + dSin);
            Point p2 = new(X2 + dCos, Y2 + dSin);
            Point p3 = new(X2 - dCos, Y2 - dSin);
            Point p4 = new(X1 - dCos, Y1 - dSin);

            context.BeginFigure(p1, true, false);
            context.LineTo(p2, false, false);
            context.LineTo(p3, false, false);
            context.LineTo(p4, false, false);
            context.LineTo(p1, false, false);

        }

        //Fillで矢印描画
        private void InternalDraw2(StreamGeometryContext context)
        {
            //直線部分(始点から終点)のラジアン
            double baseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            //直線部分のラジアンに垂直(直角)なラジアン
            double verticalRadian = baseRadian + Math.PI / 2.0;
            //指定された角度のラジアン
            double orderRadian = AngleToRadian(Angle);
            //矢じりの辺の長さ
            double sideLength = HeadSize * Math.Cos(orderRadian);
            //終点(X2,Y2)と接合部座標の差            
            double jointXDiff = sideLength * Math.Cos(baseRadian);
            double jointYDiff = sideLength * Math.Sin(baseRadian);

            double halfWidth = StrokeThickness / 2.0;
            double xDiff = halfWidth * Math.Cos(verticalRadian);
            double yDiff = halfWidth * Math.Sin(verticalRadian);

            //直線に対する矢じりのラジアン
            double arrowHeadRadian1 = baseRadian + orderRadian;
            //矢じりの翼端座標
            double wing1x = X2 + HeadSize * Math.Cos(arrowHeadRadian1);
            double wing1y = Y2 + HeadSize * Math.Sin(arrowHeadRadian1);
            //反対側の矢じりの翼端座標
            double arrowHeadRadian2 = baseRadian - orderRadian;
            double wing2x = X2 + HeadSize * Math.Cos(arrowHeadRadian2);
            double wing2y = Y2 + HeadSize * Math.Sin(arrowHeadRadian2);

            Point p1 = new(X1 + xDiff, Y1 + yDiff);
            Point p2 = new(X2 + jointXDiff + xDiff, Y2 + jointYDiff + yDiff);
            Point pWing1 = new(wing1x, wing1y);
            Point pNose = new(X2, Y2);//機首(終端)座標
            Point pWing2 = new(wing2x, wing2y);
            Point p3 = new(X2 + jointXDiff - xDiff, Y2 + jointYDiff - yDiff);
            Point p4 = new(X1 - xDiff, Y1 - yDiff);

            context.BeginFigure(p1, true, false);
            context.LineTo(p2, false, false);
            context.LineTo(pWing1, false, false);
            context.LineTo(pNose, false, false);
            context.LineTo(pWing2, false, false);
            context.LineTo(p3, false, false);
            context.LineTo(p4, false, false);
            context.LineTo(p1, false, false);

        }

        //直線を伸ばしてつなぎ目問題解決
        //矢じりの大きさを極端に小さく(4未満)すると直線がはみ出る不具合が出るけど
        //実質的には問題ない
        //Fillで描画するよりラクだし直線部分をベジェ曲線にするとかの応用も効く
        private void InternalDraw3(StreamGeometryContext context)
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

            //直線と矢じりの接点
            double zentai = HeadSize * rCos;
            zentai -= 1.5;//伸ばす
            Point pContact = new(X2 + (bCos * zentai), Y2 + bSin * zentai);

            double arrowHeadRadian = baseRadian + radian;//矢じりの角度
            Point p3 = new(
                X2 + HeadSize * Math.Cos(arrowHeadRadian),
                Y2 + HeadSize * Math.Sin(arrowHeadRadian));

            arrowHeadRadian = baseRadian - radian;//反対側の矢じりの角度
            Point p4 = new(
                X2 + HeadSize * Math.Cos(arrowHeadRadian),
                Y2 + HeadSize * Math.Sin(arrowHeadRadian));

            //直線描画、Strokeで描画
            context.BeginFigure(p1, true, false);
            context.LineTo(pContact, true, true);
            //矢じり描画、StrokeじゃなくてFillで描画
            context.BeginFigure(p2, true, true);//point, isFill, isClose
            context.LineTo(p3, false, false);//point, isStroke, isSmoothJoin
            context.LineTo(p4, false, false);
            context.LineTo(p2, false, false);
        }

        //矢印図形描画、ヘッドサイズ自動調整版
        private void InternalDraw4(StreamGeometryContext context)
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

        //先端が四角形直線
        private void InternalDraw5(StreamGeometryContext context)
        {
            Point p1 = new(X1, Y1);
            Point p2 = new(X2, Y2);

            //直線の角度
            double baseRadian = Math.Atan2(Y1 - Y2, X1 - X2);
            double bCos = Math.Cos(baseRadian);
            double bSin = Math.Sin(baseRadian);

            //ヘッドサイズ自動調整、線の太さと矢じり角度によって変化させる            
            HeadSize = StrokeThickness;

            //直線と矢じりは別々に描画している
            //直線と矢じりの接点座標が必要
            //接点は矢じり側に多少シフトさせて、重なる部分ができるようにしている、
            //もしぴったり座標の場合は隙間ができてしまう
            //座標シフト、矢じりを短く見せかければ、その分直線が伸びる
            double sideLength = HeadSize - 1.5;
            Point pContact = new(X2 + (bCos * sideLength), Y2 + bSin * sideLength);

            //角度は直線に対して直角
            double verticalRadian = baseRadian - Math.PI / 2.0;
            double xDiff = HeadSize * Math.Cos(verticalRadian);
            double yDiff = HeadSize * Math.Sin(verticalRadian);
            //座標
            Point p3 = new(X2 + xDiff, Y2 + yDiff);
            Point p4 = new(X2 - xDiff, Y2 - yDiff);
            xDiff = HeadSize * 2 * Math.Cos(baseRadian);
            yDiff = HeadSize * 2 * Math.Sin(baseRadian);
            Point p5 = new(p3.X + xDiff, p3.Y + yDiff);
            Point p6 = new(p4.X + xDiff, p4.Y + yDiff);

            //直線描画はStrokeで描画
            context.BeginFigure(p1, true, false);
            context.LineTo(pContact, true, true);
            //矢じり描画はFillで描画。もしStrokeで描画すると先端部分が伸びてしまい、見た目が指定座標とずれる
            context.BeginFigure(p3, true, true);//point, isFill, isClose
            //context.LineTo(p3, false, false);//point, isStroke, isSmoothJoin
            context.LineTo(p4, false, false);
            context.LineTo(p6, false, false);
            context.LineTo(p5, false, false);
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
