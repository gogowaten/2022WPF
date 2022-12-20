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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Input;
using System.Globalization;


namespace _20221220_TextBlockItemsControlThumb
{
    public class TThumb : Thumb
    {
        #region 依存プロパティ

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
        #endregion 依存プロパティ


        public TThumb()
        {
            DataContext = this;
            SetBinding(Canvas.LeftProperty, new Binding(nameof(X)) { Mode = BindingMode.TwoWay });
            SetBinding(Canvas.TopProperty, new Binding(nameof(Y)) { Mode = BindingMode.TwoWay });
            SizeChanged += TThumb_SizeChanged;
        }

        private void TThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Width = ActualWidth;
            Height= ActualHeight;
        }

        protected FrameworkElementFactory MakeTemplate()
        {
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Shape.StrokeProperty, Brushes.Cyan);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            waku.SetValue(Panel.ZIndexProperty, 1);
            FrameworkElementFactory basePanel = new(typeof(Grid));
            basePanel.AppendChild(waku);
            return basePanel;
        }



    }


    public class TTTextBlock : TThumb
    {
        #region 依存プロパティ
        public string TTText
        {
            get { return (string)GetValue(TTTextProperty); }
            set { SetValue(TTTextProperty, value); }
        }
        public static readonly DependencyProperty TTTextProperty =
            DependencyProperty.Register(nameof(TTText), typeof(string), typeof(TTTextBlock), new PropertyMetadata(""));


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
            DependencyProperty.Register(nameof(BackColor), typeof(Brush), typeof(TTTextBlock), new PropertyMetadata(Brushes.Transparent));


        //public double TTFontSize
        //{
        //    get { return (double)GetValue(TTFontSizeProperty); }
        //    set { SetValue(TTFontSizeProperty, value);}
        //}
        //public static readonly DependencyProperty TTFontSizeProperty =
        //    DependencyProperty.Register(nameof(TTFontSize), typeof(double), typeof(TTTextBlock), 
        //        new PropertyMetadata(20.0));

        //public double TTFontSize
        //{
        //    get { return (double)GetValue(TTFontSizeProperty); }
        //    set { SetValue(TTFontSizeProperty, value); }
        //}
        //public static readonly DependencyProperty TTFontSizeProperty =
        //    DependencyProperty.Register(nameof(TTFontSize), typeof(double), typeof(TTTextBlock),
        //        new PropertyMetadata(System.Windows.SystemFonts.MenuFontSize));
        //protected override Size MeasureOverride(Size constraint)
        //{
        //    return base.MeasureOverride(constraint);

        //}

        #endregion

        public TTTextBlock()
        {
            var tPanel = MakeTemplate();
            FrameworkElementFactory content = new(typeof(TextBlock));
            content.SetValue(TextBlock.TextProperty, new Binding(nameof(TTText)));
            content.SetValue(TextBlock.ForegroundProperty, new Binding(nameof(FontColor)));
            content.SetValue(TextBlock.BackgroundProperty, new Binding(nameof(BackColor)));
            tPanel.AppendChild(content);
            this.Template = new() { VisualTree = tPanel };
        }
    }
    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        public ObservableCollection<TThumb> Children { get; set; } = new();
        public TTGroup()
        {
            FrameworkElementFactory ic = new(typeof(ItemsControl));
            ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            FrameworkElementFactory panel = new(typeof(Canvas));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(panel));

            FrameworkElementFactory baseGridPanel = MakeTemplate();
            baseGridPanel.AppendChild(ic);
            this.Template = new() { VisualTree = baseGridPanel };

        }
    }
}
