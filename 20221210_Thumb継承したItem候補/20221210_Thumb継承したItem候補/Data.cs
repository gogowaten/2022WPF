using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _20221210_Thumb継承したItem候補
{
    public enum TType { TextBlock = 0, Rectangle, Group, }
    public class Data
    {
        public Data(TType type) { Type = type; }
        public TType Type { get; private set; }
        public ObservableCollection<Data>? Datas { get; set; }
        public string Name { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public int Z { get; set; }
        public string Text { get; set; } = "";
        public FontFamily Font { get; set; } = SystemFonts.MenuFontFamily;// = Application.Current.MainWindow.FontFamily;
        public Brush? ForeColor { get; set; }
        public Brush? BackColor { get; set; }
        public double FontSize { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Thickness { get; set; }
        public PointCollection? Points { get; set; }
    }
}
