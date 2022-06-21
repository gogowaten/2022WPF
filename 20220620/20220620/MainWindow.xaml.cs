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

namespace _20220620
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

        private void MyButtonAddText_Click(object sender, RoutedEventArgs e)
        {
            Data1 data1 = new(DataType.TextBlock);
            data1.Text = MyTextBoxText.Text;
            if (MyMainItemsControl.MyActiveMovableThumb is TThumb1 tt)
            {
                data1.X = tt.MyData.X + 32;
                data1.Y = tt.MyData.Y + 32;
            }
            MyMainItemsControl.AddItem(data1);

        }

        private void MyButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            MyMainItemsControl.MyEditingGroup?.RemoveThumb2(MyMainItemsControl.MyActiveMovableThumb);
        }
    }
}
