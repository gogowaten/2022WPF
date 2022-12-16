using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//ThumbのTemplateを変更してTextblockを表示できるようにしたものを2種類
//一つはBindingをTemplate作成中に行い、あとからの変更はできないもの
//もう一つはTemplate作成後にTextblockを取り出してフィールドに持たせる、これだと
//あとからいくらでも変更できる

//結果はどちらでも変わらないけど、フィールドに持たせるのほうがいいかも？

//サイズ更新
//TThumbのTextblockのサイズ更新はTemplateのベースがGridなら自動で変化するけど
//Canvasがベースだと変化しない、変化するのはTemplateの中のTextblockだけで、全体のTTTextblockは変化しない
//これならGridをベースにすれば済むけど
//GroupThumbだと子要素を任意の位置に配置する必要があるので
//ベースはCanvasにする必要がある、そうるすとGroupThumbのサイズが自動更新されない
//理想は
//子要素の位置変更時に枠からはみ出たらGroupの位置とサイズ更新
//子要素のサイズ変更時に枠からはみ出たらGroupの位置とサイズ更新

namespace _20221215_TemplateTest
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

        /// <summary>
        /// フォントサイズ変更
        /// サイズ変更関連イベントの発生順番は
        /// フォントサイズ変更→Render → RenderSizeChanged → SizeChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            T1.TTFontSize = 30;
            double ah = T1.ActualHeight;
            double h = T1.Height;
            Size size = T1.DesiredSize;
        }
    }
}
