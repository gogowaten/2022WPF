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

namespace _20220118
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
            //RootCanvas
            //┣GroupCanvas
            //┗SurfaceCanvas
        }

    }

    public class TThumb : BaseThumb, System.ComponentModel.INotifyPropertyChanged
    {
        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnpropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Notify

        #region 保存するプロパティ、依存プロパティ
        public TType Type;//外から変更できないようにしたけどわからん
        public Rect Bounds
        {
            get => bounds;
            private set
            {
                if (value != bounds)
                {
                    bounds = value; OnpropertyChanged();
                    X = value.X;
                    Y = value.Y;
                    TTWidth = value.Width;
                    TTHeight = value.Height;
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

        public double X { get => x; set { if (value != x) { x = value; OnpropertyChanged(); Bounds = new Rect(value, y, width, height); } } }
        public double Y { get => y; set { if (value != y) { y = value; OnpropertyChanged(); Bounds = new Rect(x, value, width, height); } } }
        public int Z { get => z; set { if (value != z) { z = value; OnpropertyChanged(); } } }
        public double TTWidth { get => width; set { if (value != width) { width = value; OnpropertyChanged(); Bounds = new Rect(x, y, value, height); } } }
        //public double Width { get => width; set { if (value != width) { width = value; OnpropertyChanged(); } } }
        public double TTHeight { get => height; set { if (value != height) { height = value; OnpropertyChanged(); Bounds = new Rect(x, y, width, value); } } }
        public Visibility VisibleFrame { get => visibleFrame; set { if (value != visibleFrame) { visibleFrame = value; OnpropertyChanged(); } } }
        public string TTName { get => name; set { if (value != name) { name = value; OnpropertyChanged(); } } }

        #endregion プロパティ

        #region フィールド
        public TThumb GroupThumb;//実質ParentThumb
        public TThumb GroupTopThumb;//実際に移動させるThumbになる

        #endregion フィールド

        public TThumb()
        {
            DataContext = this;
            MySetTwoWayModeBinding(Thumb.WidthProperty, nameof(TTWidth));
            MySetTwoWayModeBinding(Thumb.HeightProperty, nameof(TTHeight));
            MySetTwoWayModeBinding(Thumb.NameProperty, nameof(TTName));
            MySetTwoWayModeBinding(Canvas.LeftProperty, nameof(X));
            MySetTwoWayModeBinding(Canvas.TopProperty, nameof(Y));

            this.Focusable = true;

            this.DragDelta += TThumbDragDelta;
            //this.GotFocus += TThumb_GotFocus;
            //this.MovableThumb = this;

            //枠表示用Rectangle
            Rectangle rectangle = new();
            //rectangle.Width = 200;
            rectangle.SetBinding(Rectangle.WidthProperty, nameof(TTWidth));
            rectangle.SetBinding(Rectangle.HeightProperty, nameof(TTHeight));
            rectangle.Stroke = Brushes.MediumOrchid;
            rectangle.StrokeThickness = 4;
            SurfaceCanvas.Children.Add(rectangle);
            rectangle.SetBinding(VisibilityProperty, nameof(VisibleFrame));


            void MySetTwoWayModeBinding(DependencyProperty property, string path)
            {
                Binding b = new(path);
                b.Mode = BindingMode.TwoWay;
                this.SetBinding(property, b);
            }


        }
        #region イベント
        //子要素の移動終了時に自身のサイズ変更、一番上までサイズ変更？
        public void TThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            //TThumb currentT = (TThumb)sender;
            //if (currentT.ParentThumb == null) { return; }
            //TThumb cuParent = currentT.ParentThumb;
            //if (cuParent.IsGroup == false || cuParent.IsEditInsideGroup == false) { return; }

            ////currentT.TempRect.Union(new Rect(currentT.MyData.X, currentT.MyData.Y, currentT.Width, currentT.Height));

            //Rect temp = currentT.MyData.Bounds;// new(data.X, data.Y, data.Width, data.Height);
            //foreach (TThumb? item in cuParent.Children)
            //{
            //    temp.Union(item.MyData.Bounds);
            //}


            ////Parentの座標変更
            ////Layerだった場合は移動しない、サイズだけ変更
            //if (cuParent.MyData.Type == TType.Layer)
            //{
            //    double xOffset = 0;
            //    double yOffset = 0;
            //    xOffset = -temp.X;
            //    cuParent.ChildrenSource.ForEach(i => i.MyData.X += xOffset);
            //    yOffset = -temp.Y;
            //    cuParent.ChildrenSource.ForEach(i => i.MyData.Y += yOffset);

            //    //Layerはサイズだけ変更
            //    cuParent.MyData.Width = temp.Width + temp.X + xOffset;
            //    cuParent.MyData.Height = temp.Height + temp.Y + yOffset;

            //}
            //else
            //{
            //    //Parentのサイズと位置変更
            //    cuParent.MyData.X += temp.X;
            //    cuParent.MyData.Y += temp.Y;
            //    cuParent.MyData.Width = temp.Width;
            //    cuParent.MyData.Height = temp.Height;

            //    //Childrenの座標変更
            //    foreach (TThumb item in cuParent.ChildrenSource)
            //    {
            //        item.MyData.X -= temp.X;
            //        item.MyData.Y -= temp.Y;
            //    }
            //}

        }

        //子要素の移動開始時、移動しない要素全体のRect取得しておく
        //protected void TThumb_DragStarted(object sender, DragStartedEventArgs e)
        //{

        //}

        public void TThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            X += e.HorizontalChange;
            Y += e.VerticalChange;
        }

        protected void ElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is not FrameworkElement element) { return; }
            TTWidth = element.ActualWidth;
            TTHeight = element.ActualHeight;
        }

        #endregion イベント

    }


    public class Layer : TThumb
    {
        public List<TThumb> MyThumbs = new();
        public Layer()
        {
            Type = TType.Layer;
            DragDelta -= TThumbDragDelta;

        }
        public void AddThumb(TThumb thumb)
        {
            MyThumbs.Add(thumb);
            RootCanvas.Children.Add(thumb);
        }
    }
    public class TTTextBlock : TThumb
    {
        private string text;
        

        public string Text { get => text; set { if (value != text) { text = value; OnpropertyChanged(); } } }

        public TTTextBlock(string text)
        {
            Type = TType.TextBlock;
            TextBlock tb = new() { Text = text, FontSize = 30 };
            tb.SizeChanged += ElementSizeChanged;
            tb.SetBinding(TextBlock.TextProperty, new Binding($"{nameof(Text)}"));
            this.X = 10; this.Y = 10; this.TTName = "youso1";
            this.RootCanvas.Children.Add(tb);
        }
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
