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

//ContentControlにデータをBinding
//DataTemplateを使ってデータの種類によって表示を変える


namespace _20220411_ContentControlDataTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControl1 MyUserControl1_0;
        //private UserControl1 MyUserControl1_1;
        public MainWindow()
        {
            InitializeComponent();

            MyUserControl1_0 = new();
            DataText text = new("てすと1", 30, 100, 10, 1);
            MyUserControl1_0.MyData = text;
            MyCanvas.Children.Add(MyUserControl1_0);

            MyCanvas.Children.Add(new UserControl1() { MyData = new DataRect(Brushes.Cyan, 20, 10, 0, 400, 100) });
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyUserControl1_0.MyData;
            MyCanvas.Children.Add(new UserControl1() { MyData = new DataText("追加TextBlock", 20, 100, 100,2) });
            //TestControl.MyData = new DataText("追加TextBlocl", 30, 200, 2, 100, 100);//反映されない
            //MyUserControl1_1.MyData.Y = 50;//反映されない
        }
    }
}
