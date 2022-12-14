using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
//c# - WPF:キャンバスのサイズを変更して子を含める
//https://stackoverflow.com/questions/55101074/wpf-sizing-canvas-to-contain-its-children

namespace _20221214_CanvasSizeMeasure
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
    public class MyCanvas : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            Size size = new();
            foreach (var item in Children.OfType<FrameworkElement>())
            {
                var x = GetLeft(item) + item.Width;
                var y = GetTop(item) + item.Height;
                if (!double.IsNaN(x) && size.Width < x)
                {
                    size.Width = x;
                }
                if (!double.IsNaN(y) && size.Height < y)
                {
                    size.Height = y;
                }
            }
            return size;
        }
      
    }
}
