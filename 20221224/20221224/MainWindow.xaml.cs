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

namespace _20221224
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MyAddCounnt = 1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            TTTextBlock tt = new()
            {
                MyLeft = 20 * MyAddCounnt,
                MyTop = 20 * MyAddCounnt,
                MyText = TextBoxAdd.Text + MyAddCounnt
            };
            TTG_1.Children.Add(tt);
            MyAddCounnt++;
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            int itemCount = TTG_1.Children.Count;
            if (itemCount > 0)
            {
                TTG_1.Children.RemoveAt(itemCount - 1);
            }
        }
    }
}
