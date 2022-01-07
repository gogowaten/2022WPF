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

namespace _20220106_Thumb選択
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point MyStartPoint;
        bool IsGetting;
        RectangleGeometry MyGeometry = new();
        List<FrameworkElement> MyElements = new();
        Path MyPath;
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 4; i++)
            {
                Border border = new()
                {
                    Name = $"Border{i}",
                    Width = 100,
                    Height = 100,
                    Background = new SolidColorBrush(Color.FromArgb(100, 30, (byte)(100 + i * 40), 50))
                };
                Canvas.SetLeft(border, i * 20 + 40);
                Canvas.SetTop(border, i * 60 + 40);
                MyCanvas.Children.Add(border);
                MyElements.Add(border);

                TextBlock textBlock = new()
                {
                    Name = $"TextBlock{i}",
                    Text = $"TextBlock{i}",
                    FontSize = 30,
                    Background = new SolidColorBrush(Color.FromArgb(255, 120, 110, (byte)(150 + i * 30))),
                    Foreground = Brushes.White
                };
                Canvas.SetLeft(textBlock, i * 40 + 150);
                Canvas.SetTop(textBlock, i * 70 + 30);
                MyCanvas.Children.Add(textBlock);
                MyElements.Add(textBlock);
            }

            MyPath = new() { Stroke = Brushes.Red, StrokeThickness = 1 };
            MyCanvas.Children.Add(MyPath);

        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {

        }



        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            MyStartPoint = e.GetPosition(canvas);
            IsGetting = true;
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsGetting == false) { return; }
            Canvas canvas = sender as Canvas;

            MyGeometry.Rect = new Rect(MyStartPoint, e.GetPosition(canvas));
            MyPath.Data = MyGeometry;
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsGetting = false;

            List<FrameworkElement> elements = new();
            string names = "";
            foreach (FrameworkElement item in MyElements)
            {
                if (MyGeometry.Bounds.IntersectsWith(new Rect(GetLocate(item), item.RenderSize)))
                {
                    elements.Add(item);
                    names += item.Name + "\n";
                }
            }
            MyTextBlock.Text = names;
            //MyPath.Data = null;
        }
        private Point GetLocate(FrameworkElement element)
        {
            return new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
        }
    }
}
