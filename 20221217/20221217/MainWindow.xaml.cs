using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
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
            Text1();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyCanvas.Children[0];
            MyCanvas.Children.RemoveAt(0);
            MyCanvas.Children.Add(neko);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Text1()
        {
            AA strings = new();
            strings.ListChange += Strings_ListChange;
            strings.Add("text1");
            strings.Add("text2");
        }

        private void Strings_ListChange(object? sender, ListChangeEventArgs e)
        {
            var neko = e.ChangedItem;

        }

    }


    #region Collection<T> クラス (System.Collections.ObjectModel) | Microsoft Learn

    //    https://learn.microsoft.com/ja-jp/dotnet/api/system.collections.objectmodel.collection-1?view=net-7.0

    public class AA : Collection<string>
    {
        public event EventHandler<ListChangeEventArgs>? ListChange;
        public event EventHandler<DragDeltaEventArgs>? ListChangeEvent;
        protected override void ClearItems()
        {
            base.ClearItems();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        protected override void InsertItem(int index, string item)
        {
            base.InsertItem(index, item);
            ListChange?.Invoke(this, new ListChangeEventArgs(item));
        }
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, string item)
        {
            base.SetItem(index, item);
            ListChange?.Invoke(this, new ListChangeEventArgs(item));
        }
    }

    public class ListChangeEventArgs : EventArgs
    {
        public readonly string ChangedItem;
        public ListChangeEventArgs(string change)
        {
            ChangedItem = change;
        }
    }
    #endregion Collection<T> クラス (System.Collections.ObjectModel) | Microsoft Learn


    public class OBB : ObservableCollection<string>
    {
        protected override void SetItem(int index, string item)
        {
            base.SetItem(index, item);
        }
        public OBB()
        {

        }

    }

    public class SizeChangeTest : Thumb
    {

        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(SizeChangeTest), new PropertyMetadata(""));

        public double MyFontSize
        {
            get { return (double)GetValue(MyFontSizeProperty); }
            set { SetValue(MyFontSizeProperty, value); }
        }
        public static readonly DependencyProperty MyFontSizeProperty =
            DependencyProperty.Register(nameof(MyFontSize), typeof(double), typeof(SizeChangeTest), new PropertyMetadata(20.0));


        public SizeChangeTest()
        {
            DataContext = this;
            FrameworkElementFactory grid = new(typeof(Grid));
            FrameworkElementFactory rect = new(typeof(Rectangle));
            FrameworkElementFactory textb = new(typeof(TextBlock));
            grid.AppendChild(rect);
            grid.AppendChild(textb);

            textb.SetValue(TextBlock.TextProperty, new Binding(nameof(MyText)));
            textb.SetValue(TextBlock.FontSizeProperty, new Binding(nameof(MyFontSize)));
            rect.SetValue(Panel.ZIndexProperty, 1);

            this.Template = new ControlTemplate() { VisualTree = grid };
        }
    }


}
