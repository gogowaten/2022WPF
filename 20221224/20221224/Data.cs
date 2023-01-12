using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Windows.Media.Imaging;

namespace _20221224
{
    public enum DType { None = 0, TextBlock, Group, Image }
    [DataContract]
    public class Data : INotifyPropertyChanged, IExtensibleDataObject
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public DType Type { get; protected set; }
        private double _x;
        public double X { get => _x; set => SetProperty(ref _x, value); }

        private double _y;
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        public ExtensionDataObject? ExtensionData { get; set; }
        public Data() { Type = DType.None; }
    }
    public class DataText : Data
    {
        private string? _myText;
        public string? MyText { get => _myText; set => SetProperty(ref _myText, value); }

        private double _fontSize;
        public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }
        public DataText() { Type = DType.TextBlock; }

    }
    public class DataGroup : Data
    {
        private Collection<Data> _childrenData = new();
        public Collection<Data> ChildrenData { get => _childrenData; set => SetProperty(ref _childrenData, value); }
        public DataGroup() { Type = DType.Group; }

    }
    public class DataImage : Data
    {
        private BitmapSource? _myImage;
        public BitmapSource? MyImage { get => _myImage; set => SetProperty(ref _myImage, value); }
        public DataImage() { Type = DType.Image; }

    }

}
