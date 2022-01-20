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

namespace _20220118
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TTTextBlock MyTT;
        List<TThumb> MyThumbs = new();
        List<TThumb> MyGroups = new();
        int MyCount = 0;
        public MainWindow()
        {
            InitializeComponent();

            //Test1();
        }
        private void Test1()
        {
            MyTT = new($"{nameof(Test1)}", 10, 10, "T1");
            MyTT.GotFocus += MyTT_GotFocus;
            MyLayer.AddThumb(MyTT);
            MyThumbs.Add(MyTT);

            TTTextBlock ttt = new("T2", 100, 100, "T2");
            ttt.GotFocus += MyTT_GotFocus;
            MyLayer.AddThumb(ttt);
            MyThumbs.Add(ttt);

            MyStackPanel1.DataContext = MyLayer;
        }

        private void MyTT_GotFocus(object sender, RoutedEventArgs e)
        {
            TThumb thumb = sender as TThumb;
            MyStackPanelFocusedThumb.DataContext = thumb;
            MyStackPanelFocusedParent.DataContext = thumb.ParentGroupThumb;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            //MyTT.Text = "aaaaaaaa";
            var list = MyLayer.ChildrenList;
            //var focus = MyLayer.FocusedChildThumb;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            TTTextBlock ttb = new($"要素{MyCount}", MyCount * 30, MyCount * 45, $"要素{MyCount}");
            MyThumbs.Add(ttb);
            ttb.GotFocus += MyTT_GotFocus;
            MyLayer.AddThumb(ttb);
            MyCount++;
        }

        private void ButtonG1_Click(object sender, RoutedEventArgs e)
        {
            TTGroup group = MyLayer.ToGroup(MyThumbs.GetRange(0, 2), "G1");
            group.GotFocus += MyTT_GotFocus;
            MyGroups.Add(group);
        }

        private void ButtonG2_Click(object sender, RoutedEventArgs e)
        {
            TTGroup group = MyLayer.ToGroup(MyThumbs.GetRange(2, 2), "G2");
            group.GotFocus += MyTT_GotFocus;
            MyGroups.Add(group);
        }

        private void ButtonG3_Click(object sender, RoutedEventArgs e)
        {
            TTGroup group = MyLayer.ToGroup(MyGroups.GetRange(0, 2), "G3");
            group.GotFocus += MyTT_GotFocus;
            MyGroups.Add(group);
        }
    }
}
