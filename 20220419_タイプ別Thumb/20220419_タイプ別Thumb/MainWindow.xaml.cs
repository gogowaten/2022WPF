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

namespace _20220419_タイプ別Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IGThumb MyIGThumb;
        public MainWindow()
        {
            InitializeComponent();


            Data2 data3 = new(ThumbType.Path, 10, 20, new RectangleGeometry(new(0, 0, 30, 30)), Brushes.Pink);
            Data2 data4 = new(ThumbType.TextBlock, 100, 20, "aaaaa");
            Data2 data5 = new(ThumbType.Group, new() { data3, data4 }, 0, 0);
            MyIGThumb = new(data5);
            MyCanvas.Children.Add(MyIGThumb);

            this.DataContext = MyIGThumb.MyChildren[1].MyData2;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //グループに要素追加
            MyIGThumb.MyChildren.Add(new IGThumb(new Data2(ThumbType.TextBlock, 0, 100, "bbbbb")));
            //Canvasに要素追加
            MyCanvas.Children.Add(new IGThumb(new Data2(ThumbType.TextBlock, 100, 100, "ccccc")));
            MyCanvas.Children.Add(new IGThumb(new Data2(ThumbType.Path, 200, 0, new EllipseGeometry(new Rect(0, 0, 40, 40)), Brushes.Red)));

            //Thumb ー Data ー Textblockのとき
            //これだと通知プロパティじゃなくても反映される
            MyTextBlock.Text = "かきかえ";

        }
    }
}
