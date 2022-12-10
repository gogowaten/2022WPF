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

namespace _20221210_Thumb継承したItem候補
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyCanvas.Children.Add(new TTTextBlock(new Data(TType.TextBlock)
            {
                Text = "アプリ起動時に追加したもの",
                FontSize = 20,
                BackColor = Brushes.MediumOrchid,
                //Font = new FontFamily("Meiryo UI"),
                //Font = new FontFamily("ＭＳ 明朝"),
                X= 30,Y= 50
            }));
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
           var ps = MyPolyline.Points;
            ps[0] = new Point(10, 300);
        }
    }
}
