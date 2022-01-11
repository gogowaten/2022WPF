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

namespace _20220111_RangeThumb3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RectThumb MyRectThumb = new();
        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Children.Add(MyRectThumb);
            MyStackPnel.DataContext = MyRectThumb;
            MyRectThumb.MyFill = Brushes.MediumOrchid;
            MyRectThumb.MyHandleSize = 20;
            MyRectThumb.MyStrokeThickness = 5;
            MyRectThumb.MyStroke = Brushes.Orange;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(MyRectThumb.GetRect().ToString());

        }
    }
}
