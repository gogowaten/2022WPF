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
    public abstract class BaseThumb : Thumb
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
    public class TThumb : BaseThumb
    {
        public Data MyData = new();
        public FrameworkElement MyContentElement { get; private set; }
        public bool IsEditInsideGroup//グループ内での移動
        {
            get => isEditInsideGroup;
            set
            {
                if (IsGroup == false) { return; }
                isEditInsideGroup = value;
                if (value)
                {
                    this.DragDelta -= TThumb_DragDelta;
                    foreach (var item in Children)
                    {
                        item.DragDelta += item.TThumb_DragDelta;
                        item.DragStarted += item.TThumb_DragStarted;
                        item.DragCompleted += item.TThumb_DragCompleted;
                    }
                }
                else
                {
                    this.DragDelta += TThumb_DragDelta;
                    foreach (var item in Children)
                    {
                        item.DragDelta -= item.TThumb_DragDelta;
                        item.DragStarted -= item.TThumb_DragStarted;
                        item.DragCompleted -= item.TThumb_DragCompleted;
                    }
                }
            }
        }

        public List<TThumb> Children;
        private bool isEditInsideGroup;//グループ内部の編集中、移動とかに使う、グループじゃなければtrueにしたくないけど…
        public bool IsGroup;//グループ判定、childrenが0なら必ずfalse
        public bool IsMovable;//ドラッグ移動可能
        private Rect DragTempRect = new();
        public TThumb ParentThumb;//グループ時の親

        public TThumb()
        {
            DataContext = MyData;
            //this.SetBinding(Thumb.WidthProperty, new Binding(nameof(MyData.Width)));
            MySetTwoWayModeBinding(Thumb.WidthProperty, nameof(MyData.Width));
            MySetTwoWayModeBinding(Thumb.HeightProperty, nameof(MyData.Height));
            MySetTwoWayModeBinding(Canvas.LeftProperty, nameof(MyData.X));
            MySetTwoWayModeBinding(Canvas.TopProperty, nameof(MyData.Y));

            DragDelta += TThumb_DragDelta;
        }
        private void MySetTwoWayModeBinding(DependencyProperty property, string path)
        {
            Binding b = new(path);
            b.Mode = BindingMode.TwoWay;
            this.SetBinding(property, b);
        }

        private void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (ParentThumb == null) { return; }
            if (ParentThumb.IsGroup == false || ParentThumb.IsEditInsideGroup == false) { return; }
            DragTempRect.Union(new Rect(MyData.X, MyData.Y, Width, Height));
            //Parentの座標変更
            ParentThumb.MyData.X += DragTempRect.X;
            ParentThumb.MyData.Y += DragTempRect.Y;
            ParentThumb.MyData.Width = DragTempRect.Width;
            ParentThumb.MyData.Height = DragTempRect.Height;
            //Childrenの座標変更
            foreach (TThumb item in ParentThumb.Children)
            {
                item.MyData.X -= DragTempRect.X;
                item.MyData.Y -= DragTempRect.Y;
            }

        }

        private void TThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (ParentThumb == null) { return; }
            if (ParentThumb.IsGroup == false || ParentThumb.IsEditInsideGroup == false) { return; }

            TThumb t = ParentThumb.Children[0];
            if (t == this) { t = ParentThumb.Children[1]; }
            DragTempRect.X = t.MyData.X;
            DragTempRect.Y = t.MyData.Y;
            DragTempRect.Width = t.Width;
            DragTempRect.Height = t.Height;
            foreach (TThumb item in ParentThumb.Children)
            {
                if (item != this)
                {
                    DragTempRect.Union(new Rect(item.MyData.X, item.MyData.Y, item.Width, item.Height));
                }
            }
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }

        private void MyContentElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyData.Width = MyContentElement.ActualWidth;
            MyData.Height = MyContentElement.ActualHeight;
        }

        public TThumb(FrameworkElement element) : this()
        {
            AddElement(element);
            MyContentElement = element;
            MyContentElement.SizeChanged += MyContentElement_SizeChanged;

        }
        public TThumb(List<TThumb> thumbs) : this()
        {
            IsGroup = true;
            Children = new();
            for (int i = 0; i < thumbs.Count; i++)
            {
                GroupCanvas.Children.Add(thumbs[i]);
                Children.Add(thumbs[i]);
                thumbs[i].ParentThumb = this;
            }
            Loaded += (a, b) =>
            {
                Rect r = new();
                foreach (TThumb item in Children)
                {
                    Data data = item.MyData;
                    r.Union(new Rect(data.X, data.Y, data.Width, data.Height));
                    item.DragDelta -= item.TThumb_DragDelta;
                }
                MyData.X = r.X; MyData.Y = r.Y;
                MyData.Width = r.Width; MyData.Height = r.Height;
            };
        }

        //public void GetSize()
        //{
        //    var aw = MyContentElement.ActualWidth;
        //    var ah = MyContentElement.ActualHeight;
        //    Transform rt = MyContentElement.RenderTransform;
        //    Rect tfb = rt.TransformBounds(new Rect(0, 0, aw, ah));
        //    var md = MyData;
        //    var mdw = MyData.Width;
        //    var mcew = MyContentElement.Width;
        //    var tw = this.Width;
        //}
        public override string ToString()
        {
            //return base.ToString();
            return $"x,y=({MyData.X}, {MyData.Y}) w,h=({MyData.Width}, {MyData.Height})";
        }
    }

    public class TTT : TThumb
    {
        void GetSize()
        {

        }

    }





    public class Data : System.ComponentModel.INotifyPropertyChanged
    {
        public List<Data> ChildrenDatas = new();

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
