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
            //Rectangle
            MyCanvas.Children.Add(new UserControl1() { MyData = new DataRect(Brushes.Cyan, 20, 10, 0, 400, 100) });
            //直線Path
            LineGeometry lineGeometry = new(new Point(0, 0), new Point(30, 300));
            MyCanvas.Children.Add(new UserControl1() { MyData = new DataPath(lineGeometry, Brushes.Red, 0, 9, 0) });
            //直線Pathその２
            PathFigure pathFigure = new();
            pathFigure.Segments.Add(new LineSegment(new(200, 30), true));
            PathGeometry geo = new();
            geo.Figures.Add(pathFigure);
            MyCanvas.Children.Add(new UserControl1() { MyData = new DataPath(geo, Brushes.Orange, 50, 100, 0) });
            //直線＋円弧
            pathFigure = new();
            pathFigure.Segments.Add(new LineSegment(new(200, 30), true));
            pathFigure.Segments.Add(new ArcSegment(new Point(50, 50), new Size(30, 30), 290, true, SweepDirection.Clockwise, true));
            geo = new();
            geo.Figures.Add(pathFigure);
            MyCanvas.Children.Add(new UserControl1() { MyData = new DataPath(geo, Brushes.MediumSeaGreen, 150, 150, 0) });
            //角丸四角形
            RectangleGeometry rectangle = new(new Rect(0, 0, 100, 100), 5, 5);
            MyCanvas.Children.Add(new UserControl1() { MyData = new DataPath(rectangle, Brushes.SteelBlue, 150, 250, 0) });

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyUserControl1_0.MyData;
            MyCanvas.Children.Add(new UserControl1() { MyData = new DataText("追加TextBlock", 20, 100, 100, 2) });
            //TestControl.MyData = new DataText("追加TextBlocl", 30, 200, 2, 100, 100);//反映されない
            //MyUserControl1_1.MyData.Y = 50;//反映されない
            Canvas.SetLeft(MyUserControl1_0, 199);//Modeを双方向にすれば反映される
            neko = MyUserControl1_0.MyData;
            MyUserControl1_0.MyData.X = 299;//値は書き換わるけど位置は変更されない、通知プロパティにする必要がある
            neko = MyUserControl1_0.MyData;
        }
    }
}
