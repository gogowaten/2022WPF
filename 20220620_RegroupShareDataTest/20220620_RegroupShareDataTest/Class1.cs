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

namespace _20220620_RegroupShareDataTest
{
    public class ReGroupData
    {
        public static List<TTextBox> TTextBoxList = new();
        public ReGroupData(List<TTextBox> textBoxes)
        {
            TTextBoxList = textBoxes;

            foreach (var item in textBoxes)
            {
                item.MyReGroupData = this;
            }
        }
        internal void RemoveItem(TTextBox textBox)
        {
            TTextBoxList.Remove(textBox);
        }

    }
    public class TTextBox : TextBox
    {
        public ReGroupData? MyReGroupData;
        public TTextBox(string text)
        {
            Text = text;
        }
        public void RemoveItem()
        {
            this.MyReGroupData?.RemoveItem(this);
        }
    }

    public class AAA
    {
        public List<string>? MyShare;
        public string MyText;
        public AAA(string text)
        {
            MyText = text;
        }
    }

    public class BBB
    {
        public List<string>? MyShare = new();
        public string MyText;
        public BBB(string text)
        {
            MyText = text;
        }
    }
}
