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
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.ObjectModel;


//1つのTextBlockを複数にBindingしたときの表示は
//どれか1つにしか表示されない
//intとかの値型のものは同時に表示できる
//BitmapSourceも同時に表示できる
//Imageは1つだけ表示
//ってことはDataTemplateでContentControlや
//ContentPresenterを使って表示するものは
//1つしか表示できないみたい
namespace _20220628
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Data> MyDatas { get; set; } = new();
        public MainWindow()
        {
            InitializeComponent();

            Data data = new(); MyDatas.Add(data);
            data.X = 111;
            data.Y = 222;
            data.TextBlock = new() { Text = "textblock", Foreground = Brushes.Red };
            data.Bmp = MakeBitmap(Colors.MediumAquamarine);
            data.Image = new Image() { Source = MakeBitmap(Colors.DeepPink) };
            DataContext = MyDatas;
        }
        private BitmapSource MakeBitmap(Color col)
        {
            int w = 20; int h = 20;
            int stride = w * 4;
            byte[] pixels = new byte[stride * h];
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = col.B; pixels[i + 1] = col.G; pixels[i + 2] = col.R; pixels[i + 3] = col.A;
            }
            BitmapSource bmp = BitmapSource.Create(w, h, 96, 96, PixelFormats.Pbgra32, null, pixels, stride);
            return bmp;
        }
    }

    public class Data : INotifyPropertyChanged
    {
        private int _y;
        public int Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
        private int _x;
        public int X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
        private TextBlock? _textBlock;
        public TextBlock? TextBlock { get => _textBlock; set { if (_textBlock == value) { return; } _textBlock = value; OnPropertyChanged(); } }
        private BitmapSource? _bmp;

        public BitmapSource? Bmp { get => _bmp; set { if (_bmp == value) { return; } _bmp = value; OnPropertyChanged(); } }
        private Image? image;
        public Image? Image { get => image; set { if (image == value) { return; } image = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Data2 : INotifyPropertyChanged
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
}
