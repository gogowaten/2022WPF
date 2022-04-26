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

namespace _20220426_Thumb
{
public    class Data
    {
        public ThumbType DataType { get; set; }
        public ObservableCollection<Data> ChildrenData { get; set; } = new();
        public double X { get; set; }
        public double Y { get; set; }
        public string Text { get; set; }
        public Brush Background { get; set; }
        public Geometry Geometry { get; set; }
        public Brush Fill { get; set; }
        public Data() { }
        public Data(ThumbType type, double x, double y)
        {
            DataType = type; X = x; Y = y;
        }
        public Data(ThumbType type, double x, double y, string text)
        {
            DataType = type; X = x; Y = y;
            Text = text;
        }

        public override string ToString()
        {
            return $"{DataType}, {X}, {Y}";
        }
    }
}
