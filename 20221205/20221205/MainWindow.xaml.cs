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
using System.Windows.Controls.Primitives;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

using System.Windows.Markup;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;

namespace _20221205
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Test1();
            //TestSerial();

        }
        private void Test1()
        {
            //MyCanvas.Children.Add(new TThumb(new DDTextBlock()));
            DDTextBlock data = new() { FontColor = Brushes.MediumOrchid, FontSize = 20.0, Text = "TTTT" };
            MyCanvas.Children.Add(new TTTextBlock(data));
            DDRectangle rData = new() { Width = 100, Height = 30, Fill = Brushes.MediumAquamarine, X = 100, Y = 20 };
            MyCanvas.Children.Add(new TTRectAngle(rData));
            var neko = new TTRectAngle(rData);
            var inu = neko.MyData;
        }

        private void TT3_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (sender is TT3 tt)
            {
                Canvas.SetLeft(tt, Canvas.GetLeft(tt) + e.HorizontalChange);
                Canvas.SetTop(tt, Canvas.GetTop(tt) + e.VerticalChange);
            };
        }
        private void TestSerial()
        {
            //TT3 tt = new() { Text = "serialiserTest" };
            //JData tt = new() { Text ="22", Brush= Brushes.LightGray };
            //BBData tt = new BB() { MyProperty = Brushes.Yellow }.MakeDatas();
            BB tt = new BB() { MyProperty = Brushes.Yellow };

            Save($"E:\\MyDependencyData", tt);
        }
        private void Save(string filename, BB tt)
        {
            XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            DataContractSerializer serializer = new(typeof(BB));
            using (var stream = XmlWriter.Create(filename, settings))
            {
                try
                {
                    serializer.WriteObject(stream, tt);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyTT3.MyText;
            var inu = MyTT3.MyData.Text;
            MyTT3.MyText= "Testttttttttt";
            neko = MyTT3.MyText;
            inu = MyTT3.MyData.Text;

            neko = MyAAA.MyText;
            inu = MyAAA.MyData.Text;
            MyAAA.MyText = "プロパティ値を書き換えた";
            neko = MyAAA.MyText;
            inu = MyAAA.MyData.Text;
            MyAAA.MyData.Text = "Data書き換えた";
            neko = MyAAA.MyText;
            inu = MyAAA.MyData.Text;

        }
    }

    [KnownType(typeof(SolidColorBrush))]
    [KnownType(typeof(MatrixTransform))]
    public class JData : DependencyObject
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public Brush Brush { get; set; }

        [DataMember]
        public int MyProperty
        {
            get { return (int)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(nameof(MyProperty), typeof(int), typeof(JData), new PropertyMetadata(0));


    }
    
    public class BB : Thumb
    {
        [DataMember]
        public Brush MyProperty
        {
            get { return (Brush)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(nameof(MyProperty), typeof(Brush), typeof(BB), new PropertyMetadata(Brushes.MediumAquamarine));

        public BB()
        {
            DataContext = this;
            SetBinding(Thumb.BackgroundProperty, new Binding(nameof(MyProperty)));
        }
        public BBData MakeDatas()
        {
            BBData data = new() { Brush = MyProperty };
            return data;
        }
    }
    [DataContract]
    [KnownType(typeof(SolidColorBrush))]
    [KnownType(typeof(MatrixTransform))]
    public class BBData
    {
        [DataMember]
        public Brush Brush { get; set; } = Brushes.Black;
    }
}

