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

namespace _20221208
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TTTextBlock MyTTT;
        public MainWindow()
        {
            InitializeComponent();
            Test1();
        }
        private void Test1()
        {
            MyTTT = new TTTextBlock() { X = 100, Y = 100, Text = "MyTTT", FontColor = Brushes.Gold, FontSize = 30 };
            MyTTT.DragDelta += TT_DragDelta;
            MyCanvas.Children.Add(MyTTT);
            Data data = new() { Width= 100, Height = 100 ,BackColor = Brushes.Blue};
            TTRectangle rr= new TTRectangle(data);
            MyCanvas.Children.Add(rr);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyRectangle.X;
            var inu = Canvas.GetLeft(MyRectangle);
            inu = Canvas.GetLeft(MyTextBlock);
            neko = MyTextBlock.X;
        }

        private void TT_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                Canvas.SetLeft(tt, tt.X + e.HorizontalChange);
                Canvas.SetTop(tt, tt.Y + e.VerticalChange);
            }
        }
    }
}
