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

namespace _20220425_Thumb
{
    public enum ThumbType
    {
        Path,
        TextBlock,
        Image,
        Group,

    }
    public class Data
    {
        public ThumbType ItemType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString()
        {
            //return base.ToString();
            return $"(x, y)=({X}, {Y}), {ItemType}";
        }

    }

    public class DataPath : Data
    {
        public Geometry Geometry { get; set; }
        public Brush Fill { get; set; }
        public DataPath() { ItemType = ThumbType.Path; }
        public DataPath(Geometry geometry, Brush fill, double x, double y) : this()
        {
            Geometry = geometry; Fill = fill;
            X = x; Y = y;
        }
    }
    public class DataTextBlock : Data
    {
        public string Text { get; set; }
        public DataTextBlock() { ItemType = ThumbType.TextBlock; }
        public DataTextBlock(string text, double x, double y) : this()
        {
            Text = text; X = x; Y = y;
        }
    }
    public class DataGroup : Data
    {
        public ObservableCollection<Data> ChildrenData { get; set; } = new();
        public DataGroup() { ItemType = ThumbType.Group; }
        public DataGroup(List<Data> datas, double x, double y) : this()
        {
            X = x; Y = y;
            foreach (var item in datas)
            {
                ChildrenData.Add(item);
            }
        }       
        public DataGroup(DataGroup dataGroup)
        {
            ChildrenData = dataGroup.ChildrenData;
        }

        public void AddData(Data data)
        {
            ChildrenData.Add(data);
        }
    }


}
