using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace _20221201_矢印図形Fillで描画
{
    public enum ArrowHeadType { Type0, Type1, Type2, Type3 }
    internal class Arrow : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                StreamGeometry geometry = new();
                geometry.FillRule = FillRule.Nonzero;//こっちのほうがいいかも
                using (var context = geometry.Open())
                {
                    switch (HeadType)
                    {
                        case ArrowHeadType.Type0:
                            InternalDraw0(context);
                            break;
                        case ArrowHeadType.Type1:
                            InternalDraw1(context);
                            break;
                        case ArrowHeadType.Type2:
                            InternalDraw2(context);
                            break;
                        case ArrowHeadType.Type3:
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

        //Fillで直線部分だけ描画
        private void InternalDraw0(StreamGeometryContext context)
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
        private void InternalDraw1(StreamGeometryContext context)
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

            //各座標作成
            Point p1 = new(X1 + xDiff, Y1 + yDiff);
            Point p2 = new(X2 + jointXDiff + xDiff, Y2 + jointYDiff + yDiff);
            Point pWing1 = new(wing1x, wing1y);
            Point pNose = new(X2, Y2);//機首(終端)座標
            Point pWing2 = new(wing2x, wing2y);
            Point p3 = new(X2 + jointXDiff - xDiff, Y2 + jointYDiff - yDiff);
            Point p4 = new(X1 - xDiff, Y1 - yDiff);

            //描画
            context.BeginFigure(p1, true, false);//point, isFill, isClose
            context.LineTo(p2, false, false);//point, isStroke, isSmoothJoint
            context.LineTo(pWing1, false, false);
            context.LineTo(pNose, false, false);
            context.LineTo(pWing2, false, false);
            context.LineTo(p3, false, false);
            context.LineTo(p4, false, false);
            //context.LineTo(p1, false, false);//不要？

        }
        //Strokeで矢印描画
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

            //各座標作成
            Point p1 = new(X1 + xDiff, Y1 + yDiff);
            Point p2 = new(X2 + jointXDiff + xDiff, Y2 + jointYDiff + yDiff);
            Point pWing1 = new(wing1x, wing1y);
            Point pNose = new(X2, Y2);//機首(終端)座標
            Point pWing2 = new(wing2x, wing2y);
            Point p3 = new(X2 + jointXDiff - xDiff, Y2 + jointYDiff - yDiff);
            Point p4 = new(X1 - xDiff, Y1 - yDiff);

            //描画
            context.BeginFigure(p1, true, false);//point, isFill, isClose
            context.LineTo(p2, true, false);//point, isStroke, isSmoothJoint
            context.LineTo(pWing1, true, false);
            context.LineTo(pNose, true, false);
            context.LineTo(pWing2, true, false);
            context.LineTo(p3, true, false);
            context.LineTo(p4, true, false);
            //context.LineTo(p1, false, false);//不要？

        }

        //角度をラジアンに変換
        private static double AngleToRadian(double angle)
        {
            return angle / 360 * (Math.PI * 2.0);
        }


        #region 依存プロパティ

        //始点横座標
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

        //始点縦座標
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
        //終点横座標
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
        //終点縦座標
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

        //矢じりの大きさ
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

        //矢じりの角度
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

        //矢じりの形選択用
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

        #endregion

    }
}


