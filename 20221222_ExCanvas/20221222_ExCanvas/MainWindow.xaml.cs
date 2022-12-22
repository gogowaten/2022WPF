using System.Linq;
using System.Windows;
using System.Windows.Controls;

//ブログ記事
//WPF、自動サイズ調整するCanvas、子要素のサイズや位置の変更で更新 - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2022/12/22/163812

namespace _20221222_ExCanvas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Left = 200; this.Top = 200;
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MyExCanvas.Width = 200; MyExCanvas.Height = 100;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Width = 200; MyCanvas.Height = 100;
        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            MyExCanvas.Width = double.NaN; MyExCanvas.Height = double.NaN;
        }

        private void Button21_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Width = double.NaN; MyCanvas.Height = double.NaN;
        }

    }
    //c# - WPF: Sizing Canvas to contain its children - Stack Overflow
    //    https://stackoverflow.com/questions/55101074/wpf-sizing-canvas-to-contain-its-children

    //子要素をレンガのようにタイル状に敷き詰める | Do Design Space
    //    https://sakapon.wordpress.com/2010/11/18/bricktile/

    //レイアウト - WPF .NET Framework | Microsoft Learn
    //    https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/layout?view=netframeworkdesktop-4.8

    /// <summary>
    /// オートサイズのCanvas、子要素のサイズや位置の変更時にサイズ更新
    /// </summary>
    public class ExCanvas : Canvas
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (double.IsNaN(Width) && double.IsNaN(Height))
            {
                base.ArrangeOverride(arrangeSize);
                Size size = new();
                foreach (var item in Children.OfType<FrameworkElement>())
                {
                    double x = GetLeft(item) + item.ActualWidth;
                    double y = GetTop(item) + item.ActualHeight;
                    if (size.Width < x) size.Width = x;
                    if (size.Height < y) size.Height = y;
                }
                return size;
            }
            else
            {
                return base.ArrangeOverride(arrangeSize);
            }
        }
        //protected override Size MeasureOverride(Size constraint)
        //{
        //    foreach (var item in Children.OfType<FrameworkElement>())
        //    {
        //        var neko = item.DesiredSize.Width;
        //        var inu = item.ActualWidth;
        //    }
        //    return base.MeasureOverride(constraint);
        //}
    }


}
