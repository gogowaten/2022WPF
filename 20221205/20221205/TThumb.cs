using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _20221205
{
    public abstract class TThumb : Thumb
    {
        public Data MyData;
        public DataType MyDataType { get; private set; }

        public TThumb(Data data)
        {
            this.MyData = data;
            this.DataContext = MyData;
            MyDataType = data.DataType;
            SetLocate(data.X, data.Y);
        }
        private void SetLocate(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
        protected abstract void SetTemplate();


    }
    public class TTTextBlock : TThumb
    {
        public new DDTextBlock MyData;
        public TTTextBlock() : base(new DDTextBlock() { Text = "temp" })
        {
            if (base.MyData is DDTextBlock data) { MyData = data; }
            else { MyData = new DDTextBlock(); }
        }
        public TTTextBlock(DDTextBlock data) : base(data)
        {
            MyData = data;
            SetTemplate();
        }

        protected override void SetTemplate()
        {
            FrameworkElementFactory elem = new(typeof(TextBlock));
            elem.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
            elem.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(MyData.FontColor)));
            elem.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(MyData.FontSize)));
            this.Template = new() { VisualTree = elem };
        }
    }
    public class TTRectAngle : TThumb
    {
        public new DDRectangle MyData;
        public TTRectAngle(DDRectangle data) : base(data)
        {
            MyData = data;
            SetTemplate();
        }
        protected override void SetTemplate()
        {
            FrameworkElementFactory elem = new(typeof(Rectangle));
            elem.SetBinding(Rectangle.WidthProperty, new Binding(nameof(MyData.Width)));
            elem.SetBinding(Rectangle.HeightProperty, new Binding(nameof(MyData.Height)));
            elem.SetBinding(Rectangle.FillProperty, new Binding(nameof(MyData.Fill)));
            this.Template = new() { VisualTree = elem };
        }
    }


    public class TT3 : Thumb
    {

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TT3), new PropertyMetadata(""));

        public string SText
        {
            get { return (string)GetValue(STextProperty); }
            set { SetValue(STextProperty, value); }
        }
        public static readonly DependencyProperty STextProperty =
            DependencyProperty.Register(nameof(SText), typeof(string), typeof(TT3),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));
        public TT3()
        {
            this.DataContext = this;
            FrameworkElementFactory element = new(typeof(TextBlock));
            element.SetBinding(TextBlock.TextProperty, new Binding("Text"));
            Template = new() { VisualTree = element };

        }
    }






}
