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

namespace _20220508
{
    public partial class MainWindow : Window
    {
        private TThumb3? ActiveThumb;
        private TThumb3 MyGroup1;
        public MainWindow()
        {
            InitializeComponent();


            TThumb3 layer = new(new(DataType.Layer)) { Name = "MyLayer" };
            MyCanvas.Children.Add(layer);
            layer.IsEditing = true;//編集状態にする(追加されたアイテムはドラッグ移動可能)
            MyStackPanel.DataContext = layer;


            layer.AddItem(MakeTT3TextBlock("Item1", 0, 0));
            layer.AddItem(MakeTT3TextBlock("Item2", 100, 100));

            MyGroup1 = new(new Data3(DataType.Group) { X = 50, Y = 50 }) { Name = "MyGroup1" };
            //MyGroup1 = new(new Data3(DataType.Group)) { Name = "MyGroup1" };
            layer.AddItem(MyGroup1);
            MyGroup1.AddItem(MakeTT3TextBlock("Group1_Item1", 0, 0));
            MyGroup1.AddItem(MakeTT3TextBlock("Group1_Item2", 200, 150));
            MyGroupBoxGroup.DataContext = MyGroup1.MyData;



        }
        private TThumb3 MakeTT3TextBlock(string text, double x, double y)
        {
            Data3 data = new(DataType.TextBlock) { Text = text, X = x, Y = y };
            TThumb3 thumb = new(data);
            thumb.PreviewMouseDown += (sender, e) =>
            {
                this.ActiveThumb = thumb;
                MyGroupBox.DataContext = thumb.MyData;
            };
            return thumb;
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyGroup1;
            var gw = MyGroup1.Width;
            var left = Canvas.GetLeft(MyGroup1);
        }
    }



}
