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
        CanvasThumb MyCanvas;
        LayerThumb MyLayer0;
        public MainWindow()
        {
            InitializeComponent();


            DataTextBlock dtb = new(10, 20, 0, "text1", 30, Brushes.Yellow, Brushes.DeepPink);
            ttb1 = new(dtb);
            dtb = new(100, 120, 0, "text2", 30, Brushes.Yellow, Brushes.SkyBlue);
            ttb2 = new(dtb);
            

            GroupThumb group = new(new DataGroup());
            group.AddChild(ttb1);
            group.AddChild(ttb2);
            
            group.SetDragDelta();
            
            MyLayer0 = new(new DataLayer());
            MyLayer0.AddChild(group);
            MyCanvas=new(new DataCanvas());
            MyCanvas.AddChild(MyLayer0);
            var neko = MyCanvas.MyChildren;
            MyPanel.Children.Add(MyCanvas);
        }

        private void MyButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            ttb1.MyData.Text = "kakikaeta";
            Canvas.SetLeft(ttb1, 200);
        }
    }
}
