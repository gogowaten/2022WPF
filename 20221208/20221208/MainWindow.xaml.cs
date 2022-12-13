using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace _20221208
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TTTextBlock? MyTTT;
        public TTGroup MainGroup { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            //GroupDataAdd();
            //Test1();
            //ItemAddFromData();
            //ItemAddFromItem();

            

            //MainGroup = MakeTextGroup();
            //DataContext = MyRootThumb;
            //foreach (var item in MainGroup.Children)
            //{
               
            //    item.PreviewMouseLeftButtonDown += Item_PreviewMouseLeftButtonDown;

            //}
            //MyGroup_0.Add(MainGroup);
        }



        private void Item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TThumb tt) return;
            var clickedName = tt.Name;            
            var parentName = tt.ParentThumb?.Name;
            var parent = tt.ParentThumb;

            //var neko = parent?.ParentThumb?.ClickedItem?.Name;
            
        }

        private void A_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.X += e.HorizontalChange; tt.Y += e.VerticalChange;
            }
        }

        private IEnumerable<Data> MakeTextDatas2(int count, string text, Brush color, double size)
        {
            return Enumerable.Range(0, count).Select(a => new Data(TType.TextBlock)
            { Text = text + a, ForeColor = color, FontSize = size, X = a * 10, Y = a * 10, MyName = text + a });
        }
        private TTGroup MakeTextGroup()
        {
            var neko = MakeTextDatas2(3, "neko", Brushes.Red, 30);
            Data gData = new(TType.Group)
            {
                MyName = "Group",
                Datas = new ObservableCollection<Data>(neko)
            };
            return new TTGroup(gData);
        }
        private void ItemAddFromData()
        {
            Data data = new(TType.TextBlock)
            {
                MyName = nameof(ItemAddFromData) + "text",
                Text = nameof(ItemAddFromData),
                X = 30,
                Y = 30,
                ForeColor = Brushes.MediumAquamarine,
                FontSize = 30
            };
            TTTextBlock text = new(data);
            MyRootThumb.Children.Add(text);
            data = new(TType.Rectangle)
            {
                MyName = nameof(ItemAddFromData) + "rect",
                BackColor = Brushes.MediumPurple,
                Width = 20,
                Height = 300,
                X = 50,
                Y = 200
            };
            TTRectangle rect = new(data);
            MyRootThumb.Children.Add(rect);

        }
        private void ItemAddFromItem()
        {
            TTTextBlock text = new() { Name = nameof(ItemAddFromItem) + "_text", Text = nameof(ItemAddFromItem), X = 30, Y = 50, FontColor = Brushes.MediumOrchid, FontSize = 30 };
            MyRootThumb.Children.Add(text);
            TTRectangle rect = new() { Name = nameof(ItemAddFromItem) + "_rect", Fill = Brushes.Lime, Width = 100, Height = 30, X = 50, Y = 200 };
            MyRootThumb.Children.Add(rect);
        }
        private void Test1()
        {
            MyTTT = new TTTextBlock() { Name = nameof(Test1) + "text", X = 100, Y = 100, Text = "MyTTT", FontColor = Brushes.Gold, FontSize = 30 };
            //MyTTT.DragDelta += TT_DragDelta;
            MyRootThumb.Children.Add(MyTTT);
            Data data = new(TType.Rectangle) { MyName = nameof(Test1) + "rect", Width = 100, Height = 100, BackColor = Brushes.Blue, X = 200, Y = 50 };
            TTRectangle rr = new(data);
            MyRootThumb.Children.Add(rr);

        }
        private void TT_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                Canvas.SetLeft(tt, tt.X + e.HorizontalChange);
                Canvas.SetTop(tt, tt.Y + e.VerticalChange);
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.EnableThumb = MyGroup_0;
            //var act = MyRectangle.ActiveThumb;
            //var neko = Item_0_0_0_0.ParentThumb;
            //var inu = Canvas.GetLeft(MyRectangle);
            //inu = Canvas.GetLeft(MyTextBlock);

        }


        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MyRootThumb.EnableThumb = MyRootThumb;
        }
    }
}
