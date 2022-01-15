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


namespace _20220115_移動後にThumbサイズ更新
{
    public partial class MainWindow : Window
    {

        int MyCount = 10;
        List<TThumb> MyList = new();
        public MainWindow()
        {
            InitializeComponent();

        }


        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var chi = MyLayer1.Children;
        }

        private void AddElement()
        {
            TThumb t = TThumb.CreateTextBlockThumb($"要素{MyCount}", 30, MyCount * 30, MyCount * 50, $"要素{MyCount}");
            MyList.Add(t);
            MyLayer1.AddThumb(t);
            MyCount++;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddElement();
        }

    }
}
