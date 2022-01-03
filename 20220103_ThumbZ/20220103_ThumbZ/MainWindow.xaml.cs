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

namespace _20220103_ThumbZ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReThumb MyReThumb1;
        ReThumb MyReThumb2;
        ReThumb MyGroupReThumb;
        private ReThumb focusThumb;

        public ReThumb FocusThumb
        {
            get => focusThumb; set
            {
                focusThumb = value;
                //MyStackPanel.DataContext = value;
                //FocusのRootをBinding
                MyStackPanel.DataContext = value.RootReThumb;
            }
        }
        public MainWindow()
        {
            InitializeComponent();


            //Test1();
            //Test2();
            //Test3();
            //MyLayer1.AddChildren(Test4());//グループ化
            //MyLayer1.AddChildren(Test5());//グループに追加
            //Test6();
            //Test7();
            //Test8();//
            //FocusThumbをこっちに用意しておいて、Newのときに渡すかPublicにしておいて、向こうでGotFocusイベントでdatacontextに指定するようにする？
            //Test9();//GotFocus、やっぱりやめた
            Test10();
        }
        //Zオーダー
        private void Test10()
        {
            MyReThumb1 = new(MakeTextBlock($"{nameof(MyReThumb1)}"), $"{nameof(MyReThumb1)}");
            MyReThumb1.GotFocus += MyReThumb_GotFocus;
            MyLayer1.AddChildren(MyReThumb1);
            Enumerable.Range(0, 3).Select(a => new ReThumb(MakeTextBlock($"{nameof(Test10)}-{a}"), $"{nameof(Test10)}-{a}"));
            for (int i = 0; i < 3; i++)
            {
                ReThumb re = new ReThumb(MakeTextBlock($"{nameof(Test10)}-{i}"), $"{nameof(Test10)}-{i}");
                re.GotFocus += MyReThumb_GotFocus;
                MyLayer1.AddChildren(re);
            }
        }
        //FocusThumbを取得するためにMainWindowを渡すようにした、向こうのGotFocusイベント時にこっちのFocusThumbを入れ替えるようにしたけど
        //不自然かも
        private void Test9()
        {
            //for (int i = 0; i < 3; i++)
            //{   
            //    MyLayer1.AddChildren(new ReThumb(MakeTextBlock($"bind{i}"), this));
            //    //MyLayer1.AddChildren(new ReThumb(MakeTextBlock($"bind{i}"), this,null, i * 10, i * 50));
            //}
        }
        //グループ解除
        private void Test8()
        {
            ReThumb aa = new(Enumerable.Range(0, 2).
                Select(a => new ReThumb(MakeTextBlock($"要素A-{a}"), $"要素A-{a}", a * 20 + 10, a * 50 + 10)), "グループA");
            aa.Children.ToList().ForEach(a => a.GotFocus += MyReThumb_GotFocus);
            aa.GotFocus += MyReThumb_GotFocus;

            ReThumb bb = new(Enumerable.Range(0, 2).
                Select(a => new ReThumb(MakeTextBlock($"要素B-{a}"), $"要素B-{a}", a * 20 + 210, a * 50 + 10)), "グループB");
            bb.Children.ToList().ForEach(a => a.GotFocus += MyReThumb_GotFocus);
            bb.GotFocus += MyReThumb_GotFocus;

            List<ReThumb> list = new() { aa, bb };
            var cc = new ReThumb(list, "グループC");
            cc.GotFocus += MyReThumb_GotFocus;
            MyLayer1.AddChildren(cc);

        }


        private void Test7()
        {
            List<ReThumb> list = new();
            for (int i = 0; i < 3; i++)
            {
                ReThumb re = new ReThumb(MakeTextBlock($"{nameof(Test7)}"), $"解除Test{i}", i * 20 + 20, i * 50 + 30);
                re.GotFocus += MyReThumb_GotFocus;
                list.Add(re);
            }
            ReThumb group = new(list);
            group.GotFocus += MyReThumb_GotFocus;
            MyLayer1.AddChildren(group);
            //var gg = group.UnGroup();
        }

        //グループAとグループBからグループC作成
        private void Test6()
        {
            var listA = Enumerable.Range(0, 3).
                Select(a => new ReThumb(MakeTextBlock($"GroupA-{a}"), a * 20 + 10, a * 50 + 10)).ToList();
            listA.ForEach(a => a.GotFocus += MyReThumb_GotFocus);
            ReThumb groupA = new(listA, "グループA");

            var listB = Enumerable.Range(0, 3).
                Select(a => new ReThumb(MakeTextBlock($"GroupB-{a}"), a * 20 + 200, a * 50 + 20)).ToList();

            listB.ForEach(a => a.GotFocus += MyReThumb_GotFocus);
            ReThumb groupB = new(listB, "グループB");

            List<ReThumb> listC = new() { groupA, groupB };
            ReThumb groupC = new(listC, $"グループC");

            MyLayer1.AddChildren(groupC);
        }


        //既存グループに1要素を追加
        private ReThumb Test5()
        {
            //Group作成
            ReThumb group = new(Enumerable.Range(0, 3).
                Select(a => new ReThumb(MakeTextBlock($"Test5-{a}"), $"Test5-{a}", a * 20 + 200, a * 50 + 30)).
                ToList(), "テストグループ");
            group.GotFocus += MyReThumb_GotFocus;
            MyLayer1.AddChildren(group);//レイヤーに追加、しなくてもいいけど実際に使うときを再現するため

            //別のThumb
            ReThumb reThumb = new(MakeTextBlock("追加要素"), $"追加要素", 10, 20);
            MyLayer1.AddChildren(reThumb);//これもレイヤーに追加
            reThumb.GotFocus += MyReThumb_GotFocus;

            //Group用Thumb新規作成
            List<ReThumb> list = new() { group, reThumb };
            ReThumb gg = new(list, nameof(Test5));
            gg.GotFocus += MyReThumb_GotFocus;
            return gg;
        }
        //新規でグループ化
        private ReThumb Test4()
        {
            List<ReThumb> list = new();//要素作成
            for (int i = 0; i < 5; i++)
            {
                ReThumb re = new(MakeTextBlock($"test4の{i}"), $"テスト4の{i}", i * 20 + 20, i * 50 + 30);
                re.GotFocus += MyReThumb_GotFocus;
                list.Add(re);
            }
            ReThumb group = new(list, "グループ");//Group作成
            var neko = group.Children;
            group.GotFocus += MyReThumb_GotFocus;
            return group;
        }


        private void Test1()
        {
            MyReThumb1 = new ReThumb(MakeTextBlock("test1"), "test1");
            //Layer1.ChildrenOld.Add(MyReThumb1);
            MyLayer1.AddChildren(MyReThumb1);
            MyReThumb1.GotFocus += MyReThumb_GotFocus;
        }
        private void Test2()
        {
            MyReThumb2 = new ReThumb(MakeTextBlock("test2"), "Test2", 100, 0);
            MyLayer1.AddChildren(MyReThumb2);
            //Layer1.ChildrenOld.Add(MyReThumb2);
            MyReThumb2.GotFocus += MyReThumb_GotFocus;
        }
        private void Test3()
        {
            for (int i = 0; i < 5; i++)
            {
                ReThumb re = new(MakeTextBlock($"test3-{i}"), $"テスト3-{i}", i * 20 + 20, i * 50 + 100);
                re.GotFocus += MyReThumb_GotFocus;
                //Layer1.ChildrenOld.Add(re);
                MyLayer1.AddChildren(re);
            }
            //Enumerable.Range(0, 5)
            //          .Select(a => new ReThumb(MakeTextBlock($"test3-{a}"), a * 20 + 20, a * 50 + 100, $"テスト3-{a}"))
            //          .ToList()
            //          .ForEach(a => { a.GotFocus += MyReThumb_GotFocus; Layer1.Children.Add(a); });
        }

        private void MyReThumb_GotFocus(object sender, RoutedEventArgs e)
        {
            ReThumb item = sender as ReThumb;
            //FocusThumb = item.RootReThumb;
            FocusThumb = item;
        }

        private TextBlock MakeTextBlock(string text)
        {
            TextBlock tb = new();
            tb.Text = text;
            tb.Background = Brushes.MediumAquamarine;
            tb.Foreground = Brushes.White;
            tb.FontSize = 30;
            return tb;
        }

        private void ButtonUngroup_Click(object sender, RoutedEventArgs e)
        {
            ReThumb re = MyStackPanel.DataContext as ReThumb;
            re?.UnGroup();
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var fthumb = focusThumb;
            var data = MyStackPanel.DataContext;
            var zindex = focusThumb?.ZetIndex;
            var children = MyLayer1.Children;
        }

        private void ButtonZIndexUp_Click(object sender, RoutedEventArgs e)
        {
            if (focusThumb == null) { return; }
            int z = focusThumb.ZetIndex;
            focusThumb.SetZIndex(z + 1);
        }

        private void ButtonZIndexDown_Click(object sender, RoutedEventArgs e)
        {
            if (focusThumb == null) { return; }
            int z = focusThumb.ZetIndex;
            focusThumb.SetZIndex(z - 1);
        }
    }
}
