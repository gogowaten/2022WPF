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


//矩形表示に使う要素をPathからRectangleに変更
//MultiBindingを使う必要がなくなって簡潔になった

namespace _20220110_RangeThumb2_RectangleThumb
{
    public class ThumbCanvas : Thumb
    {
        private Canvas RootCanvas;
        private static string ROOT_NAME = "root";
        public ThumbCanvas()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), ROOT_NAME);
            this.Template = template;
            ApplyTemplate();

            RootCanvas = this.Template.FindName(ROOT_NAME, this) as Canvas;
            RootCanvas.Background = Brushes.Transparent;

        }
        public void AddChildren(FrameworkElement element)
        {
            RootCanvas.Children.Add(element);
        }
    }
    class RangeThumb : ThumbCanvas, System.ComponentModel.INotifyPropertyChanged
    {
        Rectangle MyRectangle = new();
        private readonly Thumb TTopLeft = new() { Width = 10, Height = 10 };
        private readonly Thumb TTopRight = new() { Width = 10, Height = 10 };
        private readonly Thumb TBottomRight = new() { Width = 10, Height = 10 };
        private readonly Thumb TBottomLeft = new() { Width = 10, Height = 10 };

        private double myLeft;
        private double myTop;
        private double myWidth;
        private double myHeight;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double MyLeft { get => myLeft; set { myLeft = value; OnPropertyChanged(); } }
        public double MyTop { get => myTop; set { myTop = value; OnPropertyChanged(); } }
        public double MyWidth { get => myWidth; set { if (value == myWidth) { return; } myWidth = value; OnPropertyChanged(); } }
        public double MyHeight { get => myHeight; set { if (value == myHeight) { return; } myHeight = value; OnPropertyChanged(); } }
        public RangeThumb()
        {
            AddChildren(MyRectangle);

            //塗りつぶし
            //MyRectangle.Fill = Brushes.Red;

            //枠だけ表示、ドラッグ移動できるように透明色で塗りつぶす
            MyRectangle.Fill = Brushes.Transparent;
            MyRectangle.Stroke = Brushes.Red;

            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            this.DragDelta += RangeThumb_DragDelta;
            MyLeft = 20;
            MyTop = 20;
            MyWidth = 100;
            MyHeight = 100;

            Binding bLeft = new(nameof(MyLeft));
            bLeft.Source = this;
            Binding bTop = new(nameof(MyTop));
            bTop.Source = this;
            Binding bWidth = new(nameof(MyWidth));
            bWidth.Source = this;
            Binding bHeight = new(nameof(MyHeight));
            bHeight.Source = this;

            this.SetBinding(Canvas.LeftProperty, bLeft);
            this.SetBinding(Canvas.TopProperty, bTop);
            MyRectangle.SetBinding(WidthProperty, bWidth);
            MyRectangle.SetBinding(HeightProperty, bHeight);

            TTopLeft.DragDelta += TTopLeft_DragDelta;
            TTopRight.DragDelta += TTopRight_DragDelta;
            TBottomRight.DragDelta += TBottomRight_DragDelta;
            TBottomLeft.DragDelta += TBottomLeft_DragDelta;

            AddChildren(TTopLeft);
            AddChildren(TTopRight);
            AddChildren(TBottomLeft);
            AddChildren(TBottomRight);

            //左上
            //右上
            TTopRight.SetBinding(Canvas.LeftProperty, bWidth);
            //左下
            TBottomLeft.SetBinding(Canvas.TopProperty, bHeight);
            //右下
            TBottomRight.SetBinding(Canvas.LeftProperty, bWidth);
            TBottomRight.SetBinding(Canvas.TopProperty, bHeight);
        }

        private void RangeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //イベントの発生源が違うときは移動させない
            if (e.OriginalSource != e.Source) { return; }

            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;
        }

        private void TBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double width = myWidth - e.HorizontalChange;
            if (width > 0) { MyWidth = width; MyLeft += e.HorizontalChange; }
            double height = myHeight + e.VerticalChange;
            if (height > 0) { MyHeight = height; }
        }

        private void TBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double width = myWidth + e.HorizontalChange;
            if (width > 0) { MyWidth = width; }
            double height = myHeight + e.VerticalChange;
            if (height > 0) { MyHeight = height; }
        }

        private void TTopRight_DragDelta(object sender, DragDeltaEventArgs e)//右上
        {
            double width = myWidth + e.HorizontalChange;
            if (width > 0) { MyWidth = width; }
            double height = myHeight - e.VerticalChange;
            if (height > 0) { MyHeight = height; MyTop += e.VerticalChange; }
        }

        private void TTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double width = myWidth - e.HorizontalChange;
            if (width > 0) { MyWidth = width; MyLeft += e.HorizontalChange; }
            double height = myHeight - e.VerticalChange;
            if (height > 0) { MyHeight = height; MyTop += e.VerticalChange; }
        }

        public Rect GetRect()
        {
            return new Rect(myLeft, myTop, myWidth, myHeight);
        }
    }
}
