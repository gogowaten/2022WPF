using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
//using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;

using System.Windows.Data;
using System.Configuration;
using System.Windows.Markup;
using System.Collections.Specialized;

namespace _20221224
{
    public class TThumb : Thumb
    {
        #region 依存プロパティ

        public double MyLeft
        {
            get { return (double)GetValue(MyLeftProperty); }
            set { SetValue(MyLeftProperty, value); }
        }
        public static readonly DependencyProperty MyLeftProperty =
            DependencyProperty.Register(nameof(MyLeft), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));

        public double MyTop
        {
            get { return (double)GetValue(MyTopProperty); }
            set { SetValue(MyTopProperty, value); }
        }
        public static readonly DependencyProperty MyTopProperty =
            DependencyProperty.Register(nameof(MyTop), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));


        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(TTTextBlock), new PropertyMetadata(""));
        #endregion 依存プロパティ

        public TTGroup? TTParentGroup { get; set; }
        public TThumb()
        {
            SetBinding(Canvas.LeftProperty, nameof(MyLeft));
            SetBinding(Canvas.TopProperty, nameof(MyTop));
        }
        public override string ToString()
        {
            //return base.ToString();
            return Name;
        }
    }
    public class TTTextBlock : TThumb
    {
        private readonly string TTNAME = "content";
        public TextBlock? Content { get; set; }
        public TTTextBlock()
        {
            DataContext = this;
            FrameworkElementFactory text = new(typeof(TextBlock), TTNAME);
            FrameworkElementFactory waku = new(typeof(Rectangle));
            FrameworkElementFactory panel = new(typeof(Grid));
            waku.SetValue(Shape.StrokeProperty, Brushes.Red);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            panel.AppendChild(text);
            panel.AppendChild(waku);
            this.Template = new() { VisualTree = panel };
            this.ApplyTemplate();
            Content = (TextBlock)this.Template.FindName(TTNAME, this);
            Content.SetBinding(TextBlock.TextProperty, nameof(MyText));

            DragDelta += TTTextBlock_DragDelta;
            DragCompleted += TTTextBlock_DragCompleted;
        }

        private void TTTextBlock_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is TThumb tt && TTParentGroup != null)
            {
                Rect rect = GetRect();
                TTParentGroup.Arrange(rect);
                //TTParentGroup.Arrange(new(100, 100, 100, 200));
                //TTParentGroup.TTParentGroup?.Arrange(new(new()));

                foreach (var item in TTParentGroup.Children)
                {

                }
            }
        }
        private Rect GetRect()
        {
            Rect rect = new();
            if (TTParentGroup != null)
            {
                double minx = double.MaxValue; double miny = double.MaxValue;
                double right = 0, bottom = 0;
                foreach (var item in TTParentGroup.Children)
                {
                    if (minx > item.MyLeft) minx = item.MyLeft;
                    if (miny > item.MyTop) miny = item.MyTop;
                    var rr = item.MyLeft + item.ActualWidth;
                    var bb = item.MyTop + item.ActualHeight;
                    if (right < rr) right = rr;
                    if (bottom < bb) bottom = bb;
                }
                rect.X = minx+TTParentGroup.MyLeft; 
                rect.Y = miny+TTParentGroup.MyTop;
                rect.Width = right; 
                rect.Height = bottom;
            }
            return rect;
        }
        private void TTTextBlock_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;

            //TTParentGroup?.Measure(new Size());
            //TTParentGroup?.ExMeasure();
            //Test(TTParentGroup);
            DependencyObject neko = VisualTreeHelper.GetParent(this);

            //if(VisualTreeHelper.GetParent(this) is ExCanvas canvas)
            //{
            //    canvas.Measure(new Size());
            //}
        }
        private void Test(TTGroup? group)
        {
            if (group == null) return;
            group.Measure(new());
            if (group.TTParentGroup != null)
            {
                Test(group.TTParentGroup);
            }
        }
    }

    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        private readonly string TTNAME = "content";

        public ItemsControl? Content { get; set; }

        public ObservableCollection<TThumb> Children { get; set; } = new();
        public TTGroup()
        {
            DataContext = this;
            Children.CollectionChanged += Children_CollectionChanged;
            SizeChanged += TTGroup_SizeChanged;

            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Rectangle.StrokeProperty, Brushes.Blue);
            waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);
            FrameworkElementFactory grid = new(typeof(Grid));
            FrameworkElementFactory ic = new(typeof(ItemsControl), TTNAME);
            //ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            FrameworkElementFactory icPanel = new(typeof(ExCanvas));
            //FrameworkElementFactory icPanel = new(typeof(Canvas));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(icPanel));
            grid.AppendChild(ic);
            grid.AppendChild(waku);
            this.Template = new() { VisualTree = grid };
            this.ApplyTemplate();
            Content = (ItemsControl)this.Template.FindName(TTNAME, this);
            Content.SetBinding(ItemsControl.ItemsSourceProperty, nameof(Children));
            Content.ApplyTemplate();


        }

        private void TTGroup_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //TTParentGroup?.Measure(ArrangeOverride(new()));
            //TTParentGroup?.Measure(new());
            if (sender is TThumb tt)
            {
                var neko = tt.DesiredSize;
                var inu = tt.ActualHeight;
                var uma = tt.ActualWidth;
                var origin = e.OriginalSource;
                var source = e.Source;
            }
        }

        public void ExMeasure()
        {
            Measure(new());
            var neko = this.RenderSize;
            //Arrange(new(new()));
            //TTParentGroup?.ExMeasure();
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is TThumb thumb) { thumb.TTParentGroup = this; }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }


    public class ExCanvas : Canvas
    {


        //protected override Size MeasureOverride(Size constraint)
        //{
        //    if (double.IsNaN(Width) && double.IsNaN(Height))
        //    {
        //        base.MeasureOverride(constraint);
        //        Size size = new();
        //        foreach (var item in InternalChildren.OfType<TThumb>())
        //        {
        //            double w = item.MyLeft + item.DesiredSize.Width;
        //            double h = item.MyTop + item.DesiredSize.Height;
        //            if (size.Width < w) { size.Width = w; }
        //            if (size.Height < h) { size.Height = h; }
        //        }
        //        //Arrange(new(size));//stack over flow
        //        return size;
        //    }
        //    else
        //    {
        //        return base.MeasureOverride(constraint);
        //    }
        //}
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            return base.ArrangeOverride(arrangeSize);
        }
    }
}
