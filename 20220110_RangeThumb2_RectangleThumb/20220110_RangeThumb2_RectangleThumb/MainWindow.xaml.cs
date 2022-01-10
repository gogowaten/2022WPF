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

namespace _20220110_RangeThumb2_RectangleThumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RangeThumb MyRangeThumb = new();
        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Children.Add(MyRangeThumb);
            MyStackPnel.DataContext = MyRangeThumb;
        }
        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(MyRangeThumb.GetRect().ToString());

        }
    }
}
