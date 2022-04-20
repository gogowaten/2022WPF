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

namespace _20220420_単一複数両用Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Data2 data1 = new(ThumbType.TextBlock, 50, 10, "data1111111");
            Data2 data2 = new(ThumbType.Path, 0, 100, new EllipseGeometry(new Point(), 30, 30), Brushes.MediumAquamarine);

            MyLayer.MySetContent(data1);
            MyLayer.MyAddChildren(data2);

        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            Data2 data3 = new(ThumbType.TextBlock, 0, 50, "data22222");
            Data2 data4 = new(ThumbType.Path, 100, 100, new EllipseGeometry(new Point(), 30, 30), Brushes.Red);
            Data2 groupData = new(ThumbType.Group, new ObservableCollection<Data2>() { data3, data4 }, 0, 0);
            MyLayer.MyAddChildren(groupData);

        }
    }
}
