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

//グループ化、グループ解除、ZIndexまでできた

namespace _20220103_ThumbZ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReThumb MyReThumb1;
        ReThumb MyReThumb2;
        ReThumb ClickedThumb;
        private ReThumb focusThumb;

        public ReThumb FocusThumb
        {
            get => focusThumb; set
            {
                focusThumb = value;
                //MyStackPanel.DataContext = value;
                //FocusのRootをBinding
                MyStackPanel.DataContext = value.RootReThumb;
                //MyStackPanel3.DataContext = MyLayer1;
            }
        }
        public MainWindow()
        {
            InitializeComponent();


            //Test1();
            //Test2();
            //Test3();
            //MyLayer1.AddChildren(Test4());//グループ化
            //Test5();//グループ
            //Test6();
            //Test7();//解除テスト
            //Test8();//
            //FocusThumbをこっちに用意しておいて、Newのときに渡すかPublicにしておいて、向こうでGotFocusイベントでdatacontextに指定するようにする？
            //Test9();//GotFocus、やっぱりやめた
            Test10();
        }
        //Zオーダー、要素0,1,2,3を配置して、0と2をグループ化
        private void Test10()
        {

            for (int i = 0; i < 4; i++)
            {
                ReThumb re = new(MakeTextBlock($"{nameof(Test10)}-{i}", new SolidColorBrush(Color.FromRgb(20, 200, 40))), $"{nameof(Test10)}-{i}", i * 20, i * 50);
                re.GotFocus += MyReThumb_GotFocus;
                //re.Group(new List<ReThumb>() { re });
                MyLayer1.AddChildren(re);
            }

            ReThumb group = new(new List<ReThumb>() { MyLayer1.Children[2], MyLayer1.Children[0] }, "group");
            //ReThumb group = new(new List<ReThumb>() { MyLayer1.Children[0], MyLayer1.Children[2] }, "group");
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
            //一番下に四角形図形
            ReThumb rect1 = new(MakeRectangle(null, 100, 200), "下rectangle");
            rect1.GotFocus += MyReThumb_GotFocus;
            rect1.PreviewMouseDown += MyReThumb_PreviewMouseDown;
            MyLayer1.AddChildren(rect1);

            string name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            List<ReThumb> list = new();
            for (int i = 0; i < 3; i++)
            {
                ReThumb re = new ReThumb(MakeTextBlock($"name{i}"), $"要素{i}", i + 20, i * 50 + 30);
                re.GotFocus += MyReThumb_GotFocus;
                re.PreviewMouseDown += MyReThumb_PreviewMouseDown;
                MyLayer1.AddChildren(re);
                list.Add(re);
            }
            ReThumb group = new(list, "グループ");
            group.GotFocus += MyReThumb_GotFocus;
            group.PreviewMouseDown += MyReThumb_PreviewMouseDown;
            group.Focus();

            //一番上にも四角形図形
            ReThumb rect2 = new(MakeRectangle(Brushes.MediumBlue, 100, 200), "上rectangle", 200, 0);
            rect2.GotFocus += MyReThumb_GotFocus;
            rect2.PreviewMouseDown += MyReThumb_PreviewMouseDown;
            MyLayer1.AddChildren(rect2);
        }

        private void MyReThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //ClickedThumb = sender as ReThumb;
            MyStackPanel2.DataContext = sender as ReThumb;
        }

        //既存グループ同士からグループ作成、グループAとグループBからグループC作成
        private void Test6()
        {
            string name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            //グループA作成してLayerに追加、LINQで書いてみた
            var listA = Enumerable.Range(0, 3).Select(a => new ReThumb(MakeTextBlock($"GroupA-{a}", new SolidColorBrush(Color.FromRgb(20, (byte)(100 + a * 40), 50))), name, a * 20 + 10, a * 30 + 10)).ToList();
            listA.ForEach(a => { a.GotFocus += MyReThumb_GotFocus; MyLayer1.AddChildren(a); });
            ReThumb groupA = new(listA, "グループA");

            //グループB作成、普通のForで書いてみた
            List<ReThumb> listB = new();
            for (int i = 0; i < 3; i++)
            {
                SolidColorBrush brush = new(Color.FromRgb((byte)(100 + (i * 40)), 50, (byte)(100 + (i * 40))));
                ReThumb thumb = new ReThumb(MakeTextBlock($"GroupB-{i}", brush), name, i * 20 + 200, i * 30 + 20);
                thumb.GotFocus += MyReThumb_GotFocus;
                MyLayer1.AddChildren(thumb);
                listB.Add(thumb);
            }
            ReThumb groupB = new(listB, "グループB");

            //グループAとBから新規にグループC作成
            List<ReThumb> listC = new() { groupA, groupB };
            ReThumb groupC = new(listC, $"グループC");
            groupC.GotFocus += MyReThumb_GotFocus;

            groupC.Focus();
        }


        //既存要素群からグループ作成、Zオーダー有効、要素0,1,2,3を配置して、0と2をグループ化
        private void Test5()
        {
            //要素群作成、Layerに追加
            string name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            for (int i = 0; i < 4; i++)
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(20, (byte)(100 + i * 30), 40));
                ReThumb re = new(MakeTextBlock($"{name}-{i}", brush), $"{name}-{i}", i * 20, i * 40);
                re.GotFocus += MyReThumb_GotFocus;
                MyLayer1.AddChildren(re);//要素を実際に配置
            }

            //Layerにある要素群からGroup新規作成、配置は自動、配置先はもとの要素群と同じになる
            ReThumb group = new(new List<ReThumb>() { MyLayer1.Children[2], MyLayer1.Children[0] }, "group");
            group.GotFocus += MyReThumb_GotFocus;
            group.Focus();
        }
        //新規でグループ化、Layerに追加していない要素群からグループ作成
        private ReThumb Test4()
        {
            List<ReThumb> list = new();//要素作成
            for (int i = 0; i < 3; i++)
            {
                ReThumb re = new(MakeTextBlock($"{nameof(Test4)}の{i}"), $"{nameof(Test4)}の{i}", i * 20 + 20, i * 50 + 30);
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

        private TextBlock MakeTextBlock(string text, Brush brush = null)
        {
            if (brush == null) { brush = Brushes.MediumAquamarine; }
            TextBlock tb = new();
            tb.Text = text;
            tb.Background = brush;
            tb.Foreground = Brushes.White;
            tb.FontSize = 30;
            return tb;
        }
        private Rectangle MakeRectangle(Brush brush, double width = 100, double height = 100)
        {
            if (brush == null) { brush = Brushes.HotPink; }
            Rectangle rectangle = new();
            rectangle.Fill = brush;
            rectangle.Width = width;
            rectangle.Height = height;

            return rectangle;
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
            var zzindex = Panel.GetZIndex(focusThumb);
            var children = MyLayer1.Children;
            var fChildren = focusThumb.Children;
        }

        private void ButtonZIndexUp_Click(object sender, RoutedEventArgs e)
        {
            if (focusThumb == null) { return; }
            int z = focusThumb.ZetIndex;
            focusThumb.ChangeZIndex(z + 1);
        }

        private void ButtonZIndexDown_Click(object sender, RoutedEventArgs e)
        {
            if (focusThumb == null) { return; }
            int z = focusThumb.ZetIndex;
            focusThumb.ChangeZIndex(z - 1);
        }
    }
}
