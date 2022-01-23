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

namespace _20220122
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MyCount = 0;
        private List<BaseThumb> MyThumbs = new();
        private List<BaseThumb> MyGroupThumbs = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            var neko = tb.Name;
        }

        private void ButtonTest1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyThumbs[0].ParentLayer;
            var inu = MyLayer1.LastClickedThumb;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            TTTextBlock tb = new($"要素{MyCount}", MyCount * 50, MyCount * 50, $"要素{MyCount}");
            MyThumbs.Add(tb);
            MyLayer1.AddThumb(tb);
            MyCount++;
        }

        private void ButtonG1_Click(object sender, RoutedEventArgs e)
        {
            MyGroupThumbs.Add(MyLayer1.MakeGroupFromChildren(MyThumbs.GetRange(0, 2), "G1"));
        }

        private void ButtonG2_Click(object sender, RoutedEventArgs e)
        {
            MyGroupThumbs.Add(MyLayer1.MakeGroupFromChildren(MyThumbs.GetRange(2, 2), "G2"));
        }

        private void ButtonG3_Click(object sender, RoutedEventArgs e)
        {
            MyGroupThumbs.Add(MyLayer1.MakeGroupFromChildren(MyGroupThumbs.GetRange(0, 2), "G3"));
        }
    }
}
