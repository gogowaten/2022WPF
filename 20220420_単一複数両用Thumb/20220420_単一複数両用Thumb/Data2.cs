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
using System.Windows.Controls.Primitives;

namespace _20220420_単一複数両用Thumb
{
    public class Data2
    {
        public ObservableCollection<Data2> Children { get; set; }
        public ThumbType ItemType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Text { get; set; }
        public Geometry Geometry { get; set; }
        public Brush Stroke { get; set; }
        public Data2() { }
        public Data2(ThumbType type, double x, double y, Geometry geometry, Brush brush)
        {
            ItemType = type;
            X = x; Y = y;
            Geometry = geometry;
            Stroke = brush;
        }
        public Data2(ThumbType type, double x, double y, string text)
        {
            ItemType = type;
            X = x; Y = y;
            Text = text;
        }
        public Data2(ThumbType type, ObservableCollection<Data2> children, double x, double y)
        {
            ItemType = type;
            Children = children;
            X = x; Y = y;
        }

    }
    public enum ThumbType
    {
        Path,
        TextBlock,
        Image,
        Group,

    }

}
