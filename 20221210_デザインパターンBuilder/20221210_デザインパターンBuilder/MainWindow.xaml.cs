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

//なんか違う、これじゃないみたい

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
        private Builder Builder;
        public Director(Builder builder)
        {
            this.Builder = builder;
        }
        public void Create()
        {
            Builder.SetFillBrush(Brushes.Red);
        }

    }
    public interface Builder
    {
        void Reset();
        void SetFillBrush(Brush fillBrush);
        //void GetProduct();
    }
    public class RectangleBuilder : Builder
    {
        public MyRectangle Product { get; private set; }
        public RectangleBuilder()
        {
            Product = new MyRectangle();
            Reset();
        }

        //public MyRectangle GetProduct { get; }
        
        public void Reset()
        {
            Product = new();
        }

        public void SetFillBrush(Brush fillBrush)
        {
            Product.FillBrush = fillBrush;
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
