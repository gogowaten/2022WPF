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
    [DataContract]
    [KnownType(typeof(BitmapSource))]
    public class Data : INotifyPropertyChanged
    {
        private double _y; public double Y { get => _y; set => SetProperty(ref _y, value); }
        private double _x; public double X { get => _x; set => SetProperty(ref _x, value); }


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
        private string? _text; public string? Text { get => _text; set => SetProperty(ref _text, value); }
        private Brush? _foreColor; public Brush? ForeColor { get => _foreColor; set => SetProperty(ref _foreColor, value); }
        private Brush? _backColor; public Brush? BackColor { get => _backColor; set => SetProperty(ref _backColor, value); }

    }
    public class DataPath : Data
    {
        private Brush? _fill; public Brush? Fill { get => _fill; set => SetProperty(ref _fill, value); }
        private Geometry? _geometry; public Geometry? Geometry { get => _geometry; set => SetProperty(ref _geometry, value); }

        public DataPath(Brush? fill = null)
        {
            this.Fill = fill ?? Brushes.Yellow;
        }

    }

}
