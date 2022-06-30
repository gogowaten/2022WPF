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
using System.ComponentModel;

namespace _20220630_通知プロパティ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    //今まで普通に使ってきた書き方
    public class AAA : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int _x;
        public int X { get => _x; set { if (_x == value) return; _x = value; OnPropertyChanged(); } }

        private string? _name;
        public string? Name { get => _name; set { if (_name == value) return; _name = value; OnPropertyChanged(); } }

    }

    //【WPF】Binding入門1。DataContextの伝搬 | さんさめのC＃ブログ
    //https://threeshark3.com/wpf-binding-datacontext/
    public class BBB : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if(EqualityComparer<T>.Default.Equals(field, value)) return;
            //if (Equals(field, value)) return;//これだと値型の場合にメモリを消費してしまう？
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int _x;
        public int X { get => _x; set => OnPropertyChanged(ref _x, value); }

        private string? _name;
        public string? Name { get => _name; set => OnPropertyChanged(ref _name, value); }

    }
}
