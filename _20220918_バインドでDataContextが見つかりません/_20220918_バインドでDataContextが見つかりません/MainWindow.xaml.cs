using System.Windows;

namespace _20220918_バインドでDataContextが見つかりません
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class Data
    {
        public string TextAAA { get; set; }
        public string TextBBB { get; set; } = "確認用BBB";
        public string TextCCC { get; set; }

        public Data() { TextCCC = "確認用CCC"; }
        public Data(string textAAA, string textBBB,string textCCC)
        {
            TextAAA = textAAA;
            TextBBB = textBBB;
            TextCCC = textCCC;
        }
    }
}
