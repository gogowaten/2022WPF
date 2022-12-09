using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace _20221208
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TTTextBlock? MyTTT;
        public MainWindow()
        {
            InitializeComponent();

            //GroupDataAdd();
            //Test1();
            ItemAddFromData();
            //ItemAddFromItem();
        }
        private IEnumerable<Data> MakeTextDatas2(int count, string text, Brush color, double size)
        {
            return Enumerable.Range(0, count).Select(a => new Data(TType.TextBlock)
            { Text = text + a, ForeColor = color, FontSize = size, X = a * 10, Y = a * 10 });
        }
        private void GroupDataAdd()
        {
            var neko = MakeTextDatas2(3, "neko", Brushes.Red, 30);
            Data gData = new(TType.Group) { Datas = new ObservableCollection<Data>(neko) };
            var gt = new TTGroup(gData);
            MyCanvas.Children.Add(gt);
        }
        private void ItemAddFromData()
        {
            Data data = new(TType.TextBlock) { Name = nameof(ItemAddFromData) + "text",
                Text = nameof(ItemAddFromData), X = 30, Y = 30, ForeColor = Brushes.MediumAquamarine, FontSize = 30 };
            TTTextBlock text = new(data);
            MyCanvas.Children.Add(text);
            data = new(TType.Rectangle) { Name = nameof(ItemAddFromData) + "rect",
                BackColor = Brushes.MediumPurple, Width = 20, Height = 300, X = 50, Y = 200 };
            TTRectangle rect = new(data);
            MyCanvas.Children.Add(rect);

        }
        private void ItemAddFromItem()
        {
            TTTextBlock text = new() { Name = nameof(ItemAddFromItem) + "_text", Text = nameof(ItemAddFromItem), X = 30, Y = 50, FontColor = Brushes.MediumOrchid, FontSize = 30 };
            MyCanvas.Children.Add(text);
            TTRectangle rect = new() { Name = nameof(ItemAddFromItem) + "_rect", Fill = Brushes.Lime, Width = 100, Height = 30, X = 50, Y = 200 };
            MyCanvas.Children.Add(rect);
        }
        private void Test1()
        {
            MyTTT = new TTTextBlock() { Name = nameof(Test1) + "text", X = 100, Y = 100, Text = "MyTTT", FontColor = Brushes.Gold, FontSize = 30 };
            //MyTTT.DragDelta += TT_DragDelta;
            MyCanvas.Children.Add(MyTTT);
            Data data = new(TType.Rectangle) { Name = nameof(Test1) + "rect", Width = 100, Height = 100, BackColor = Brushes.Blue, X = 200, Y = 50 };
            TTRectangle rr = new(data);
            MyCanvas.Children.Add(rr);

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var id = Guid.NewGuid();
            var neko = MyRectangle.X;
            var inu = Canvas.GetLeft(MyRectangle);
            inu = Canvas.GetLeft(MyTextBlock);
            neko = MyTextBlock.X;
        }

        private void TT_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                Canvas.SetLeft(tt, tt.X + e.HorizontalChange);
                Canvas.SetTop(tt, tt.Y + e.VerticalChange);
            }
        }
    }
}
