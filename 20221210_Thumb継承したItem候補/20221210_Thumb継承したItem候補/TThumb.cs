using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _20221210_Thumb継承したItem候補
{
    /// <summary>
    /// 各種アイテムのベースになる親クラス、共通するプロパティを設置、設定する
    /// </summary>
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


        public string MyName
        {
            get { return (string)GetValue(MyNameProperty); }
            set { SetValue(MyNameProperty, value); }
        }
        public static readonly DependencyProperty MyNameProperty =
            DependencyProperty.Register(nameof(MyName), typeof(string), typeof(TThumb), new PropertyMetadata(""));

        #endregion

        //public TTGroup? ParentThumb { get; internal set; }
        #region コンストラクタ

        public TThumb()
        {
            DataContext = this;
            SetBinding(Canvas.LeftProperty, new Binding(nameof(X)) { Mode = BindingMode.TwoWay });
            SetBinding(Canvas.TopProperty, new Binding(nameof(Y)) { Mode = BindingMode.TwoWay });
            SetBinding(Panel.ZIndexProperty, new Binding(nameof(Z)) { Mode = BindingMode.TwoWay });
            SetBinding(NameProperty, new Binding(nameof(MyName)) { Mode = BindingMode.TwoWay });
        }

        public TThumb(Data data) : this()
        {
            SetData(data);
        }


        #endregion コンストラクタ

        protected virtual void SetData(Data data)
        {
            X = data.X; Y = data.Y; Z = data.Z;
            if (string.IsNullOrEmpty(MyName) && string.IsNullOrEmpty(data.Name) == false)
                MyName = data.Name;
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
            DependencyProperty.Register(nameof(FontColor), typeof(Brush), typeof(TTTextBlock), new PropertyMetadata(Brushes.Black));


        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register(nameof(BackColor), typeof(Brush), typeof(TTTextBlock), new PropertyMetadata(Brushes.Transparent));


        //メタデータにはSystemFontsを渡している。nullだとXAMLでエラーになる
        public FontFamily Font
        {
            get { return (FontFamily)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }
        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register(nameof(Font),
                typeof(FontFamily), typeof(TTTextBlock), new PropertyMetadata(System.Windows.SystemFonts.MenuFontFamily));

        #endregion

        public TTTextBlock()
        {
            FrameworkElementFactory elem = new(typeof(TextBlock));
            elem.SetValue(TextBlock.TextProperty, new Binding(nameof(Text)));
            elem.SetValue(TextBlock.ForegroundProperty, new Binding(nameof(FontColor)));
            elem.SetValue(TextBlock.BackgroundProperty, new Binding(nameof(BackColor)));
            elem.SetValue(TextBlock.FontFamilyProperty, new Binding(nameof(Font)));
            this.Template = new() { VisualTree = elem };

        }
        public TTTextBlock(Data data) : this() { SetData(data); }
        protected override void SetData(Data data)
        {
            if (data is TextBlockData td) { base.SetData(data); SetData(td); }
            else { throw new ArgumentException(); }
        }
        private void SetData(TextBlockData data)
        {
            Text = data.Text;
            FontColor = data.FontColorBrush ?? Brushes.Black;
            BackColor = data.BackColorBrush ?? Brushes.Transparent;
            if (data.FontSize > 0.0) FontSize = data.FontSize;
            Font = data.Font;// ?? this.Font;
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


        #endregion 依存プロパティ

        public TTRectangle()
        {
            FrameworkElementFactory elem = new(typeof(Rectangle));
            elem.SetValue(Shape.FillProperty, new Binding(nameof(Fill)));
            elem.SetValue(WidthProperty, new Binding(nameof(Width)));
            elem.SetValue(HeightProperty, new Binding(nameof(Height)));

            this.Template = new() { VisualTree = elem };

        }
        public TTRectangle(Data data) : this()
        {
            SetData(data);
        }
        protected override void SetData(Data data)
        {
            if (data is RectangleData dd) { base.SetData(data); SetData(dd); }
            else { throw new ArgumentException("aaaaaaaa"); }
        }
        private void SetData(RectangleData data)
        {
            Fill = data.FillColorBrush ?? Brushes.Red;
            Width = data.ShapeWidth;
            Height = data.ShapeHeight;

        }
    }

    public class TTPolyline : TThumb
    {
        #region 依存プロパティ

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(TTPolyline), new PropertyMetadata(null));

        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register(nameof(LineColor), typeof(Brush), typeof(TTPolyline), new PropertyMetadata(Brushes.Orange));

        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(TTPolyline), new PropertyMetadata(1.0));

        #endregion 依存プロパティ
        public TTPolyline()
        {
            FrameworkElementFactory elem = new(typeof(Polyline));
            elem.SetValue(Polyline.PointsProperty, new Binding(nameof(Points)));
            //elem.SetValue(Polyline.FillProperty, new Binding(nameof(LineColor)));
            elem.SetValue(Polyline.StrokeProperty, new Binding(nameof(LineColor)));
            elem.SetValue(Polyline.StrokeThicknessProperty, new Binding(nameof(LineThickness)));
            this.Template = new() { VisualTree = elem };
        }
        public TTPolyline(Data data) : this()
        {
            SetData(data);
        }
        protected override void SetData(Data data)
        {
            if (data is PolylineData dd) { base.SetData(data); SetData(dd); }
            else { throw new ArgumentException(nameof(data)); }
        }
        private void SetData(PolylineData data)
        {
            Points ??= data.Points ?? new();
            LineColor = data.LineColorBrush ?? Brushes.Orange;
            if (data.Thickness > 0.0) { LineThickness = data.Thickness; }
        }
    }

}
