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

namespace _20220415_ItemsControlThumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Children.Add(new ItemThumb(10, 10));
            MyCanvas.Children.Add(new ItemThumb(new TextBlock { Text = "textblock", Background = Brushes.Gold }, 100, 10));
            Rectangle rect = new() { Width = 100, Height = 100, Fill = Brushes.MediumAquamarine };
            ItemThumb item = new(rect, 10, 100);
            item.DragDelta += Element_DragDelta;
            MyCanvas.Children.Add(item);


            GroupThumb group = new(100, 100);
            group.Items.Add(new(new TextBlock() { Text = "item1", Background = Brushes.Magenta }, 0, 0));
            group.Items.Add(new(new TextBlock() { Text = "item2", Background = Brushes.Orange }, 100, 100));
            group.DragDelta += Element_DragDelta;
            MyCanvas.Children.Add(group);
        }

        private void Element_DragDelta(object sender, DragDeltaEventArgs e)
        {
            UIElement element = sender as UIElement;
            Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
            Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
        }
    }
}
