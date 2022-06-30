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
    class TThumbsControl : ItemsControl
    {
        public TThumbsControl()
        {
            ItemsPanelTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas));
            this.ItemsPanel = template;

            Style style = new();
            this.ItemContainerStyle = style;
            style.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("X")));
            style.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Y")));
        }
    }

    [DataContract]
    [KnownType(typeof(BitmapSource))]
    public class Data : INotifyPropertyChanged
    {
        private int _y;
        public int Y { get => _y; set => SetProperty(ref _y, value); }

        private int _x;
        public int X { get => _x; set => SetProperty(ref _x, value); }

        private TextBlock? _textBlock;
        public TextBlock? TextBlock { get => _textBlock; set => SetProperty(ref _textBlock, value); }

        private BitmapSource? _bmp;
        public BitmapSource? Bmp { get => _bmp; set => SetProperty(ref _bmp, value); }

        private Image? image;
        public Image? Image { get => image; set => SetProperty(ref image, value); }

        public event PropertyChangedEventHandler? PropertyChanged;

        //        【Unity】【C#】Genericな型の等価判定によるメモリアロケーション及びUnity組み込み構造体のIEquatable実装状況 - LIGHT11
        //https://light11.hatenadiary.com/entry/2020/08/03/210816
        //【WPF】Binding入門1。DataContextの伝搬 | さんさめのC＃ブログ
        //https://threeshark3.com/wpf-binding-datacontext/
        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            //if(Equals(field, value)) return;//これだと値型の場合にメモリを消費してしまう？
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
   public class TThumb : Thumb
    {
        public TThumb()
        {
            ControlTemplate template = new(typeof(Thumb));

        }
    }
}
