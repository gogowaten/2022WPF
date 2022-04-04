using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace _20220404_Zindex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Data MyData { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<Data> Datas { get; set; } = new();
        public MainWindow()
        {
            InitializeComponent();

            //MyData = new Data { Top = 60, ZIndex = 1 };
            //R4.DataContext = MyData;

            Datas.Add(new Data() { Top = 60, ZIndex = 1 });
            Datas.Add(new Data() { Top = 80, ZIndex = 2 });
            R4.DataContext = Datas[1];
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var d = (Data)R4.DataContext;
            d.Top = 20;
            d.ZIndex = -20;
            
        }
    }
    public class Data : System.ComponentModel.INotifyPropertyChanged
    {
        private double top;
        private int zIndex;

        public double Top
        {
            get => top;

            set
            {
                if (value == top) { return; }
                top = value;
                RaisePropertyChanged();
            }
        }
        public int ZIndex
        {
            get => zIndex; set
            {
                if (value == zIndex) { return; }
                zIndex = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
