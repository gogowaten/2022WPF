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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace _20221205
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Test1();
            TestJson();

        }
        private void Test1()
        {
            //MyCanvas.Children.Add(new TThumb(new DDTextBlock()));
            DDTextBlock data = new() { FontColor = Brushes.MediumOrchid, FontSize = 20.0, Text = "TTTT" };
            MyCanvas.Children.Add(new TTTextBlock(data));
            DDRectangle rData = new() { Width = 100, Height = 30, Fill = Brushes.MediumAquamarine, X = 100, Y = 20 };
            MyCanvas.Children.Add(new TTRectAngle(rData));
            var neko = new TTRectAngle(rData);
            var inu = neko.MyData;
        }

        private void TT3_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (sender is TT3 tt)
            {
                Canvas.SetLeft(tt, Canvas.GetLeft(tt) + e.HorizontalChange);
                Canvas.SetTop(tt, Canvas.GetTop(tt) + e.VerticalChange);
            };
        }

        private void TestJson()
        {
            TT3 tt = new();
            string js=JsonSerializer.Serialize(tt);
        }

    }
}

