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

        public PointCollection MyPointC { get; set; } = new() { new(100, 100), new(200, 100), new(200, 200) };
        public ObservableCollection<Thumb> MyThumbs { get; set; } = new();
        private int MyCount;
        private TThumb? MyCurrentThumb;
        private Polyline MyPolyLine;

        private Matome MyMatome;
        public List<TThumb> ThumbList;
        public PointCollection PointCollection;
        public List<int> IntList;
        public List<Point> PointList;


        public MainWindow()
        {
#if DEBUG
            Left = 10; Top = 10;
#endif
            InitializeComponent();
            //MyPolyLine = new() { Stroke = Brushes.Red, StrokeThickness = 4, Points = MyPointC };
            //MyCanvas.Children.Add(MyPolyLine);
            //ThumbList = new() { new TThumb(MyPointC[0].X, MyPointC[0].Y) { Name = "ttt" } };
            //IntList = new() { 9 };
            //PointList = new() { new Point(10, 10) };
            //MyMatome = new(ThumbList, MyPointC, IntList, PointList);
            //MyCanvas.Children.Add(ThumbList[0]);

            //MyMatome.IntList[0] = 99;
            //MyMatome.PointList[0] = new(99, 99);
            ////MyMatome.ThumbList[0] = null;
            //MyMatome.PointCollection[0] = new(9, 9);
            //ViewModel viewModel = new();
            //ObservableCollection<Vertex> vertices = new ObservableCollection<Vertex>();
            //vertices.Add(new Vertex() { Point=new(100,100)});
            //viewModel.Vertices = vertices;
            MyPolyLine1.Points = MyPointC;
        }


        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {

            MyCount++;
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var vertex = (Vertex)((Thumb)sender).DataContext;
            vertex.Point = new Point(
                vertex.Point.X + e.HorizontalChange,
                vertex.Point.Y + e.VerticalChange);
        }
    }

    public class TThumb : Thumb
    {
        public string Text { get; set; }
        public NotifyXY MyNotifyXY { get; set; }


        public TThumb(double x, double y)
        {
            MyNotifyXY = new NotifyXY(x, y);
            Text = "text";
            SetTemplate();
            DataContext = this;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            var neko = Canvas.GetLeft(this);
            Binding b = new(nameof(MyNotifyXY.X));
            b.Source = MyNotifyXY;
            b.Mode = BindingMode.TwoWay;
            this.SetBinding(Canvas.LeftProperty, b);
            neko = Canvas.GetLeft(this);
            b = new(nameof(MyNotifyXY.Y));
            b.Source = MyNotifyXY;
            b.Mode = BindingMode.TwoWay;
            this.SetBinding(Canvas.TopProperty, b);
            DragDelta += TThumb_DragDelta;
            neko = Canvas.GetLeft(this);
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not TThumb tt) { return; }
            MyNotifyXY.X = Canvas.GetLeft(tt) + e.HorizontalChange;
            MyNotifyXY.Y = Canvas.GetTop(tt) + e.VerticalChange;
        }

        private void SetTemplate()
        {
            FrameworkElementFactory panelFactory = new(typeof(Grid));
            FrameworkElementFactory textblockFactory = new(typeof(TextBlock));
            textblockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            textblockFactory.SetValue(TextBlock.PaddingProperty, new Thickness(4));
            Binding b = new(nameof(Text));
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
            return $"Name x{MyNotifyXY.X} y{MyNotifyXY.Y}";
        }
    }
    public class Matome
    {
        public List<TThumb> ThumbList;
        public PointCollection PointCollection;
        public List<int> IntList;
        public List<Point> PointList;

        public Matome(List<TThumb> thumbList, PointCollection pointCollection, List<int> intList, List<Point> pointList)
        {
            ThumbList = thumbList;
            PointCollection = pointCollection;
            IntList = intList;
            PointList = pointList;
            PointCollection.Changed += PointCollection_Changed;
        }

        private void PointCollection_Changed(object? sender, EventArgs e)
        {
            var neko = 0;
        }
    }
    public class NotifyXY : INotifyPropertyChanged
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
        public NotifyXY(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public class ObservablePoints : ObservableCollection<NotifyXY>
    {
        public ObservablePoints() { }
    }
    public class Data
    {
        public List<NotifyXY> NPoints { get; set; }
        public PointCollection PC { get; set; }
        public Data(List<NotifyXY> nPoints, PointCollection pC)
        {
            NPoints = nPoints;
            PC = pC;
        }
    }

    #region netyori
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Vertex : ViewModelBase
    {
        private Point point;
        public Point Point
        {
            get { return point; }
            set { point = value; OnPropertyChanged(); }
        }
    }
    public class ViewModel : ViewModelBase
    {


        public ObservableCollection<Vertex> Vertices { get; } = new();
        private void VerticesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged += VertexPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (INotifyPropertyChanged? item in e.OldItems.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged -= VertexPropertyChanged;
                }              
            }
            OnPropertyChanged(nameof(Vertices));
        }

    

        public ViewModel()
        {
            Vertices.CollectionChanged += VerticesCollectionChanged;
        }

        private void VertexPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Vertices));
        }
    }

    public class VerticesConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vertices = value as IEnumerable<Vertex>;
            //return vertices != null
            //    ? new PointCollection(vertices.Select(v => v.Point))
            //    : null;
            return new PointCollection(vertices.Select(v => v.Point));
        }
        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    #endregion netyori
}

