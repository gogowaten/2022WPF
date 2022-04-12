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
        private GThumb MyGroup2;
        private UserControl2 My2;
        private UserControl2 My21;
        private Path MyPath2;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            MyPath2 = new();
            MyGrid.Children.Add(MyPath2);
            RenderOptions.SetEdgeMode(MyPath2, EdgeMode.Aliased);
            Canvas.SetLeft(MyPath2, 10);Canvas.SetTop(MyPath2, 10);
            MyPath2.Stroke = Brushes.Red;
            //MyPath2.Data = new LineGeometry(new(10, 0), new(50, 50));
            //MyPath2.Data = new EllipseGeometry(new Rect(new Size(50, 50)));
            MyPath2.Data = new RectangleGeometry(new Rect(new Size(50, 50)));

            PolyBezierSegment polyBezierSegment;
            Polygon polygon;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyPath.Data;
            var x = Canvas.GetLeft(My2);
            //My2.Data.X = 200;
            
        }
    }

    public class AThumb : Thumb
    {
        public ContentControl MyContent { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public AThumb(UIElement element, double x = 0, double y = 0)
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(ContentControl), nameof(MyContent));
            this.Template = template;
            this.ApplyTemplate();
            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);
            MyContent.Content = element;
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);

        }

        public void AddDragDelta()
        {
            this.DragDelta += AThumb_DragDelta;
        }
        public void RemoveDragDelta()
        {
            this.DragDelta -= AThumb_DragDelta;
        }
        private void AThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Canvas.SetLeft(this, X + e.HorizontalChange);
            //Canvas.SetTop(this, Y + e.VerticalChange);
        }
    }
    public class GThumb : Thumb
    {
        public ObservableCollection<AThumb> Items { get; set; } = new();
        public IItemsControl BasePanel { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public GThumb(double x = 0, double y = 0)
        {
            this.X = x; this.Y = y;
            this.DataContext = this;
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(IItemsControl), nameof(BasePanel));
            this.Template = template;
            this.ApplyTemplate();
            BasePanel = (IItemsControl)template.FindName(nameof(BasePanel), this);
            BasePanel.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            //Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            AddDragDelta();
        }

        public void AddItem(AThumb item)
        {
            Items.Add(item);
        }
        public void AddDragDelta()
        {
            this.DragDelta += GThumb_DragDelta;
        }
        public void RemoveDragDelta()
        {
            this.DragDelta -= GThumb_DragDelta;
        }
        private void GThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, X + e.HorizontalChange);
            Canvas.SetTop(this, Y + e.VerticalChange);
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

            //this.DataContext = this.ItemsSource;
        }
    }

    public class Data
    {
        public Data() { }
        public Data(double x, double y, double z, double width, double height)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class DataText : Data
    {
        public DataText(string text, double x, double y, double z, double width = 0, double height = 0) : base(x, y, z, width, height)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
    public class DataRect : Data
    {
        public DataRect(double x, double y, double z, double width, double height) : base(x, y, z, width, height)
        {
        }

        public Brush Brush { get; set; }
    }
    public class DataPath : Data
    {
        public DataPath(double x, double y, double z, double width=double.NaN, double height=double.NaN) : base(x, y, z, width, height)
        {
        }
        
    }
}
