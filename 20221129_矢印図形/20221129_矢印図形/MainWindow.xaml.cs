using System.Windows;

//矢印先端の描画位置を正しくした
//これは矢じり部分の描画をStrokeではなくてFillで描画するようにしたからできた
//直線部分は前回同様Strokeで描画している

//新たな問題
//直線部分と矢じり部分の描画を別々にしたせいか接合部で僅かな隙間ができてしまう

namespace _20221129_矢印図形
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
}
