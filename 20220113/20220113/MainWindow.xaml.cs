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
        int MyCount = 0;
        List<TThumb> MyList = new();
        public MainWindow()
        {
            InitializeComponent();


            //AddElement();
            //AddElement();

            //MyTThumb = TThumb.CreateTextBlockThumb("初期要素1",40,0,0,"要素1");
            //MyLayer1.AddThumb(MyTThumb);
            //TThumb tt = TThumb.CreateTextBlockThumb("初期要素2", 40, 30, 70,"要素2");
            //MyLayer1.AddThumb(tt);

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

        private void AddElement()
        {
            TThumb t= TThumb.CreateTextBlockThumb($"要素{MyCount}", 30, MyCount * 30, MyCount * 50, $"要素{MyCount}");
            MyList.Add(t);
            MyLayer1.AddThumb(t);
            MyCount++;
        }
        private void ButtonTest2_Click(object sender, RoutedEventArgs e)
        {
            
            //MyLayer1.AddThumb(TThumb.CreateTextBlockThumb("追加要素1", 40, 140, 140,"追加要素1"));
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

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddElement();
            //MyGTThumb.chil
            //MyGTThumb.GroupCanvas.Children.Add
        }

        private void ButtonGroup1_2_Click(object sender, RoutedEventArgs e)
        {
            //TThumb g = new(MyList.Take(2).ToList());
            TThumb.ToGroup(MyList.Take(2).ToList());
        }
    }
}