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

namespace _20221224_AutoResizeCanvasFromWeb
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
    }

    //public class AutoResizeCanvas : Panel
    //{
    //    public AutoResizeCanvas() { }

    //    //public double Left
    //    //{
    //    //    get { return (double)GetValue(LeftProperty); }
    //    //    set { SetValue(LeftProperty, value); }
    //    //}
    //    public double Left;
    //    public static double GetLeft(DependencyObject obj)
    //    {
    //        return (double)obj.GetValue(LeftProperty);
    //    }
    //    public static void SetLeft(DependencyObject obj, double value)
    //    {
    //        obj.SetValue(LeftProperty, value);
    //    }
    //    public static readonly DependencyProperty LeftProperty =
    //        DependencyProperty.Register(nameof(Left), typeof(double), typeof(AutoResizeCanvas), new FrameworkPropertyMetadata(0, 0, OnLayoutParameterChanged));
    //    private static void OnLayoutParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        while (d != null)
    //        {
    //            if (d is AutoResizeCanvas canvas)
    //            {
    //                canvas.InvalidateMeasure();
    //                return;
    //            }
    //            d = VisualTreeHelper.GetParent(d);
    //        }
    //    }

    //    public double Top;
    //    public static double GetTop(DependencyObject obj)
    //    {
    //        return (double)obj.GetValue(TopProperty);
    //    }
    //    public static void SetTop(DependencyObject obj, double value)
    //    {
    //        obj.SetValue(TopProperty, value);
    //    }
    //    //public double Top
    //    //{
    //    //    get { return (double)GetValue(TopProperty); }
    //    //    set { SetValue(TopProperty, value); }
    //    //}
    //    public static readonly DependencyProperty TopProperty =
    //        DependencyProperty.Register(nameof(Top), typeof(double), typeof(AutoResizeCanvas), new FrameworkPropertyMetadata(0.0, OnLayoutParameterChanged));

    //    public double MinimumWidth
    //    {
    //        get { return (double)GetValue(MinimumWidthProperty); }
    //        set { SetValue(MinimumWidthProperty, value); }
    //    }
    //    public static readonly DependencyProperty MinimumWidthProperty =
    //        DependencyProperty.Register(nameof(MinimumWidth), typeof(double), typeof(AutoResizeCanvas), new FrameworkPropertyMetadata(300.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

    //    public double MinimumHeight
    //    {
    //        get { return (double)GetValue(MinimumHeightProperty); }
    //        set { SetValue(MinimumHeightProperty, value); }
    //    }
    //    public static readonly DependencyProperty MinimumHeightProperty =
    //        DependencyProperty.Register(nameof(MinimumHeight), typeof(double), typeof(AutoResizeCanvas), new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsMeasure));


    //    private void GetRequestedBounts(FrameworkElement el, out Rect bounds, out Rect marginBounds)
    //    {
    //        double left = 0, top = 0;
    //        Thickness margin = new();
    //        DependencyObject content = el;
    //        if (el is ContentPresenter)
    //        {
    //            content = VisualTreeHelper.GetChild(el, 0);
    //        }
    //        if (content != null)
    //        {
    //            left = GetLeft(content);
    //            top = GetTop(content);
    //            if (content is FrameworkElement)
    //            {
    //                margin = ((FrameworkElement)content).Margin;
    //            }
    //        }
    //        if (double.IsNaN(left)) left = 0;
    //        if (double.IsNaN(top)) top = 0;
    //        Size size = el.DesiredSize;
    //        bounds = new Rect(left + margin.Left, top + margin.Top, size.Width, size.Height);
    //        marginBounds = new Rect(left, top, size.Width + margin.Left + margin.Right, size.Height + margin.Top + margin.Bottom);
    //    }
    //    protected override Size MeasureOverride(Size constraint)
    //    {
    //        Size availableSize = new(double.MaxValue, double.MaxValue);
    //        double requestedWidth = MinimumWidth;
    //        double requestedHeight = MinimumHeight;
    //        foreach (var item in InternalChildren)
    //        {
    //            if (item is FrameworkElement el)
    //            {
    //                el.Measure(availableSize);
    //                Rect bounts, margin;
    //                GetRequestedBounts(el, out bounts, out margin);
    //                requestedWidth = Math.Max(requestedWidth, margin.Right);
    //                requestedHeight = Math.Max(requestedHeight, margin.Bottom);
    //            }
    //        }
    //        return new Size(requestedWidth, requestedHeight);
    //        //return base.MeasureOverride(constraint);
    //    }
    //    protected override Size ArrangeOverride(Size arrangeSize)
    //    {
    //        Size availableSize = new(double.MaxValue, double.MaxValue);
    //        double requestedWidth = MinWidth;
    //        double requestedHeight = MinHeight;
    //        foreach (var item in InternalChildren.OfType<FrameworkElement>())
    //        {
    //            Rect bounds, marginBounts;
    //            GetRequestedBounts(item, out bounds, out marginBounts);
    //            requestedWidth = Math.Max(marginBounts.Right, requestedWidth);
    //            requestedHeight = Math.Max(marginBounts.Bottom, requestedHeight);
    //            item.Arrange(bounds);
    //        }
    //        return new Size(requestedWidth, requestedHeight);
    //        //return base.ArrangeOverride(arrangeSize);
    //    }
    //}


    public class AutoResizeCanvas : Panel
    {
        //.net - WPF: How to make canvas auto-resize? - Stack Overflow
//        https://stackoverflow.com/questions/855334/wpf-how-to-make-canvas-auto-resize
//なぜかオートリサイズにならない、というか理解できん、依存プロパティのあたりがわからん



        public static double GetLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(LeftProperty);
        }

        public static void SetLeft(DependencyObject obj, double value)
        {
            obj.SetValue(LeftProperty, value);
        }

        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.RegisterAttached("Left", typeof(double),
            typeof(AutoResizeCanvas),
            new FrameworkPropertyMetadata(0.0, OnLayoutParameterChanged));

        private static void OnLayoutParameterChanged(
                DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // invalidate the measure of the enclosing AutoResizeCanvas.
            while (d != null)
            {
                AutoResizeCanvas? canvas = d as AutoResizeCanvas;
                if (canvas != null)
                {
                    canvas.InvalidateMeasure();
                    return;
                }
                d = VisualTreeHelper.GetParent(d);
            }
        }



        //public double Top { get; set; }
        public static double GetTop(DependencyObject obj)
        {
            return (double)obj.GetValue(TopProperty);
        }

        public static void SetTop(DependencyObject obj, double value)
        {
            obj.SetValue(TopProperty, value);
        }

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.RegisterAttached("Top",
                typeof(double), typeof(AutoResizeCanvas),
                new FrameworkPropertyMetadata(0.0, OnLayoutParameterChanged));




        protected override Size MeasureOverride(Size constraint)
        {
            //base.MeasureOverride(constraint);
            Size availableSize = new Size(double.MaxValue, double.MaxValue);
            double requestedWidth = MinimumWidth;
            double requestedHeight = MinimumHeight;
            foreach (var child in base.InternalChildren)
            {
                FrameworkElement? el = child as FrameworkElement;

                if (el != null)
                {
                    el.Measure(availableSize);
                    Rect bounds, margin;
                    GetRequestedBounds(el, out bounds, out margin);

                    requestedWidth = Math.Max(requestedWidth, margin.Right);
                    requestedHeight = Math.Max(requestedHeight, margin.Bottom);
                }
            }
            return new Size(requestedWidth, requestedHeight);
        }

        private void GetRequestedBounds(
                            FrameworkElement el,
                            out Rect bounds, out Rect marginBounds
                            )
        {
            double left = 0, top = 0;
            Thickness margin = new Thickness();
            DependencyObject content = el;
            if (el is ContentPresenter)
            {
                content = VisualTreeHelper.GetChild(el, 0);
            }
            if (content != null)
            {
                left = AutoResizeCanvas.GetLeft(content);
                top = AutoResizeCanvas.GetTop(content);
                if (content is FrameworkElement)
                {
                    margin = ((FrameworkElement)content).Margin;
                }
            }
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;
            Size size = el.DesiredSize;
            bounds = new Rect(left + margin.Left, top + margin.Top, size.Width, size.Height);
            marginBounds = new Rect(left, top, size.Width + margin.Left + margin.Right, size.Height + margin.Top + margin.Bottom);
        }


        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);
            Size availableSize = new Size(double.MaxValue, double.MaxValue);
            double requestedWidth = MinimumWidth;
            double requestedHeight = MinimumHeight;
            foreach (var child in base.InternalChildren)
            {
                FrameworkElement? el = child as FrameworkElement;

                if (el != null)
                {
                    Rect bounds, marginBounds;
                    GetRequestedBounds(el, out bounds, out marginBounds);

                    requestedWidth = Math.Max(marginBounds.Right, requestedWidth);
                    requestedHeight = Math.Max(marginBounds.Bottom, requestedHeight);
                    el.Arrange(bounds);
                }
            }
            return new Size(requestedWidth, requestedHeight);
        }

        public double MinimumWidth
        {
            get { return (double)GetValue(MinimumWidthProperty); }
            set { SetValue(MinimumWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimumWidthProperty =
            DependencyProperty.Register("MinimumWidth", typeof(double), typeof(AutoResizeCanvas),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));



        public double MinimumHeight
        {
            get { return (double)GetValue(MinimumHeightProperty); }
            set { SetValue(MinimumHeightProperty, value); }
        }

        public static readonly DependencyProperty MinimumHeightProperty =
            DependencyProperty.Register("MinimumHeight", typeof(double), typeof(AutoResizeCanvas),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));



    }

}
