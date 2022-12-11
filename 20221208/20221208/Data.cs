using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _20221208
{
    public enum TType { TextBlock = 0, Rectangle, Group, }
    public class Data
    {
        public Data(TType type)
        {
            Type = type;
        }
        public TType Type { get; private set; }
        public ObservableCollection<Data>? Datas { get; set; }
        public string MyName { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public int Z { get; set; }
        public string Text { get; set; } = "";
        public Brush? ForeColor { get; set; }
        public Brush? BackColor { get; set; }
        public double FontSize { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class Builder
    {
        private TThumb? Product { get; set; }
        public Builder()
        {
            
        }
        public void Build(Data data)
        {
            switch (data.Type)
            {
                case TType.TextBlock:
                    Product = new TTTextBlock()
                    {
                        Text = data.Text,
                        X = data.X,
                        Y = data.Y,
                        Z = data.Z,
                        FontColor = data.ForeColor ?? Brushes.Red,
                    };
                    break;
                case TType.Rectangle:
                    break;
                case TType.Group:
                    break;
                default:
                    break;
            }
        }
        public TThumb? GetProduct() { return Product; }
    }
}
