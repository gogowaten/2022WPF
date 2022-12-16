using System;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;

namespace _20221216_Group用Template
{
    public class TThumb : Thumb
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

        public double TTWidth
        {
            get { return (double)GetValue(TTWidthProperty); }
            set { SetValue(TTWidthProperty, value); }
        }
        public static readonly DependencyProperty TTWidthProperty =
            DependencyProperty.Register(nameof(TTWidth), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double TTHeight
        {
            get { return (double)GetValue(TTHeightProperty); }
            set { SetValue(TTHeightProperty, value); }
        }
        public static readonly DependencyProperty TTHeightProperty =
            DependencyProperty.Register(nameof(TTHeight), typeof(double), typeof(TThumb),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        public string MyName
        {
            get { return (string)GetValue(ZNameProperty); }
            set { SetValue(ZNameProperty, value); }
        }
        public static readonly DependencyProperty ZNameProperty =
            DependencyProperty.Register(nameof(MyName), typeof(string), typeof(TThumb), new PropertyMetadata(""));

        #endregion

        public TTGroup? ParentGroup { get; set; }
        public TThumb()
        {
            DataContext = this;
            SizeChanged += TThumb_SizeChanged;

            SetBinding(Canvas.LeftProperty, new Binding(nameof(X)));
            SetBinding(Canvas.TopProperty, new Binding(nameof(Y)));

        }

        private void TThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ParentGroup?.SetGroupRect(ParentGroup);
        }
    }

    /// <summary>
    /// TextblockとのBindingをTemplate作成中に行う
    /// </summary>
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
            DependencyProperty.Register(nameof(FontColor), typeof(Brush), typeof(TTTextBlock),
                new PropertyMetadata(Brushes.Black));

        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register(nameof(BackColor), typeof(Brush), typeof(TTTextBlock),
                new PropertyMetadata(Brushes.Transparent));

        public double TTFontSize
        {
            get { return (double)GetValue(TTFontSizeProperty); }
            set { SetValue(TTFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TTFontSizeProperty =
            DependencyProperty.Register(nameof(TTFontSize), typeof(double), typeof(TTTextBlock), new PropertyMetadata(20.0));


        #endregion


        public TTTextBlock()
        {
            SetTemplate();
        }
        private void SetTemplate()
        {
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Shape.StrokeProperty, Brushes.Orange);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            waku.SetValue(Panel.ZIndexProperty, 1);
            FrameworkElementFactory elem = new(typeof(TextBlock));
            elem.SetValue(TextBlock.TextProperty, new Binding(nameof(Text)));
            elem.SetValue(TextBlock.ForegroundProperty, new Binding(nameof(FontColor)));
            elem.SetValue(TextBlock.BackgroundProperty, new Binding(nameof(BackColor)));
            elem.SetValue(TextBlock.FontSizeProperty, new Binding(nameof(TTFontSize)));
            FrameworkElementFactory panel = new(typeof(Grid));
            //FrameworkElementFactory panel = new(typeof(Canvas));//Canvasだとサイズが常に0になる
            panel.AppendChild(elem);
            panel.AppendChild(waku);
            this.Template = new() { VisualTree = panel };

        }
    }


    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        public ObservableCollection<TThumb> Children { get; protected set; } = new();
        /// <summary>
        /// 対象GroupThumbのRectを取得、UnionRect
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private (double x, double y, double w, double h) GetGroupRect(TTGroup group, bool actualSize)
        {
            if (Children.Count == 0) { return (0, 0, 0, 0); }
            double x = double.MaxValue, y = double.MaxValue;
            double w = double.MinValue, h = double.MinValue;
            foreach (var item in group.Children)
            {
                x = Math.Min(x, item.X);
                y = Math.Min(y, item.Y);
                if (actualSize)
                {

                    w = Math.Max(w, item.X + item.ActualWidth);
                    h = Math.Max(h, item.Y + item.ActualHeight);
                }
                else
                {
                    w = Math.Max(w, item.X + item.Width);
                    h = Math.Max(h, item.Y + item.Height);
                }
            }
            w -= x; h -= y;
            return (x, y, w, h);
        }
        /// <summary>
        /// 対象Groupの位置とサイズ更新、親要素に伝播する
        /// </summary>
        /// <param name="target"></param>
        public void SetGroupRect(TTGroup target)
        {
            var (x, y, w, h) = GetGroupRect(target, true);
            if (w == 0 && h == 0) return;
            if (X == x && Y == y && ActualWidth == w && ActualHeight == h) return;
            target.Width = w; target.Height = h;
            if (x != 0 || y != 0)
            {
                foreach (var item in target.Children)
                {
                    item.X -= x; item.Y -= y;
                }
                //親要素も更新
                if (target.ParentGroup is TTGroup parent)
                {
                    SetGroupRect(parent);
                }
            }
        }
    }
    public class TTGroupA : TTGroup
    {

        public TTGroupA()
        {
            SetTemplate();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems?[0] is TThumb tt)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        tt.ParentGroup = this;
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

        private void SetTemplate()
        {
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Shape.StrokeProperty, Brushes.Purple);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            waku.SetValue(Panel.ZIndexProperty, 1);
            FrameworkElementFactory canvas = new(typeof(Canvas));
            FrameworkElementFactory ic = new(typeof(ItemsControl));
            ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvas));

            FrameworkElementFactory panel = new(typeof(Grid));
            panel.AppendChild(waku);
            panel.AppendChild(ic);

            this.Template = new() { VisualTree = panel };

        }

    }




}
