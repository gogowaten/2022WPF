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

//Pointは実質的にはバインドできないと考えたほうが良さそう、とくに双方向はできない

namespace _20220603
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region notifyProperty
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion notifyProperty

        public PointCollection MyPointC { get; set; } = new();
        public ObservableCollection<Thumb> MyThumbs { get; set; } = new();
        private int MyCount;
        private Thumb? MyActiveThumb;
        public MainWindow()
        {
#if DEBUG
            Left = 10; Top = 10;
#endif
            InitializeComponent();
            
            MyThumbs.CollectionChanged += MyThumbs_CollectionChanged;
            MyCanvas.Children.Add(MakePolyline(MyPointC, Brushes.Red, 10));
            AddPoint(new Point(100, 200));
            AddPoint(new Point(200, 300));
            //for (int i = 0; i < 5000; i++)
            //{
            //    AddPoint(new Point(i, i));
            //}
        }

        private void MyThumbs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                
                if (e.NewItems?[0] is Thumb t)
                {
                    t.Tag = MyThumbs.Count - 1;
                }
            }
            else if(e.Action== System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                
                int ii = e.OldStartingIndex;
                if(e.OldItems?[0] is Thumb t)
                {
                    for (int i = ii; i < MyThumbs.Count; i++)
                    {
                        MyThumbs[i].Tag = i;
                    }
                }
            }
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
            int i = (int)t.Tag;
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
            if(sender is Thumb t)
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
            Canvas.SetTop(t, y);
            int i = (int)t.Tag;
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
    }
  

    public class NPoint : INotifyPropertyChanged
    {
        private double _x;
        private double _y;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double X
        {
            get => _x; set
            {
                if (_x == value) return;
                _x = value; OnPropertyChanged();
            }
        }
        public double Y
        {
            get => _y; set
            {
                if (_y == value) return;
                _y = value; OnPropertyChanged();
            }
        }
        public NPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public class ObservablePoints : ObservableCollection<NPoint>
    {
        public ObservablePoints() { }
    }
    public class Data
    {
        public List<NPoint> NPoints { get; set; }
        public PointCollection PC { get; set; }
        public Data(List<NPoint> nPoints, PointCollection pC)
        {
            NPoints = nPoints;
            PC = pC;
        }
    }
    
}

