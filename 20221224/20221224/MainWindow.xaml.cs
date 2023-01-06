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

namespace _20221224
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MyAddCounnt = 0;
        public MainWindow()
        {
            InitializeComponent();
            Left = 100; Top = 100;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            TTTextBlock tt = new()
            {
                MyLeft = 20 * MyAddCounnt,
                MyTop = 20 * MyAddCounnt,
                MyText = TextBoxAdd.Text + "\n" + MyAddCounnt,
                Name = TextBoxAdd.Text + MyAddCounnt,
            };
            //tt.MyData.X = 20 * MyAddCounnt;
            //tt.MyData.Y = 20 * MyAddCounnt;

            MyRootThumb.AddThumb(tt);
            MyAddCounnt++;

            //int bunkatu = 360;
            //double radius = 200;
            //for (int i = 0; i < bunkatu; i++)
            //{
            //    double radian = DegreeToRadian(i);
            //    double x = Math.Cos(radian) * radius;
            //    double y = Math.Sin(radian) * radius;
            //    TTTextBlock tx = new()
            //    {
            //        MyLeft = x,
            //        MyTop = y,
            //        MyText = TextBoxAdd.Text + MyAddCounnt,
            //        Name = TextBoxAdd.Text + MyAddCounnt,
            //    };
            //    MyRootThumb.AddThumb(tx); MyAddCounnt++;
            //}

        }
        private double DegreeToRadian(double x)
        {
            return Math.PI * (x / 180.0);
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.RemoveThumb();
            //↓これを禁止したいけど方法がわからん
            //MyRootThumb.InternalChildren.Remove(MyRootThumb.ActiveThumb);
        }

        private void ButtonRootActive_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ActiveGroup = MyRootThumb;
        }
        private void ButtonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.AddGroup();
        }

        private void ButtonUnGroup_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.UnGroup();
        }

        private void ButtonActiveInside_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ActiveGroupInside();
        }

        private void ButtonActiveOutside_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ActiveGroupOutside();
        }

        private void ButtonZUp_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ZUp();
        }

        private void ButtonZDown_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ZDown();
        }

        private void ButtonZUpFrontMost_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ZUpFrontMost();
        }

        private void ButtonZDownBackMost_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.ZDownBackMost();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.SaveImage();
            //MyRootThumb.SaveImage(MyRootThumb, MyScrollViewer);
        }

        private void ButtonActiveG_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.SaveImage(MyRootThumb.ActiveGroup);
        }

        private void ButtonActiveT_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.SaveImage(MyRootThumb.ActiveThumb);
        }
    }
}
