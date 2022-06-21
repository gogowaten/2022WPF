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

//失敗、
//再グループ化のときに必要な情報を、指定した異なるインスタンス間で共有するために考えたけど意味なかった
//思うような動作にならないと思ったけど、普通にコレクション型を指定するだけで良かった

namespace _20220620_RegroupShareDataTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //これじゃなかった
            TTextBox tt0 = new("tt000");
            TTextBox tt1 = new("tt111");
            TTextBox tt2 = new("tt222");
            TTextBox tt3 = new("tt333");
            TTextBox tt4 = new("tt444");

            ReGroupData data = new(new List<TTextBox>() { tt0, tt1, tt2 });
            var neko = tt0.MyReGroupData;
            var inu = tt1.MyReGroupData;

            tt1.RemoveItem();
            var uma = tt2.MyReGroupData;

            data = new(new List<TTextBox>() { tt3, tt4 });
            var tako = tt3.MyReGroupData;


            //test2、普通にこれで良かった
            AAA a0 = new("a000");
            AAA a1 = new("a111");
            AAA a2 = new("a222");
            AAA a3 = new("a333");
            List<string> strings = new() { a0.MyText, a1.MyText };
            a0.MyShare = strings;
            a1.MyShare = strings;

            strings = new() { a2.MyText, a3.MyText };
            a2.MyShare = strings;
            a3.MyShare = strings;
            a3.MyShare.Remove(a3.MyText);

            //でも、使い方次第、こうすると共有じゃなくなる
            a2.MyShare = new() { a2.MyText, a3.MyText };
            a3.MyShare = new() { a2.MyText, a3.MyText };
            a3.MyShare.Remove(a3.MyText);

            //test3
            BBB b0 = new("b000");
            BBB b1 = new("b111");
            BBB b2 = new("b222");
            BBB b3 = new("b333");
            strings = new() { b0.MyText, b1.MyText };
            b0.MyShare = strings;
            b1.MyShare = strings;

            strings = new() { b2.MyText, b3.MyText };
            b2.MyShare = strings;
            b3.MyShare = strings;
            b3.MyShare.Remove(b3.MyText);

            b0.MyShare = null;
        }
    }
}
