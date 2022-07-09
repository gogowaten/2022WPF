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
using System.Windows.Controls.Primitives;

namespace _20220704
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TTextBlock ttb1;
        TTextBlock ttb2;
        TTextBlock ttb3;
        CanvasThumb MyCanvas;
        LayerThumb MyLayer0;
        GroupThumb MyGroup;

        public MainWindow()
        {
            InitializeComponent();

            MyTest();

            MyCanvas = new(new DataCanvas(), "MyCanvas");
            DataContext = MyCanvas;

            DataTextBlock dtb = new(10, 20, 0, "text1", 30, Brushes.Yellow, Brushes.DeepPink);
            ttb1 = new(dtb);
            dtb = new(100, 120, 0, "text2", 30, Brushes.Yellow, Brushes.SkyBlue);
            ttb2 = new(dtb);
            ttb3 = new(new DataTextBlock(110, 40, 0, "test3", 30, Brushes.Yellow, Brushes.MediumPurple));


            MyGroup = new(new DataGroup(), "Group1");
            MyGroup.AddThumb(ttb1);
            MyGroup.AddThumb(ttb2);

            //group.AddDragEvents();

            MyLayer0 = new(new DataLayer(), "MyLayer0");
            MyLayer0.AddThumb(MyGroup);
            MyLayer0.AddThumb(ttb3);
            MyCanvas.AddLayer(MyLayer0);
            var neko = MyCanvas.MyChildren;
            MyPanel.Children.Add(MyCanvas);

            MyCanvas.MyEditingThumb = MyGroup;
        }

        private void MyTest()
        {

        }
        private void MyButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            ttb1.MyData.Text = "kakikaeta";
            Canvas.SetLeft(ttb1, 200);
            MyCanvas.MyEditingThumb = MyLayer0;
            var neko = MyCanvas.MyCurrentItem;
        }

        private void MyButtonCheck2_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.MyEditingThumb = MyGroup;
        }
    }
}
