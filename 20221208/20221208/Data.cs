using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _20221208
{
    public enum TType { TextBlock = 0, Rectangle, Group, }
    public class Data
    {
        public Data(TType type)
        {
            Type = type;
        }
        public TType Type { get; private set; }
        public ObservableCollection<Data>? Datas { get; set; }
        public string MyName { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public int Z { get; set; }
        public string Text { get; set; } = "";
        public Brush? ForeColor { get; set; }
        public Brush? BackColor { get; set; }
        public double FontSize { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    //未使用
    public class Builder
    {
        private TThumb? Product { get; set; }
        public Builder()
        {

        }
        public void Build(Data data)
        {
            switch (data.Type)
            {
                case TType.TextBlock:
                    Product = new TTTextBlock()
                    {
                        Text = data.Text,
                        X = data.X,
                        Y = data.Y,
                        Z = data.Z,
                        FontColor = data.ForeColor ?? Brushes.Red,
                    };
                    break;
                case TType.Rectangle:
                    break;
                case TType.Group:
                    break;
                default:
                    break;
            }
        }
        public TThumb? GetProduct() { return Product; }
    }

    //未使用、TTRootへ移行
    public class Manager : Canvas, INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private TThumb? _active;
        public TThumb? Active { get => _active; set => SetProperty(ref _active, value); }

        private TThumb? _enable;
        public TThumb? Enable { get => _enable; set => SetProperty(ref _enable, value); }

        private TThumb? _clicked;
        public TThumb? Clicked { get => _clicked; set => SetProperty(ref _clicked, value); }
        public Manager()
        {
            PreviewMouseLeftButtonDown += Manager_PreviewMouseLeftButtonDown;
        }

        private void Manager_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var source = e.Source;
            var origin = e.OriginalSource;
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //クリックされたThumbを登録
            if (((FrameworkElement)e.OriginalSource).TemplatedParent is TThumb tt)
            {
                Clicked = tt;
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }
    }
}
