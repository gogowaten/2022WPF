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
using System.Xml;

namespace _20220620
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MyCoutnt = 0;
        public MainWindow()
        {
            InitializeComponent();
            Top = 10; Left = 10;

        }

        private void MyButtonAddText_Click(object sender, RoutedEventArgs e)
        {//追加
            Data1 data1 = new(DataType.TextBlock);
            data1.Text = $"{MyTextBoxText.Text}_{MyCoutnt}";
            MyCoutnt++;

            if (MyMainItemsControl.MyActiveMovableThumb is TThumb1 tt)
            {
                data1.X = tt.MyData.X + 32;
                data1.Y = tt.MyData.Y + 32;
            }
            MyMainItemsControl.AddItem(data1);

        }

        private void MyButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            //MyMainItemsControl.MyEditingGroup?.RemoveThumb2(MyMainItemsControl.MyActiveMovableThumb);
            //MyMainItemsControl.RemoveActiveThumb();
            MyMainItemsControl.RemoveSelectedThumbs();
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            var sele = MyMainItemsControl.MySelectedThumbs;
        }

        private void MyButtonGroup_Click(object sender, RoutedEventArgs e)
        {//gurup
            MyMainItemsControl.MakeGroup();
        }

        private void MyButtonEditingNarrow_Click(object sender, RoutedEventArgs e)
        {//編集範囲を狭くする
            MyMainItemsControl.EditingNarrow(MyMainItemsControl.MyCurrentItem);
        }

        private void MyButtonEditingReset_Click(object sender, RoutedEventArgs e)
        {//編集範囲をLayerにする
            MyMainItemsControl.EditingSetMyCurrentLayer();
        }

        private void MyButtonUngroup_Click(object sender, RoutedEventArgs e)
        {//Activeなグループを解除
            MyMainItemsControl.Ungroup();
        }

        private void MyButtonRegroup_Click(object sender, RoutedEventArgs e)
        {//再グループ化
            MyMainItemsControl.Regroup(MyMainItemsControl.MyCurrentItem);
        }

        private void MyButtonUp_Click(object sender, RoutedEventArgs e)
        {//Z UP
            MyMainItemsControl.ZAgeActiveThumb();
        }

        private void MyButtonDown_Click(object sender, RoutedEventArgs e)
        {//Z DOWN
            MyMainItemsControl.ZSageActiveThumb();
        }

        private void MyButtonSave_Click(object sender, RoutedEventArgs e)
        {//全体セーブ、MainItemsControlを保存する
            MyMainItemsControl.DataSaveCurrentLayer($"E:\\MyData.xml");
        }

        private void MyButtonLoad_Click(object sender, RoutedEventArgs e)
        {//全体ロード、MainItemsControlを入れ替える
            string fileName = $"E:\\MyData.xml";

            if (DataLoad(fileName) is Data1 data)
            {
                MyGrid.Children.Remove(MyMainItemsControl);
                MyMainItemsControl = new(data);
                MyGrid.Children.Add(MyMainItemsControl);
            }
        }
        private Data1? DataLoad(string fileName)
        {
            DataContractSerializer serializer = new(typeof(Data1));
            try
            {
                using XmlReader reader = XmlReader.Create(fileName); ;
                return (Data1?)serializer.ReadObject(reader);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
