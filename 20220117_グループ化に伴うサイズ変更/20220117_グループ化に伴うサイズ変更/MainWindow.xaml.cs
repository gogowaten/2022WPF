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
using System.Windows.Controls.Primitives;

//要素追加ボタンで追加してからグループ化ボタン
//クリックでFocus
//Focusがグループのときに編集開始ボタンで中の要素を移動できる、けど
//グループの中のグループの中の要素はうごかない

namespace _20220117_グループ化に伴うサイズ変更
{
 
    //
    public partial class MainWindow : Window
    {
        int MyCount = 0;
        List<TThumb> MyList = new();
        private TThumb FocusTT;
        TThumb MyG1;
        TThumb MyG2;



        public MainWindow()
        {
            InitializeComponent();


        }



        private void AddElement()
        {
            TThumb t = TThumb.CreateTextBlockThumb($"要素{MyCount}", 30, MyCount * 30, MyCount * 50, $"要素{MyCount}");
            MyList.Add(t);
            MyLayer1.AddThumb(t);
            t.GotFocus += T_GotFocus;
            MyCount++;
        }

        private void T_GotFocus(object sender, RoutedEventArgs e)
        {
            TThumb tt = (TThumb)sender;
            MyStackPanel.DataContext = tt;
            FocusTT = tt;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var chi = MyLayer1.Children;
            var focus = MyLayer1.FocusTT;
        }
        private void ButtonTest2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddElement();
            //MyGTThumb.chil
            //MyGTThumb.GroupCanvas.Children.Add
        }

        private void ButtonGroup0_1_Click(object sender, RoutedEventArgs e)
        {
            //TThumb g = new(MyList.Take(2).ToList());
            MyG1 = MyLayer1.ToGroup(MyList.Take(2).ToList(), "G1");
            MyG1.GotFocus += T_GotFocus;
        }

        private void ButtonBeginGroupEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MyLayer1.FocusTT != null)
            {
                MyLayer1.FocusTT.IsEditInsideGroup = true;
            }
        }

        private void ButtonEndGroupEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MyLayer1.FocusTT != null)
            {
                MyLayer1.FocusTT.IsEditInsideGroup = false;
            }
        }

        private void ButtonGroup2_3_Click(object sender, RoutedEventArgs e)
        {
            MyG2 = MyLayer1.ToGroup(MyList.GetRange(2, 2), "G2");
            MyG2.GotFocus += T_GotFocus;
        }

        private void ButtonGroupG1G2_Click(object sender, RoutedEventArgs e)
        {
            MyLayer1.ToGroup(new List<TThumb>() { MyG1, MyG2 }).GotFocus += T_GotFocus;
        }
    }
}
