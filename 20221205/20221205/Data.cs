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
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace _20221205
{
    public enum DataType
    {
        None = 0, TextBlock,
    }
    
    [DataContract]
    public abstract class Data : INotifyPropertyChanged
    {
        public Data() { }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private double _x; public double X { get => _x; set => SetProperty(ref _x, value); }

        private double _y; public double Y { get => _y; set => SetProperty(ref _y, value); }
        public abstract DataType DataType { get; }

        
    }
    public class DDTextBlock : Data
    {
        public DDTextBlock() { }

        private string _text = "Default";
        public string Text { get => _text; set => SetProperty(ref _text, value); }

        private Brush _fontColor = Brushes.Black;
        public Brush FontColor { get => _fontColor; set => SetProperty(ref _fontColor, value); }

        private double _fontSize = 12.0;
        public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

        public override DataType DataType => DataType.TextBlock;
    }
}
