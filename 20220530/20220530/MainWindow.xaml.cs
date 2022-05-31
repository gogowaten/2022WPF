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

namespace _20220530
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MyItemsCount = 0;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            Left = 10; Top = 10;
#endif
            Item4 item1 = new(MakeTextBloclData1(0, 0, "Item1", 4));
            MyLayer1.AddThumb(item1);
            item1 = new(MakeTextBloclData1(50, 20, "Item2", 4));
            MyLayer1.AddThumb(item1);

            Group4 group = new();
            MyLayer1.AddThumb(group);
            group.MyData.X = 20; group.MyData.Y = 100;
            item1 = new(MakeTextBloclData1(0, 0, "Item3", 4));
            group.AddThumb(item1);
            item1 = new(MakeTextBloclData1(50, 20, "Item4", 4));
            group.AddThumb(item1);
        }
        private Data1 MakeTextBloclData1(double x, double y, string text, double padding)
        {
            Data1 data = new(DataType.TextBlock) { X = x, Y = y, Text = text, Padding = padding };
            data.Background = new SolidColorBrush(
                Color.FromArgb(
                    255,
                    100,
                    (byte)(MyItemsCount * 20 + 100),
                    (byte)(MyItemsCount * 20 + 150)));
            MyItemsCount++;
            return data;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            MyLayer1.AddThumb(new Item4(MakeTextBloclData1(MyItemsCount * 10, MyItemsCount * 10, MyTextBox.Text, 4)));
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if(MyLayer1.SelectedThumbs is not null)
            {
                foreach (var item in MyLayer1.SelectedThumbs)
                {
                    item.MyParentGroup?.RemoveThumb(item);
                }
            }
            
        }
    }
}
