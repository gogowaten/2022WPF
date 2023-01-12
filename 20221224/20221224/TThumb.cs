using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Imaging;
using System.IO;



//前面移動、背面移動できた
//対象は選択状態のThumbすべて
//ZIndexは使わずに、要素のIndexを変更することで移動
//Indexの変更は削除と追加を繰り返して行ったけど、
//moveメソッドを使ったほうが良かったかも
namespace _20221224
{

    public abstract class TThumb : Thumb, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public Data MyData { get; set; }
        public TThumb()
        {
            MyData = new Data();
            DataContext = MyData;

            SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
        }
        protected abstract void SetTemplateAndBinding<T>(T data);
    }
    public class TTTextBlock : TThumb
    {
        public new DataText? MyData { get; private set; }
        public TTTextBlock()
        {
            SetTemplateAndBinding(new DataText());
        }
        public TTTextBlock(DataText data)
        {
            SetTemplateAndBinding(data);
        }
        protected override void SetTemplateAndBinding<T>(T data)
        {
            if (data is DataText dtext) { MyData = dtext; }
            DataContext = MyData;
            FrameworkElementFactory factory = new(typeof(TextBlock));
            factory.SetValue(TextBlock.TextProperty, new Binding(nameof(MyData.MyText)));
            Template = new() { VisualTree = factory };

        }

    }
    public class TTImage : TThumb
    {
        public new DataImage? MyData { get; set; }
        protected override void SetTemplateAndBinding<T>(T data)
        {
            if (data is DataImage dimage) { MyData = dimage; }
            DataContext = MyData;
            FrameworkElementFactory factory = new(typeof(Image));
            factory.SetValue(Image.SourceProperty, new Binding(nameof(MyData.MyImage)));
            Template = new() { VisualTree = factory };

        }
        public TTImage()
        {
            SetTemplateAndBinding(new DataImage());
        }
        public TTImage(DataImage data)
        {
            SetTemplateAndBinding(data);
        }
    }

    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        public new DataGroup? MyData { get; set; }
        public ObservableCollection<TThumb> Children { get; set; } = new();
        protected override void SetTemplateAndBinding<T>(T data)
        {
            FrameworkElementFactory factory = new(typeof(ItemsControl));
            factory.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Canvas))));
            factory.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)) { Source = this });
            Template = new() { VisualTree = factory };

        }
        public TTGroup()
        {
            SetTemplateAndBinding(new DataGroup());
        }
        public TTGroup(DataGroup data)
        {
            SetTemplateAndBinding(data);
        }
    }

}
