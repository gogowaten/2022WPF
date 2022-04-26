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

namespace _20220426_Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TThumb thumb5_1;
        TThumb thumb5_2;
        TThumb thumbGeo;
        public MainWindow()
        {
            InitializeComponent();


            Data data1 = new(ThumbType.TextBlock, 10, 20, "Test1");
            thumb5_1 = new TThumb(data1);
            MyLayer.AddItem(thumb5_1);

            Data data3 = new(ThumbType.TextBlock, 0, 0, "Test2");
            Data data4 = new(ThumbType.TextBlock, 0, 30, "Test3");
            Data data2 = new(ThumbType.Group, 20, 40);
            data2.ChildrenData.Add(data3);
            data2.ChildrenData.Add(data4);
            thumb5_2 = new(data2);
            MyLayer.AddItem(thumb5_2);

            Data geoData1 = new(ThumbType.Path, 40, 120) { Fill = Brushes.MediumAquamarine };
            geoData1.Geometry = new EllipseGeometry(new Rect(0, 0, 40, 40));
            thumbGeo = new TThumb(geoData1);
            MyLayer.AddItem(thumbGeo);

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var data = MyLayer.MyData;
            var datad = MyLayer.Items;
            var neko = thumb5_2.MyData;
            thumb5_1.MyData.X += 20;
            thumb5_1.MyData.Text = "kakikae";
            thumb5_2.MyData.ChildrenData[0].Text = "kakikae2";
            thumb5_1.MyData.Foreground = Brushes.Red;
            thumbGeo.MyData.Fill =
                new LinearGradientBrush(
                    new GradientStopCollection(){
                        new GradientStop(Colors.Magenta, 0.2),
                        new GradientStop(Colors.Cyan, 0.5),
                        new GradientStop(Colors.Orange, 0.7)});
        }
    }
}
