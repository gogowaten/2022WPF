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

namespace _20220118
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TTTextBlock MyTT;
        List<TThumb> MyThumbs = new();
        public MainWindow()
        {
            InitializeComponent();

            Test1();
        }
        private void Test1()
        {
            MyTT = new($"{nameof(Test1)}", 10, 10, "T1");
            //MyTT = new("ooo");
            MyLayer.AddThumb(MyTT);
            MyThumbs.Add(MyTT);

            TTTextBlock ttt = new("T2", 100, 100, "T2");
            MyLayer.AddThumb(ttt);
            MyThumbs.Add(ttt);

            MyStackPanel.DataContext = MyLayer;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            MyTT.Text = "aaaaaaaa";
            var list = MyLayer.MyThumbs;
        }
    }
}
