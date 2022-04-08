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
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace _20220406
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CThumb MainCanvas;
        private CThumb G1;
        private TItem MyItem1;
        public MainWindow()
        {
            InitializeComponent();
            //this.LayoutUpdated += MainWindow_LayoutUpdated;
            MainCanvas = new CThumb();
            MainCanvas.Background = Brushes.Snow;
            MainCanvas.Width = 200;
            MainCanvas.Height = 100;
            MyCanvas.Children.Add(MainCanvas);
            //AddG();
            Test1();
        }
        public void Test1()
        {
            TextBlock text = MakeTextBlock("test1", 20);
            MyItem1 = new TItem();
            MyItem1.AddContent(text);
            MyCanvas.Children.Add(MyItem1);
        }
        public TextBlock MakeTextBlock(string text, double fontSize)
        {
            return new TextBlock() { Text = text, FontSize = fontSize, Margin = new Thickness(10), Background = Brushes.MediumAquamarine };
        }
        private void AddG()
        {
            G1 = new CThumb(new TextBlock() { Text = "text1", Margin = new Thickness(10) });
            MainCanvas.AddElement(G1, 20, 0);

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MyItem1.Left++;
        }
    }

    public class CThumb : Thumb
    {
        internal Canvas RootCanvas;
        internal CThumb Parent;
        public CThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), "ROOT");
            this.Template = template;
            ApplyTemplate();
            RootCanvas = (Canvas)template.FindName("ROOT", this);

            RootCanvas.Background = Brushes.Snow;
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);

        }
        public CThumb(UIElement element) : this()
        {
            AddElement(element, 0, 0);

        }

        private void TC1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }


        public void AddElement(UIElement element, double left, double top)
        {
            RootCanvas.Children.Add(element);
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);

            CThumb ct = element as CThumb;
            if (ct == null)
            {
                this.DragDelta += TC1_DragDelta;
            }
            //if (element is CThumb cThumb)
            //{
            //    cThumb.DragDelta += TC1_DragDelta;
            //}
        }
        private void RemoveDragDelta()
        {
            this.DragDelta -= TC1_DragDelta;
        }
        public void RemoveElement(UIElement control)
        {
            RootCanvas.Children.Remove(control);
        }

    }

    public abstract class TThumb : Thumb, System.ComponentModel.INotifyPropertyChanged
    {
        private double left;
        private double top;
        private int zin;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public double Left { get => left; set { if (left != value) { left = value; OnPropertyChanged(); } } }
        public double Top { get => top; set { if (top != value) { top = value; OnPropertyChanged(); } } }
        public int ZIndex { get => zin; set { if (zin != value) { zin = value; OnPropertyChanged(); } } }

        public TThumb()
        {
            DragDelta += TThumb_DragDelta;

            Binding binding = new(nameof(Left));
            binding.Source = this;
            this.SetBinding(Canvas.LeftProperty, binding);

            binding = new(nameof(Top));
            binding.Source = this;
            this.SetBinding(Canvas.TopProperty, binding);
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }

    }
    public class TItem : TThumb
    {
        private ContentControl MyContent;
        public TItem()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(ContentControl), nameof(MyContent));
            this.Template = template;
            this.ApplyTemplate();
            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);
        }
        public void AddContent(UIElement content)
        {
            MyContent.Content = content;
        }
    }

    public class TGroup : TThumb
    {
        private Canvas MyCanvas;
        private System.Collections.ObjectModel.ObservableCollection<TThumb> myThumbs;
        public TGroup()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(MyCanvas.GetType(), nameof(MyCanvas));
            this.Template = template;
            this.ApplyTemplate();
            MyCanvas = (Canvas)template.FindName(nameof(MyCanvas), this);
            MyCanvas.
        }

        public ObservableCollection<TThumb> MyThumbs { get => myThumbs; set => myThumbs = value; }
    }
}
