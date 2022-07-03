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

namespace _20220628
{
    public enum DataType
    {
        None = 0,
        TextBlock,
        Path
    }
    public enum DataMainType
    {
        None = 0,
        Item,
        Group
    }

    [DataContract]
    [KnownType(typeof(BitmapSource))]
    public class Data : INotifyPropertyChanged
    {
        [DataMember] private DataType _type; public DataType Type { get => _type; set => SetProperty(ref _type, value); }
        [DataMember] private DataMainType _mainType; public DataMainType MainType { get => _mainType; set => SetProperty(ref _mainType, value); }

        [DataMember] private double _y; public double Y { get => _y; set => SetProperty(ref _y, value); }
        [DataMember] private double _x; public double X { get => _x; set => SetProperty(ref _x, value); }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            //if(Equals(field, value)) return;//これだと値型の場合にメモリを消費してしまう？
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public class DataTextBlock : Data
    {
        [DataMember] private string? _text; public string? Text { get => _text; set => SetProperty(ref _text, value); }
        [DataMember] private double _fontSize; public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }
        [DataMember] private Brush? _foreColor; public Brush? ForeColor { get => _foreColor; set => SetProperty(ref _foreColor, value); }
        [DataMember] private Brush? _backColor; public Brush? BackColor { get => _backColor; set => SetProperty(ref _backColor, value); }
        public DataTextBlock(double x, double y, string? text,double fontsize, Brush? foreColor, Brush? backColor)
        {
            X = x; Y = y;
            FontSize = fontsize;
            Text = text;
            ForeColor = foreColor;
            BackColor = backColor;
        }
    }
    public class DataPath : Data
    {
        [DataMember] private Brush? _fill; public Brush? Fill { get => _fill; set => SetProperty(ref _fill, value); }
        [DataMember] private Geometry? _geometry; public Geometry? Geometry { get => _geometry; set => SetProperty(ref _geometry, value); }

        public DataPath( Geometry geometry,Brush? fill = null)
        {
            this.Fill = fill ?? Brushes.Yellow;
            this.Geometry = geometry;
        }

    }

}
