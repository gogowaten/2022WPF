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

            Style style = new();
            this.ItemContainerStyle = style;
            style.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("X")));
            style.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Y")));

            //DataTemplate dtemp = new(typeof(Data));
            //this.ItemTemplate = dtemp;

            //DataTemplateSelector dselect = new();

            //this.ItemTemplateSelector = dselect;

            //ResourceDictionary resources = new();

        }
    }

 
   public class TThumb : Thumb
    {
        public TThumb()
        {
            //SetTemplate();

            //Style style = new();
            //style.Setters.Add(new Setter(Canvas.LeftProperty,new Binding("X")));
            //style.Setters.Add(new Setter(Canvas.TopProperty,new Binding("Y")));
            //this.Style = style;

            Canvas.SetLeft(this, 0);Canvas.SetTop(this, 0);


            DragDelta += TThumb_DragDelta;
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if(sender is TThumb tt)
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
}
