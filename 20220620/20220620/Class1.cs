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
  

    public class ReGroupData
    {
        public static List<TTextBox> TTextBoxList = new();
        public ReGroupData(List<TTextBox> textBoxes)
        {
            TTextBoxList = textBoxes;
            
            foreach (var item in textBoxes)
            {
                item.ReGroupData = this;
            }
        }
        internal void RemoveItem(TTextBox textBox)
        {
            TTextBoxList.Remove(textBox);
        }
        
    }
    public class TTextBox : TextBox
    {
        public ReGroupData? ReGroupData;
        public TTextBox(string text)        {
            Text = text;            
        }
        public void RemoveItem()
        {
            this.ReGroupData?.RemoveItem(this);
        }
    }
}
