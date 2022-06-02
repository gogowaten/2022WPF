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


namespace _20220530
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MyItemsCount = 0;
        private Item4? MyActiveItemThumb;
        public Layer1? MyLayer1;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            Left = 10; Top = 10;
#endif
            MyLayer1 = new Layer1();
            DataContext = MyLayer1;
            MyCanvas.Children.Add(MyLayer1);
            MyLayer1.AddThumb(MakeItem4(MakeTextBloclData1(0, 0, "Item1", 4)));
            MyLayer1.AddThumb(MakeItem4(MakeTextBloclData1(50, 50, "Item2", 4)));

            //Group4 group = new();
            //MyLayer1.AddThumb(group);
            //group.MyData.X = 20; group.MyData.Y = 100;
            //group.AddThumb(MakeItem4(MakeTextBloclData1(0, 0, "Item3", 4)));
            //group.AddThumb(MakeItem4(MakeTextBloclData1(50, 20, "Item4", 4)));
        }

        private void Item1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Item4 item) { MyActiveItemThumb = item; }
            //MyGroupBox1.DataContext = MyLayer1.LastClickedItem;
            //if (sender is Item4 item)
            //{
            //    MyGroupBox1.DataContext = item;
            //}
        }
        private Item4 MakeItem4(Data1 data)
        {
            Item4 item = new(data);
            item.PreviewMouseDown += Item1_PreviewMouseDown;
            return item;
        }
        private Data1 MakeTextBloclData1(double x, double y, string text, double padding)
        {
            Data1 data = new(DataType.TextBlock) { X = x, Y = y, Text = text, Padding = padding };
            data.Background = new SolidColorBrush(
                Color.FromArgb(
                    255,
                    (byte)((MyItemsCount * 20)),
                    (byte)((255 - MyItemsCount * 20)),
                    (byte)(255)
                    ));
            MyItemsCount++;
            return data;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Item4 item = new Item4(
                MakeTextBloclData1(MyItemsCount * 10 + 100, MyItemsCount * 10, MyTextBox.Text + MyItemsCount, 4));
            item.PreviewMouseDown += Item1_PreviewMouseDown;
            MyLayer1?.AddThumb(item);
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (MyLayer1?.SelectedThumbs is not null)
            {
                //foreach (var item in MyLayer1.SelectedThumbs)
                //{
                //    item.MyParentGroup?.RemoveThumb(item);
                //}
                for (int i = MyLayer1.SelectedThumbs.Count - 1; i >= 0; i--)
                {
                    MyLayer1.SelectedThumbs[i].MyParentGroup?.RemoveThumb(MyLayer1.SelectedThumbs[i]);
                }
            }

        }

        private void MyButtonCheck1_Click(object sender, RoutedEventArgs e)
        {
            var layer = MyLayer1;
            var items = MyLayer1?.Items;
            var item0data = items?[0].MyData;
            var item1data = items?[1].MyData;
            var item00data = MyLayer1?.MyData.ChildrenData[1];
            var tost = MyLayer1?.ToString();
        }

        private void ButtonZUp_Click(object sender, RoutedEventArgs e)
        {
            //最後にクリックされたThumbのZを1上げる
            //if (MyLayer1.LastClickedItem is TThumb1 thumb)
            //{
            //    thumb.GetMyActiveMoveThumb()?.SetZIndex(thumb.MyData.Z + 1);
            //}

            //ActiveMoveThumbのZを1上げる
            if (MyActiveItemThumb?.MyActiveMovableThumb is TThumb1 thumb)
            {
                thumb.SetZIndex(thumb.MyData.Z + 1);
            }
        }

        private void ButtonZDown_Click(object sender, RoutedEventArgs e)
        {
            //ActiveMoveThumbのZを1下げる
            if (MyActiveItemThumb?.MyActiveMovableThumb is TThumb1 thumb)
            {
                thumb.SetZIndex(thumb.MyData.Z - 1);
            }
        }

        private void ButtonGroup_Click(object sender, RoutedEventArgs e)
        {
            //グループ化
            //選択されているThumbをグループ化
            //最後にクリックしたThumbの親グループに新グループ追加
            //MyActiveItemThumb?.MyParentGroup?.MakeGroupFromChildren2(MyLayer1.SelectedThumbs);

            //選択Thumbの親グループに新グループ追加
            if (MyLayer1?.SelectedThumbs[0]?.MyParentGroup is Group1Base group)
            {
                group.MakeGroupFromChildren3(MyLayer1.SelectedThumbs);
            }
        }

        private void ButtonUngroup_Click(object sender, RoutedEventArgs e)
        {
            //グループ解除
            //クリックしたThumbの親グループ解除
            //MyActiveItemThumb?.MyParentGroup?.Ungroup2();

            //クリックしたThumbに関連した移動可能グループThumbを解除
            
            MyActiveItemThumb?.GetMyActiveMoveThumb()?.Ungroup2();
        }

        private void ButtonRegroup_Click(object sender, RoutedEventArgs e)
        {
            //再グループ化
            MyActiveItemThumb?.Regroup();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            //状態をファイに保存
            if(MyLayer1?.MyData is Data1 data)
            {
                DataSave($"E:\\MyData.xml", data);
            }
            
        }
        private void DataSave(string fileName, Data1 data)
        {
            XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            XmlWriter writer;
            DataContractSerializer serializer = new(typeof(Data1));
            using (writer = XmlWriter.Create(fileName, settings))
            {
                try
                {
                    serializer.WriteObject(writer, data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //ファイルから読み込み
        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            //古い方を削除
            MyCanvas.Children.Remove(MyLayer1);
            //MyLayer1 = null;

            //ファイルから読み込み
            Data1? data = DataLoad($"E:\\MyData.xml");
            if(data == null) { return; }
            Layer1? layer = new(data);
            MyLayer1 = layer;            
            MyCanvas.Children.Add(layer);

            //全てのItemThumbにマウスダウンイベント付加
            AddMouseDownEvent(layer);

            //再Binding
            DataContext = MyLayer1;
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
        //グループをファイルに保存
        private void ButtonSaveGroup_Click(object sender, RoutedEventArgs e)
        {
            if(MyActiveItemThumb?.MyActiveMovableThumb is Group4 group)
            {
                DataSave($"E:\\MyDataGroup.xml",group.MyData);
            }
        }
        //ファイルからグループを追加
        private void ButtonAddGroupFromFile_Click(object sender, RoutedEventArgs e)
        {
            Data1? data = DataLoad($"E:\\MyDataGroup.xml");
            if (data == null) { return; }
            Group4 group = new(data);
            MyLayer1?.AddThumb(group);

            //全てのItemThumbにマウスダウンイベント付加
            AddMouseDownEvent(group);
        }
        private void AddMouseDownEvent(Group1Base thumb)
        {
            foreach (var item in thumb.Items)
            {
                if(item is Group1Base group)
                {
                    AddMouseDownEvent(group);
                }
                else
                {
                    item.PreviewMouseDown += Item1_PreviewMouseDown;
                }
            }
        }
    }
}
