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

namespace _20220410_CanvasBinging
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public ObservableCollection<Item> Items { get; } = new();

        public UserControl1()
        {
            InitializeComponent();
            DataContext = this;
        }


    }
    public class Item
    {
        public Item(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class RectItem : Item
    {
        public Brush Fill { get; set; }

        public RectItem(double x, double y, double widht, double height, Brush fill) : base(x, y, widht, height)
        {
            Fill = fill;
        }
    }

    public class EllipseItem : Item
    {
        public EllipseItem(double x, double y, double width, double height, Brush fill) : base(x, y, width, height)
        {
            Fill = fill;
        }

        public Brush Fill { get; set; }
    }
}
