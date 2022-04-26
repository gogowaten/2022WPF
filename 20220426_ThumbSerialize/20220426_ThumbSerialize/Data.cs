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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel;


namespace _20220426_ThumbSerialize
{
    public enum ThumbType
    {
        Layer = 0,
        Path,
        TextBlock,
        Image,
        Group,

    }


    [System.Runtime.Serialization.DataContract]
    [System.Runtime.Serialization.KnownType(typeof(Data4)),
        System.Runtime.Serialization.KnownType(typeof(MatrixTransform)),
        System.Runtime.Serialization.KnownType(typeof(EllipseGeometry))]
    public class Data4 : System.ComponentModel.INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private string _text;

        [DataMember]
        public ThumbType DataType { get; set; }
        [DataMember]
        public ObservableCollection<Data4> ChildrenData { get; set; } = new();
        [DataMember]
        public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
        [DataMember]
        public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
        [DataMember]
        public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
        [DataMember]
        public Brush Background { get; set; }
        [DataMember]
        public Geometry Geometry { get; set; }
        [DataMember]
        public Brush Fill { get; set; }

        public Data4() { }
        public Data4(ThumbType type, double x, double y)
        {
            DataType = type; X = x; Y = y;
        }
        public Data4(ThumbType type, double x, double y, string text)
        {
            DataType = type; X = x; Y = y;
            Text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
