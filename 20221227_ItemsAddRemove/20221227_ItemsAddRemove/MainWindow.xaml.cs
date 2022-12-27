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

namespace _20221227_ItemsAddRemove
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
                MyText = TextBoxAdd.Text + MyAddCounnt,
                Name = TextBoxAdd.Text + MyAddCounnt,
            };
            MyRootThumb.AddThumb(tt);
            MyAddCounnt++;
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            //MyRootThumb.RemoveTTMovable();
            MyRootThumb.RemoveThumb(Text_22);
            //MyRootThumb.RemoveTT(TTG_1);
        }

        private void ButtonRootEnable_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.EnableGroup = MyRootThumb;
        }

        private void ButtonTTG1Enable_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.EnableGroup = TTG_1;
        }

        private void ButtonTTG2Enable_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.EnableGroup = TTG_2;
        }
    }
}
