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
        private CThumb MainCanvas;
        private CThumb G1;
        public MainWindow()
        {
            InitializeComponent();
            //this.LayoutUpdated += MainWindow_LayoutUpdated;
            MainCanvas = new CThumb();
            MainCanvas.Background = Brushes.Snow;
            MainCanvas.Width = 200;
            MainCanvas.Height = 100;
            MyCanvas.Children.Add(MainCanvas);
            AddG();

        }


        private void AddG()
        {
            G1 = new CThumb(new TextBlock() { Text = "text1", Margin = new Thickness(10) });
            MainCanvas.AddElement(G1, 20, 0);

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
}
