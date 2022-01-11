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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;

namespace _20220111
{
    internal class BaseThumb : Thumb
    {
        protected Canvas RootCanvas;
        private readonly string ROOT_CANVAS_NAME = "root";
        public Canvas GroupCanvas = new();
        public Canvas SurfaceCanvas = new();

        protected BaseThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), ROOT_CANVAS_NAME);
            this.Template = template;
            ApplyTemplate();
            RootCanvas = this.Template.FindName(ROOT_CANVAS_NAME, this) as Canvas;


            RootCanvas.Children.Add(GroupCanvas);
            RootCanvas.Children.Add(SurfaceCanvas);
        }
        public BaseThumb(FrameworkElement element) : this()
        {
            AddElement(element);
        }
        public void AddElement(FrameworkElement element)
        {
            RootCanvas.Children.Add(element);
            //this.Width = element.ActualWidth;
            //this.Height = element.ActualHeight;
        }
    }
    class TThumb : BaseThumb
    {
        public Data MyData = new();
        public FrameworkElement MyContentElement { get; private set; }
        public List<TThumb> Children;

        public TThumb()
        {
            DataContext = MyData;
            this.SetBinding(Thumb.WidthProperty, new Binding(nameof(MyData.Width)));
            this.SetBinding(Thumb.HeightProperty, new Binding(nameof(MyData.Height)));
        }


        public TThumb(FrameworkElement element) : this()
        {

            AddElement(element);//この時点ではサイズが取得できないのでLoadedイベントで
            MyContentElement = element;
            Loaded += (a, b) =>
            {
                MyData.Width = MyContentElement.ActualWidth;
                MyData.Height = MyContentElement.ActualHeight;
            };
        }
        public TThumb(List<TThumb> thumbs) : this()
        {
            Children = new();
            for (int i = 0; i < thumbs.Count; i++)
            {
                GroupCanvas.Children.Add(thumbs[i]);
                Children.Add(thumbs[i]);
            }
            Loaded += (a, b) =>
            {
                Rect r = new();
                foreach (var item in Children)
                {
                    Data data = item.MyData;
                    r.Union(new Rect(data.X, data.Y, data.Width, data.Height));
                }
                MyData.X = r.X; MyData.Y = r.Y;
                MyData.Width = r.Width; MyData.Height = r.Height;
            };
        }

        public void GetSize()
        {
            var aw = MyContentElement.ActualWidth;
            var ah = MyContentElement.ActualHeight;
            Transform rt = MyContentElement.RenderTransform;
            Rect tfb = rt.TransformBounds(new Rect(0, 0, aw, ah));
            var md = MyData;
            var mdw = MyData.Width;
            var mcew = MyContentElement.Width;
            var tw = this.Width;
        }
    }
    public class Data : System.ComponentModel.INotifyPropertyChanged
    {
        private double x;
        private double y;
        private int z;
        private double width;
        private double height;
        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnpropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Notify

        public double X { get => x; set { if (value != x) { x = value; OnpropertyChanged(); } } }
        public double Y { get => y; set { if (value != y) { y = value; OnpropertyChanged(); } } }
        public int Z { get => z; set { if (value != z) { z = value; OnpropertyChanged(); } } }
        public double Width { get => width; set { if (value != width) { width = value; OnpropertyChanged(); } } }
        //public double Width { get => width; set { if (value != width) { width = value; OnpropertyChanged(); } } }
        public double Height { get => height; set { if (value != height) { height = value; OnpropertyChanged(); } } }
    }


}
