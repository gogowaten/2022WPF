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

namespace _20221121
{
    class TThumb : Thumb
    {
        public Data MyData { get; protected set; }

        public TThumb(Data data)
        {
            MyData = data;
            DataContext = this;
            SetItem();
            SetDataBinding();
        }
        private void SetDataBinding()
        {
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            Binding b;
            b = new(nameof(MyData.X)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.LeftProperty, b);
            b = new(nameof(MyData.Y)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.TopProperty, b);
            b = new(nameof(MyData.Z)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Panel.ZIndexProperty, b);

        }
        private void SetItem()
        {
            //FrameworkElementFactory panel = new(typeof(Grid));
            FrameworkElementFactory textblock = new(typeof(TextBlock));
            MySetBinding(textblock, TextBlock.TextProperty, nameof(MyData.Text));
            //panel.AppendChild(textblock);
            ControlTemplate template = new();
            template.VisualTree = textblock;
            this.Template = template;

            void MySetBinding(FrameworkElementFactory elem, DependencyProperty dp, string path)
            {
                elem.SetBinding(dp, MakeTwoWayBinding(path, MyData));
            }
            Binding MakeTwoWayBinding(string path, object source) =>
                new Binding(path) { Source = source, Mode = BindingMode.TwoWay };

        }
    }

    public class TTGroup : Thumb
    {
        public Data MyData { get; protected set; }
        public ObservableCollection<TTGroup> Children { get; } = new();

        public TTGroup(Data data)
        {
            MyData = data;
            if (data.Children.Count > 0)
            {
                SetTemplate();
                foreach (var item in data.Children)
                {
                    Children.Add(new TTGroup(item));
                }
            }
            else { SetTemplate2(data); }
        }

        //for gourp
        private void SetTemplate()
        {
            //Grid
            // ┣ItemsControl
            FrameworkElementFactory panel = new(typeof(Grid));

            FrameworkElementFactory content = new(typeof(ItemsControl));
            content.SetValue(ItemsControl.ItemsPanelProperty,
                new ItemsPanelTemplate(
                    new FrameworkElementFactory(typeof(Canvas))));
            content.SetValue(ItemsControl.ItemsSourceProperty,
                new Binding(nameof(Children)));
            panel.AppendChild(content);

            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;

        }

        //for one item
        private void SetTemplate2(Data data)
        {
            FrameworkElementFactory textblock = new(typeof(TextBlock));
            MySetBinding(textblock, TextBlock.TextProperty, nameof(data.Text));
            //panel.AppendChild(textblock);
            ControlTemplate template = new();
            template.VisualTree = textblock;
            this.Template = template;

            void MySetBinding(FrameworkElementFactory elem, DependencyProperty dp, string path)
            {
                elem.SetBinding(dp, MakeTwoWayBinding(path, data));
            }
            Binding MakeTwoWayBinding(string path, object source) =>
                new Binding(path) { Source = source, Mode = BindingMode.TwoWay };

        }
    }

    public class TTGroup2 : Thumb
    {
        public Data MyData { get;protected set; }
        public ObservableCollection<TTGroup2> Items { get; } = new();
        
        public TTGroup2(Data data)
        {
            MyData = data;
        }
        

        private void SetTemplate()
        {
            //Grid
            // ┣ItemsControl
            FrameworkElementFactory panel = new(typeof(Grid));
            FrameworkElementFactory content = new(typeof(ItemsControl));
            content.SetValue(ItemsControl.ItemsPanelProperty,
                new ItemsPanelTemplate(
                    new FrameworkElementFactory(typeof(Canvas))));
            content.SetValue(ItemsControl.ItemsSourceProperty,
                new Binding(nameof(Items)));
            panel.AppendChild(content);

            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;
        }
    }
}
