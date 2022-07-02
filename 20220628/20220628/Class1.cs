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

            //Style style = new();
            //this.ItemContainerStyle = style;
            //style.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("X")));
            //style.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Y")));

            //DataTemplate dtemp = new(typeof(Data));
            //this.ItemTemplate = dtemp;

            //DataTemplateSelector dselect = new();

            //this.ItemTemplateSelector = dselect;

            //ResourceDictionary resources = new();

        }
    }

 
   public class TThumb : Thumb
    {


        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(Brush), typeof(TThumb),
                new PropertyMetadata(Brushes.MediumAquamarine));


        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(TThumb), new PropertyMetadata(0.0));



        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(TThumb), new PropertyMetadata(0.0));



        public TThumb()
        {
            SetTemplateContent();
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
        public void SetTemplateContent()
        {
            FrameworkElementFactory elemF = new(typeof(Grid));
            elemF.SetBinding(Panel.BackgroundProperty, new Binding(nameof(BackColor)));
            ControlTemplate template = new(typeof(Thumb));
            template.VisualTree = elemF;
            this.Template = template;
        }

    }
}
