using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

//矢印クラスをTemplateにしたThumb

namespace _20221127_図形矢印Thumb
{
    internal class TThumbArrow : Thumb
    {

        public double TX1
        {
            get { return (double)GetValue(TX1Property); }
            set { SetValue(TX1Property, value); }
        }
        public static readonly DependencyProperty TX1Property =
            DependencyProperty.Register(nameof(TX1), typeof(double), typeof(TThumbArrow),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double TY1 { get; set; }
        public double TX2 { get; set; }
        public double TY2 { get; set; }
        public double THeadSize { get; set; }
        public Brush? TStroke { get; set; }
        public double StrokeWidth { get; set; }

        public TThumbArrow()
        {
            DataContext = this;
            SetTemplate();
        }
        private void SetTemplate()
        {
            //FrameworkElementFactory arrow = new(typeof(Arrow));
            //arrow.SetBinding(Arrow.X1Property, MakeBind(nameof(TX1)));
            //arrow.SetBinding(Arrow.Y1Property, MakeBind(nameof(TY1)));
            //arrow.SetBinding(Arrow.X2Property, MakeBind(nameof(TX2)));
            //arrow.SetBinding(Arrow.Y2Property, MakeBind(nameof(TY2)));
            //arrow.SetBinding(Arrow.HeadSizeProperty, MakeBind(nameof(THeadSize)));
            //arrow.SetBinding(Arrow.StrokeProperty, MakeBind(nameof(TStroke)));
            //arrow.SetBinding(Arrow.StrokeThicknessProperty, MakeBind(nameof(StrokeWidth)));

            //↕どちらでも同じ、TwoWayにしても依存プロパティにしないと変化しない

            FrameworkElementFactory arrow = new(typeof(Arrow));
            arrow.SetBinding(Arrow.X1Property, new Binding(nameof(TX1)));
            arrow.SetBinding(Arrow.Y1Property, new Binding(nameof(TY1)));
            arrow.SetBinding(Arrow.X2Property, new Binding(nameof(TX2)));
            arrow.SetBinding(Arrow.Y2Property, new Binding(nameof(TY2)));
            arrow.SetBinding(Arrow.HeadSizeProperty, new Binding(nameof(THeadSize)));
            arrow.SetBinding(Arrow.StrokeProperty, new Binding(nameof(TStroke)));
            arrow.SetBinding(Arrow.StrokeThicknessProperty, new Binding(nameof(StrokeWidth)));

            ControlTemplate template = new();
            template.VisualTree = arrow;
            this.Template = template;
        }
        private Binding MakeBind(string path)
        {
            Binding b = new(path);
            b.Mode = BindingMode.TwoWay;
            return b;
        }
      
    }
}
