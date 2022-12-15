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


        public string MyName
        {
            get { return (string)GetValue(ZNameProperty); }
            set { SetValue(ZNameProperty, value); }
        }
        public static readonly DependencyProperty ZNameProperty =
            DependencyProperty.Register(nameof(MyName), typeof(string), typeof(TThumb), new PropertyMetadata(""));

        #endregion

        
        public TThumb()
        {
            DataContext = this;

        }


    }

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
            FrameworkElementFactory panel = new(typeof(Grid));
            //FrameworkElementFactory panel = new(typeof(Canvas));//Canvasだとサイズが常に0になる
            panel.AppendChild(elem);
            panel.AppendChild(waku);
            this.Template = new() { VisualTree = panel };
            
        }
    }
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
            this.TemplateTextBlock = (TextBlock)this.Template.FindName(MY_TEMPLATE_NAME, this);

        }
    }
}
