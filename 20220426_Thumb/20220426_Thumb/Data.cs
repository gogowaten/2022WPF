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

namespace _20220426_Thumb
{
    public class Data : System.ComponentModel.INotifyPropertyChanged
    {
        private double x;
        private double y;
        private string text;
        private Brush foreground;
        private Geometry geometry;
        private Brush fill;

        public ThumbType ThumbType { get; set; }
        public ObservableCollection<Data> ChildrenData { get; set; } = new();
        public double X { get => x; set { if (x == value) { return; } x = value; OnPropertyChanged(); } }
        public double Y { get => y; set { if (y == value) { return; } y = value; OnPropertyChanged(); } }
        public string Text { get => text; set { if (text == value) { return; } text = value; OnPropertyChanged(); } }
        public Brush Background { get; set; }
        public Brush Foreground
        {
            get => foreground; set
            {
                if (foreground == value)
                {
                    return;
                }
                foreground = value;
                OnPropertyChanged();
            }
        }
        public Geometry Geometry { get => geometry; set { if (geometry == value) { return; } geometry = value; OnPropertyChanged(); } }
        public Brush Fill { get => fill; set { if (fill == value) { return; } fill = value; OnPropertyChanged(); } }
        public Data() { }
        public Data(ThumbType type, double x, double y)
        {
            ThumbType = type; X = x; Y = y;
        }
        public Data(ThumbType type, double x, double y, string text)
        {
            ThumbType = type; X = x; Y = y;
            Text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public override string ToString()
        {
            return $"{ThumbType}, {X}, {Y}";
        }
    }
}
