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
        public MainWindow()
        {
            InitializeComponent();


            Data3 data1 = new(DataType.Layer);
            TThumb3 layer = new(data1) { Name = "MyLayer" };
            MyCanvas.Children.Add(layer);
            layer.AddItem(MakeTT3TextBlock("AddItem1", 0, 0));
            layer.AddItem(MakeTT3TextBlock("AddItem2", 100, 100));


            //MyCanvas.Children.Add(MakeTT3TextBlock("Item1", 0, 0));
            //MyCanvas.Children.Add(MakeTT3TextBlock("Item2", 100, 100));
            //MyCanvas.Children.Add(MakeTT3TextBlock("Item3", 50, 50));

            //#region T1
            //TThumb1 thumb1 = new TThumb1();
            //thumb1.Text = "serialTest";
            //thumb1.X = 100;
            //thumb1.Y = 100;
            //thumb1.Geometry = new RectangleGeometry(new Rect(0, 20, 10, 40));
            //thumb1.DataSave();
            //#endregion T1
            //T2Layer t2l = new();
            //t2l.SetEditing();

        }
        private TThumb3 MakeTT3TextBlock(string text, double x, double y)
        {
            Data3 data = new(DataType.TextBlock) { Text = text, X = x, Y = y };
            TThumb3 thumb = new(data);
            thumb.PreviewMouseDown += (sender, e) =>
            {
                this.ActiveThumb = thumb;
                MyStackPanel.DataContext = thumb.MyData;
            };
            return thumb;
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {

        }
    }



}
