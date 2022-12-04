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

//くっきり描画のShape
//普通にLineToのStrokeで直線を描画すると、
//太さが整数の奇数だと真横の線でも輪郭がぼやける、これは
//座標の取り方によるみたいで、例えば太さ1の横線の縦座標に10を指定した場合は
//座標10を中心に上下0.5の直線が描画されるのでぼやける
//太さが偶数のときはぼやけない、太さ2なら、座標10を中心に上下1ピクセルづつなのでぼやけない

//ぼやけを回避するには指定座標に0.5足せばいい、あとは
//横座標と斜めの線のときも考えて三角関数で描画位置を計算
 
namespace _20221204_矢印図形
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
