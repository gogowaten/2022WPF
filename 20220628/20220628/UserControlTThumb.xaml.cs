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

namespace _20220628
{
    /// <summary>
    /// UserControlTThumb.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControlTThumb : UserControl
    {
        public Data? Data { get; set; }
        public UserControlTThumb(Data? data = null)
        {
            InitializeComponent();

            this.Data = data;
            DataContext = Data;
            
        }
        
    }
}
