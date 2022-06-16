using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace _20220616_ComboBox_Colors_Binding
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Dictionary<string, Brush>? dict = MakeBrushesDictionary();
            MyStackPanel.DataContext = dict;
        }

        /// <summary>
        /// BrushesのBrushと名前一覧を取得
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, Brush> MakeBrushesDictionary()
        {
            //Brushesの情報(PublicとStaticなプロパティ全部)取得
            System.Reflection.PropertyInfo[]? brushInfos =
                typeof(Brushes).GetProperties(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Static);

            Dictionary<string, Brush> dict = new();
            foreach (var item in brushInfos)
            {
                if (item.GetValue(null) is not Brush b)
                {
                    continue;
                }
                dict.Add(item.Name, b);
            }
            return dict;
        }

    }
}
