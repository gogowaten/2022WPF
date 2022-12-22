using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _20221222_ExCanvas_Thumb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //MyExCanvas.Children.Add(AddTest(2000));//多少カクつく
            //MyExCanvas.Children.Add(AddTest(200));//全く問題ない

        }
        private Thumb MakeThumb(double left, double top)
        {
            Thumb t = new() { Width = 100, Height = 30 };
            Canvas.SetLeft(t, left); Canvas.SetTop(t, top);
            t.DragDelta += T_DragDelta;
            return t;
        }
        private void AddExCanvas()
        {
            ExCanvas ex = new();
            //ex.Children.Add(MakeThumb());
        }
        private ExCanvas AddTest(int count)
        {
            ExCanvas ex = new() { Background = Brushes.Gold };
            for (int i = 0; i < count; i++)
            {
                ex.Children.Add(MakeThumb(i, i));
            }
            return ex;
        }

        private void T_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb t)
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
                Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            }

        }
    }

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
