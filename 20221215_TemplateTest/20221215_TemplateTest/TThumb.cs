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

namespace _20221215_TemplateTest
{
    public class TThumb : Thumb
    {
     

        
        public TThumb()
        {
            DataContext = this;
            SizeChanged += TThumb_SizeChanged;
        }

        private void TThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            string name = this.Name;
            double ah = this.ActualHeight;
            double h = this.Height;
            Size size = this.DesiredSize;
            var source = e.Source;
            var origin = e.OriginalSource;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
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
            waku.SetValue(Shape.StrokeProperty, Brushes.Blue);
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
    /// <summary>
    /// TextblockとのBindingをTemplateから取り出したTextblockと行う
    /// あとからでもTextblockに対して色々できる
    /// </summary>
    public class TTTextBlock2 : TTTextBlock
    {
        protected const string MY_TEMPLATE_NAME = "myTemplate";
        TextBlock? TemplateTextBlock;
        public TTTextBlock2()
        {
            SetTemplate();
            if(TemplateTextBlock!= null)
            {
                TemplateTextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)));
                TemplateTextBlock.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(FontColor)));
                TemplateTextBlock.SetBinding(TextBlock.BackgroundProperty, new Binding(nameof(BackColor)));
                TemplateTextBlock.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(TTFontSize)));
            }
            

        }
        private void SetTemplate()
        {
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Shape.StrokeProperty, Brushes.Blue);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            waku.SetValue(Panel.ZIndexProperty, 1);
            FrameworkElementFactory elem = new(typeof(TextBlock), MY_TEMPLATE_NAME);
            FrameworkElementFactory panel = new(typeof(Grid));
            //FrameworkElementFactory panel = new(typeof(Canvas));//Canvasだとサイズが常に0になる
            panel.AppendChild(elem);
            panel.AppendChild(waku);
            this.Template = new() { VisualTree = panel };
            this.ApplyTemplate();
            //Textblock取り出し
            this.TemplateTextBlock = (TextBlock)this.Template.FindName(MY_TEMPLATE_NAME, this);

        }
    }
}
