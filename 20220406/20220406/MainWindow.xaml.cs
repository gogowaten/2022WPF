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

namespace _20220406
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Canvas G1;
        private Canvas G2;
        public MainWindow()
        {
            InitializeComponent();
            //this.LayoutUpdated += MainWindow_LayoutUpdated;
            AddG();

        }

        private void MainWindow_LayoutUpdated(object sender, EventArgs e)
        {
        }

        private void AddG()
        {
            G1 = new Canvas();
            TextBlock textBlock = new() { Text = "text1", Margin = new Thickness(10) };
            CThumb cThumb = new(textBlock);
            MyCanvas.AddElement(cThumb, 20, 0);

        }
    }
    public class TCC1 : CThumb
    {
        //System.Collections.ObjectModel.ObservableCollection<TT> Children;
        public TCC1()
        {
            Width = 100;
            Height = 100;
        }

    }
    public class CThumb : Thumb
    {
        internal Canvas RootCanvas;
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
            this.DragDelta += TC1_DragDelta;
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
        }
        public void RemoveElement(UIElement control)
        {
            RootCanvas.Children.Remove(control);
        }

    }
}
