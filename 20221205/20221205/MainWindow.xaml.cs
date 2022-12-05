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

namespace _20221205
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
            //MyCanvas.Children.Add(new TThumb(new DDTextBlock()));
            DDTextBlock data = new() { FontColor = Brushes.MediumOrchid, FontSize = 20.0,Text="TTTT" };
            MyCanvas.Children.Add(new TTTextBlock(data));
        }

     
    }
}
