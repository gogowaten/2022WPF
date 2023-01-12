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
using System.Runtime.Serialization;
using System.Text;
using System.Xml;


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

        public Data? MyData { get; set; }
        public TThumb()
        {
            //MyData = new Data();
            DataContext = MyData;

            SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
        }
        public void MySerialize<T>(string filePath, T data)
        {
            XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            DataContractSerializer serializer = new(typeof(T));
            using var writer = XmlWriter.Create(filePath, settings);
            try { serializer.WriteObject(writer, data); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void MySerializeData(string filePath, Data data)
        {
            XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            DataContractSerializer serializer = new(typeof(Data));
            using var writer = XmlWriter.Create(filePath, settings);
            try { serializer.WriteObject(writer, data); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public T? MyDeserialize<T>(string filePath)
        {
            DataContractSerializer serializer = new(typeof(T));
            try
            {
                using XmlReader reader = XmlReader.Create(filePath);
                return (T?)serializer.ReadObject(reader);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return default;
            }
        }
    }
   
    public class TTImage : TThumb
    {
        public new DataImage? MyData { get; set; }
      
        private void SetTemplateAndBinding(DataImage data)
        {
            MyData = data;//順番大事、DataContextは後
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

    public class TTTextBlock : TThumb
    {
        public new DataMyText? MyData { get; set; }
        private void SetTemplateAndBinding(DataMyText data)
        {
            MyData = data;//順番大事、DataContextは後
            DataContext = MyData;
            FrameworkElementFactory factory = new(typeof(TextBlock));
            factory.SetValue(TextBlock.TextProperty, new Binding(nameof(MyData.MyText)));
            factory.SetValue(TextBlock.FontSizeProperty, new Binding(nameof(MyData.FontSize)));
            Template = new() { VisualTree = factory };
        }

        public TTTextBlock()
        {
            SetTemplateAndBinding(new DataMyText());
        }
        public TTTextBlock(DataMyText data)
        {
            SetTemplateAndBinding(data);
        }
    }
    public class TTRectangle : TThumb
    {
        public new DataRectangle? MyData { get; set; }
        private void SetTemplateAndBinding(DataRectangle data)
        {
            MyData = data;//順番大事、DataContextは後
            DataContext = MyData;
            FrameworkElementFactory factory = new(typeof(Rectangle));
            factory.SetValue(Rectangle.FillProperty, new Binding(nameof(MyData.FillBrush)));
            factory.SetValue(Rectangle.WidthProperty, new Binding(nameof(MyData.W)));
            factory.SetValue(Rectangle.HeightProperty, new Binding(nameof(MyData.H)));
            Template = new() { VisualTree = factory };
        }

        public TTRectangle()
        {
            SetTemplateAndBinding(new DataRectangle());
        }
        public TTRectangle(DataRectangle data)
        {
            SetTemplateAndBinding(data);
        }
    }

    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        private ItemsControl? MyItems;
        public new DataGroup? MyData { get; set; }
        public ObservableCollection<TThumb> Children { get; set; } = new();
        //protected override void SetTemplateAndBinding<T>(T data)
        //{
        //    FrameworkElementFactory factory = new(typeof(ItemsControl), "myitems");
        //    factory.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Canvas))));

        //    factory.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)) { Source = this });
        //    ControlTemplate template = new() { VisualTree = factory };
        //    this.Template = template;
        //    ApplyTemplate();
        //    MyItems = (ItemsControl)template.FindName("myitems", this);
        //    MyItems?.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)) { Source = this });
        //    //FrameworkElementFactory factory = new(typeof(ItemsControl));
        //    //factory.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Canvas))));

        //    //factory.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)) { Source = this });
        //    //Template = new() { VisualTree = factory };

        //}
        protected  void SetTemplateAndBinding(DataGroup data)
        {
            MyData = data;
            DataContext = MyData;
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
