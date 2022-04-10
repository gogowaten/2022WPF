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
using System.Collections.ObjectModel;

namespace _20220410_CanvasBinging
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            
            UserControl1 userControl1 = new();
            MyGrid.Children.Add(userControl1);
            userControl1.Items.Add(new RectItem(10, 10, 100, 40, Brushes.Cyan));
            userControl1.Items.Add(new EllipseItem(100, 100, 100, 40, Brushes.Cyan));
        }
    }

}
