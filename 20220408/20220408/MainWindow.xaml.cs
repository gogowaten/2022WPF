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
        ItemThumb3 MyItemThumb3;
        GroupThumb3 MyG3;
        public MainWindow()
        {
            InitializeComponent();

            //MyTT = new("tttext");
            //MyGrid.Children.Add(MyTT);

            //DataText dataText = new("datatext2", 50, 50, 0);
            //MyTT2 = new(dataText);
            //MyGrid.Children.Add(MyTT2);

            //PolyLineSegment polyLineSegment = new();
            //polyLineSegment.Points = new PointCollection() { new(15, 0), new(30, 30) };
            //PathFigure pathFigure = new(new(0, 30), new List<PathSegment>() { polyLineSegment }, true);
            //PathGeometry pathGeo = new();
            //pathGeo.Figures.Add(pathFigure);
            //Path path = new() { Data = pathGeo, Fill = Brushes.MediumAquamarine };
            //MyGrid.Children.Add(path);

            Data2 data1 = new(ThumbType.Path, 10, 20, new RectangleGeometry(new(0, 0, 30, 30)), Brushes.Red);
            Data2 data2 = new(ThumbType.TextBlock, 100, 0, "asdfa");

            Data2 data3 = new(ThumbType.Path, 100, 120, new RectangleGeometry(new(0, 0, 130, 30)), Brushes.DarkCyan);
            Data2 data4 = new(ThumbType.TextBlock, 100, 20, "aaaaa");
            Data2 data5 = new(ThumbType.Group, new() { data3, data4 }, 0, 0);

            MyItemThumb3 = new(data1);
            MyGrid.Children.Add(MyItemThumb3);

            MyG3 = new(100, 10);
            ItemThumb3 item = new(data2);
            MyG3.AddItem(item);
            MyG3.AddItem(new ItemThumb3(data3));

            MyGrid.Children.Add(MyG3);
            //MyG3.AddData(data2);

        }
        private void SetLocate(UIElement element, double x, double y)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //Dataが通知プロパティじゃなくても
            //要素ーDataー要素
            //のとき要素の値を変更するとすべて変化する
            //ただしDataの値を変更しても要素には伝わらない
            //var con = MyIThumb.MyContentControl.Content;
            //MyBorder.Background = Brushes.Blue;
            //var data2 = MyIThumb.Data2;
            //MyIThumb.Data2.Stroke = Brushes.Orange;
            //data2 = MyIThumb.Data2;
            Data2 data = new(ThumbType.Path, 100, 120, new RectangleGeometry(new(0, 0, 130, 130)), Brushes.Cyan);
            MyItemThumb3.Data.Stroke = Brushes.Cyan;
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




}
