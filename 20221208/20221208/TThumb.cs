using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Runtime.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace _20221208
{
    public abstract class TThumb : Thumb
    {
        #region 依存プロパティ


        //public double X
        //{
        //    get { return (double)GetValue(XProperty); }
        //    set { SetValue(XProperty, value); }
        //}
        //public static readonly DependencyProperty XProperty =
        //    DependencyProperty.Register(nameof(X), typeof(double), typeof(TTTextBlock), new PropertyMetadata(0.0));

        //デザイン画面での値変更時に描画更新対応した、依存プロパティ
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(nameof(X), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(nameof(Y), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public int Z
        {
            get { return (int)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }
        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register(nameof(Z), typeof(int), typeof(TThumb),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        #endregion
        public TThumb()
        {
            DataContext = this;
            //Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            SetBinding(Canvas.LeftProperty, new Binding(nameof(X)) { Mode = BindingMode.TwoWay });            
            SetBinding(Canvas.TopProperty, new Binding(nameof(Y)) { Mode = BindingMode.TwoWay });
            SetBinding(Panel.ZIndexProperty, new Binding(nameof(Z)) { Mode = BindingMode.TwoWay });
            DragDelta += TThumb_DragDelta;
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            X += e.HorizontalChange;Y += e.VerticalChange;
        }

        protected abstract void SetData(Data data);
        public TThumb(Data data) : this()
        {
            SetData(data);
        }
    }
    //public interface ISetData
    //{
    //    public void SetData(Data data);
    //}
    public class TTTextBlock : TThumb
    {
        #region 依存プロパティ
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TTTextBlock), new PropertyMetadata(""));


        public Brush FontColor
        {
            get { return (Brush)GetValue(FontColorProperty); }
            set { SetValue(FontColorProperty, value); }
        }
        public static readonly DependencyProperty FontColorProperty =
            DependencyProperty.Register(nameof(FontColor), typeof(Brush), typeof(TTTextBlock), new PropertyMetadata(Brushes.Black));


        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register(nameof(BackColor), typeof(Brush), typeof(TTTextBlock), new PropertyMetadata(Brushes.Snow));

        #endregion

        public TTTextBlock()
        {
            FrameworkElementFactory elem = new(typeof(TextBlock));
            elem.SetValue(TextBlock.TextProperty, new Binding(nameof(Text)));
            elem.SetValue(TextBlock.ForegroundProperty, new Binding(nameof(FontColor)));
            elem.SetValue(TextBlock.BackgroundProperty, new Binding(nameof(BackColor)));
            this.Template = new() { VisualTree = elem };

        }
        public TTTextBlock(Data data) : this() { SetData(data); }
        protected override void SetData(Data data)
        {
            Text = data.Text;
            FontColor = data.ForeColor ?? Brushes.Black;
            BackColor = data.BackColor ?? Brushes.Transparent;
            FontSize = data.FontSize;
        }
    }

    public class TTRectangle : TThumb
    {

        #region 依存プロパティ
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(TTRectangle), new PropertyMetadata(Brushes.Red));

        //public Brush FillBrush
        //{
        //    get { return (Brush)GetValue(FillBrushProperty); }
        //    set { SetValue(FillBrushProperty, value); }
        //}
        //public static readonly DependencyProperty FillBrushProperty =
        //    DependencyProperty.Register(nameof(FillBrush), typeof(Brush), typeof(TTRectangle),
        //        new FrameworkPropertyMetadata(Brushes.Red,
        //            FrameworkPropertyMetadataOptions.AffectsRender |
        //            FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double ShapeWidth
        {
            get { return (double)GetValue(ShapeWidthProperty); }
            set { SetValue(ShapeWidthProperty, value); }
        }
        public static readonly DependencyProperty ShapeWidthProperty =
            DependencyProperty.Register(nameof(ShapeWidth), typeof(double), typeof(TTRectangle), new PropertyMetadata(0.0));

        public double ShapeHeight
        {
            get { return (double)GetValue(ShapeHeightProperty); }
            set { SetValue(ShapeHeightProperty, value); }
        }
        public static readonly DependencyProperty ShapeHeightProperty =
            DependencyProperty.Register(nameof(ShapeHeight), typeof(double), typeof(TTRectangle), new PropertyMetadata(0.0));


        #endregion 依存プロパティ

        public TTRectangle()
        {
            //DataContext = this;
            FrameworkElementFactory elem = new(typeof(Rectangle));            
            elem.SetValue(Shape.FillProperty, new Binding(nameof(Fill)));            
            elem.SetValue(WidthProperty, new Binding(nameof(ShapeWidth)));
            elem.SetValue(HeightProperty, new Binding(nameof(ShapeHeight)));
            this.Template = new() { VisualTree = elem };

        }
        public TTRectangle(Data data) : this()
        {
            SetData(data);
        }
        protected override void SetData(Data data)
        {
            //Fill = data.BackColor ?? Brushes.Red;
            //ShapeWidth = data.Width; ShapeHeight = data.Height;
        }
    }

    public class TTGroup : TThumb
    {
        public ObservableCollection<TThumb> Children { get; protected set; } = new();
        public TTGroup()
        {
            FrameworkElementFactory panel = new(typeof(Canvas));
            FrameworkElementFactory ic = new(typeof(ItemsControl));
            ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(panel));
            this.Template = new() { VisualTree = ic };
        }
        public TTGroup(Data data) : this()
        {
            SetData(data);
        }
        protected override void SetData(Data data)
        {
            if (data.Datas != null)
            {
                foreach (var item in data.Datas)
                {
                    Add(item);
                }
            }
        }
        private void Add(Data data)
        {
            switch (data.Type)
            {
                case TType.TextBlock:
                    Children.Add(new TTTextBlock(data));
                    break;
                case TType.Rectangle:
                    Children.Add(new TTRectangle(data));
                    break;
                case TType.Group:
                    Children.Add(new TTGroup(data));
                    break;
                default:
                    break;
            }
        }
        public void Add(TThumb thumb) { Children.Add(thumb); }
    }

}
