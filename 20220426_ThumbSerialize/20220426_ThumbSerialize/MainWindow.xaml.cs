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

namespace _20220426_ThumbSerialize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TThumb5 thumb5_1;
        TThumb5 thumb5_2;

        public MainWindow()
        {
            InitializeComponent();

            Data4 data1 = new(ThumbType.TextBlock, 10, 20, "Test1");
            thumb5_1 = new TThumb5(data1);
            MyLayer.AddItem(thumb5_1);

            Data4 data3 = new(ThumbType.TextBlock, 0, 0, "Test2");
            Data4 data4 = new(ThumbType.TextBlock, 0, 30, "Test3");
            Data4 data2 = new(ThumbType.Group, 20, 40);
            data2.ChildrenData.Add(data3);
            data2.ChildrenData.Add(data4);
            thumb5_2 = new(data2);
            MyLayer.AddItem(thumb5_2);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var items = MyLayer.Items;
            var datas = MyLayer.MyData;

            var neko = thumb5_1.MyData;
            thumb5_1.MyData.X += 10;
            thumb5_1.MyData.Text = "kakikae";
            thumb5_2.MyData.X += 20;
            thumb5_2.MyData.ChildrenData[0].Text = "kakikae2";
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            DataSave($"E:\\MyData.xml", MyLayer.MyData);
        }
        private void DataSave(string fileName, Data4 data)
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
        private Data4 DataLoad(string fileName)
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
}
