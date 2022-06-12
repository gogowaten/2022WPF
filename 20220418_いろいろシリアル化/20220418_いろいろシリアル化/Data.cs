using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

//20211218_シリアル化より
//2021WPF/20211218_シリアル化/20211218_シリアル化 at master · gogowaten/2021WPF
//https://github.com/gogowaten/2021WPF/tree/master/20211218_%E3%82%B7%E3%83%AA%E3%82%A2%E3%83%AB%E5%8C%96/20211218_%E3%82%B7%E3%83%AA%E3%82%A2%E3%83%AB%E5%8C%96


namespace _20220418_いろいろシリアル化
{
    [DataContract]
    [KnownType(typeof(Data)),
        KnownType(typeof(RectangleGeometry)),
        KnownType(typeof(MatrixTransform)),
        KnownType(typeof(LineSegment)),
        KnownType(typeof(FontFamily)),
        KnownType(typeof(SolidColorBrush)),
        KnownType(typeof(GeometryCollection)),
        KnownType(typeof(EllipseGeometry)),
        KnownType(typeof(LineGeometry)),
        KnownType(typeof(Path))]
    public class Data : INotifyPropertyChanged
    {
        #region 通知プロパティ
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _X;
        [DataMember]
        public double X { get => _X; set { if (_X == value) { return; } _X = value; OnPropertyChanged(); } }
        [DataMember]
        private double _Y;
        public double Y { get => _Y; set { if (_Y == value) { return; } _Y = value; OnPropertyChanged(); } }
        #endregion 通知プロパティ

        #region クラスの頭で属性の宣言が必要系
        [DataMember]
        public SolidColorBrush SolidColorBrush { get; set; }
        [DataMember]
        public LinearGradientBrush LinearGradientBrush { get; set; }
        [DataMember]
        public Pen Pen { get; set; }

        [DataMember]
        public RectangleGeometry RectangleGeometry { get; set; }
        [DataMember]
        public GeometryCollection GeometryCollection { get; set; }
        [DataMember]
        public CombinedGeometry CombinedGeometry { get; set; }
        [DataMember]
        public PathGeometry PathGeometry { get; set; }
        [DataMember]
        public GeometryGroup GeometryGroup { get; set; }
        
        [DataMember]
        public LineSegment LineSegment { get; set; }
        [DataMember]
        public ArcSegment ArcSegment { get; set; }
        [DataMember]
        public BezierSegment BezierSegment { get; set; }
        [DataMember]
        public PolyBezierSegment PolyBezierSegment { get; set; }
        [DataMember]
        public PolyLineSegment PolyLineSegment { get; set; }
        [DataMember]
        public PolyQuadraticBezierSegment PolyQuadraticBezierSegment { get; set; }
        [DataMember]
        public QuadraticBezierSegment QuadraticBezierSegment { get; set; }

        [DataMember]
        public PathFigure PathFigure { get; set; }

        

        #endregion クラスの頭で属性の宣言が必要系

        #region コレクション系
        [DataMember]
        public ObservableCollection<Data> Children { get; set; } = new();
        [DataMember]
        public List<Point> Points { get; set; } = new();
        [DataMember]
        public Dictionary<int, string> KeyValuePairs { get; set; } = new();
        #endregion コレクション系

        #region Cursorがシリアル化できないとかなんとかエラーになる
        //public Path Path { get; set; } = new();
        //Polygon, PathSegment, Polyline, Line
        #endregion Cursorがシリアル化できないとかなんとかエラーになる

        //XmlLanguageがどうのこうのエラー
        //FontFamily
    }
}
