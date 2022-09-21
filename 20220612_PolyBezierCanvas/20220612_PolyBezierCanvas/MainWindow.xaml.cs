using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
//WPF、ベジェ曲線で直線表示、アンカー点の追加と削除 - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2022/06/14/132217

namespace _20220612_PolyBezierCanvas
{
    public partial class MainWindow : Window
    {
        private PolyBezierCanvas2 MyPolyBezierCanvas2 { get; set; }

        public MainWindow()
        {
#if DEBUG
            Left = 100; Top = 100;
#endif
            InitializeComponent();

            MyPolyBezierCanvas2 = new PolyBezierCanvas2(
                new Point(100, 100), new Point(200, 200), Brushes.DarkMagenta, 10);
            MyCanvas.Children.Add(MyPolyBezierCanvas2.MyBezierPath);
            MyPolyBezierCanvas2.AddAnchorPoint(new Point(300, 150));
            DataContext = MyPolyBezierCanvas2;
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            //ランダムな位置にアンカー点追加
            Random r = new(DateTime.Now.Millisecond);
            int x = r.Next(300); int y = r.Next(300);
            MyPolyBezierCanvas2.AddAnchorPoint(new Point(x, y));
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyPolyBezierCanvas2.RemoveAnchorPoint(MyListBox.SelectedIndex);
        }
    }


    /// <summary>
    /// ベジェ曲線のアンカー点の追加と削除のテスト用
    /// </summary>
    public class PolyBezierCanvas2
    {
        public Path MyBezierPath = new();//ベジェ曲線表示用
        //StartPointの管理に使う
        public PathFigure MyBezierFigure { get; } = new();
        //BezierSegmentのPoints
        public PointCollection MyBezierPoints { get; } = new();
        //ベジェ曲線を表示するPathの構成
        //Path                               MyBezierPath    ベジェ曲線表示用
        //┗Data(PathGeometry型)
        // ┗PathFigures
        //  ┗PathFigure                      MyBezierFigure  StartPointの指定で使う
        //   ┗Segments
        //    ┗PolyBezierSegment
        //     ┗Points(PointCollection型)    MyBezierPoints  StartPoint以外の全てのPoint

        //アンカー点管理用Collection
        public ObservableCollection<Point> MyAnchorPoints { get; } = new();
        //ベジェ曲線の頂点はアンカー点と制御点の2種類で構成されている
        //頂点の追加や削除はアンカー点を基準に行い、制御点だけを追加削除することはしない
        //アンカー点追加時は同時に制御点2つを追加
        //アンカー点削除時は同時に制御点2つを削除
        public PolyBezierCanvas2() { }
        public PolyBezierCanvas2(Point anchor0, Point anchor1, Brush stroke, double thickness)
        {
            MyAnchorPoints.CollectionChanged += MyAnchorPoints_CollectionChanged;
            MyBezierPath = new() { Stroke = stroke, StrokeThickness = thickness };
            //ベジェ曲線のdata作成
            PolyBezierSegment seg = new(); seg.Points = MyBezierPoints;
            MyBezierFigure = new(); MyBezierFigure.Segments.Add(seg);
            PathGeometry geo = new(); geo.Figures.Add(MyBezierFigure);
            MyBezierPath.Data = geo;
            //スタートポイントとアンカー点追加
            MyBezierFigure.StartPoint = anchor0;
            AddAnchorPoint(anchor0);
            AddAnchorPoint(anchor1);
        }

        //アンカー点の追加、削除時
        private void MyAnchorPoints_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //追加時は制御点も2点追加する
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?[0] is not Point p) { return; }

                if (MyAnchorPoints.Count > 1)
                {
                    //制御点2つとアンカー点を追加
                    //1個めの制御点座標は1個前のアンカー点と同じにする
                    //2個めの制御点座標は追加されたアンカー点と同じにする
                    int pi = e.NewStartingIndex - 1;
                    Point preAncor;//1個前のアンカー点
                    if (pi == 0) { preAncor = MyBezierFigure.StartPoint; }
                    else { preAncor = MyAnchorPoints[pi]; }
                    MyBezierPoints.Add(preAncor);//制御点
                    MyBezierPoints.Add(p);//制御点
                    MyBezierPoints.Add(p);//アンカー点
                }

            }
            //削除時、アンカー点に付随する制御点2点削除する
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //削除されたアンカー点のIndex
                int pi = e.OldStartingIndex;
                //BezierSegmentのPointsから削除する点のIndex
                int api = pi * 3 - 2;
                //削除したアンカー点が始点だった場合
                if (pi == 0)
                {
                    //FigureのstartPointの入れ替え(前に詰める)
                    MyBezierFigure.StartPoint = MyAnchorPoints[0];
                    api = 0;
                }
                //削除したアンカー点が終点だった場合
                else if (pi == MyAnchorPoints.Count)
                {
                    api = pi * 3 - 3;
                }
                //削除
                //Collectionからの削除は削除はだるま落としなので同じIndex指定
                MyBezierPoints.RemoveAt(api);
                MyBezierPoints.RemoveAt(api);
                MyBezierPoints.RemoveAt(api);
            }
        }

        public void AddAnchorPoint(Point p)
        {
            MyAnchorPoints.Add(p);
        }

        public void RemoveAnchorPoint(int pi)
        {
            //無効なIndexなら削除しない
            if (0 > pi || pi > MyAnchorPoints.Count) { return; }
            //アンカー点が2個なら削除しない(2個以上を保つ)
            if (MyAnchorPoints.Count == 2) { return; }

            MyAnchorPoints.RemoveAt(pi);
        }


    }
}
