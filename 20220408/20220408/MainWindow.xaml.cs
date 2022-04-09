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
using System.Collections.ObjectModel;
//Canvasコントロールの子要素を動的に増減させたい
//https://teratail.com/questions/359699


namespace _20220408
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Item> Items { get; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Items = new ObservableCollection<Item>
            {
                new EllipseItem { X = 200, Y = 50, Width = 50, Height = 100, Fill = Brushes.Green, },
            };
            Items.Add(new ThumbItem() { BackGround = Brushes.Cyan, Height = 100, Width = 100, X = 10, Y = 10 });
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(new ThumbItem() { BackGround = Brushes.MidnightBlue, Height = 100, Y = 100, X = 100, Width = 100 });
        }
    }
    public class Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class ThumbItem : Item
    {
        public Brush BackGround { get; set; }
    }


    public class EllipseItem : Item
    {
        public Brush Fill { get; set; }
    }




}
