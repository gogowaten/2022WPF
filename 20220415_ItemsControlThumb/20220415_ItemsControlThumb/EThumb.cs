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


namespace _20220415_ItemsControlThumb
{
    public class EThumb : Thumb
    {
        public EThumb(double x = 0, double y = 0)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
    }
    public class ItemThumb : EThumb
    {
        private ContentControl MyContent;
        public ItemThumb(double x = 0, double y = 0) : base(x, y)
        {
            ControlTemplate template = new();
            template.VisualTree = new(typeof(ContentControl), nameof(MyContent));
            this.Template = template;
            this.ApplyTemplate();

            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);
        }
        public ItemThumb(UIElement element, double x = 0, double y = 0) : this(x, y)
        {
            MyContent.Content = element;
        }

    }
    public class GroupThumb : EThumb
    {
        public ObservableCollection<ItemThumb> Items { get; set; }
        public GroupThumb() { }
        public GroupThumb(double x = 0, double y = 0) : base(x, y)
        {
            this.DataContext = this;
            Items = new();

            FrameworkElementFactory canvasF = new(typeof(Canvas));
            canvasF.SetValue(BackgroundProperty, Brushes.Beige);

            FrameworkElementFactory itemsControlF = new(typeof(ItemsControl));
            itemsControlF.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            itemsControlF.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvasF));

            ControlTemplate template = new();
            template.VisualTree = itemsControlF;
            this.Template = template;
            this.ApplyTemplate();


        }
    }
}
