using System;
using System.Collections.Generic;
using System.Globalization;
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

//意味無し
namespace _20221217
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
           var neko =  MyCanvas.Children[0];
            MyCanvas.Children.RemoveAt(0);
            MyCanvas.Children.Add(neko);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
  
 




}
