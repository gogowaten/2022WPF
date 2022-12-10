using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _20221210_Thumb継承したItem候補
{
    public enum TType { TextBlock = 0, Rectangle, Group, Polyline }
    public abstract class Data
    {

        public abstract TType Type { get; protected set; }

        public string Name { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public int Z { get; set; }

    }
    public class GroupData : Data
    {
        public override TType Type { get; protected set; }
        public ObservableCollection<Data> Datas { get; set; } = new();
        public GroupData()
        {
            Type = TType.Group;
        }
    }
    public class TextBlockData : Data
    {
        public override TType Type { get; protected set; }
        public string Text { get; set; } = "";
        public FontFamily Font { get; set; } = SystemFonts.MenuFontFamily;// = Application.Current.MainWindow.FontFamily;
        public Brush FontColorBrush { get; set; } = Brushes.Purple;
        public double FontSize { get; set; }
        public Brush BackColorBrush { get; set; } = Brushes.Orange;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextBlockData() { Type = TType.TextBlock; }
    }

    public class RectangleData : Data
    {
        public override TType Type { get; protected set; }
        public double ShapeWidth { get; set; }
        public double ShapeHeight { get; set; }
        public Brush? FillColorBrush { get; set; }
        public Brush? StrokeColorBrush { get; set; }

        public RectangleData() { Type = TType.Rectangle; }
    }

    public class PolylineData : Data
    {
        public override TType Type { get; protected set; }
        public double Thickness { get; set; }
        public PointCollection? Points { get; set; }
        public Brush LineColorBrush { get; set; } = Brushes.Orange;

        public PolylineData() { Type = TType.Polyline; }
    }

}
