using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace _20220615_TextBoxThumb0
{
    public partial class MainWindow : Window
    {
        private TBThumb MyTBThumb;
        private TBThumb2 MyTBThumb2;
        public MainWindow()
        {
#if DEBUG
            Left = 100; Top = 100;
#endif
            InitializeComponent();

            MyTBThumb = new TBThumb();
            MyCanvas.Children.Add(MyTBThumb);
            MyTBThumb.TextBox.Text = "移動できないTextBoxThumb";
            MyTBThumb.DragDelta += Thumb_DragDelta;

            MyTBThumb2 = new TBThumb2();
            MyCanvas.Children.Add(MyTBThumb2);
            MyTBThumb2.TextBox.Text = "ダブルクリックで\nテキスト編集と移動を切り替え";
            MyTBThumb2.TextBox.AcceptsReturn = true;//改行を有効
            MyTBThumb2.DragDelta += Thumb_DragDelta;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is not FrameworkElement elem) { return; }
            Canvas.SetLeft(elem, Canvas.GetLeft(elem) + e.HorizontalChange);
            Canvas.SetTop(elem, Canvas.GetTop(elem) + e.VerticalChange);
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
            Keyboard.ClearFocus();
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
    }
}