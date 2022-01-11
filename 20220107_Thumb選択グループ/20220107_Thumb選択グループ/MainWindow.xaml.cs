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

namespace _20220107_Thumb選択グループ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NeoThumb MyNeoThumb1;
        NeoThumb MyNeoThumb2;
        NeoThumb ClickedThumb;
        private NeoThumb focusThumb;
        private List<NeoThumb> MyNeoThumbs = new();

        Point MyStartPoint;
        bool IsSelection;
        RectangleGeometry MySelectionGeometry = new();
        Path MySelectionPath = new();

        public NeoThumb FocusThumb
        {
            get => focusThumb; set
            {
                focusThumb = value;
                //MyStackPanel.DataContext = value;
                //FocusのRootをBinding
                MyStackPanel.DataContext = value.RootNeoThumb;
                //MyStackPanel3.DataContext = MyLayer1;
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            //MyLayer1.MouseLeftButtonDown += MyLayer1_MouseLeftButtonDown;
            //MyLayer1.PreviewMouseLeftButtonDown += MyLayer1_PreviewMouseLeftButtonDown;
            //MyLayer1.MouseMove += MyLayer1_MouseMove;
            //MyLayer1.MouseLeftButtonUp += MyLayer1_MouseLeftButtonUp;
            MyCanvas.Children.Add(MySelectionPath);
            MySelectionPath.Stroke = Brushes.Red;
            MySelectionPath.StrokeThickness = 1;

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
                NeoThumb re = new(MakeTextBlock($"{nameof(Test10)}-{i}", new SolidColorBrush(Color.FromRgb(20, 200, 40))), $"{nameof(Test10)}-{i}", i * 20, i * 50);
                re.GotFocus += MyNeoThumb_GotFocus;
                //re.Group(new List<NeoThumb>() { re });
                MyLayer1.AddChildren(re);
            }

            NeoThumb group = new(new List<NeoThumb>() { MyLayer1.Children[2], MyLayer1.Children[0] }, "group");
            //NeoThumb group = new(new List<NeoThumb>() { MyLayer1.Children[0], MyLayer1.Children[2] }, "group");
        }
        //FocusThumbを取得するためにMainWindowを渡すようにした、向こうのGotFocusイベント時にこっちのFocusThumbを入れ替えるようにしたけど
        //不自然かも
        private void Test9()
        {
            //for (int i = 0; i < 3; i++)
            //{   
            //    MyLayer1.AddChildren(new NeoThumb(MakeTextBlock($"bind{i}"), this));
            //    //MyLayer1.AddChildren(new NeoThumb(MakeTextBlock($"bind{i}"), this,null, i * 10, i * 50));
            //}
        }
        //グループ解除
        private void Test8()
        {
            NeoThumb aa = new(Enumerable.Range(0, 2).
                Select(a => new NeoThumb(MakeTextBlock($"要素A-{a}"), $"要素A-{a}", a * 20 + 10, a * 50 + 10)), "グループA");
            aa.Children.ToList().ForEach(a => a.GotFocus += MyNeoThumb_GotFocus);
            aa.GotFocus += MyNeoThumb_GotFocus;

            NeoThumb bb = new(Enumerable.Range(0, 2).
                Select(a => new NeoThumb(MakeTextBlock($"要素B-{a}"), $"要素B-{a}", a * 20 + 210, a * 50 + 10)), "グループB");
            bb.Children.ToList().ForEach(a => a.GotFocus += MyNeoThumb_GotFocus);
            bb.GotFocus += MyNeoThumb_GotFocus;

            List<NeoThumb> list = new() { aa, bb };
            var cc = new NeoThumb(list, "グループC");
            cc.GotFocus += MyNeoThumb_GotFocus;
            MyLayer1.AddChildren(cc);

        }


        private void Test7()
        {
            //一番下に四角形図形
            NeoThumb rect1 = new(MakeRectangle(null, 100, 200), "下rectangle");
            rect1.GotFocus += MyNeoThumb_GotFocus;
            rect1.PreviewMouseDown += MyNeoThumb_PreviewMouseDown;
            MyLayer1.AddChildren(rect1);

            string name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            List<NeoThumb> list = new();
            for (int i = 0; i < 3; i++)
            {
                NeoThumb re = new NeoThumb(MakeTextBlock($"name{i}"), $"要素{i}", i + 20, i * 50 + 30);
                re.GotFocus += MyNeoThumb_GotFocus;
                re.PreviewMouseDown += MyNeoThumb_PreviewMouseDown;
                MyLayer1.AddChildren(re);
                list.Add(re);
            }
            NeoThumb group = new(list, "グループ");
            group.GotFocus += MyNeoThumb_GotFocus;
            group.PreviewMouseDown += MyNeoThumb_PreviewMouseDown;
            group.Focus();

            //一番上にも四角形図形
            NeoThumb rect2 = new(MakeRectangle(Brushes.MediumBlue, 100, 200), "上rectangle", 200, 0);
            rect2.GotFocus += MyNeoThumb_GotFocus;
            rect2.PreviewMouseDown += MyNeoThumb_PreviewMouseDown;
            MyLayer1.AddChildren(rect2);
        }

        private void MyNeoThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //ClickedThumb = sender as NeoThumb;
            MyStackPanel2.DataContext = sender as NeoThumb;
        }

        //既存グループ同士からグループ作成、グループAとグループBからグループC作成
        private void Test6()
        {
            string name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            //グループA作成してLayerに追加、LINQで書いてみた
            var listA = Enumerable.Range(0, 3).Select(a => new NeoThumb(MakeTextBlock($"GroupA-{a}", new SolidColorBrush(Color.FromRgb(20, (byte)(100 + a * 40), 50))), name, a * 20 + 10, a * 30 + 10)).ToList();
            listA.ForEach(a => { a.GotFocus += MyNeoThumb_GotFocus; MyLayer1.AddChildren(a); });
            NeoThumb groupA = new(listA, "グループA");

            //グループB作成、普通のForで書いてみた
            List<NeoThumb> listB = new();
            for (int i = 0; i < 3; i++)
            {
                SolidColorBrush brush = new(Color.FromRgb((byte)(100 + (i * 40)), 50, (byte)(100 + (i * 40))));
                NeoThumb thumb = new NeoThumb(MakeTextBlock($"GroupB-{i}", brush), name, i * 20 + 200, i * 30 + 20);
                thumb.GotFocus += MyNeoThumb_GotFocus;
                MyLayer1.AddChildren(thumb);
                listB.Add(thumb);
            }
            NeoThumb groupB = new(listB, "グループB");

            //グループAとBから新規にグループC作成
            List<NeoThumb> listC = new() { groupA, groupB };
            NeoThumb groupC = new(listC, $"グループC");
            groupC.GotFocus += MyNeoThumb_GotFocus;

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
                NeoThumb re = new(MakeTextBlock($"{name}-{i}", brush), $"{name}-{i}", i * 20, i * 40);
                re.GotFocus += MyNeoThumb_GotFocus;
                MyLayer1.AddChildren(re);//要素を実際に配置
            }

            //Layerにある要素群からGroup新規作成、配置は自動、配置先はもとの要素群と同じになる
            NeoThumb group = new(new List<NeoThumb>() { MyLayer1.Children[2], MyLayer1.Children[0] }, "group");
            group.GotFocus += MyNeoThumb_GotFocus;
            group.Focus();
        }
        //新規でグループ化、Layerに追加していない要素群からグループ作成
        private NeoThumb Test4()
        {
            List<NeoThumb> list = new();//要素作成
            for (int i = 0; i < 3; i++)
            {
                NeoThumb re = new(MakeTextBlock($"{nameof(Test4)}の{i}"), $"{nameof(Test4)}の{i}", i * 20 + 20, i * 50 + 30);
                re.GotFocus += MyNeoThumb_GotFocus;
                list.Add(re);
            }
            NeoThumb group = new(list, "グループ");//Group作成
            var neko = group.Children;
            group.GotFocus += MyNeoThumb_GotFocus;
            return group;
        }


        private void Test1()
        {
            MyNeoThumb1 = new NeoThumb(MakeTextBlock("test1"), "test1");
            //Layer1.ChildrenOld.Add(MyNeoThumb1);
            MyLayer1.AddChildren(MyNeoThumb1);
            MyNeoThumb1.GotFocus += MyNeoThumb_GotFocus;
        }
        private void Test2()
        {
            MyNeoThumb2 = new NeoThumb(MakeTextBlock("test2"), "Test2", 100, 0);
            MyLayer1.AddChildren(MyNeoThumb2);
            //Layer1.ChildrenOld.Add(MyNeoThumb2);
            MyNeoThumb2.GotFocus += MyNeoThumb_GotFocus;
        }
        private void Test3()
        {
            for (int i = 0; i < 5; i++)
            {
                NeoThumb re = new(MakeTextBlock($"test3-{i}"), $"テスト3-{i}", i * 20 + 20, i * 50 + 100);
                re.GotFocus += MyNeoThumb_GotFocus;
                //Layer1.ChildrenOld.Add(re);
                MyLayer1.AddChildren(re);
            }
            //Enumerable.Range(0, 5)
            //          .Select(a => new NeoThumb(MakeTextBlock($"test3-{a}"), a * 20 + 20, a * 50 + 100, $"テスト3-{a}"))
            //          .ToList()
            //          .ForEach(a => { a.GotFocus += MyNeoThumb_GotFocus; Layer1.Children.Add(a); });
        }

        private void MyNeoThumb_GotFocus(object sender, RoutedEventArgs e)
        {
            NeoThumb item = sender as NeoThumb;
            //FocusThumb = item.RootNeoThumb;
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
            NeoThumb re = MyStackPanel.DataContext as NeoThumb;
            re?.UnGroup();
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var fthumb = focusThumb;
            var data = MyStackPanel.DataContext;
            var zindex = focusThumb?.MyZIndex;
            var zzindex = Panel.GetZIndex(focusThumb);
            var children = MyLayer1.Children;
            var fChildren = focusThumb.Children;
        }

        private void ButtonZIndexUp_Click(object sender, RoutedEventArgs e)
        {
            if (focusThumb == null) { return; }
            int z = focusThumb.MyZIndex;
            focusThumb.ChangeZIndex(z + 1);
        }

        private void ButtonZIndexDown_Click(object sender, RoutedEventArgs e)
        {
            if (focusThumb == null) { return; }
            int z = focusThumb.MyZIndex;
            focusThumb.ChangeZIndex(z - 1);
        }

        private void ButtonGroup_Click(object sender, RoutedEventArgs e)
        {
            Rect bound = MySelectionGeometry.Bounds;

            List<NeoThumb> neoThumbs = new();
            foreach (var item in MyLayer1.Children)
            {
                //item.Test();
               var neko = item.Children;
                Rect r = new(item.MyLeft, item.MyTop, item.ActualWidth, item.ActualHeight);
                if (r.IntersectsWith(bound))
                {
                    neoThumbs.Add(item);
                }
            }
            NeoThumb neoThumb = new(neoThumbs, "");
        }
        private void ButtonSelectionPath_Click(object sender, RoutedEventArgs e)
        {
           
            //選択矩形表示切り替え
            Visibility visible = MySelectionPath.Visibility;
            if (visible == Visibility.Visible)
            {
                visible = Visibility.Collapsed;
            }
            else
            {
                visible = Visibility.Visible;

            }
            MySelectionPath.Visibility = visible;
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            MyStartPoint = e.GetPosition(canvas);
            IsSelection = true;
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsSelection == false) { return; }
            Canvas canvas = sender as Canvas;

            MySelectionGeometry.Rect = new Rect(MyStartPoint, e.GetPosition(canvas));
            MySelectionPath.Data = MySelectionGeometry;
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsSelection = false;
        }

    }
}
