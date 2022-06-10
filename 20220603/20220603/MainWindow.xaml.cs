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
using System.Collections.Specialized;

//PointCollectionの要素はPoint、これが頂点座標、座標を変更したいときはPoint自体を入れ替える
//xだけを変更したいときも新たにPointを作成して、元のPointと入れ替える必要があって
//Point.X=新しい値ってのはできない、エラーになる
//
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

        //public PointCollection MyPointC { get; set; } = new() { new(100, 100), new(200, 100), new(200, 200) };
        private PolyLineCanvas MyPolyLineCanvas;

        public MainWindow()
        {
#if DEBUG
            Left = 10; Top = 10;
#endif
            InitializeComponent();


            MyPolyLineCanvas = new(Brushes.OrangeRed, 4);
            MyPolyLineCanvas.AddPoint(new Point(20, 20));
            MyPolyLineCanvas.AddPoint(new Point(120, 20));
            //MyGrid.Children.Add(MyPolyLineCanvas);
            MyCanvas.Children.Add(MyPolyLineCanvas);

        }


        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.AddPoint(new Point(300, 200));

        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyPolyLineCanvas.RemovePoint();
            //MyThumbsAndPoints.RemovePoint();
        }


    }

    public class PolyLineCanvas : Canvas
    {
        private List<Thumb> MyThumbs = new();
        private PointCollection MyPoints = new();
        public Thumb? MyCurrentThumb { get; private set; }
        public Polyline MyPolyline;
        private Polyline TempPolyline;
        private PointCollection TempPoints = new();
        public PolyLineCanvas(Brush stroke, double thickness)
        {
            MyPolyline = new() { Stroke = stroke, StrokeThickness = thickness, Points = MyPoints };
            this.Children.Add(MyPolyline);
            TempPolyline = new() { Stroke = stroke, StrokeThickness = thickness, Points = TempPoints };
            TempPolyline.Visibility = Visibility.Collapsed;
            this.Children.Add(TempPolyline);
        }

        public void AddPoint(Point p)
        {
            Thumb t = new() { Width = 20, Height = 20 };
            t.DragStarted += T_DragStarted;
            t.DragDelta += Thumb_DragDelta;
            t.DragCompleted += T_DragCompleted;
            t.PreviewMouseDown += Thumb_PreviewMouseDown;
            Canvas.SetLeft(t, p.X); Canvas.SetTop(t, p.Y);
            MyPoints.Add(p);
            MyThumbs.Add(t);
            this.Children.Add(t);
        }


        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MyCurrentThumb = sender as Thumb;
        }

        public void RemovePoint()
        {
            if (MyCurrentThumb is null) { return; }
            int i = MyThumbs.IndexOf(MyCurrentThumb);
            MyPoints.RemoveAt(i);
            MyThumbs.Remove(MyCurrentThumb);
            this.Children.Remove(MyCurrentThumb);
            MyCurrentThumb = null;
        }
        private void T_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            if (MyThumbs.Count > 2)
            {
                foreach (var item in MyThumbs)
                {
                    item.Visibility = Visibility.Collapsed;
                }
                t.Visibility = Visibility.Visible;
                MyPolyline.Visibility = Visibility.Collapsed;
                MovePolyLine(MyThumbs.IndexOf(t));
                TempPolyline.Visibility = Visibility.Visible;
            }
        }

        private void T_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (MyThumbs.Count > 2)
            {
                foreach (var item in MyThumbs)
                {
                    item.Visibility = Visibility.Visible;
                }
                MyPolyline.Visibility = Visibility.Visible;
                TempPolyline.Visibility = Visibility.Collapsed;
                TempPolyline.Points.Clear();
            }
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not Thumb t) { return; }
            double x = Canvas.GetLeft(t) + e.HorizontalChange;
            double y = Canvas.GetTop(t) + e.VerticalChange;
            Point p = new(x, y);
            int i = MyThumbs.IndexOf(t);
            MyPoints[i] = p;
            Canvas.SetLeft(t, x); Canvas.SetTop(t, y);
            if (MyThumbs.Count > 2) { TempPoints[1] = p; }
            //else { TempPoints[0] = p; }
        }
        private void MovePolyLine(int tIndex)
        {
            TempPoints.Clear();
            int end = tIndex + 2;
            if (end > MyThumbs.Count) { end = MyThumbs.Count; }
            int begin = tIndex - 1;
            if (begin < 0) { begin = 0; }
            for (int i = begin; i < end; i++)
            {
                TempPoints.Add(MyPoints[i]);
            }

        }
    }



    public class TThumb : Thumb
    {
        public string MyText { get; set; }
        public int MyIndex { get; set; }
        //public NotifyXY MyNotifyXY { get; set; }


        public TThumb(double x, double y, int index)
        {
            //MyNotifyXY = new NotifyXY(x, y);
            MyText = "text";
            SetTemplate();
            DataContext = this;
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);
            MyIndex = index;
            //var neko = Canvas.GetLeft(this);
            //Binding b = new(nameof(MyNotifyXY.X));
            //b.Source = MyNotifyXY;
            //b.Mode = BindingMode.TwoWay;
            //this.SetBinding(Canvas.LeftProperty, b);
            //neko = Canvas.GetLeft(this);
            //b = new(nameof(MyNotifyXY.Y));
            //b.Source = MyNotifyXY;
            //b.Mode = BindingMode.TwoWay;
            //this.SetBinding(Canvas.TopProperty, b);
            //DragDelta += TThumb_DragDelta;
            //neko = Canvas.GetLeft(this);
        }

        //private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    if (sender is not TThumb tt) { return; }
        //    MyNotifyXY.X = Canvas.GetLeft(tt) + e.HorizontalChange;
        //    MyNotifyXY.Y = Canvas.GetTop(tt) + e.VerticalChange;
        //}

        private void SetTemplate()
        {
            FrameworkElementFactory panelFactory = new(typeof(Grid));
            FrameworkElementFactory textblockFactory = new(typeof(TextBlock));
            textblockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            textblockFactory.SetValue(TextBlock.PaddingProperty, new Thickness(4, 2, 4, 2));
            Binding b = new(nameof(MyIndex));
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textblockFactory.SetBinding(TextBlock.TextProperty, b);
            panelFactory.AppendChild(textblockFactory);
            panelFactory.SetValue(Panel.BackgroundProperty, Brushes.Blue);
            panelFactory.SetValue(OpacityProperty, 0.2);
            ControlTemplate template = new();
            template.VisualTree = panelFactory;
            this.Template = template;
            //ApplyTemplate();
        }
        public override string ToString()
        {
            return $"Name index{MyIndex}";
        }
    }


}

