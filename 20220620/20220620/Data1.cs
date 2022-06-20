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

namespace _20220620
{

    #region Data4

    [DataContract]
    [KnownType(typeof(RectangleGeometry)),
     KnownType(typeof(MatrixTransform)),
        KnownType(typeof(SolidColorBrush))]
    public class Data1 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _x;
        private double _y;
        private int _z;
        private string _text = "";
        private Brush? _background;
        private Brush _foreground = Brushes.Black;
        private double _padding = 0;
        [DataMember] public ObservableCollection<Data1> ChildrenData { get; set; } = new();
        [DataMember] public DataType DataType { get; set; }
        [DataMember] public DataTypeMain DataTypeMain { get; set; }
        [DataMember]
        public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
        [DataMember]
        public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
        [DataMember]
        public int Z { get => _z; set { if (_z == value) { return; } _z = value; OnPropertyChanged(); } }

        [DataMember]
        public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
        [DataMember]
        public Brush? Background
        {
            get => _background; set
            {
                if (_background == value) { return; }
                _background = value; OnPropertyChanged();
            }
        }
        [DataMember]
        public Brush Foreground
        {
            get => _foreground;
            set
            {
                if (_foreground == value) { return; }
                _foreground = value; OnPropertyChanged();
            }
        }
        [DataMember]
        public double Padding
        {
            get => _padding;
            set { if (value == _padding) { return; } _padding = value; OnPropertyChanged(); }
        }
        [DataMember]
        public Geometry? Geometry { get; set; }


        public Data1(DataType type)
        {
            DataType = type;
            if (type == DataType.Group || type == DataType.Layer)
            {
                DataTypeMain = DataTypeMain.Group;
            }
            else DataTypeMain = DataTypeMain.Item;
        }
        public override string ToString()
        {
            return $"{X},{Y},{Z},{DataType},{Text}";
        }
    }
    #endregion Data4


    public class MyConverterItemWaku1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //List<Brush> brushes = (List<Brush>)parameter;
            //bool b1 = (bool)values[0];
            //bool b2 = (bool)values[1];
            //if (b1) { return brushes[0]; }
            //else if (b2) { return brushes[1]; }
            //else { return Brushes.Transparent; }

            List<Brush> brushes = (List<Brush>)parameter;
            bool b2 = (bool)values[1];
            if (b2) { return brushes[1]; }
            else { return Brushes.Transparent; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class MyConverterGroupWaku4 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<Brush> brushes = (List<Brush>)parameter;
            bool b1 = (bool)values[0];
            bool b2 = (bool)values[1];
            if (b1) { return brushes[0]; }
            else if (b2) { return brushes[1]; }
            //else { return Brushes.Transparent; }
            else { return brushes[2]; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
