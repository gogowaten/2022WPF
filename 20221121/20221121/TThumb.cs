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
            MakeItem();
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
        private void MakeItem()
        {
            FrameworkElementFactory panel = new(typeof(Grid));
            FrameworkElementFactory textblock = new(typeof(TextBlock));
            MySetBinding(textblock, TextBlock.TextProperty, nameof(MyData.Text));
            panel.AppendChild(textblock);
            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;

            void MySetBinding(FrameworkElementFactory elem, DependencyProperty dp, string path)
            {
                elem.SetBinding(dp, MakeTowWayBinding(path, MyData));
            }
            Binding MakeTowWayBinding(string path, object source)
            {
                return new(path) { Source = source, Mode = BindingMode.TwoWay };
            }
        }
    }

    public class TTGroup : Thumb
    {

    }
}
