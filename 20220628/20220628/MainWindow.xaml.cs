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
using System.Windows.Controls.Primitives;


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

            DataContext = this;
            MyDatas.Add(MakeDataTextBlock(10, 20, "test1"));
            MyDatas.Add(MakeDataTextBlock(100, 40, "test2"));
        }

        private Data MakeDataTextBlock(double x, double y, string text)
        {
            DataTextBlock data = new();
            data.X = x; data.Y = y; data.Text = text;
            data.ForeColor = Brushes.Red;
            data.BackColor = Brushes.Orange;
            return data;
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

        private void TThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if(sender is Thumb tt)
            {
               var inu= MyItemsControl.ItemsSource;
               var uma= MyItemsControl.Items;
               var tako = MyItemsControl.ItemBindingGroup;
                //MyDatas[0].X += e.HorizontalChange;
                var x = Canvas.GetLeft(tt);
               var neko = tt.Parent;
                Canvas.SetLeft(tt, Canvas.GetLeft(tt) + e.HorizontalChange);
                Canvas.SetTop(tt, Canvas.GetTop(tt) + e.VerticalChange);
            }
            
        }
    }

    //public class Data1 : INotifyPropertyChanged
    //{
    //    private int _y;
    //    public int Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
    //    private int _x;
    //    public int X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
    //    private TextBlock? _textBlock;
    //    public TextBlock? TextBlock { get => _textBlock; set { if (_textBlock == value) { return; } _textBlock = value; OnPropertyChanged(); } }
    //    private BitmapSource? _bmp;

    //    public BitmapSource? Bmp { get => _bmp; set { if (_bmp == value) { return; } _bmp = value; OnPropertyChanged(); } }
    //    private Image? image;
    //    public Image? Image { get => image; set { if (image == value) { return; } image = value; OnPropertyChanged(); } }

    //    public event PropertyChangedEventHandler? PropertyChanged;
    //    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    //    }
    //}

}
