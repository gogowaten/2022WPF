using System.Linq;
using System.Windows;
using System.Windows.Controls;

//参照したところ
//c# - WPF:キャンバスのサイズを変更して子を含める
//https://stackoverflow.com/questions/55101074/wpf-sizing-canvas-to-contain-its-children

namespace _20221221_子要素のサイズと位置でサイズ更新するCanvas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MyExCanvas.Width = 200; MyExCanvas.Height = 100;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Width = 200; MyCanvas.Height = 100;
        }
    }

    /// <summary>
    /// 子要素のサイズor位置変更で自身のサイズ(Actual)を変更するCanvas
    /// 弊害？としてはWidthやHeightを指定しても無視される
    /// </summary>
    public class ExCanvas : Canvas
    {
        //MeasureOverrideは
        //子要素のサイズ変更で実行される
        //子要素の位置変更では実行されない
        //CanvasのActualサイズが更新される(よくやった！)
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            double w = 0; double h = 0;
            foreach (var item in Children.OfType<FrameworkElement>())
            {
                var desiredW = GetLeft(item) + item.DesiredSize.Width;
                if (desiredW > w) w = desiredW;
                var desiredH = GetTop(item) + item.DesiredSize.Height;
                if (desiredH > h) h = desiredH;
            }
            return new Size(w, h);
        }

        //ArrangeOverrideは
        //子要素のサイズ変更で実行される
        //子要素の位置変更でも実行される(よくやった！)
        //CanvasのActualサイズは更新されない、のでMeasureを実行する
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);
            //子要素全体が収まるサイズを計算
            //子要素の幅サイズ取得にWidthやActualWidthを使わずにDesiredSize.Widthを使っているのは、
            //例えばTextBlockのようにフォントサイズ変更で自動でサイズが変更される要素は
            //Widthは常にNaNだし、このArrangeが実行された時点ではではActualWidthはまだ更新されていないけど
            //DesiredSizeは更新済みだから
            double w = 0; double h = 0;
            foreach (var item in Children.OfType<FrameworkElement>())
            {
                var desiredW = GetLeft(item) + item.DesiredSize.Width;
                if (desiredW > w) w = desiredW;
                var desiredH = GetTop(item) + item.DesiredSize.Height;
                if (desiredH > h) h = desiredH;
            }
            Size size = new(w, h);
            //差異があればサイズ更新(Measure実行)
            if (arrangeSize.Width != w || arrangeSize.Height != h)
            {
                Measure(size);
            }
            return size;
        }
    }
}
