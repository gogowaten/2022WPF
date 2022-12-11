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
using System.ComponentModel;

//Canvasコントロールの子要素を動的に増減させたい
//https://teratail.com/questions/359699

namespace _20220409CanvasSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Item> Items { get; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Items = new ObservableCollection<Item>
            {
                new PathItem { X = 50, Y = 50, Width = 50, Height = 50, Stroke = Brushes.Red, Data = "M0,25L25,50L50,25L25,0Z", },
                new EllipseItem { X = 200, Y = 50, Width = 50, Height = 100, Fill = Brushes.Green, },
                new ImageItem { X = 50, Y = 200, Width = 128, Height = 128, Source = new BitmapImage(new Uri("D:\\ブログ用\\テスト用画像\\collection2.png")), },
                new RectangleItem { X = 200, Y = 200, Width = 100, Height = 50, Fill = Brushes.Blue, },
                new RichTextBoxItem { X = 400, Y = 50, Width = 300, Height = 300, Text = "RichTextBoxItem" },
            };
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Items[0].X += 10.0;//移動
            //移動はできるけどマウスドラッグ移動はできない？
        }
    }
    public class Item : INotifyPropertyChanged
    {
        //public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        #region 通知プロパティ
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _x;
        public double X { get => _x; set => SetProperty(ref _x, value); }
        #endregion 通知プロパティ

    }

    public class PathItem : Item
    {
        public Brush Stroke { get; set; }
        public string Data { get; set; }
    }

    public class EllipseItem : Item
    {
        public Brush Fill { get; set; }
    }

    public class ImageItem : Item
    {
        public ImageSource Source { get; set; }
    }

    public class RectangleItem : Item
    {
        public Brush Fill { get; set; }
    }

    public class RichTextBoxItem : Item
    {
        public string Text { get; set; }
    }
}
