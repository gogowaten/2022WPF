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
}
