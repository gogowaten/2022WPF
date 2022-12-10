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
            MyCanvas.Children.Add(new TTTextBlock(new TextBlockData()
            {
                Text = "アプリ起動時に追加したもの",
                FontSize = 20,
                BackColorBrush = Brushes.MediumOrchid,
                FontColorBrush = Brushes.White,
                //Font = new FontFamily("Meiryo UI"),
                Font = new FontFamily("ＭＳ 明朝"),
                X = 30,
                Y = 50
            }));
            MyCanvas.Children.Add(new TTPolyline(new PolylineData()
            {
                Points = new PointCollection(new List<Point>() { new(100, 100), new(110, 200), new(200, 200) }),
                LineColorBrush = Brushes.BlueViolet,
                Thickness = 20,
            }));
            TTTextBlock tb=new TTTextBlock(,)
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var ps = MyPolyline.Points;
            ps[0] = new Point(10, 300);
        }
    }
}
