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

//Canvasコントロールの子要素を動的に増減させたい
//https://teratail.com/questions/359699


namespace _20220408
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private AThumb MyTT;
        private GThumb MyGroup1;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            MyTT = new(new TextBlock() { Text = "test", FontSize = 20 });
            MyGrid.Children.Add(MyTT);

            MyGroup1 = new();
            MyGrid.Children.Add(MyGroup1);
            MyGroup1.AddItem(new AThumb(new TextBlock() { Text = "atse", FontSize = 30 }));
            //MyTT.AddItem(new TextBlock() { Text = "test", FontSize = 20 }, 10, 100);

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //MyGroup1.AddToEventRoute()
            MyGroup1.AddItem(new AThumb(new TextBlock() { Text = "addItem1", FontSize = 30 }));

        }
    }

    public class AThumb : Thumb
    {
        public ContentControl MyContent { get; set; }
        public AThumb(UIElement element)
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(ContentControl), nameof(MyContent));
            this.Template = template;
            this.ApplyTemplate();
            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);
            MyContent.Content = element;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            DragDelta += AThumb_DragDelta;
        }

        private void AThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }
    }
    public class GThumb : Thumb
    {
        public ObservableCollection<AThumb> Items { get; set; } = new();
        public IItemsControl BasePanel { get; set; }
        public GThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(IItemsControl), nameof(BasePanel));
            this.Template = template;
            this.ApplyTemplate();
            BasePanel = (IItemsControl)template.FindName(nameof(BasePanel), this);
            BasePanel.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }
        public void AddItem(AThumb item)
        {
            BasePanel.Items.Add(item);
        }
        public void AddDragDelta()
        {
            this.DragDelta += GThumb_DragDelta;
        }
        private void GThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }
        public void RemoveDragDelta()
        {
            this.DragDelta -= GThumb_DragDelta;
        }

    }
    public class TThumb : Thumb
    {
        public IItemsControl MyItems { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public TThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(IItemsControl), nameof(MyItems));
            this.Template = template;
            this.ApplyTemplate();
            MyItems = (IItemsControl)template.FindName(nameof(MyItems), this);
            MyItems.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(MyItems)));

            MyItems.Background = Brushes.Beige;

            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);

        }
        public TThumb(UIElement element, double x, double y) : this()
        {
            AddItem(element, x, y);
        }
        public void AddItem(UIElement element, double x, double y)
        {
            MyItems.Items.Add(element);
            Canvas.SetLeft(element, x); Canvas.SetTop(element, y);
            DragDelta += TThumb_DragDelta;
        }
        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }
    }

    public class IItemsControl : ItemsControl
    {

        public IItemsControl()
        {
            ItemsPanelTemplate template = new(new FrameworkElementFactory(typeof(Canvas)));
            this.ItemsPanel = template;

            Style style = new();
            this.ItemContainerStyle = style;
            style.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("X")));
            style.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Y")));
        }
    }
}
