using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace _20221204_矢印図形
{
    public enum MyEnum { Test0, Test1 }
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
                    switch (TestType)
                    {
                        case MyEnum.Test0:
                            InternalDraw0(context);
                            StrokeStartLineCap = PenLineCap.Flat;
                            StrokeEndLineCap = PenLineCap.Flat;
                            Fill = Stroke;
                            break;
                        case MyEnum.Test1:
                            InternalDraw1(context);
                            Fill = Stroke;
                            break;
                        default:
                            break;
                    }

                }
                geometry.Freeze();
                //UseLayoutRounding = true;
                return geometry;
            }
        }


        //直線をくっきり描画、Stroke編
        private void InternalDraw0(StreamGeometryContext context)
        {
            double baseRadian = Math.Atan2(Y2 - Y1, X2 - X1);
            double xDiff = 0.5 * Math.Sin(baseRadian);
            double yDiff = 0.5 * Math.Cos(baseRadian);
            //Stroke
            context.BeginFigure(new(X1 + xDiff, Y1 + yDiff), false, false);
            context.LineTo(new(X2 + xDiff, Y2 + yDiff), true, false);

        }


        //Fillで直線部分だけ普通に描画は、ぼやける
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

        public MyEnum TestType
        {
            get { return (MyEnum)GetValue(TestTypeProperty); }
            set { SetValue(TestTypeProperty, value); }
        }
        public static readonly DependencyProperty TestTypeProperty =
            DependencyProperty.Register(nameof(TestType), typeof(MyEnum), typeof(Arrow),
                new FrameworkPropertyMetadata(MyEnum.Test0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

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


    }
}
