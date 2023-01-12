using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace _20221224
{
    public class Item : Thumb, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public DType DType { get; set; }
        public Data MyData { get; set; }
        public Item()
        {
            DType = DType.None;
            MyData = new Data();
            DataContext = MyData;
            SetMyBinding();
        }
        public Item(Data data)
        {
            DType = DType.None;
            MyData = data;
            DataContext = MyData;
            SetMyBinding();
        }
        private void SetMyBinding()
        {
            SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
        }
    }
    public class IITextBlock : Item
    {

        public IITextBlock()
        {
            DType = DType.TextBlock;
            MyData = new Data();
            DataContext = MyData;
            
        }

    }
}
