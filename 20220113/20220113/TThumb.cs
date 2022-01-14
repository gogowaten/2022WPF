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

namespace _20220113
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
            RootCanvas = (Canvas)this.Template.FindName(ROOT_CANVAS_NAME, this);

            RootCanvas.Children.Add(GroupCanvas);//グループ内要素用
            RootCanvas.Children.Add(SurfaceCanvas);//枠表示用、最前面
        }

    }



    public class TThumb : BaseThumb
    {
        public Data MyData = new();
        public FrameworkElement? MyContentElement { get; private set; }//表示する要素用、1個だけに限定したい
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
                    foreach (var item in ChildrenSource)
                    {
                        item.DragDelta += item.TThumb_DragDelta;
                        item.DragStarted += item.TThumb_DragStarted;
                        item.DragCompleted += item.TThumb_DragCompleted;
                    }
                }
                else
                {
                    this.DragDelta += TThumb_DragDelta;
                    foreach (var item in ChildrenSource)
                    {
                        item.DragDelta -= item.TThumb_DragDelta;
                        item.DragStarted -= item.TThumb_DragStarted;
                        item.DragCompleted -= item.TThumb_DragCompleted;
                    }
                }
            }
        }

        public IReadOnlyList<TThumb> Children => ChildrenSource;
        protected List<TThumb> ChildrenSource { get; set; } = new();
        private bool isEditInsideGroup;//グループ内部の編集中、移動とかに使う、グループじゃなければtrueにしたくないけど…
        public bool IsGroup;//グループ判定、childrenが0なら必ずfalse
        //public bool IsMovable;//ドラッグ移動可能
        //private Rect TempRect = new();
        public TThumb? ParentThumb;//グループ時の親


        public TThumb()
        {
            DataContext = MyData;
            //this.SetBinding(Thumb.WidthProperty, new Binding(nameof(MyData.Width)));
            MySetTwoWayModeBinding(Thumb.WidthProperty, nameof(MyData.Width));
            MySetTwoWayModeBinding(Thumb.HeightProperty, nameof(MyData.Height));
            MySetTwoWayModeBinding(Canvas.LeftProperty, nameof(MyData.X));
            MySetTwoWayModeBinding(Canvas.TopProperty, nameof(MyData.Y));

            DragDelta += TThumb_DragDelta;

            //枠表示用Rectangle
            Rectangle rectangle = new();
            //rectangle.Width = 200;
            rectangle.SetBinding(Rectangle.WidthProperty, nameof(MyData.Width));
            rectangle.SetBinding(Rectangle.HeightProperty, nameof(MyData.Height));
            rectangle.Stroke = Brushes.MediumOrchid;
            rectangle.StrokeThickness = 4;
            SurfaceCanvas.Children.Add(rectangle);
            rectangle.SetBinding(VisibilityProperty, nameof(MyData.VisibleFrame));

        }
        private void MySetTwoWayModeBinding(DependencyProperty property, string path)
        {
            Binding b = new(path);
            b.Mode = BindingMode.TwoWay;
            this.SetBinding(property, b);
        }

        public TThumb(FrameworkElement element) : this()
        {
            RootCanvas.Children.Add(element);
            //RootCanvas.UpdateLayout();
            MyContentElement = element;
            MyContentElement.SizeChanged += MyContentElement_SizeChanged;

        }
        public TThumb(List<TThumb> thumbs) : this()
        {
            IsGroup = true;

            for (int i = 0; i < thumbs.Count; i++)
            {
                GroupCanvas.Children.Add(thumbs[i]);
                ChildrenSource.Add(thumbs[i]);
                thumbs[i].ParentThumb = this;
            }
            Loaded += (a, b) =>
            {
                Rect r = new();
                foreach (TThumb item in ChildrenSource)
                {
                    Data data = item.MyData;
                    r.Union(new Rect(data.X, data.Y, data.Width, data.Height));
                    item.DragDelta -= item.TThumb_DragDelta;
                }
                MyData.X = r.X; MyData.Y = r.Y;
                MyData.Width = r.Width; MyData.Height = r.Height;
            };
        }

        public static TThumb CreateTextBlockThumb(string text = "TextBlock", double fontSize = 10, double x = 0, double y = 0, string name = "")
        {
            TextBlock tb = new() { Text = text, FontSize = fontSize };
            TThumb t = new(tb);
            t.MyData.X = x;
            t.MyData.Y = y;
            t.Name = name;
            t.MyData.Type = TType.TextBlock;

            return t;
        }



        #region イベント
        //子要素の移動終了時に自身のサイズ変更、一番上までサイズ変更？
        protected void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            TThumb currentT = (TThumb)sender;
            if (currentT.ParentThumb == null) { return; }
            TThumb cuParent = currentT.ParentThumb;
            if (cuParent.IsGroup == false || cuParent.IsEditInsideGroup == false) { return; }

            //currentT.TempRect.Union(new Rect(currentT.MyData.X, currentT.MyData.Y, currentT.Width, currentT.Height));

            Rect temp = currentT.MyData.Bounds;// new(data.X, data.Y, data.Width, data.Height);
            foreach (TThumb? item in cuParent.Children)
            {
                temp.Union(item.MyData.Bounds);
            }


            //Parentの座標変更
            //Layerだった場合は移動しない、サイズだけ変更
            if (cuParent.MyData.Type == TType.Layer)
            {
                double xOffset = 0;
                double yOffset = 0;
                //全体Rectの座標がマイナスのときは全要素をOffsetする
                if (temp.X < 0)
                {
                    xOffset = -temp.X;
                    cuParent.ChildrenSource.ForEach(i => i.MyData.X += xOffset);
                }
                if (temp.Y < 0)
                {
                    yOffset = -temp.Y;
                    cuParent.ChildrenSource.ForEach(i => i.MyData.Y += yOffset);
                }
                //Layerはサイズだけ変更
                cuParent.MyData.Width = temp.Width + temp.X + xOffset;
                cuParent.MyData.Height = temp.Height + temp.Y + yOffset;

            }
            else
            {
                //Parentのサイズと位置変更
                cuParent.MyData.SetBounds(temp);
                //Childrenの座標変更
                foreach (TThumb item in cuParent.ChildrenSource)
                {
                    item.MyData.X -= temp.X;// TempRect.X;
                    item.MyData.Y -= temp.Y;// TempRect.Y;
                }
            }

        }

        //子要素の移動開始時、移動しない要素全体のRect取得しておく
        protected void TThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            //TThumb currentT = (TThumb)sender;
            //if (currentT.ParentThumb == null) { return; }
            //TThumb cuParent = currentT.ParentThumb;
            //if (cuParent.IsGroup == false || cuParent.IsEditInsideGroup == false) { return; }

            //TThumb t = cuParent.ChildrenSource[0];
            //if (t == currentT) { t = cuParent.ChildrenSource[1]; }

            //currentT.TempRect.X = t.MyData.X;
            //currentT.TempRect.Y = t.MyData.Y;
            //currentT.TempRect.Width = t.Width;
            //currentT.TempRect.Height = t.Height;
            //foreach (TThumb item in cuParent.ChildrenSource)
            //{
            //    if (item != currentT)
            //    {
            //        currentT.TempRect.Union(new Rect(item.MyData.X, item.MyData.Y, item.Width, item.Height));
            //    }
            //}
        }

        protected void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }

        private void MyContentElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (MyContentElement == null) { return; }
            MyData.Width = MyContentElement.ActualWidth;
            MyData.Height = MyContentElement.ActualHeight;
        }

        #endregion イベント


        public override string ToString()
        {
            //return base.ToString();
            return $"{Name}, x,y=({MyData.X}, {MyData.Y}) w,h=({MyData.Width}, {MyData.Height})";
        }
    }

    //public class TTT : TThumb
    //{
    //    void GetSize()
    //    {

    //    }

    //}





    public class Data : System.ComponentModel.INotifyPropertyChanged
    {
        public List<Data> ChildrenDatas = new();
        public TType Type;//外から変更できないようにしたけどわからん
        public Rect Bounds { get; private set; }

        private double x;
        private double y;
        private int z;
        private double width;
        private double height;
        private Visibility visibleFrame = Visibility.Visible;
        private Rect rR;
        private Rect rR1;

        public event PropertyChangedEventHandler? PropertyChanged;
        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;
        protected void OnpropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Notify

        public double X { get => x; set { if (value != x) { x = value; OnpropertyChanged(); Bounds = new Rect(value, y, width, height); } } }
        public double Y { get => y; set { if (value != y) { y = value; OnpropertyChanged(); Bounds = new Rect(x, value, width, height); } } }
        public int Z { get => z; set { if (value != z) { z = value; OnpropertyChanged(); } } }
        public double Width { get => width; set { if (value != width) { width = value; OnpropertyChanged(); Bounds = new Rect(x, y, value, height); } } }
        //public double Width { get => width; set { if (value != width) { width = value; OnpropertyChanged(); } } }
        public double Height { get => height; set { if (value != height) { height = value; OnpropertyChanged(); Bounds = new Rect(x, y, width, value); } } }
        public Visibility VisibleFrame { get => visibleFrame; set { if (value != visibleFrame) { visibleFrame = value; OnpropertyChanged(); } } }


        public void SetBounds(Rect bounds)
        {
            Bounds = bounds;
            X = bounds.X;
            Y = bounds.Y;
            Width = bounds.Width;
            Height = bounds.Height;
        }
    }

    public class LayerThumb : TThumb
    {
        public LayerThumb()
        {
            IsGroup = true;

            IsEditInsideGroup = true;

            this.DragDelta -= TThumb_DragDelta;
            this.DragStarted -= TThumb_DragStarted;
            this.DragCompleted -= TThumb_DragCompleted;

            Loaded += (a, b) =>
            {
                Rect r = new();
                foreach (TThumb item in ChildrenSource)
                {
                    Data data = item.MyData;
                    r.Union(new Rect(data.X, data.Y, data.Width, data.Height));
                    //item.DragDelta -= item.TThumb_DragDelta;
                }
                MyData.X = r.X; MyData.Y = r.Y;
                MyData.Width = r.Width; MyData.Height = r.Height;
            };
        }

        /// <summary>
        /// グループにThumbを追加
        /// </summary>
        /// <param name="thumb"></param>
        public void AddThumb(TThumb thumb)
        {
            if (IsGroup == false) { return; }
            GroupCanvas.Children.Add(thumb);

            //サイズ取得からLayerのサイズ変更まで
            thumb.UpdateLayout();//これでThumbのサイズが取得できるようになる

            //GroupCanvas.UpdateLayout();これでもいいけど
            //UpdateLayout();//これでもいいけど
            //GroupCanvas.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

            ChildrenSource.Add(thumb);
            thumb.ParentThumb = this;
            //thumb.DragDelta -= thumb.TThumb_DragDelta;

            Rect r = new(MyData.X, MyData.Y, MyData.Width, MyData.Height);
            r.Union(new Rect(thumb.MyData.X, thumb.MyData.Y, thumb.MyData.Width, thumb.MyData.Height));
            MyData.X = r.X; MyData.Y = r.Y;
            MyData.Width = r.Width; MyData.Height = r.Height;

            //
            thumb.DragStarted += TThumb_DragStarted;
            thumb.DragCompleted += TThumb_DragCompleted;
        }

    }

    public class SizeCanvas : Canvas
    {
        public SizeCanvas()
        {

        }

        private void SizeCanvas_LayoutUpdated(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public enum TType
    {
        Layer,
        Image,
        TextBox,
        TextBlock,
        PathGeometry,
    }



}