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
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace _20220508
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            #region T1
            TThumb1 thumb1 = new TThumb1();
            thumb1.Text = "serialTest";
            thumb1.X = 100;
            thumb1.Y = 100;
            thumb1.Geometry = new RectangleGeometry(new Rect(0, 20, 10, 40));
            thumb1.DataSave();
            #endregion T1
            T2Layer t2l = new();
            t2l.SetEditing();

        }
    }

    public enum DataType
    {
        Layer = 0,
        Group,
        TextBlock,
        Path,
        Image,

    }

    #region //データも含めたThumbのシリアライズテスト

    //データも含めたThumbのシリアライズテスト
    //シリアライズはこのままだとできないので、別クラスで行うようにしたけど
    //シリアライズする項目が重複になるので冗長
    //項目を増やすときは両方に各必要がある、めんどくさい
    public class TThumb1 : Thumb, INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private string _text = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        //public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public TThumb1? MyGroup { get; set; }
        public DataType DataType { get; set; }


        public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }

        public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }

        public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
        public Brush Brush { get; set; }
        public Geometry Geometry { get; set; }
        public TThumb1()
        {

        }
        public void DataSave()
        {
            //シリアライズ
            TThumb1Serial serial = new();
            serial.Save(this, $"E:\\MyData.xml");
        }
    }

    //シリアライズ用クラス
    [DataContract]
    [KnownType(typeof(RectangleGeometry)),
        KnownType(typeof(MatrixTransform))]
    public class TThumb1Serial
    {
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public double X { get; set; }
        [DataMember]
        public double Y { get; set; }
        [DataMember]
        public string Text { get; set; } = "";
        [DataMember]
        public Geometry? Geometry { get; set; }
        [DataMember]
        public Brush? Brush { get; set; }

        public TThumb1Serial() { }


        private void SubSave(string fileName)
        {
            System.Xml.XmlWriterSettings settings = new()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
                ConformanceLevel = System.Xml.ConformanceLevel.Fragment
            };
            System.Xml.XmlWriter xmlWriter;
            System.Runtime.Serialization.DataContractSerializer serializer = new(this.GetType());
            using (xmlWriter = System.Xml.XmlWriter.Create(fileName, settings))
            {
                try
                {
                    serializer.WriteObject(xmlWriter, this);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void Save(TThumb1 thumb, string fileName)
        {
            X = thumb.X; Y = thumb.Y; Text = thumb.Text;
            Geometry = thumb.Geometry;
            DataType = thumb.DataType;
            Brush = thumb.Brush;
            SubSave(fileName);
        }
    }
    #endregion //データも含めたThumbのシリアライズテスト


    public abstract class TThumb2 : Thumb
    {
        public T2Group? MyParentGroup;
        public T2Layer? MyLayer;
        public Data2? MyData;
        public TThumb2()
        {

        }
        public TThumb2(Data2 data):this()
        {
            
            this = new T2Layer();
        }

        public override string ToString()
        {
            string str = Name;
            if (string.IsNullOrEmpty(Name))
            {
                str = MyData.Text;
            }
            return $"{MyData.DataType}, {str}";
        }
        //テンプレート用
        protected FrameworkElementFactory MakeWaku()
        {
            //枠表示            
            FrameworkElementFactory rect = new(typeof(Rectangle));
            rect.SetValue(Rectangle.StrokeProperty, Brushes.Orange);
            if (this.MyData.DataType == DataType.Group)
            {
                rect.SetValue(Rectangle.StrokeProperty, Brushes.MediumBlue);
                rect.SetValue(Rectangle.StrokeThicknessProperty, 2.0);
            }
            Binding b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualWidthProperty);
            rect.SetBinding(Rectangle.WidthProperty, b);
            b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualHeightProperty);
            rect.SetValue(Rectangle.HeightProperty, b);
            return rect;
        }
    }
    public class T2Item : TThumb2
    {
        private Canvas? MyRootCanvas;
        public FrameworkElement? MyElement { get; private set; }
        public T2Item()
        {
            bool result = SetTemplate();
            if (result == false) { throw new Exception(); }
        }
        public T2Item(Data2 data) : this()
        {
            this.MyData = data;
            this.DataContext = MyData;

            switch (MyData.DataType)
            {
                case DataType.Layer:
                    break;
                case DataType.Group:
                    break;
                case DataType.TextBlock:
                    MyElement = new TextBlock() { FontSize = 24 };
                    MyElement.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
                    break;
                case DataType.Path:
                    break;
                case DataType.Image:
                    break;
                default:
                    break;
            }
            if (MyRootCanvas == null)
            {
                throw new ArgumentNullException("テンプレートがうまくできんかった");
            }
            MyRootCanvas.Children.Add(MyElement);
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
            //Canvasと自身のサイズを表示要素のサイズにバインドする
            Binding b = new() { Source = MyElement, Path = new PropertyPath(ActualWidthProperty) };
            MyRootCanvas.SetBinding(WidthProperty, b);
            this.SetBinding(WidthProperty, b);
            b = new() { Source = MyElement, Path = new PropertyPath(ActualHeightProperty) };
            MyRootCanvas.SetBinding(HeightProperty, b);
            this.SetBinding(HeightProperty, b);

        }

        #region テンプレート作成
        //単一要素表示用テンプレートに書き換える
        private bool SetTemplate()
        {
            //Canvas
            //  Element
            FrameworkElementFactory waku = MakeWaku();
            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyRootCanvas));

            baseCanvas.AppendChild(waku);
            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            MyRootCanvas = (Canvas)template.FindName(nameof(MyRootCanvas), this);
            if (MyRootCanvas != null) { return true; }
            else { return false; }
        }
        #endregion テンプレート作成


    }
    public class T2Group : TThumb2
    {
        public bool IsNowEditing { get; private set; }
        private ItemsControl? MyItemsControl;
        private ObservableCollection<TThumb2> Children = new();//内部用
        public ReadOnlyObservableCollection<TThumb2> Items;//公開用
        #region コンストラクタ

        public T2Group()
        {
            SetTemplate();
            Items = new(Children);
        }
        public T2Group(Data2 data) : this()
        {
            if (data.ChildrenData == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            MyData = data;
            foreach (var item in data.ChildrenData)
            {
                if (item.DataType == DataType.Group)
                {
                    T2Group group = new(item);
                    this.Children.Add(group);
                    group.MyParentGroup = this;                    
                }
                else if (item.DataType == DataType.Layer){
                    T2Group group = new(item);
                    this.Children.Add(group);
                    group.MyParentGroup = this;
                    group.MyLayer = this;
                }
                else
                {
                    T2Item item1 = new(item);
                    this.Children.Add(item1);
                }
            }

        }

        #endregion コンストラクタ

        public void SetEditing()
        {
            IsNowEditing = true;
        }

        #region テンプレート        
        //複数要素表示用テンプレートに書き換える
        private void SetTemplate()
        {
            FrameworkElementFactory itemsCanvas = new(typeof(Canvas));
            //canvas.SetValue(BackgroundProperty, Brushes.Transparent);
            itemsCanvas.SetValue(BackgroundProperty, Brushes.Beige);
            //アイテムズコントロール
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(itemsCanvas));
            //枠追加
            FrameworkElementFactory waku = MakeWaku();

            FrameworkElementFactory baseCanvas = new(typeof(Canvas));
            baseCanvas.AppendChild(itemsControl);
            baseCanvas.AppendChild(waku);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            MyItemsControl = (ItemsControl)template.FindName(nameof(MyItemsControl), this);

        }
        #endregion テンプレート

    }
    public class T2Layer : T2Group
    {
        public T2Layer() : base()
        {

        }

    }

    [DataContract]
    [KnownType(typeof(RectangleGeometry)),
        KnownType(typeof(MatrixTransform))]
    public class Data2 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _x;
        private double _y;
        private string _text = "";
        public TThumb1? ParentGroup { get; set; }

        [DataMember]
        public ObservableCollection<Data2>? ChildrenData { get; set; }
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
        [DataMember]
        public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
        [DataMember]
        public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
        [DataMember]
        public Brush? Brush { get; set; }
        [DataMember]
        public Geometry? Geometry { get; set; }
    }
}
