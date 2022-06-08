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
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;


//PolyLineで折れ線表示
//アンカーポイントにThumbを表示
//Thumbをマウスドラッグ移動でアンカーポイントも移動
//アンカーポイントの動的追加と削除

//アンカーポイントとThumbは手動で同期させる
//同期ってのは個数と順番
//アンカーポイント追加するときはThumbも追加する
//アンカーポイント削除するときは最後にクリックされたThumbと対応するアンカーポイントを削除

namespace _20220608_PathとThumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //PolyLineのアンカーポイント
        public PointCollection MyPointC { get; set; } = new();
        //アンカーポイントに表示するThumbを入れておく用コレクション
        public ObservableCollection<Thumb> MyThumbs { get; set; } = new();
        private int MyCount;
        private Thumb? MyActiveThumb;//最後にクリックされたThumbを入れておく用
        bool IsThumbVisible = true;//Thumbの表示の有無フラグ
        public MainWindow()
        {
#if DEBUG
            Left = 10; Top = 10;
#endif
            InitializeComponent();

            MyCanvas.Children.Add(MakePolyline(MyPointC, Brushes.Magenta, 4));
            AddPoint(new Point(100, 200));
            AddPoint(new Point(200, 300));

            #region 負荷テスト用
            //AddPointRound(1500);//円環
            AddPoint円環はお断り(7000);

            //1ピクセル斜めに追加
            //for (int i = 0; i < 5000; i++)
            //{
            //    AddPoint(new Point(i, i));
            //}
            #endregion 負荷テスト用
        }

        private void AddPointRound(int count)
        {
            double r = 200; double s = 360.0 / count;
            double x; double y; int c = 0;
            for (double i = 0.0; i < 360.0; i += s)
            {
                double rad = Radian(i);
                x = Math.Cos(rad) * r + r;
                y = Math.Sin(rad) * r + r;
                AddPoint(new Point(x, y));
                c++;
            }
        }
        private void AddPoint円環はお断り(int count)
        {
            double r = 200; double s = 3600.0 / count;
            double rr = r / (count * 2.0); double rrr = r;
            double x; double y; int c = 0;
            for (double i = 0.0; i < 3600.0; i += s)
            {
                double rad = Radian(i);
                x = Math.Cos(rad) * (rrr) + r;
                y = Math.Sin(rad) * (rrr) + r;
                AddPoint(new Point(x, y));
                c++; rrr -= rr;
            }
        }

        public double Radian(double degrees)
        {
            return Math.PI / 180.0 * degrees;
        }
        private void AddPoint(Point p)
        {
            MyPointC.Add(p);
            var t = MakeThumb(p);
            MyThumbs.Add(t);
            MyCanvas.Children.Add(t);
        }
        private void RemovePoint(Thumb? t)
        {
            if (t is not Thumb) return;
            int i = MyThumbs.IndexOf(t);
            MyPointC.RemoveAt(i);
            MyThumbs.Remove(t);
            MyCanvas.Children.Remove(t);
            MyActiveThumb = null;
        }


        private Thumb MakeThumb(Point p)
        {
            Thumb t = new() { Width = 20, Height = 20 };
            t.DragDelta += T_DragDelta;
            t.PreviewMouseDown += Thumb_PreviewMouseDown;
            Canvas.SetLeft(t, p.X); Canvas.SetTop(t, p.Y);
            return t;
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Thumb t)
            {
                MyActiveThumb = t;
            }
        }

        private Polyline MakePolyline(PointCollection pc, Brush stroke, double thickness)
        {
            Polyline pl = new();
            pl.Points = pc;
            pl.Stroke = stroke;
            pl.StrokeThickness = thickness;
            return pl;
        }



        private void T_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            double x = Canvas.GetLeft(t) + e.HorizontalChange;
            double y = Canvas.GetTop(t) + e.VerticalChange;
            Canvas.SetLeft(t, x);
            Canvas.SetTop(t, y); ;
            int i = MyThumbs.IndexOf(t);
            MyPointC[i] = new Point(x, y);
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            AddPoint(new Point(MyCount * 30, MyCount * 20));
            MyCount++;
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            RemovePoint(MyActiveThumb);
        }

        private void MyButton3_Click(object sender, RoutedEventArgs e)
        {
            if (IsThumbVisible)
            {
                //Thumbの表示を最初の1つだけにする
                for (int i = 1; i < MyThumbs.Count; i++)
                {
                    MyThumbs[i].Visibility = Visibility.Collapsed;
                }
                IsThumbVisible = false;
            }
            else
            {
                //全てのThumbを表示する
                for (int i = 1; i < MyThumbs.Count; i++)
                {
                    MyThumbs[i].Visibility = Visibility.Visible;
                }
                IsThumbVisible = true;
            }
            
        }
    }


}