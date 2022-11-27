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
using System.Windows.Controls.Primitives;


//矢印図形クラスをThumbのテンプレートにしてドラッグ移動

namespace _20221127_図形矢印Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TTArrow.DragDelta += TTArrow_DragDelta;
        }

        private void TTArrow_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is TThumbArrow tt)
            {
                Canvas.SetLeft(tt, Canvas.GetLeft(tt) + e.HorizontalChange);
                Canvas.SetTop(tt, Canvas.GetTop(tt) + e.VerticalChange);
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            TTArrow.TX1 = 200.0;
        }
    }
}
