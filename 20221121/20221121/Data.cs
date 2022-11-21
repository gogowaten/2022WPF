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
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace _20221121
{
    class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string Text { get; set; } = "";
        public Data(string text, double x, double y, double z)
        {
            Text = text; X = x; Y = y; Z = z;
        }
    }
}
