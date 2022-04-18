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
using System.Xml;
using System.Windows.Markup;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;


namespace _20220418_いろいろシリアル化
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data MyData;
        private Data Children1;
        private Data Children2;
        public MainWindow()
        {
            InitializeComponent();
            MyData = new();
            MyData.X = 111;
            MyData.Y = 222;
            MyData.RectangleGeometry = new(new(1, 2, 3, 4));
            MyData.KeyValuePairs = new Dictionary<int, string>() { { 333, "valu1" }, { 444, "value2" } };
            MyData.Points = new() { new(000, 555), new(666, 777) };
            MyData.Children.Add(new Data() { X = 0.1, Y = 0.2 });
            MyData.SolidColorBrush = Brushes.MediumAquamarine;
            MyData.LineSegment = new();
            MyData.GeometryCollection = new(new List<Geometry>() { new EllipseGeometry(), new LineGeometry() });
            MyData.CombinedGeometry = new(new LineGeometry(), new EllipseGeometry());
            MyData.PathGeometry = new PathGeometry();
            MyData.GeometryGroup = new GeometryGroup();
            MyData.ArcSegment = new ArcSegment();
            MyData.BezierSegment = new();
            MyData.PolyBezierSegment = new();
            MyData.PolyLineSegment = new();
            MyData.PolyQuadraticBezierSegment = new();
            MyData.QuadraticBezierSegment = new();
            MyData.LinearGradientBrush = new();
            MyData.Pen = new();

            MyCanvas.DataContext = MyData;

            DataSave($"E:\\MyData.xml", MyData);
            Data loadData = DataLoad($"E:\\MyData.xml");

        }
        private void DataSave(string fileName, Data data)
        {
            XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            XmlWriter writer;
            DataContractSerializer serializer = new(typeof(Data));
            using (writer = XmlWriter.Create(fileName, settings))
            {
                try
                {
                    serializer.WriteObject(writer, data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private Data DataLoad(string fileName)
        {
            DataContractSerializer serializer = new(typeof(Data));
            try
            {
                using XmlReader reader = XmlReader.Create(fileName); ;
                return (Data)serializer.ReadObject(reader);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
