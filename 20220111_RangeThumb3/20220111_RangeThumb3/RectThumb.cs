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

//Rectangleの見た目の変更をできるようにした
//MyFill            塗りつぶしの色
//MyStroke          枠の色
//MyStrokeThickness 枠の太さ
//MyHandleSize      つまみの大きさ

namespace _20220111_RangeThumb3
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

    public class RectThumb : ThumbCanvas, System.ComponentModel.INotifyPropertyChanged
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
        private Brush myFill = Brushes.MediumAquamarine;
        private Brush myStroke;
        private double myHandleSize = 10;
        private double myStrokeThickness = 1.0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double MyLeft { get => myLeft; set { if (value != myLeft) { myLeft = value; OnPropertyChanged(); } } }
        public double MyTop { get => myTop; set { if (value != myTop) { myTop = value; OnPropertyChanged(); } } }
        public double MyWidth { get => myWidth; set { if (value != myWidth) { myWidth = value; OnPropertyChanged(); } } }
        public double MyHeight { get => myHeight; set { if (value != myHeight) { myHeight = value; OnPropertyChanged(); } } }

        public Brush MyFill { get => myFill; set { if (value != myFill) { myFill = value; OnPropertyChanged(); } } }
        public Brush MyStroke { get => myStroke; set { if (value != myStroke) { myStroke = value; OnPropertyChanged(); } } }
        public double MyStrokeThickness { get => myStrokeThickness; set { if (value != myStrokeThickness) { myStrokeThickness = value; OnPropertyChanged(); } } }
        public double MyHandleSize { get => myHandleSize; set { if (value != myHandleSize) { myHandleSize = value; OnPropertyChanged(); } } }

        public RectThumb()
        {
            AddChildren(MyRectangle);

            Binding bFill = new(nameof(MyFill));
            bFill.Source = this;
            MyRectangle.SetBinding(Rectangle.FillProperty, bFill);

            Binding bStroke = new(nameof(MyStroke));
            bStroke.Source = this;
            MyRectangle.SetBinding(Rectangle.StrokeProperty, bStroke);

            Binding bStrokeThickness = new(nameof(MyStrokeThickness));
            bStrokeThickness.Source = this;
            MyRectangle.SetBinding(Rectangle.StrokeThicknessProperty, bStrokeThickness);


            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            this.DragDelta += SThumb_DragDelta;
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

            Binding bHandleSize = new(nameof(MyHandleSize));
            bHandleSize.Source = this;
            //左上
            TTopLeft.SetBinding(Thumb.WidthProperty, bHandleSize);
            TTopLeft.SetBinding(Thumb.HeightProperty, bHandleSize);
            //右上
            TTopRight.SetBinding(Thumb.WidthProperty, bHandleSize);
            TTopRight.SetBinding(Thumb.HeightProperty, bHandleSize);
            TTopRight.SetBinding(Canvas.LeftProperty, bWidth);
            //左下
            TBottomLeft.SetBinding(Thumb.WidthProperty, bHandleSize);
            TBottomLeft.SetBinding(Thumb.HeightProperty, bHandleSize);
            TBottomLeft.SetBinding(Canvas.TopProperty, bHeight);
            //右下
            TBottomRight.SetBinding(Thumb.WidthProperty, bHandleSize);
            TBottomRight.SetBinding(Thumb.HeightProperty, bHandleSize);
            TBottomRight.SetBinding(Canvas.LeftProperty, bWidth);
            TBottomRight.SetBinding(Canvas.TopProperty, bHeight);
        }

        private void SThumb_DragDelta(object sender, DragDeltaEventArgs e)
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
