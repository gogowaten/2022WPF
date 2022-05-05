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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

//Canvasコントロールの子要素を動的に増減させたい
//https://teratail.com/questions/359699


namespace _20220408
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TThumb5 thumb5_1;
        TThumb5 thumb5_2;
        TTItem TTItem1;
        TTItem TTItem2;

        public MainWindow()
        {
            InitializeComponent();
            //Init5();
            TTItem1 = new(DataType.TextBlock) { Text = "testtti" };
            MyLayer2.AddItem(TTItem1);
            TTItem2 = new(DataType.TextBlock) { Text = "item2" };
            MyLayer2.AddItem(TTItem2);


            //TTGroup group = new();
            //TTLayer layer = new();
            //var neko = group.GetType();
            //var inu = layer.GetType();
            //var uma = layer is TTGroup;
            //var tako = layer is TTLayer;
            //var gtype = group is TTLayer;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //Check5();
            var neko = TTItem1.Text;
            var inu = TTItem1.DataType;
            var uma = TTItem1.Width;

        }
        #region thumb5
        private void Init5()
        {
            Data4 data1 = new(ThumbType.TextBlock, 0, 0, "Item1");
            thumb5_1 = new TThumb5(data1);
            MyLayer.AddItem(thumb5_1);

            Data4 data3 = new(ThumbType.TextBlock, 0, 0, "Item1-1");
            Data4 data4 = new(ThumbType.TextBlock, 100, 100, "Item1-2");
            Data4 data2 = new(ThumbType.Group, 40, 40, "Group1");
            data2.ChildrenData.Add(data3);
            data2.ChildrenData.Add(data4);
            thumb5_2 = new(data2);
            MyLayer.AddItem(thumb5_2);
            Loaded += MainWindow_Loaded;

            Data4 data6 = new(ThumbType.TextBlock, 0, 0, "Item2-1-1");
            Data4 data7 = new(ThumbType.TextBlock, 40, 40, "Item2-1-2");
            Data4 data8 = new(ThumbType.Group, 0, 0, "Group2-1");
            data8.ChildrenData.Add(data6);
            data8.ChildrenData.Add(data7);
            Data4 data9 = new(ThumbType.TextBlock, 0, 0, "Item2-2-1");
            Data4 data10 = new(ThumbType.TextBlock, 40, 40, "Item2-2-2");
            Data4 data11 = new(ThumbType.Group, 100, 100, "Group2-2");
            data11.ChildrenData.Add(data9);
            data11.ChildrenData.Add(data10);
            Data4 data12 = new(ThumbType.Group, 40, 160, "Group2");
            data12.ChildrenData.Add(data8);
            data12.ChildrenData.Add(data11);
            TThumb5 g2 = new(data12);
            MyLayer.AddItem(g2);
        }
        private void Check5()
        {
            var items = MyLayer.Items;
            var datas = MyLayer.MyData;
            var width = MyLayer.Width;
            var item = MyLayer.Items[0];
            var ww = item.ActualWidth;
            var icaw = thumb5_2.MyItemsControl.ActualWidth;
            var conten = thumb5_1.ContentPresenter.Content;
            var w1 = thumb5_1.ActualWidth;
            //var neko = thumb5_1.MyData;
            //thumb5_1.MyData.X += 10;
            //thumb5_1.MyData.Text = "kakikae";
            //thumb5_2.MyData.X += 20;
            thumb5_2.Items[0].MyData.X += 40;
            thumb5_2.Items[1].MyData.Y += 40;
            //thumb5_2.MyData.ChildrenData[0].Text = "kakikae2";
            TThumb5.AjustParentSize(thumb5_2.Items[0]);//位置調整、必須

        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var datatemp = (DataTemplate)Resources["dataTemplate"];
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //            【WPF】XAMLでテンプレートを定義とコードで定義は違う。 | 創造的プログラミングと粘土細工
            //http://pro.art55.jp/?eid=1150138

            var factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.BorderBrushProperty, Brushes.AliceBlue);
            factory.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            factory.SetValue(MarginProperty, new Thickness(5));
            factory.SetValue(PaddingProperty, new Thickness(10));//効かない
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("."));
            textBlockFactory.SetValue(PaddingProperty, new Thickness(10));//効かない
            textBlockFactory.SetValue(MarginProperty, new Thickness(10));
            factory.AppendChild(textBlockFactory);
            var dataTemplate2 = new DataTemplate { VisualTree = factory };

            Button button = (Button)sender;
            button.ContentTemplate = dataTemplate2;

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            //DataSave($"E:\\MyData.xml", MyLayer.MyData);//data4
            DataSave2($"E:\\MyData.xml", TTItem1);//data4

        }
        private static void DataSave(string fileName, Data4 data)
        {
            System.Xml.XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = System.Xml.ConformanceLevel.Fragment
            };
            System.Xml.XmlWriter xmlWriter;
            System.Runtime.Serialization.DataContractSerializer serializer = new(typeof(Data4));
            using (xmlWriter = System.Xml.XmlWriter.Create(fileName, settings))
            {
                try
                {
                    serializer.WriteObject(xmlWriter, data);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Data4 d = DataLoad($"E:\\MyData.xml");
            foreach (var item in d.ChildrenData)
            {
                TThumb5 t = new(item);
                var neko = t.MyData;
                MyLayer.AddItem(new TThumb5(item));
            }

        }
        private static Data4 DataLoad(string fileName)
        {
            System.Runtime.Serialization.DataContractSerializer serializer = new(typeof(Data4));
            try
            {
                using System.Xml.XmlReader reader = System.Xml.XmlReader.Create(fileName);
                return (Data4)serializer.ReadObject(reader);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private static void DataSave2(string fileName, TThumb6 thumb)
        {
            System.Xml.XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = System.Xml.ConformanceLevel.Fragment
            };
            System.Xml.XmlWriter xmlWriter;
            System.Runtime.Serialization.DataContractSerializer serializer = new(typeof(TThumb6));
            using (xmlWriter = System.Xml.XmlWriter.Create(fileName, settings))
            {
                try
                {
                    serializer.WriteObject(xmlWriter, thumb);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            Data4 d = new(ThumbType.Path, 0, 0);
            double w = double.Parse(EllipseWidth.Text);
            double h = double.Parse(EllipseHeight.Text);
            d.Geometry = new EllipseGeometry(new Rect(0, 0, w, h));
            TThumb5 t = new(d);
            MyLayer.AddItem(t);
        }

        private void TextBlockButton_Click(object sender, RoutedEventArgs e)
        {
            Data4 data4 = new(ThumbType.TextBlock, 0, 0, MyTextBox.Text);
            TThumb5 thumb5 = new(data4);
            MyLayer.AddItem(thumb5);
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            MyLayer.RemoveAllItem();
        }
    }

    public class AThumb : Thumb
    {
        public ContentControl MyContent { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public AThumb(UIElement element, double x = 0, double y = 0)
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(ContentControl), nameof(MyContent));
            this.Template = template;
            this.ApplyTemplate();
            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);
            MyContent.Content = element;
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);

        }

        public void AddDragDelta()
        {
            this.DragDelta += AThumb_DragDelta;
        }
        public void RemoveDragDelta()
        {
            this.DragDelta -= AThumb_DragDelta;
        }
        private void AThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Canvas.SetLeft(this, X + e.HorizontalChange);
            //Canvas.SetTop(this, Y + e.VerticalChange);
        }
    }
    public class GThumb : Thumb
    {
        public ObservableCollection<AThumb> Items { get; set; } = new();
        public IItemsControl BasePanel { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public GThumb(double x = 0, double y = 0)
        {
            this.X = x; this.Y = y;
            this.DataContext = this;
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(IItemsControl), nameof(BasePanel));
            this.Template = template;
            this.ApplyTemplate();
            BasePanel = (IItemsControl)template.FindName(nameof(BasePanel), this);
            BasePanel.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            //Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            AddDragDelta();
        }

        public void AddItem(AThumb item)
        {
            Items.Add(item);
        }
        public void AddDragDelta()
        {
            this.DragDelta += GThumb_DragDelta;
        }
        public void RemoveDragDelta()
        {
            this.DragDelta -= GThumb_DragDelta;
        }
        private void GThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, X + e.HorizontalChange);
            Canvas.SetTop(this, Y + e.VerticalChange);
        }

    }
    public class TThumb : Thumb
    {
        public IItemsControl MyItems { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public TThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(IItemsControl), nameof(MyItems));
            this.Template = template;
            this.ApplyTemplate();
            MyItems = (IItemsControl)template.FindName(nameof(MyItems), this);
            MyItems.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(MyItems)));

            MyItems.Background = Brushes.Beige;

            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);

        }
        public TThumb(UIElement element, double x, double y) : this()
        {
            AddItem(element, x, y);
        }
        public void AddItem(UIElement element, double x, double y)
        {
            MyItems.Items.Add(element);
            Canvas.SetLeft(element, x); Canvas.SetTop(element, y);
            DragDelta += TThumb_DragDelta;
        }
        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }
    }

    public class IItemsControl : ItemsControl
    {

        public IItemsControl()
        {
            ItemsPanelTemplate template = new(new FrameworkElementFactory(typeof(Canvas)));
            this.ItemsPanel = template;

            Style style = new();
            this.ItemContainerStyle = style;
            style.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("X")));
            style.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Y")));

            //this.DataContext = this.ItemsSource;
        }
    }




}
