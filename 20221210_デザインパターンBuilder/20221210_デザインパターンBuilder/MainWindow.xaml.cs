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

//Builder
//https://refactoring.guru/ja/design-patterns/builder

namespace _20221210_デザインパターンBuilder
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

    public class Director
    {
        public Director()
        {

        }

    }
    public interface Builder
    {
        void Reset();
        void SetFillBrush(Brush fillBrush);
        void GetProduct();
    }
    public class RectangleBuilder : Builder
    {
        private MyRectangle _rectangle = new();
        public RectangleBuilder()
        {
            Reset();
        }

        public void GetProduct()
        {
            
        }

        public void Reset()
        {
            _rectangle = new MyRectangle();
        }

        public void SetFillBrush(Brush fillBrush)
        {
            _rectangle.FillBrush = fillBrush;
        }
    }
    public class EllipseBuilder : Builder
    {
        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void SetFillBrush(Brush fillBrush)
        {
            throw new NotImplementedException();
        }
    }
    public class MyRectangle
    {
        public Brush? FillBrush { get; set; }
        public double LineWidth { get; set; }
        public MyRectangle() { }
    }
    public class MyEllipse
    {
        public Brush? FillBrush { get; set; }
        public double LineWidth { get; set; }
        public MyEllipse() { }
    }
}
