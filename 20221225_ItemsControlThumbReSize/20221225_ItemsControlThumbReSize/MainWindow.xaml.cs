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
//子要素の移動とサイズ変更で親要素の位置とサイズ更新
//ドラッグ移動完了後に更新する。リアルタイム更新は難しくてできなかった
//ItemはTextBlockThumb、GroupはItemsControlThumb

namespace _20221225_ItemsControlThumbReSize
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
