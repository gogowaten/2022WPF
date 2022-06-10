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


//C#wpfポリラインポイントに四角形を描画する方法 - 初心者向けチュートリアル
//https://tutorialmore.com/questions-2546680.htm

//使い方がわからんけど、PointCollectionの中のPointの更新を通知しているのが参考になるかも？


namespace _20220610_PointCollectionBindingできるかも_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var vertex = (Vertex)((Thumb)sender).DataContext;
            vertex.Point = new Point(
                vertex.Point.X + e.HorizontalChange,
                vertex.Point.Y + e.VerticalChange);
        }
    }

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
        public ViewModel()
        {
            Vertices.CollectionChanged += VerticesCollectionChanged;
        }
        public ObservableCollection<Vertex> Vertices { get; }
            = new ObservableCollection<Vertex>();
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
                foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged -= VertexPropertyChanged;
                }
            }
            OnPropertyChanged(nameof(Vertices));
        }
        private void VertexPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Vertices));
        }
    }

    public class VerticesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var vertices = value as IEnumerable<Vertex>;
            //return vertices != null
            //    ? new PointCollection(vertices.Select(v => v.Point))
            //    : null;
           
            var vertices = value as IEnumerable<Vertex>;
            var ss = new PointCollection(vertices.Select(e => e.Point));
            return ss;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


}
