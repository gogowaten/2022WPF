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

namespace _20221231_SelectedThumbs
{
    public partial class MainWindow : Window
    {
        private int MyAddCounnt = 1;
        public MainWindow()
        {
            InitializeComponent();
            Left = 100; Top = 100;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            TTTextBlock tt = new()
            {
                MyLeft = 20 * MyAddCounnt,
                MyTop = 20 * MyAddCounnt,
                MyText = TextBoxAdd.Text + MyAddCounnt,
                Name = TextBoxAdd.Text + MyAddCounnt,
            };
            MyRootThumb.AddThumb(tt);
            MyAddCounnt++;
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.RemoveThumb();
            //↓これを禁止したいけど方法がわからん
            //MyRootThumb.InternalChildren.Remove(MyRootThumb.ActiveThumb);
        }

        private void ButtonRootActive_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ActiveGroup = MyRootThumb;
        }

        private void ButtonTTG1Active_Click(object sender, RoutedEventArgs e)
        {
            //MyRootThumb.ActiveGroup = TTG_1;
        }

        private void ButtonTTG2Active_Click(object sender, RoutedEventArgs e)
        {
            //MyRootThumb.ActiveGroup = TTG_2;
        }

        private void ButtonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.AddGroup();
        }

        private void ButtonUnGroup_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.UnGroup();
        }

        private void ButtonActiveInside_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ActiveGroupInside();
        }

        private void ButtonActiveOutside_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ActiveGroupOutside();
        }

    }
}
