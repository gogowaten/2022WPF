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

namespace _20220408
{
    internal class TCanvas:Canvas
    {
        public ItemsControl MyItemsControl;

        public TCanvas()
        {
            
            //FrameworkElementFactory canvas = new(typeof(Canvas));
            //canvas.SetValue(BackgroundProperty, Brushes.Bisque);

            ////Binding b = new();
            ////b.Source = this;
            ////b.Converter = new BB();
            ////b.ConverterParameter = this.Items;
            ////canvas.SetValue(Canvas.WidthProperty, b);

            //FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            //itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            //itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvas));
            ////Binding b = new();
            ////b.Source = itemsControl;
            ////b.Path = new PropertyPath(ItemsControl.ActualWidthProperty);
            ////b.Mode = BindingMode.OneWay;
            //////canvas.SetValue(Canvas.WidthProperty, b);
            ////canvas.SetBinding(Canvas.WidthProperty, b);
            //IItemsControl ic = new();
            
            //ControlTemplate template = new();
            //template.VisualTree = itemsControl;

            //this.Template = template;
            //this.ApplyTemplate();
            //MyItemsControl = (ItemsControl)template.FindName(nameof(MyItemsControl), this);
        }
    }
}
