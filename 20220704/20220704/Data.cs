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
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace _20220704
{
    public enum DataType
    {
        None = 0,
        TextBlock,
        Path,
        Rectangle
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
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        [DataMember] public DataType DataType { get; protected set; }
        [DataMember] public DataMainType DataMainType { get; protected set; }
        private double _x; [DataMember] public double X { get => _x; set => SetProperty(ref _x, value); }
        private double _y; [DataMember] public double Y { get => _y; set => SetProperty(ref _y, value); }
        private double _z; [DataMember] public double Z { get => _z; set => SetProperty(ref _z, value); }

    }
    public class DataItem : Data
    {
        public DataItem() { DataMainType = DataMainType.Item; }
    }
    public class DataTextBlock : DataItem
    {
        public DataTextBlock()
        {
            DataType = DataType.TextBlock;
        }
        public DataTextBlock(double x, double y, double z, string text, double fontsize, Brush colorfont, Brush colorback)
        {
            X = x; Y = y; Z = z;
            Text = text; FontSize = fontsize;
            ColorFont = colorfont; ColorBack = colorback;
        }
        private string? _text; [DataMember] public string? Text { get => _text; set => SetProperty(ref _text, value); }

        private Brush? _colorFont; [DataMember] public Brush? ColorFont { get => _colorFont; set => SetProperty(ref _colorFont, value); }

        private Brush? _colorBack; [DataMember] public Brush? ColorBack { get => _colorBack; set => SetProperty(ref _colorBack, value); }

        private double _fontSize; [DataMember] public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

    }
    public class DataRectangle : DataItem
    {
        public DataRectangle()
        {
            DataType = DataType.Rectangle;
        }

        private Brush? _colorFill; [DataMember] public Brush? ColorFill { get => _colorFill; set => SetProperty(ref _colorFill, value); }

        private Brush? _colorStroke; [DataMember] public Brush? ColorStroke { get => _colorStroke; set => SetProperty(ref _colorStroke, value); }

        private double _strokeThickness; [DataMember] public double StrokeThickness { get => _strokeThickness; set => SetProperty(ref _strokeThickness, value); }

    }
}
