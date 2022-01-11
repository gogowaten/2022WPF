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

namespace _20220111
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TThumb MyTThumb;
        public MainWindow()
        {
            InitializeComponent();

            TextBlock tb = new() { Text = "test", FontSize = 30, Background = Brushes.MediumAquamarine };
            tb.RenderTransform = new ScaleTransform(2, 2);
            MyTThumb = new(tb);
            MyCanvas.Children.Add(MyTThumb);
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyTThumb.ActualWidth;
            var inu = MyTThumb.Width;
            var element = MyTThumb.MyContentElement;
            var ew = element.Width;
            var eaw = element.ActualWidth;
            MyTThumb.GetSize();
           var mcaw= MyCanvas.ActualWidth;
        }
    }
}
