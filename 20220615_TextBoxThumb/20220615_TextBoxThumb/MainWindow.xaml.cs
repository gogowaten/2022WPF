using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace _20220615_TextBoxThumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private TBThumb MyTBThumb;
        private TBThumb2 MyTBThumb2;
        public MainWindow()
        {
#if DEBUG
            Left = 100; Top = 100;
#endif
            InitializeComponent();


            TBThumb2 t = MakeTBThumb2("WPFのTextBoxのPropertyだけで文字の装飾");
            MyCanvas.Children.Add(t);
            MyTBThumb2 = t;


            MyGroupBox.DataContext = MyTBThumb2.TextBox;

            //MyTBThumb2.TextBox.CaretBrush = Brushes.Red;
            MyTBThumb2.TextBox.AcceptsReturn = true;
            //MyTBThumb2.TextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            MyTBThumb2.TextBox.FontStyle = FontStyles.Italic;
            MyTBThumb2.TextBox.FontWeight = FontWeights.Bold;
            //MyTBThumb2.TextBox.Padding = new Thickness(10);
            //MyTBThumb2.TextBox.TextAlignment = TextAlignment.Center;//horizontalcontentalignmentとほぼ同じ
            TextDecoration deco = new();
            deco.Location = TextDecorationLocation.Strikethrough;//全4種類のライン
            deco.Pen = new Pen(Brushes.Red, 1.0);
            deco.PenOffset = 0;
            deco.PenOffsetUnit = TextDecorationUnit.FontRenderingEmSize;//全3種類
            //deco.PenThicknessUnit = TextDecorationUnit.FontRecommended;//全3種類
            TextDecorationCollection decoCollection = new();
            decoCollection.Add(deco);
            //MyTBThumb2.TextBox.TextDecorations = decoCollection;
            MyTBThumb2.TextBox.VerticalContentAlignment = VerticalAlignment.Top;
            //MyTBThumb2.TextBox.FontStretch = FontStretches.ExtraExpanded;
            
            
            
            InitializeComboBox();

            //System.Windows.Media.Effects.EdgeProfile.CurvedOut;

            //System.Windows.Media.Effects.DropShadowEffect dropShadow = new();
            //TextBlock tb = new() { Text = "textblock" ,FontSize=50,Effect=dropShadow};
            //MyGrid.Children.Add(tb);


        }

        private TBThumb2 MakeTBThumb2(string text)
        {
            TBThumb2 t = new();
            t.DragDelta += MyTThumb2_DragDelta;
            t.TextBox.Text = text;
            return t;
        }
        private static Dictionary<string, object> MakePropertyDictionary(Type t)
        {
            System.Reflection.PropertyInfo[]? info = t.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static);

            Dictionary<string, object>? dict = new();
            foreach (var item in info)
            {
                if (item.GetValue(null) is not object o)
                {
                    continue;
                }
                dict.Add(item.Name, o);
            }
            return dict;
        }

        private static ComboBox MakeBaseComboBox()
        {
            //DataTemplateの設定、Textblockに名前表示
            FrameworkElementFactory textblockF = new(typeof(TextBlock));
            textblockF.SetBinding(TextBlock.TextProperty, new Binding("Key"));
            DataTemplate dataTemplate = new();
            dataTemplate.VisualTree = textblockF;

            ComboBox comboBox = new()
            {
                SelectedValuePath = "Value",
                ItemTemplate = dataTemplate
            };
            return comboBox;
        }

        private void SetComboBoxSelectedValueBinding(DependencyProperty prop, ComboBox box)
        {
            Binding b = new();
            b.Path = new PropertyPath(prop);
            box.SetBinding(ComboBox.SelectedValueProperty, b);
        }

        /// <summary>
        /// フォント一覧のComboBoxを作成
        /// </summary>
        /// <returns></returns>
        private static ComboBox MakeFontComboBox()
        {
            //DataTemplateの設定、Textblockにフォント名表示
            FrameworkElementFactory textblockF = new(typeof(TextBlock));
            textblockF.SetBinding(TextBlock.TextProperty, new Binding("Key"));
            //↓フォント名表示にそのフォントを使う、リスト表示に時間がかかる
            //textblockF.SetBinding(TextBlock.FontFamilyProperty,new Binding("Value"));
            DataTemplate dataTemplate = new();
            dataTemplate.VisualTree = textblockF;

            ComboBox comboBox = new()
            {
                ItemsSource = MakeFontFamiliesDictionary(),
                SelectedValuePath = "Value",
                ItemTemplate = dataTemplate
            };
            return comboBox;
        }

        private static Dictionary<string, Brush> MakeBrushesDictionary()
        {
            var brushInfos = typeof(Brushes).GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static);

            Dictionary<string, Brush> dict = new();
            foreach (var item in brushInfos)
            {
                if (item.GetValue(null) is not Brush bu)
                {
                    continue;
                }
                dict.Add(item.Name, bu);
            }
            return dict;
        }
        private void InitializeComboBox()
        {
            Dictionary<string, Brush> dict = MakeBrushesDictionary();
            MyComboBoxFore.ItemsSource = dict;
            MyComboBoxBack.ItemsSource = dict;
            //フォント名一覧ComboBox
            ComboBox box = MakeFontComboBox();
            SetComboBoxSelectedValueBinding(TextBox.FontFamilyProperty, box);
            MyStackPanel.Children.Add(box);
            //フォントスタイル
            var pDict = MakePropertyDictionary(typeof(FontStyles));
            box = MakeBaseComboBox();
            box.ItemsSource = pDict.ToDictionary(a => a.Key, a => (FontStyle)a.Value);
            MyStackPanel.Children.Add(box);
            SetComboBoxSelectedValueBinding(TextBox.FontStyleProperty, box);
            //FontWeight
            MyStackPanel.Children.Add(new TextBlock() { Text = "FontWeight" });
            pDict = MakePropertyDictionary(typeof(FontWeights));
            box = MakeBaseComboBox();
            box.ItemsSource = pDict.ToDictionary(a => a.Key, a => (FontWeight)a.Value);
            MyStackPanel.Children.Add(box);
            SetComboBoxSelectedValueBinding(TextBox.FontWeightProperty, box);
            //TextAlignment
            MyStackPanel.Children.Add(new TextBlock() { Text = "TextAlignment" });
            box = new ComboBox() { ItemsSource = Enum.GetValues(typeof(TextAlignment)) };
            MyStackPanel.Children.Add(box);
            SetComboBoxSelectedValueBinding(TextBox.TextAlignmentProperty, box);
            //HorizontalContentAlignment
            MyStackPanel.Children.Add(new TextBlock() { Text = "HorizontalContentAlignment" });
            box = new ComboBox() { ItemsSource = Enum.GetValues(typeof(HorizontalAlignment)) };
            MyStackPanel.Children.Add(box);
            SetComboBoxSelectedValueBinding(TextBox.HorizontalContentAlignmentProperty, box);


            GroupBox gb = new();
            gb.Header = "TextDecoration";
            MyStackPanel2.Children.Add(gb);
            StackPanel sp = new();
            gb.Content = sp;
            //TextDecorationLocation
            sp.Children.Add(new TextBlock() { Text = "TextDecorationLocation" });
            box = new ComboBox() { ItemsSource = Enum.GetValues(typeof(TextDecorationLocation)) };
            box.SelectedIndex = 0;
            sp.Children.Add(box);
            MultiBinding mb = new();
            mb.Mode = BindingMode.TwoWay;
            mb.Converter = new MyConverter();
            mb.Bindings.Add(MakeBindingForComboBox(box));

            //TextDecorationUnit
            sp.Children.Add(new TextBlock() { Text = "TextDecorationUnit" });
            box = new ComboBox() { ItemsSource = Enum.GetValues(typeof(TextDecorationUnit)) };
            box.SelectedIndex = 0;
            sp.Children.Add(box);
            mb.Bindings.Add(MakeBindingForComboBox(box));

            //PenThicknessUnit
            sp.Children.Add(new TextBlock() { Text = "PenThicknessUnit" });
            box = new ComboBox() { ItemsSource = Enum.GetValues(typeof(TextDecorationUnit)) };
            box.SelectedIndex = 0;
            sp.Children.Add(box);
            mb.Bindings.Add(MakeBindingForComboBox(box));

            //Pen
            sp.Children.Add(new TextBlock() { Text = "Pen" });
            pDict = MakePropertyDictionary(typeof(Brushes));
            box = MakeBaseComboBox();
            box.ItemsSource = pDict.ToDictionary(a => a.Key, a => (Brush)a.Value);
            box.SelectedIndex = 2;
            sp.Children.Add(box);
            mb.Bindings.Add(MakeBindingForComboBox(box));

            //PenThickness
            sp.Children.Add(new TextBlock() { Text = "PenThickness" });
            Slider sl = new();
            sl.Value = 1.0;
            sp.Children.Add(sl);
            Binding b = new();
            b.Path = new PropertyPath(Slider.ValueProperty);
            b.Source = sl;
            mb.Bindings.Add(b);


            //PenOffset
            sp.Children.Add(new TextBlock() { Text = "PenOffset" });
            sl = new();
            sp.Children.Add(sl);
            b = new();
            b.Path = new PropertyPath(Slider.ValueProperty);
            b.Source = sl;
            mb.Bindings.Add(b);


            MyTBThumb2.TextBox.SetBinding(TextBox.TextDecorationsProperty, mb);
        }
        private Binding MakeBindingForComboBox(FrameworkElement elem)
        {
            Binding b = new();
            b.Source = elem;
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath(ComboBox.SelectedValueProperty);
            return b;
        }
        private void MyTThumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not FrameworkElement elem) { return; }
            Canvas.SetLeft(elem, Canvas.GetLeft(elem) + e.HorizontalChange);
            Canvas.SetTop(elem, Canvas.GetTop(elem) + e.VerticalChange);
        }

        /// <summary>
        /// SystemFontFamiliesから日本語フォント名で並べ替えたフォント一覧を返す、1ファイルに別名のフォントがある場合も取得
        /// </summary>
        /// <returns></returns>
        private static SortedDictionary<string, FontFamily> MakeFontFamiliesDictionary()
        {
            //今のPCで使っている言語(日本語)のCulture取得
            //var language =
            // System.Windows.Markup.XmlLanguage.GetLanguage(
            // CultureInfo.CurrentCulture.IetfLanguageTag);
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;//日本
            System.Globalization.CultureInfo cultureUS = new("en-US");//英語？米国？

            List<string> uName = new();//フォント名の重複判定に使う
            Dictionary<string, FontFamily> tempDictionary = new();
            foreach (var item in Fonts.SystemFontFamilies)
            {
                var typefaces = item.GetTypefaces();
                foreach (var typeface in typefaces)
                {
                    _ = typeface.TryGetGlyphTypeface(out GlyphTypeface gType);
                    if (gType != null)
                    {
                        //フォント名取得はFamilyNamesではなく、Win32FamilyNamesを使う
                        //FamilyNamesだと違うフォントなのに同じフォント名で取得されるものがあるので
                        //Win32FamilyNamesを使う
                        //日本語名がなければ英語名
                        string fontName = gType.Win32FamilyNames[culture] ?? gType.Win32FamilyNames[cultureUS];
                        //string fontName = gType.FamilyNames[culture] ?? gType.FamilyNames[cultureUS];

                        //フォント名で重複判定
                        var uri = gType.FontUri;
                        if (uName.Contains(fontName) == false)
                        {
                            uName.Add(fontName);
                            tempDictionary.Add(fontName, new(uri, fontName));
                        }
                    }
                }
            }
            SortedDictionary<string, FontFamily> fontDictionary = new(tempDictionary);
            return fontDictionary;
        }
    }

    public class MyThicknessConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness t = (Thickness)value;
            return t.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = (double)value;
            return new Thickness(v);
        }
    }

    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TextDecorationLocation location = (TextDecorationLocation)values[0];
            TextDecorationUnit penOffsetProperty = (TextDecorationUnit)values[1];
            TextDecorationUnit unit = (TextDecorationUnit)values[2];
            Brush b = (Brush)values[3];
            double penThickness = (double)values[4];
            double penOffset = (double)values[5];

            TextDecoration deco = new();
            deco.Location = location;
            deco.PenOffsetUnit = penOffsetProperty;
            deco.PenThicknessUnit = unit;
            deco.Pen = new Pen(b, penThickness);
            deco.PenOffset = penOffset;
            TextDecorationCollection texts = new();
            texts.Add(deco);
            return texts;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            TextDecorationCollection textDecorations = (TextDecorationCollection)value;
            object[] result = new object[6];
            TextDecoration deco = textDecorations[0];
            result[0] = deco.Location;
            result[1] = deco.PenOffset;
            result[2] = deco.PenOffsetUnit;
            result[3] = deco.Pen.Brush;
            result[4] = deco.Pen.Thickness;
            result[5] = deco.PenOffset;
            return result;
        }
    }

    //TemplateをTextBoxにしたThumb、マウスドラッグ移動できない
    public class TBThumb : Thumb
    {
        private const string TEXTBOX_NAME = "ttt";
        public TextBox TextBox;
        public TBThumb()
        {
            this.Template = MakeControlTemplate();
            //Templateの更新、必要
            ApplyTemplate();
            //Templateの中のTextBoxを検索、取得
            this.TextBox = (TextBox)Template.FindName(TEXTBOX_NAME, this);
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }
        //TextBoxをベースにしたControlTemplateを作成
        private ControlTemplate MakeControlTemplate()
        {
            FrameworkElementFactory textF = new(typeof(TextBox), TEXTBOX_NAME);
            ControlTemplate template = new();
            template.VisualTree = textF;
            return template;
        }
    }

    //TemplateをGridを乗せたTextBoxにしたThumb、マウスドラッグ移動できる
    //ダブルクリックで移動モードとText編集モードを切り替える
    public class TBThumb2 : Thumb
    {
        private const string COVER_NAME = "cover";
        private const string TEXTBOX_NAME = "ttt";
        public Grid CoverGrid;
        public TextBox TextBox;
        public TBThumb2()
        {
            this.Template = MakeControlTemplate();
            //Templateの更新、必要
            ApplyTemplate();
            //Templateの中のTextBoxと蓋用のGridを検索、取得
            this.TextBox = (TextBox)Template.FindName(TEXTBOX_NAME, this);
            this.CoverGrid = (Grid)Template.FindName(COVER_NAME, this);
            //表示位置を指定
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            //ダブルクリック時の動作
            MouseDoubleClick += TBThumb2_MouseDoubleClick;
        }

        //ダブルクリックでテキスト編集状態の切り替え
        private void TBThumb2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //蓋の背景が透明色ならTextBoxを編集状態にする、背景をnullにする
            if (CoverGrid.Background == Brushes.Transparent)
            {
                CoverGrid.Background = null;
                Keyboard.Focus(TextBox);
            }
            //それ以外なら編集状態終了、背景を透明色にしてキーボードのフォーカスを外す
            else
            {
                CoverGrid.Background = Brushes.Transparent;
                Keyboard.ClearFocus();//キーボードのフォーカスを外す
            }
        }

        //ControlTemplate作成
        private static ControlTemplate MakeControlTemplate()
        {
            FrameworkElementFactory coverF = new(typeof(Grid), COVER_NAME);//蓋
            FrameworkElementFactory textF = new(typeof(TextBox), TEXTBOX_NAME);
            FrameworkElementFactory underF = new(typeof(Grid));//ベースGrid
            //蓋の背景は透明色
            coverF.SetValue(Panel.BackgroundProperty, Brushes.Transparent);
            //ベースGridに要素追加、順番はTextBox、蓋Gridの順
            underF.AppendChild(textF);
            underF.AppendChild(coverF);
            //テンプレート作成、VisualTreeにベースを指定
            ControlTemplate template = new();
            template.VisualTree = underF;
            return template;
        }

    }











}