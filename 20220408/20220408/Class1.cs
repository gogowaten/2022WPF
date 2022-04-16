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
    class Class1 : Thumb
    {
        public ObservableCollection<Class1Item> MyDatas { get; set; }
        //Thumb.Template
        // ItemsControl
        //  ItemsPanelTemplate.Canvas
        public Class1()
        {
            this.DataContext = this;
            MyDatas = new();

            FrameworkElementFactory canvasF = new(typeof(Canvas));
            canvasF.SetValue(BackgroundProperty, Brushes.Beige);

            FrameworkElementFactory itemsControlF = new(typeof(ItemsControl));
            itemsControlF.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvasF));
            itemsControlF.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(MyDatas)));

            ControlTemplate template = new();
            template.VisualTree = itemsControlF;
            this.Template = template;
            this.ApplyTemplate();

            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            DragDelta += Class1_DragDelta;
        }

        private void Class1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }



    }
    public class Class1Item : Thumb
    {
        protected ContentControl MyContentControl;
        public Class1Item()
        {
            FrameworkElementFactory content = new(typeof(ContentControl), nameof(MyContentControl));

            //↓はエラーになる、VisualまたはContentElementから派生した値はサポートされません
            //content.SetValue(ContentControl.ContentProperty, element);

            ControlTemplate template = new();
            template.VisualTree = content;
            this.Template = template;
            this.ApplyTemplate();
            MyContentControl = (ContentControl)template.FindName(nameof(MyContentControl), this);
        }
        public Class1Item(UIElement element, double x = 0, double y = 0):this()
        {
            //FrameworkElementFactory content = new(typeof(ContentControl), nameof(MyContentControl));

            ////↓はエラーになる、VisualまたはContentElementから派生した値はサポートされません
            ////content.SetValue(ContentControl.ContentProperty, element);

            //ControlTemplate template = new();
            //template.VisualTree = content;
            //this.Template = template;
            //this.ApplyTemplate();
            //MyContentControl = (ContentControl)template.FindName(nameof(MyContentControl), this);
            MyContentControl.Content = element;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
    }

    public class TText : Class1Item
    {
        public DataText DataText;
        public TText(string text)
        {
            DataText = new(text, 0, 0, 0);
            TextBox textBox = new();
            textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(DataText.Text)));
            this.MyContentControl.Content = textBox;

            this.DataContext = DataText;
        }
        public TText(DataText dataText)
        {
            DataText = dataText;
            TextBox box = new();
            box.SetBinding(TextBox.TextProperty, new Binding(nameof(DataText.Text)));
            this.MyContentControl.Content = box;
            

            this.DataContext = DataText;
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(DataText.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(DataText.Y)));
        }
    }
}
