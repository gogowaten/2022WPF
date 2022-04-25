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

namespace _20220425_Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test1_AddItem();
            Test2AddGroup();
            Test3();

        }
        private void Test1_AddItem()
        {
            DataTextBlock dataTextBlock = new("test1", 10, 20);
            MyLayer.AddData(dataTextBlock);
            DataTextBlock dataTextBlock1 = new("test1-1", 10, 40);
            ItemTThumb item = new(dataTextBlock1);
            MyLayer.AddItem(item);
        }
        private void Test2AddGroup()
        {
            List<Data> datas = new() { new DataTextBlock("test2-1", 100, 20), new DataTextBlock("test2-2", 100, 40) };
            DataGroup dataGroup = new(datas, 0, 0);
            GroupTThumb groupTThumb = new(dataGroup);
            MyLayer.AddItem(groupTThumb);
        }
        private void Test3()
        {
            List<Data> datas1 = new() { new DataTextBlock("test3-1", 100, 20), new DataTextBlock("test3-2", 100, 40) };
            List<Data> datas2 = new() { new DataTextBlock("test3-3", 100, 20), new DataTextBlock("test3-4", 100, 40) };
            List<Data> datas3 = new() { new DataTextBlock("test3-5", 100, 20), new DataTextBlock("test3-6", 100, 40) };
            DataGroup dataGroup1 = new(datas1, 100, 20);
            DataGroup dataGroup2 = new(datas2, 100, 40);
            dataGroup1.AddData(dataGroup2);
            //DataGroup dataGroup3 = new(datas3, 0, 60);
            //dataGroup2.ChildrenData.Add(dataGroup3);
            GroupTThumb groupTThumb = new(dataGroup1);
            MyLayer.AddItem(groupTThumb);
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var children = MyLayer.MyChildrenItems;
            var chilchil = MyLayer.MyChildrenItems[0];
            var data = MyLayer.MyData;
        }
    }
}
