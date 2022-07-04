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
        TTextBlock ttb;
        public MainWindow()
        {
            InitializeComponent();

            DataTextBlock dtb = new(10, 20, 0, "text1", 30, Brushes.Yellow, Brushes.DeepPink);
            ttb = new(dtb);
            MyPanel.Children.Add(ttb);

            Thumb t = new() { Width = 100, Height = 20 };
            MyPanel.Children.Add(t);
            Canvas.SetLeft(t, 0);Canvas.SetTop(t, 0);
            t.DragDelta += (a, b) =>
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) + b.HorizontalChange);
                Canvas.SetTop(t, Canvas.GetTop(t) + b.VerticalChange);
            };
        }
    }
}
