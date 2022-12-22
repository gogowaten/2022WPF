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

namespace _20221222_ExCanvas
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

    public class ExCanvas : Canvas
    {

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    if (double.IsNaN(Width) && double.IsNaN(Height))
        //    {
        //        base.MeasureOverride(constraint);//これを実行してから計算
        //        Size size = new();
        //        foreach (var item in Children.OfType<FrameworkElement>())
        //        {
        //            double x = GetLeft(item) + item.DesiredSize.Width;//ActualWidthは更新されていない
        //            double y = GetTop(item) + item.DesiredSize.Height;
        //            if (size.Width < x) size.Width = x;
        //            if (size.Height < y) size.Height = y;
        //        }
        //        return size;
        //    }
        //    else { return base.MeasureOverride(constraint); }
        //}

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);
            Size size = new();
            foreach (var item in Children.OfType<FrameworkElement>())
            {
                double x = GetLeft(item) + item.DesiredSize.Width;//ActualWidthは更新されていない
                double y = GetTop(item) + item.DesiredSize.Height;
                if (size.Width < x) size.Width = x;
                if (size.Height < y) size.Height = y;
            }
            return size;
            //return base.ArrangeOverride(arrangeSize);

        }

    }


}
