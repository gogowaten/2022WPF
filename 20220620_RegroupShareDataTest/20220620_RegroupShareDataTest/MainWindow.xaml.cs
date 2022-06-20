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

namespace _20220620_RegroupShareDataTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TTextBox tt0 = new("tt000");
            TTextBox tt1 = new("tt111");
            TTextBox tt2 = new("tt222");

            ReGroupData share = new(new List<TTextBox>() { tt0, tt1, tt2 });
            var neko = tt0.ReGroupData;
            var inu = tt1.ReGroupData;

            tt1.RemoveItem();
            var uma = tt2.ReGroupData;
        }
    }
}
