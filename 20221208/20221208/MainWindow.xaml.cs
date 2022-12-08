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

namespace _20221208
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test1();
        }
        private void Test1()
        {
            
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyRectangle.X;
            var inu = Canvas.GetLeft(MyRectangle);
            inu = Canvas.GetLeft(MyTextBlock);
            neko = MyTextBlock.X;
        }

        private void TT_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if(sender is TThumb tt)
            {
                Canvas.SetLeft(tt, tt.X + e.HorizontalChange);
                Canvas.SetTop(tt, tt.Y + e.VerticalChange);
            }
        }
    }
}
