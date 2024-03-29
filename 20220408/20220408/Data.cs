﻿using System;
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

namespace _20220408
{
    public enum ThumbType
    {
        Layer = 0,
        Path,
        TextBlock,
        Image,
        Group,

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
        public DataText(string text, double x, double y, double z, double width = double.NaN, double height = double.NaN) : base(x, y, z, width, height)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
    public class DataRect : Data
    {
        public DataRect(double x, double y, double z, double width, double height) : base(x, y, z, width, height)
        {
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

    public class Data3Base
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
    public class Data3Path : Data3Base
    {
        public Geometry Geometry { get; set; }
        public Brush Fill { get; set; }
        public Data3Path() { ItemType = ThumbType.Path; }
        public Data3Path(Geometry geometry, Brush fill, double x, double y) : this()
        {
            Geometry = geometry; Fill = fill;
            X = x; Y = y;
        }
    }
    public class Data3TextBlock : Data3Base
    {
        public string Text { get; set; }
        public Data3TextBlock() { ItemType = ThumbType.TextBlock; }
        public Data3TextBlock(string text, double x, double y) : this()
        {
            Text = text; X = x; Y = y;
        }
    }
    public class Data3Group : Data3Base
    {
        public ObservableCollection<Data3Base> ChildrenData { get; set; } = new();
        public Data3Group() { ItemType = ThumbType.Group; }
        public Data3Group(List<ItemTThumb4> items, double x, double y) : this()
        {
            X = x; Y = y;
            foreach (ItemTThumb4 item in items)
            {
                ChildrenData.Add(item.MyData);
            }
        }
        public Data3Group(Data3Group data3Group)
        {
            ChildrenData = data3Group.ChildrenData;
        }
    }

    #region Data4

    //System.Runtime.Serialization.DataContract
    //System.Runtime.Serialization.KnownType
    [DataContract]
    [KnownType(typeof(Data4)),
        KnownType(typeof(MatrixTransform)),
        KnownType(typeof(EllipseGeometry))]
    //System.ComponentModel.INotifyPropertyChanged
    public class Data4 : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private string _text;

        //public TThumb5 Parent { get; set; }
        [DataMember]
        public ThumbType DataType { get; set; }
        [DataMember]
        public ObservableCollection<Data4> ChildrenData { get; set; } = new();
        public bool IsEditing { get; set; }//trueの場合は直下のItemが移動可能状態
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
    #endregion Data4

    #region Data7

    //System.Runtime.Serialization.DataContract
    //System.Runtime.Serialization.KnownType
    [DataContract]
    [KnownType(typeof(Data7)),
        KnownType(typeof(MatrixTransform)),
        KnownType(typeof(EllipseGeometry))]
    //System.ComponentModel.INotifyPropertyChanged
    public class Data7 : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private string _text;


        [DataMember]
        public DataType DataType { get; set; }

        [DataMember]
        public string Name { get; set; }
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

        public ObservableCollection<Data7> ChildrenData { get; set; } = new();
        public bool IsEditing { get; set; }//trueの場合は直下のItemが移動可能状態


        public Data7() { }

        public Data7(DataType type, double x, double y)
        {
            DataType = type; X = x; Y = y;
        }
        public Data7(DataType type, double x, double y, string text)
        {
            DataType = type; X = x; Y = y;
            Text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString()
        {
            return $"{DataType} {X} {Y}";
        }
    }
    //[DataContract]
    //public class Data7Item : Data7
    //{
    //    public Data7Item() { }
    //    public Data7Item(DataType type, double x, double y) : base(type, x, y) { }
    //}
    //[DataContract]
    //public class Data7Group : Data7
    //{
    //    [DataMember]
    //    public ObservableCollection<Data7> ChildrenData { get; set; } = new();
    //    public bool IsEditing { get; set; }//trueの場合は直下のItemが移動可能状態
    //    public Data7Group() { DataType = DataType.Group; }
    //    public Data7Group(double x, double y) : base(DataType.Group, x, y)
    //    {

    //    }
    //}
    //public class Data7Layer : Data7Group
    //{
    //    public Data7Layer() { DataType = DataType.Layer; }
    //}
    #endregion Data7

}
