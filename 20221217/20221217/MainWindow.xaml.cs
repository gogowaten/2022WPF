using System;
using System.Collections.Generic;
using System.Globalization;
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

//意味無し
namespace _20221217
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BBB bb1 = new();
            var neko = bb1.MyBool;
            bb1.MyProperty = new();
            var inu = bb1.MyBool;
            bb1.MyProperty = null;
            var uma = bb1.MyBool;
            bb1.MyBool = true;
        }
    }
    public class AAA : TextBlock
    {

        public bool MyBool
        {
            get { return (bool)GetValue(MyBoolProperty); }
            set { SetValue(MyBoolProperty, value); }
        }
        public static readonly DependencyProperty MyBoolProperty =
            DependencyProperty.Register(nameof(MyBool), typeof(bool), typeof(AAA), new PropertyMetadata(false));

    }
    public class BBB : AAA
    {
        //public TextBlock MyTextBlock
        //{
        //    get { return (TextBlock)GetValue(MyTextBlockProperty); }
        //    set { SetValue(MyTextBlockProperty, value); }
        //}
        //public static readonly DependencyProperty MyTextBlockProperty =
        //    DependencyProperty.Register(nameof(MyTextBlock), typeof(TextBlock), typeof(BBB), new PropertyMetadata(null));

        public AAA? MyProperty
        {
            get { return (AAA)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(nameof(MyProperty), typeof(AAA), typeof(AAA), new PropertyMetadata(null));

        public BBB()
        {
            DataContext = this;
            
            //SetBinding(MyBoolProperty, new Binding(nameof(MyProperty)) { Converter = new ConvAB() });
            SetBinding(MyPropertyProperty,new Binding(nameof(MyBool)) { Converter=new ConvAB() });
        }
    }

    public class ConvAB : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
