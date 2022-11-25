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

namespace _20221121
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                MyCanvas.Children.Add(
                    new TThumb(new Data("test", i, i, i)));
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var data = new Data("group", 10, 101, 1);
            data.Children.Add(new("child",10,10,10));
            var groupT = new TTGroup(data);
            MyCanvas.Children.Add(groupT);
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            MyLLine.Fill = Brushes.MediumAquamarine;
            MyLLine.Test1();
            var ll = new LLine();
        }
    }
}
