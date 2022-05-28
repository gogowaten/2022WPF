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

        //private int AddThumbCount = 0;
        //private List<TThumb3> MyThumbs = new();
        //private TThumb3? ActiveThumb;
        //private TThumb3 MyLayer;
        //private TThumb3 MyGroup1;
        //private TThumb3 MyGroup2;

        private int AddThumbCount = 0;
        private List<TThumb4> MyThumbs = new();
        private TThumb4? ActiveThumb;
        private Layer4? MyLayer;
        private Group4? MyGroup1;
        private Group4? MyGroup2; WakuTest myWaku;
        public MainWindow()
        {
            InitializeComponent();
            Left = 100;
            Top = 100;
            T4Initialize();

            //myWaku=new() { Width = 100, Height = 100 };
            //MyCanvas.Children.Add(myWaku);

            Rectangle rectangle = new() { Width = 100, Height = 100 ,Stroke=Brushes.Firebrick};
            rectangle.StrokeDashArray = new DoubleCollection() { 5.0, 5.0 };
            rectangle.StrokeDashOffset = 3;
            MyCanvas.Children.Add(rectangle);

        }
        private void T4Initialize()
        {
            MyLayer = new(new(DataType.Layer)) { Name = "MyLayer" };
            MyCanvas.Children.Add(MyLayer);
            MyLayer.IsMyEditing = true;//編集状態にする(追加されたアイテムはドラッグ移動可能)
            MyStackPanel.DataContext = MyLayer;
            MyGroupBoxLayer.DataContext = MyLayer;

            MyLayer.AddThumb(MakeTT4TextBlock("Item1", 0, 0));
            MyLayer.AddThumb(MakeTT4TextBlock("Item2", 80, 50));

            MyGroup1 = new(new Data4(DataType.Group) { X = 40, Y = 140 }) { Name = "MyGroup1" };
            //MyGroup1 = new(new Data3(DataType.Group)) { Name = "MyGroup1" };
            MyLayer.AddThumb(MyGroup1);
            MyGroup1.AddThumb(MakeTT4TextBlock("Item1_1", 0, 0));
            MyGroup1.AddThumb(MakeTT4TextBlock("Item1_2", 100, 50));
            MyGroupBoxGroup.DataContext = MyGroup1.MyData;

            T4AddGroupGroup();

        }
        private Item4 MakeTT4TextBlock(string text, double x, double y)
        {
            Data4 data = new(DataType.TextBlock) { Text = text, X = x, Y = y };
            data.Background = new SolidColorBrush(
                Color.FromArgb(
                    55,
                    50,
                    (byte)(AddThumbCount * 20 + 50),
                    (byte)(AddThumbCount * 20 + 50)));
            data.Foreground = Brushes.Snow;
            Item4 thumb = new(data);
            thumb.PreviewMouseDown += (sender, e) =>
            {
                this.ActiveThumb = thumb;
                MyGroupBox.DataContext = thumb;
                MyGroupBoxGroup.DataContext = thumb.MyParentGroup;
            };
            AddThumbCount++;
            return thumb;
        }
        private void T4AddGroupGroup()
        {
            MyGroup2 = new(new Data4(DataType.Group) { X = 120, Y = 10 }) { Name = "MyGroup2" };
            Group4 group21 = new(new Data4(DataType.Group) { X = 0, Y = 0 }) { Name = "MyGroup2_1" };
            Group4 group22 = new(new Data4(DataType.Group) { X = 100, Y = 100 }) { Name = "MyGroup2_2" };
            group21.AddThumb(MakeTT4TextBlock("Item2-1-1", 0, 0));
            group21.AddThumb(MakeTT4TextBlock("Item2-1-2", 30, 30));
            group22.AddThumb(MakeTT4TextBlock("Item2-2-1", 0, 0));
            group22.AddThumb(MakeTT4TextBlock("Item2-2-2", 30, 30));
            MyGroup2.AddThumb(group21);
            MyGroup2.AddThumb(group22);
            MyLayer.AddThumb(MyGroup2);
        }
        #region TThumb3用
        private TThumb3 MakeTT3TextBlock(string text, double x, double y)
        {
            Data3 data = new(DataType.TextBlock) { Text = text, X = x, Y = y };
            TThumb3 thumb = new(data);
            thumb.PreviewMouseDown += (sender, e) =>
            {
                MyGroupBox.DataContext = thumb;
                //MyGroupBox.DataContext = thumb.MyData;
            };
            return thumb;
        }
        private void T3Initialize()
        {
            MyLayer = new(new(DataType.Layer)) { Name = "MyLayer" };
            MyCanvas.Children.Add(MyLayer);
            MyLayer.IsMyEditing = true;//編集状態にする(追加されたアイテムはドラッグ移動可能)
            MyStackPanel.DataContext = MyLayer;


            //MyLayer.AddItem(MakeTT3TextBlock("Item1", 0, 0));
            //MyLayer.AddItem(MakeTT3TextBlock("Item2", 100, 100));

            //MyGroup1 = new(new Data3(DataType.Group) { X = 50, Y = 50 }) { Name = "MyGroup1" };
            ////MyGroup1 = new(new Data3(DataType.Group)) { Name = "MyGroup1" };
            //MyLayer.AddItem(MyGroup1);
            //MyGroup1.AddItem(MakeTT3TextBlock("Group1_Item1", 0, 0));
            //MyGroup1.AddItem(MakeTT3TextBlock("Group1_Item2", 200, 150));
            //MyGroupBoxGroup.DataContext = MyGroup1.MyData;

        }
        private void T3AddGroupGroup()
        {
            //MyGroup2 = new(new Data3(DataType.Group) { X = 100, Y = 100 }) { Name = "MyGroup2" };
            //TThumb3 group21 = new(new Data3(DataType.Group) { X = 0, Y = 0 }) { Name = "MyGroup2_1" };
            //TThumb3 group22 = new(new Data3(DataType.Group) { X = 100, Y = 100 }) { Name = "MyGroup2_2" };
            //group21.AddItem(MakeTT3TextBlock("Group2-1-Item1", 0, 0));
            //group21.AddItem(MakeTT3TextBlock("Group2-1-Item2", 30, 30));
            //group22.AddItem(MakeTT3TextBlock("Group2-2-Item1", 0, 0));
            //group22.AddItem(MakeTT3TextBlock("Group2-2-Item2", 30, 30));
            //MyGroup2.AddItem(group21);
            //MyGroup2.AddItem(group22);
            //MyLayer.AddItem(MyGroup2);
        }
        private void T3AddItem()
        {
            //var thumb = MakeTT3TextBlock($"AddItem{AddThumbCount}", AddThumbCount * 10, AddThumbCount * 20);
            //MyLayer.AddItem(thumb);
            //MyThumbs.Add(thumb);
            //AddThumbCount++;
        }
        private void T3ADdGroup()
        {
            //List<TThumb3> thumbs = new();
            //thumbs.Add(MyThumbs[0]);
            //thumbs.Add(MyThumbs[1]);
            //MyLayer.MakeGroup(thumbs);

        }
        private void T3AddGroup()
        {
            //MyGroup1 = new(new Data3(DataType.Group) { X = 50, Y = 50 }) { Name = "MyGroup1" };
            ////MyGroup1 = new(new Data3(DataType.Group)) { Name = "MyGroup1" };
            //MyLayer.AddItem(MyGroup1);
            //MyGroup1.AddItem(MakeTT3TextBlock("Group1_Item1", 0, 0));
            //MyGroup1.AddItem(MakeTT3TextBlock("Group1_Item2", 200, 150));
            //MyGroupBoxGroup.DataContext = MyGroup1.MyData;
        }
        private void T3GroupKaijo()
        {
            //if (ActiveThumb?.MyParentGroup is TThumb3 group)
            //{
            //    if (group.MyParentGroup is TThumb3 parent)
            //    {
            //        parent.Groupkaijo(group);
            //    }
            //}
        }

        #endregion TThumb3用




        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            //myWaku.Waku1 = true;
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            List<string> thumbsData = new();
            GetDataList(MyLayer);
            void GetDataList(Group4Base group)
            {
                if (group == null) { return; }
                foreach (var item in group.Items)
                {
                    thumbsData.Add($"{item.MyData.Z}, {item.MyData.Text}, {item.Name}");
                    if (item.MyData.DataTypeMain is DataTypeMain.Group) GetDataList((Group4Base)item);
                }
            }
        }

        private void ButtonAdd2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddGroup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            ActiveThumb?.MyParentGroup?.RemoveThumb(ActiveThumb);
        }

        private void ButtonAddG1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonKaijo_Click(object sender, RoutedEventArgs e)
        {
            //ActiveThumb?.MyParentGroup?.Ungroup();
            ActiveThumb?.GetMyMoveTargetThumb()?.Ungroup2();
            //ActiveThumb?.GetMyUnderEditingThumb()?.Ungroup();
        }

        private void ButtonCheck1_Click(object sender, RoutedEventArgs e)
        {

            if (MyLayer.NowEditingThumb == MyLayer)
            {
                MyLayer.NowEditingThumb = MyGroup1;
            }
            else if (MyLayer.NowEditingThumb == MyGroup1)
            {
                MyLayer.NowEditingThumb = MyGroup2;
            }
            else
            {
                MyLayer.NowEditingThumb = MyLayer;
            }
            //MyGroup1.IsEditing = !MyGroup1.IsEditing;
            if (MyGroup2 == null) return;
            //MyLayer.NowEditingThumb = MyGroup2;
        }

        private void ButtonMakeGroup_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyLayer?.SelectedThumbs.ToList();
            MyLayer?.MakeGroupFromChildren2(neko);
            //MyLayer?.MakeGroupFromChildren(neko);
        }

        private void ButtonReGroup_Click(object sender, RoutedEventArgs e)
        {
            //ActiveThumb?.MyParentGroup?.Regroup(ActiveThumb?.RegroupThumbs);
            ActiveThumb?.Regroup();
        }
    }



}
