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

namespace _20220113
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TThumb MyTThumb;
        TThumb MyGTThumb;
        public MainWindow()
        {
            InitializeComponent();

           


            MyTThumb = TThumb.CreateTextBlockThumb();
            MyLayer1.AddThumb(MyTThumb);
            TThumb tt = TThumb.CreateTextBlockThumb("test2", 20, 30, 30);
            MyLayer1.AddThumb(tt);

            //TextBlock tb = new() { Text = "test", FontSize = 30, Background = Brushes.MediumAquamarine };
            //tb.RenderTransform = new ScaleTransform(2, 2);
            //MyTThumb = new(tb);
            //MyCanvas.Children.Add(MyTThumb);
            //TTT tTT = new();
            ////MyTThumb.DragDelta += MyTThumb_DragDelta;


            //List<TThumb> thumbs = new();
            //for (int i = 0; i < 3; i++)
            //{
            //    TThumb t = new TThumb(MakeTextBlock($"test{i}"));
            //    t.MyData.X = i * 30; t.MyData.Y = i * 30;
            //    thumbs.Add(t);
            //}
            //MyGTThumb = new TThumb(thumbs);
            //MyCanvas.Children.Add(MyGTThumb);

        }

        private TextBlock MakeTextBlock(string text)
        {
            TextBlock tb = new()
            {
                Text = text,
                FontSize = 30,
                Background = new SolidColorBrush(Color.FromArgb(200, 100, 200, 150)),
                Foreground = Brushes.White,
                Padding = new Thickness(10),
                //RenderTransform = new ScaleTransform(2, 2),
            };
            return tb;
        }

        //private void MyTThumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    TThumb t = (TThumb)sender;
        //    t.MyData.X += e.HorizontalChange;
        //    t.MyData.Y += e.VerticalChange;
        //}

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            //var neko = MyTThumb.ActualWidth;
            //var inu = MyTThumb.Width;
            //var element = MyTThumb.MyContentElement;
            //var ew = element.Width;
            //var eaw = element.ActualWidth;
            //var mcaw = MyCanvas.ActualWidth;

            //var aw = MyGTThumb.ActualWidth;
            //var w = MyGTThumb.Width;
            //var left = MyGTThumb.MyData.X;
            //var top = MyGTThumb.MyData.Y;
            //var data = MyGTThumb.MyData;

            var chi = MyLayer1.Children;
        }

        private void ButtonTest2_Click(object sender, RoutedEventArgs e)
        {
            
            MyLayer1.AddThumb(TThumb.CreateTextBlockThumb("addthumb", 30, 40, 40));
            //MyGTThumb.IsEditInsideGroup = !MyGTThumb.IsEditInsideGroup;
            //if (MyGTThumb.MyData.VisibleFrame == Visibility.Visible)
            //{
            //    MyGTThumb.MyData.VisibleFrame = Visibility.Collapsed;
            //}
            //else
            //{
            //    MyGTThumb.MyData.VisibleFrame = Visibility.Visible;
            //}
        }
    }
}