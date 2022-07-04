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

namespace _20220704
{
    public abstract class TThumb : Thumb
    {
        public Data MyData { get; protected set; }
        public TThumb(Data myData)
        {
            MyData = myData;
            DataContext = MyData;

            DragDelta += TThumb_DragDelta;
            SetDragDeltaBinding();
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }
        private void SetDragDeltaBinding()
        {
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            Binding b;
            b = new(nameof(MyData.X)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.LeftProperty, b);
            b = new(nameof(MyData.Y)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.TopProperty, b);
        }
    }

    public abstract class ItemThumb : TThumb
    {
        protected FrameworkElementFactory waku;
        protected FrameworkElementFactory panel;
        public ItemThumb(Data myData) : base(myData)
        {
            waku = new(typeof(Rectangle));
            panel = new(typeof(Grid));
            panel.AppendChild(waku);

            SetTemplate();

        }
        protected abstract void SetTemplate();
        protected Binding MakeTwoWayBinding(string path, object source)
        {
            return new(path) { Source = source, Mode = BindingMode.TwoWay };
        }

        protected void MySetBinding(FrameworkElementFactory elem, DependencyProperty dp, string path)
        {
            elem.SetBinding(dp, MakeTwoWayBinding(path, MyData));
        }
    }
    public class TTextBlock : ItemThumb
    {
        public new DataTextBlock MyData { get; protected set; }
        public TTextBlock(DataTextBlock data) : base(data)
        {
            MyData = data;

            
        }

        protected override void SetTemplate()
        {
            FrameworkElementFactory content = new(typeof(TextBlock));
            MySetBinding(content, TextBlock.TextProperty, nameof(MyData.Text));
            MySetBinding(content, TextBlock.ForegroundProperty, nameof(MyData.ColorFont));
            MySetBinding(content, TextBlock.BackgroundProperty, nameof(MyData.ColorBack));
            MySetBinding(content, TextBlock.FontSizeProperty, nameof(MyData.FontSize));
            panel.AppendChild(content);
            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;
        }
    }

}
