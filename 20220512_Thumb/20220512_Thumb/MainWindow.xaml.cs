﻿using System;
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

namespace _20220512_Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TThumb3? ActiveThumb;
        private TThumb3 MyLayer;
        private TThumb3 MyGroup1;
        private TThumb3 MyGroup2;
        public MainWindow()
        {
            InitializeComponent();
            MyLayer = new(new(DataType.Layer)) { Name = "MyLayer" };
            MyCanvas.Children.Add(MyLayer);
            MyLayer.IsEditing = true;//編集状態にする(追加されたアイテムはドラッグ移動可能)
            MyStackPanel.DataContext = MyLayer;

            //アイテムを追加
            MyLayer.AddItem(MakeTT3TextBlock("Item1", 0, 0));
            MyLayer.AddItem(MakeTT3TextBlock("Item2", 100, 100));

            //グループを追加
            MyGroup1 = new(new Data3(DataType.Group) { X = 50, Y = 50 }) { Name = "MyGroup1" };
            //MyGroup1 = new(new Data3(DataType.Group)) { Name = "MyGroup1" };
            MyLayer.AddItem(MyGroup1);
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
            //Group2
            // Group2_1
            //  Item2-1-1
            //  Item2-1-2
            // Group2_2
            //  Item2-2-1
            //  Item2-2-2
            //グループを含んだグループを追加
            MyGroup2 = new(new Data3(DataType.Group) { X = 100, Y = 100 }) { Name = "MyGroup2" };
            TThumb3 group21 = new(new Data3(DataType.Group) { X = 0, Y = 0 }) { Name = "MyGroup2_1" };
            TThumb3 group22 = new(new Data3(DataType.Group) { X = 100, Y = 100 }) { Name = "MyGroup2_2" };
            group21.AddItem(MakeTT3TextBlock("Group2-1-Item1", 0, 0));
            group21.AddItem(MakeTT3TextBlock("Group2-1-Item2", 30, 30));
            group22.AddItem(MakeTT3TextBlock("Group2-2-Item1", 0, 0));
            group22.AddItem(MakeTT3TextBlock("Group2-2-Item2", 30, 30));
            MyGroup2.AddItem(group21);
            MyGroup2.AddItem(group22);
            MyLayer.AddItem(MyGroup2);
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyGroup1;
            var gw = MyGroup1.Width;
            var left = Canvas.GetLeft(MyGroup1);
        }
    }



}