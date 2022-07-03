using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace _20220628
{
    class TThumbsControl : ItemsControl
    {
        public TThumbsControl()
        {
            ItemsPanelTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas));
            this.ItemsPanel = template;

            Style s = new();
            s.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("X")));
            s.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Y")));
            this.ItemContainerStyle = s;

            DataTemplate dt = new();
            dt.DataType = typeof(DataTextBlock);
            FrameworkElementFactory elemF = new(typeof(TTTextBlock));
            //elemF.SetBinding(TextBlockThumb.TextProperty, new Binding("Text"));
            //dt.VisualTree =
            //this.ItemTemplate =
        }
    }

    public class TTTextBlock : Kyoutuu
    {

        public new DataTextBlock MyData { get; set; }


        public TTTextBlock(DataTextBlock data):base(data)
        {
            MyData = data;



            FrameworkElementFactory content = new(typeof(TextBlock));
            FrameworkElementFactory waku = new(typeof(Rectangle));
            FrameworkElementFactory panel = new(typeof(Grid));
            panel.AppendChild(content);
            panel.AppendChild(waku);
            Binding b = new(nameof(MyData.Text))
            {
                Source = MyData,
                Mode = BindingMode.TwoWay
            };
            content.SetValue(TextBlock.TextProperty, b);
            b = new(nameof(MyData.ForeColor)) { Source = MyData, Mode = BindingMode.TwoWay };
            content.SetValue(TextBlock.ForegroundProperty, b);
            b = new(nameof(MyData.BackColor)) { Source = MyData, Mode = BindingMode.TwoWay };
            content.SetValue(TextBlock.BackgroundProperty, b);
            b = new(nameof(MyData.FontSize)) { Source = MyData, Mode = BindingMode.TwoWay };
            content.SetValue(TextBlock.FontSizeProperty, b);

            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;

        }

    }

    public class Kyoutuu : Thumb
    {
        public Data MyData { get; set; }
        public Kyoutuu(Data data)
        {
            MyData = data;
            DataContext = MyData;

            DragDelta += Kyoutuu_DragDelta;
            SetBindingDaragDelta();

        }

        private void Kyoutuu_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }
        private void SetBindingDaragDelta()
        {
            Canvas.SetLeft(this, 0);Canvas.SetTop(this, 0);
            Binding b;
            b = new(nameof(MyData.X)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.LeftProperty, b);
            b = new(nameof(MyData.Y)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.TopProperty, b);
        }
     
    }
    public class TTPath : Kyoutuu
    {

        public new DataPath MyData { get; set; }


        public TTPath(DataPath data) : base(data)
        {
            MyData = data;

            //Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);


            FrameworkElementFactory content = new(typeof(Path));
            FrameworkElementFactory waku = new(typeof(Rectangle));
            FrameworkElementFactory panel = new(typeof(Grid));
            panel.AppendChild(content);
            panel.AppendChild(waku);
            Binding b = new(nameof(MyData.Geometry))
            {
                Source = MyData,
                Mode = BindingMode.TwoWay
            };
            content.SetValue(Path.DataProperty, b);
            b = new(nameof(MyData.Fill)) { Source = MyData, Mode = BindingMode.TwoWay };
            content.SetValue(Path.FillProperty, b);

            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;

        }

    }


    public class TThumb : Thumb
    {


        public Data Data
        {
            get { return (Data)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Data), typeof(TThumb), new PropertyMetadata(null));




        public TThumb(Data data)
        {
            this.Data = data;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(Data.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(Data.Y)));

            DragDelta += TThumb_DragDelta;
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                Canvas.SetLeft(tt, Canvas.GetLeft(tt) + e.HorizontalChange);
                Canvas.SetTop(tt, Canvas.GetTop(tt) + e.VerticalChange);
            }
        }

        public void SetTemplate()
        {
            FrameworkElementFactory gridF = new(typeof(Grid));

            ControlTemplate template = new(typeof(Thumb));
            template.VisualTree = gridF;
            this.Template = template;
        }

    }
    public class ItemThumb : TThumb
    {
        public Data MyData { get; set; }
        public ItemThumb(Data data) : base(data)
        {
            MyData = data;
            DataContext = this;

            FrameworkElement? elem = null;
            if (data is DataTextBlock dd)
            {
                TextBlock textBlock = new()
                {
                    Text = dd.Text,
                    Foreground = dd.ForeColor,
                    Background = dd.BackColor
                };
                elem = textBlock;
            }
        }
        private void SetTemplateContent(Data data)
        {
            FrameworkElementFactory elemF = new(typeof(Grid));
            if (data is DataTextBlock dd)
            {
                FrameworkElementFactory ee = new(typeof(TextBlock));
                //ee.SetBinding(TextBlock.TextProperty, nameof(MyData.te))
            }

            ControlTemplate template = new(typeof(Thumb));
            template.VisualTree = elemF;
            this.Template = template;
        }

    }
}
