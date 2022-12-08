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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Windows.Themes;
using System.Collections.ObjectModel;

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
    public class TTEllipse : TThumb
    {
        public new DDRectangle MyData;
        public Ellipse? MyTemplate { get; private set; }
        public TTEllipse(DDRectangle data) : base(data)
        {
            MyData = data;
            SetTemplate();
            MyTemplate?.SetBinding(Ellipse.WidthProperty, new Binding(nameof(MyData.Width)));
            MyTemplate?.SetBinding(Ellipse.HeightProperty, new Binding(nameof(MyData.Height)));
            MyTemplate?.SetBinding(Ellipse.FillProperty, new Binding(nameof(MyData.Fill)));

        }
        public TTEllipse() : base(new DDRectangle())
        {


        }
        protected override void SetTemplate()
        {
            FrameworkElementFactory element = new(typeof(Ellipse), "element");
            ControlTemplate template = new();
            template.VisualTree = element;
            this.Template = template;
            this.ApplyTemplate();
            MyTemplate = (Ellipse)this.Template.FindName("element", this);
        }
    }

    public class TT3 : Thumb
    {
        [DataMember]
        public string MyText
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(TT3), new PropertyMetadata(""));

        public Brush FontColor
        {
            get { return (Brush)GetValue(FontColorProperty); }
            set { SetValue(FontColorProperty, value); }
        }
        public static readonly DependencyProperty FontColorProperty =
            DependencyProperty.Register(nameof(FontColor), typeof(Brush), typeof(TT3), new PropertyMetadata(Brushes.Red));


        public DDTextBlock MyData { get; private set; }
        public TT3()
        {
            MyData = new DDTextBlock();
            this.DataContext = this;
            FrameworkElementFactory element = new(typeof(TextBlock));
            Binding bind = new(nameof(MyText));
            //Binding bind = new(nameof(MyData.Text));
            //element.SetBinding(TT3.TextProperty, bind);
            //SetBinding(TT3.TextProperty, bind);
            element.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
            SetBinding(TextBlock.TextProperty, new Binding(nameof(MyText)));

            //element.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)));

            bind = new(nameof(MyData.FontColor));
            element.SetBinding(TT3.FontColorProperty, bind);
            SetBinding(TT3.FontColorProperty, bind);

            Template = new() { VisualTree = element };

        }
    }

    //依存プロパティとDataの両方所持
    public class AAA : Thumb
    {
        public DDTextBlock MyData { get; set; } = new();
        public AAA()
        {
            DataContext = MyData;

            FrameworkElementFactory elem = new(typeof(TextBlock));

            elem.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyText)) { Source = this });

            Template = new() { VisualTree = elem };

            //TwoWay必須
            SetBinding(MyTextProperty, new Binding(nameof(MyData.Text)) { Mode = BindingMode.TwoWay });
        }
        public AAA(DDTextBlock data) : this()
        {
            MyData = data;
            DataContext = MyData;

        }

        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(AAA), new PropertyMetadata("MyText"));

    }

    //依存プロパティだけの所持
    public class BBB : Thumb
    {

        public BBB()
        {
            DataContext = this;

            FrameworkElementFactory elem = new(typeof(TextBlock));

            elem.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyText)));

            Template = new() { VisualTree = elem };
        }

        public BBB(DDTextBlock data) : this()
        {
            MyText = data.Text;
        }

        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(BBB), new PropertyMetadata("MyText"));

    }

    public class GBBB : Thumb
    {
        public ObservableCollection<BBB> Children { get; set; } = new();
        public GBBB()
        {
            DataContext = this;

            FrameworkElementFactory canvas = new(typeof(Canvas));

            FrameworkElementFactory elem = new(typeof(ItemsControl));
            elem.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            elem.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvas));
            Template = new() { VisualTree = elem };
        }
    }

}
