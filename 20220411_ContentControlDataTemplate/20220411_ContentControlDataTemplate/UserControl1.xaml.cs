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

namespace _20220411_ContentControlDataTemplate
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public Data MyData { get; set; }
        public UserControl1()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }

    public class Data
    {
        public Data() { }
        public Data(double x, double y, double z, double width, double height)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class DataText : Data
    {      
        public DataText(string text, double fontSize, double x, double y, double z) : base(x, y, z, double.NaN, double.NaN)
        {
            Text = text;
            FontSize = fontSize;
        }

        public string Text { get; set; }
        public double FontSize { get; set; }
    }
    public class DataRect : Data
    {
        public DataRect(Brush brush, double x, double y, double z, double width, double height) : base(x, y, z, width, height)
        {
            Brush = brush;
        }

        public Brush Brush { get; set; }
    }
    public class DataPath : Data
    {
        public DataPath(Geometry geometry, Brush stroke, double x, double y, double z, double width = double.NaN, double height = double.NaN) : base(x, y, z, width, height)
        {
            Geometry = geometry;
            Stroke = stroke;
        }
        public Geometry Geometry { get; set; }
        public Brush Stroke { get; set; }
    }

}
