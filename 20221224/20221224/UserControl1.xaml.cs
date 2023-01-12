using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _20221224
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl1 : UserControl
    {


        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(UserControl1),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure|
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DataMyText MyData { get; set; }

        public UserControl1()
        {
            InitializeComponent();
            MyData = new DataMyText();
            DataContext = MyData;
            SetBinding(MyTextProperty, new Binding(nameof(MyData.MyText)));

        }
        public UserControl1(DataMyText data)
        {
            InitializeComponent();
            MyData = data;
            DataContext = MyData;
        }

        private void UserControl1_Initialized(object? sender, EventArgs e)
        {
            //DataContext=this;
        }
    }
}
