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

//通知プロパティを上下左右から上下幅縦に変更して完結にした

namespace _20220110_RangeThumb
{
    public class ThumbCanvas : Thumb
    {
        private Canvas RootCanvas;
        private static readonly string ROOT_NAME = "root";
        public ThumbCanvas()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), ROOT_NAME);
            this.Template = template;
            ApplyTemplate();

            RootCanvas = this.Template.FindName(ROOT_NAME, this) as Canvas;
            RootCanvas.Background = Brushes.Transparent;
            //RootCanvas.Background = Brushes.MediumAquamarine;

        }
        public void AddChildren(FrameworkElement element)
        {
            RootCanvas.Children.Add(element);
        }
    }

    class RangeThumb : ThumbCanvas, System.ComponentModel.INotifyPropertyChanged
    {
        Path MyPath = new();
        private readonly Thumb TTopLeft = new() { Width = 10, Height = 10, Name = "左上" };
        private readonly Thumb TTopRight = new() { Width = 10, Height = 10, Name = "右上" };
        private readonly Thumb TBottomRight = new() { Width = 10, Height = 10, Name = "左下" };
        private readonly Thumb TBottomLeft = new() { Width = 10, Height = 10, Name = "右下" };

        private double myLeft = 0;
        private double myTop = 0;
        private double myWidth = 100;
        private double myHeight = 100;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double MyLeft { get => myLeft; set { myLeft = value; OnPropertyChanged(); } }
        public double MyTop { get => myTop; set { myTop = value; OnPropertyChanged(); } }
        public double MyWidth
        {
            get => myWidth;
            set { if (value == myWidth) { return; } myWidth = value; OnPropertyChanged(); }
        }
        public double MyHeight
        {
            get => myHeight;
            set { if (value == myHeight) { return; } myHeight = value; OnPropertyChanged(); }
        }
        public RangeThumb()
        {
            AddChildren(MyPath);

            MyPath.Fill = Brushes.Red;
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            this.DragDelta += SThumb_DragDelta;
            this.Name = "範囲";

            MyLeft = 20;
            MyTop = 20;
            MyWidth = 100;
            MyHeight = 100;

            Binding bLeft = new(nameof(MyLeft));
            bLeft.Source = this;
            Binding bTop = new(nameof(MyTop));
            bTop.Source = this;
            this.SetBinding(Canvas.LeftProperty, bLeft);
            this.SetBinding(Canvas.TopProperty, bTop);

            Binding bWidth = new(nameof(MyWidth));
            bWidth.Source = this;
            Binding bHeight = new(nameof(MyHeight));
            bHeight.Source = this;
            MultiBinding mb = new();
            mb.Bindings.Add(bWidth);
            mb.Bindings.Add(bHeight);
            mb.Converter = new MyRectConverter();
            MyPath.SetBinding(Path.DataProperty, mb);

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

        private void SThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //イベントの発生源が違うときは移動させない
            if (e.OriginalSource != e.Source) { return; }

            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;
        }

        private void TBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)//左下
        {
            double width = myWidth - e.HorizontalChange;
            if (width > 0) { MyWidth = width; MyLeft += e.HorizontalChange; }
            double height = myHeight + e.VerticalChange;
            if (height > 0) { MyHeight = height; }
        }

        private void TBottomRight_DragDelta(object sender, DragDeltaEventArgs e)//右下
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

        private void TTopLeft_DragDelta(object sender, DragDeltaEventArgs e)//左上
        {
            double width = myWidth - e.HorizontalChange;
            if (width > 0) { MyWidth = width; MyLeft += e.HorizontalChange; }
            double height = myHeight - e.VerticalChange;
            if (height > 0) { MyHeight = height; MyTop += e.VerticalChange; }
        }

        public Rect GetRect()
        {
            return new Rect(new Point(MyLeft, myTop), new Size(MyWidth, MyHeight));
        }

        public override string ToString()
        {
            //return base.ToString();
            return $"{Name}, x,y={myLeft},{myTop}, w,h={myWidth},{myHeight}";
        }
    }

    public class MyRectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            double height = (double)values[1];
            return new RectangleGeometry(new Rect(new Point(0, 0), new Size(width, height)));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
