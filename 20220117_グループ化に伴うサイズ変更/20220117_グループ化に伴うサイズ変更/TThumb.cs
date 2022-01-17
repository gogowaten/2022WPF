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

namespace _20220117_グループ化に伴うサイズ変更
{
    public abstract class BaseThumb : Thumb
    {
        protected Canvas RootCanvas;
        private readonly string ROOT_CANVAS_NAME = "root";
        protected Canvas GroupCanvas = new();
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
        public Data MyData { get; set; } = new();
        public FrameworkElement? MyContentElement { get; private set; }//表示する要素用、1個だけに限定したい
        public bool IsEditInsideGroup//グループ内での移動
        {
            get => isEditInsideGroup;
            set
            {
                if (value == isEditInsideGroup) { return; }
                if (IsGroup == false) { return; }
                isEditInsideGroup = value;
                if (value)
                {
                    this.DragDelta -= TThumb_DragDelta;
                    foreach (var item in ChildrenSource)
                    {
                        item.DragDelta += item.TThumb_DragDelta;
                        //item.DragStarted += item.TThumb_DragStarted;
                        item.DragCompleted += item.TThumb_DragCompleted;
                    }
                }
                else
                {
                    this.DragDelta += TThumb_DragDelta;
                    foreach (var item in ChildrenSource)
                    {
                        item.DragDelta -= item.TThumb_DragDelta;
                        //item.DragStarted -= item.TThumb_DragStarted;
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
        public TThumb? MovableThumb { get; set; }//一番上のグループ、通常動かすThumb、なければ自分
        public TThumb? FocusTT;

        public TThumb()
        {
            DataContext = MyData;
            //this.SetBinding(Thumb.WidthProperty, new Binding(nameof(MyData.Width)));
            MySetTwoWayModeBinding(Thumb.WidthProperty, nameof(MyData.Width));
            MySetTwoWayModeBinding(Thumb.HeightProperty, nameof(MyData.Height));
            MySetTwoWayModeBinding(Thumb.NameProperty, nameof(MyData.Name));
            MySetTwoWayModeBinding(Canvas.LeftProperty, nameof(MyData.X));
            MySetTwoWayModeBinding(Canvas.TopProperty, nameof(MyData.Y));

            this.Focusable = true;
            this.DragDelta += TThumb_DragDelta;
            this.GotFocus += TThumb_GotFocus;
            this.MovableThumb = this;

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

        protected void TThumb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.ParentThumb != null)
            {
                this.ParentThumb.FocusTT = this;
                //TThumb tt = GetFocusGroupThumb(this);
                //tt.Focus();
            }
        }
        protected TThumb GetFocusGroupThumb(TThumb thumb)
        {
            if (thumb.ParentThumb == null) { return thumb; }
            TThumb pa = thumb.ParentThumb;
            TThumb tt = thumb;
            if (pa.IsGroup && pa.MyData.Type != TType.Layer)
            {
                tt = GetFocusGroupThumb(pa);
            }
            return tt;
        }
        public void ReplaceMovableThumb(TThumb thumb, TThumb movable)
        {
            thumb.MovableThumb = movable;
            if (thumb.ChildrenSource.Count == 0) { return; }
            foreach (TThumb? item in thumb.ChildrenSource)
            {
                item.ReplaceMovableThumb(item, movable);
            }
          
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


        public void AddThumb(TThumb thumb, int zIndex = -1)
        {
            if (IsGroup == false) { return; }
            GroupCanvas.Children.Add(thumb);
            thumb.ParentThumb = this;


            //ZIndexの位置にThumb追加、指定がなければ末尾に追加
            if (zIndex == -1)
            {
                thumb.MyData.Z = ChildrenSource.Count;
                ChildrenSource.Add(thumb);
            }
            else
            {
                thumb.MyData.Z = zIndex;
                ChildrenSource.Insert(zIndex, thumb);
            }
            Panel.SetZIndex(thumb, thumb.MyData.Z);

            //サイズ取得からLayerのサイズ変更まで
            thumb.UpdateLayout();//これでThumbのサイズが取得できるようになる

            //GroupCanvas.UpdateLayout();これでもいいけど
            //UpdateLayout();//これでもいいけど
            //GroupCanvas.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

            Rect r = new(MyData.X, MyData.Y, MyData.Width, MyData.Height);
            r.Union(new Rect(thumb.MyData.X, thumb.MyData.Y, thumb.MyData.Width, thumb.MyData.Height));
            MyData.X = r.X; MyData.Y = r.Y;
            MyData.Width = r.Width; MyData.Height = r.Height;

            if (this.MyData.Type == TType.Layer)
            {
                thumb.DragCompleted += thumb.TThumb_DragCompleted;
            }



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
        public void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
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
                xOffset = -temp.X;
                cuParent.ChildrenSource.ForEach(i => i.MyData.X += xOffset);
                yOffset = -temp.Y;
                cuParent.ChildrenSource.ForEach(i => i.MyData.Y += yOffset);

                //Layerはサイズだけ変更
                cuParent.MyData.Width = temp.Width + temp.X + xOffset;
                cuParent.MyData.Height = temp.Height + temp.Y + yOffset;

            }
            else
            {
                //Parentのサイズと位置変更
                cuParent.MyData.X += temp.X;
                cuParent.MyData.Y += temp.Y;
                cuParent.MyData.Width = temp.Width;
                cuParent.MyData.Height = temp.Height;

                //Childrenの座標変更
                foreach (TThumb item in cuParent.ChildrenSource)
                {
                    item.MyData.X -= temp.X;
                    item.MyData.Y -= temp.Y;
                }
            }

        }

        //子要素の移動開始時、移動しない要素全体のRect取得しておく
        //protected void TThumb_DragStarted(object sender, DragStartedEventArgs e)
        //{

        //}

        public void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
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
            return $"{Name}, x,y,z=({MyData.X}, {MyData.Y}, {MyData.Z}) w,h=({MyData.Width}, {MyData.Height})";
        }
    }

  




    public class Data : System.ComponentModel.INotifyPropertyChanged
    {
        public List<Data> ChildrenDatas = new();
        public TType Type;//外から変更できないようにしたけどわからん
        public Rect Bounds
        {
            get => bounds;
            private set
            {
                if (value != bounds)
                {
                    bounds = value; OnpropertyChanged();
                    SetBounds(value);
                }
            }
        }

        private double x;
        private double y;
        private int z;
        private double width;
        private double height;
        private Visibility visibleFrame = Visibility.Visible;
        private string name = "";
        private Rect bounds;

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
        public string Name { get => name; set { if (value != name) { name = value; OnpropertyChanged(); } } }

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
            this.MyData.Type = TType.Layer;
            IsEditInsideGroup = true;

            this.Focusable = false;
            this.DragDelta -= TThumb_DragDelta;
            this.DragCompleted -= TThumb_DragCompleted;
            this.GotFocus -= TThumb_GotFocus;

            Loaded += (a, b) =>
            {
                Rect r = new();
                foreach (TThumb item in ChildrenSource)
                {
                    Data data = item.MyData;
                    r.Union(new Rect(data.X, data.Y, data.Width, data.Height));
                }
                MyData.X = r.X; MyData.Y = r.Y;
                MyData.Width = r.Width; MyData.Height = r.Height;
            };
        }


        public TThumb ToGroup(List<TThumb> thumbs, string groupName = "")
        {
            if (thumbs.Count < 2) { throw new ArgumentException("Thumb数が2未満"); }
            //グループThumb作成
            TThumb group = new();
            group.IsGroup = true;
            group.ParentThumb = this;
            group.Name = groupName;
            group.MyData.Type = TType.Group;
            group.DragCompleted += TThumb_DragCompleted;

            //ZIndex
            int maxZ = thumbs.Max(a => a.MyData.Z);
            int minZ = thumbs.Min(a => a.MyData.Z);
            //追加
            this.ChildrenSource.Insert(maxZ, group);
            this.GroupCanvas.Children.Add(group);

            //要素群をZIndex順に並べ替えたリスト作成
            List<TThumb>? items = thumbs.OrderBy(i => i.MyData.Z).ToList();
            //要素群を元の親から外してから、グループThumbの子要素にする
            for (int i = 0; i < items.Count; i++)
            {
                TThumb tt = items[i];
                //Layerから外す
                this.ChildrenSource.Remove(tt);
                this.GroupCanvas.Children.Remove(tt);

                //グループThumbに追加
                tt.ParentThumb = group;
                tt.MyData.Z = i;
                group.AddThumb(tt);

                //ドラッグイベント
                tt.DragDelta -= tt.TThumb_DragDelta;
                tt.DragCompleted -= tt.TThumb_DragCompleted;
                ////フォーカスしないようにする
                //tt.Focusable = false;

                //下にある全ての要素のMovableThumbの書き換え
                tt.MovableThumb = group;
                tt.ReplaceMovableThumb(tt, group);
            }

            //グループThumb
            //ZIndexを指定する、前に詰める
            for (int i = minZ; i < this.ChildrenSource.Count; i++)
            {
                this.ChildrenSource[i].MyData.Z = i;
                Panel.SetZIndex(this.ChildrenSource[i], i);
            }
            //サイズと座標
            Rect rect = items[0].MyData.Bounds;
            for (int i = 1; i < items.Count; i++)
            {
                rect.Union(items[i].MyData.Bounds);
            }
            group.MyData.SetBounds(rect);

            //要素群の座標調整
            foreach (var item in items)
            {
                item.MyData.X -= rect.X;
                item.MyData.Y -= rect.Y;
            }
            return group;
        }

        //private static bool IsParentEqual(List<TThumb> items)
        //{
        //    TThumb? parent = items[0].ParentThumb;
        //    foreach (var item in items)
        //    {
        //        if (parent != item.ParentThumb)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
    }

 

    public enum TType
    {
        Layer = 1,
        Group,
        Image,
        TextBox,
        TextBlock,
        PathGeometry,
    }

}
