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
using System.Collections.Specialized;

namespace _20220508
{

    public enum DataType
    {
        None = 0,
        Layer,
        Group,
        TextBlock,
        Path,
        Image,

    }
    public enum DataTypeMain
    {
        Item = 1,
        Group
    }

    #region //データも含めたThumbのシリアライズテスト、終了、使わない

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


    //#region TThumb2






    //public abstract class TThumb2 : Thumb
    //{
    //    public T2Group? MyParentGroup;
    //    public T2Layer? MyLayer;
    //    public Data2? MyData;
    //    public TThumb2()
    //    {

    //    }
    //    public TThumb2(Data2 data) : this()
    //    {

    //    }


    //    public override string ToString()
    //    {
    //        string str = Name;
    //        if (string.IsNullOrEmpty(Name))
    //        {
    //            str = MyData.Text;
    //        }
    //        return $"{MyData.DataType}, {str}";
    //    }
    //    //テンプレート用
    //    protected abstract void SetTemplate();
    //    protected FrameworkElementFactory MakeWaku()
    //    {
    //        //枠表示            
    //        FrameworkElementFactory rect = new(typeof(Rectangle));
    //        rect.SetValue(Rectangle.StrokeProperty, Brushes.Orange);
    //        if (this.MyData.DataType == DataType.Group)
    //        {
    //            rect.SetValue(Rectangle.StrokeProperty, Brushes.MediumBlue);
    //            rect.SetValue(Rectangle.StrokeThicknessProperty, 2.0);
    //        }
    //        Binding b = new();
    //        b.Source = this;
    //        b.Path = new PropertyPath(ActualWidthProperty);
    //        rect.SetBinding(Rectangle.WidthProperty, b);
    //        b = new();
    //        b.Source = this;
    //        b.Path = new PropertyPath(ActualHeightProperty);
    //        rect.SetValue(Rectangle.HeightProperty, b);
    //        return rect;
    //    }
    //}
    //public class T2Item : TThumb2
    //{
    //    private Canvas? MyRootCanvas;
    //    public FrameworkElement? MyElement { get; private set; }
    //    public T2Item()
    //    {
    //        SetTemplate();
    //        //if (result == false) { throw new Exception(); }
    //    }
    //    public T2Item(Data2 data) : this()
    //    {
    //        this.MyData = data;
    //        this.DataContext = MyData;

    //        switch (MyData.DataType)
    //        {
    //            case DataType.Layer:
    //                break;
    //            case DataType.Group:
    //                break;
    //            case DataType.TextBlock:
    //                MyElement = new TextBlock() { FontSize = 24 };
    //                MyElement.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
    //                break;
    //            case DataType.Path:
    //                break;
    //            case DataType.Image:
    //                break;
    //            default:
    //                break;
    //        }
    //        if (MyRootCanvas == null)
    //        {
    //            throw new ArgumentNullException("テンプレートがうまくできんかった");
    //        }
    //        MyRootCanvas.Children.Add(MyElement);
    //        this.SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
    //        this.SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
    //        //Canvasと自身のサイズを表示要素のサイズにバインドする
    //        Binding b = new() { Source = MyElement, Path = new PropertyPath(ActualWidthProperty) };
    //        MyRootCanvas.SetBinding(WidthProperty, b);
    //        this.SetBinding(WidthProperty, b);
    //        b = new() { Source = MyElement, Path = new PropertyPath(ActualHeightProperty) };
    //        MyRootCanvas.SetBinding(HeightProperty, b);
    //        this.SetBinding(HeightProperty, b);

    //    }

    //    #region テンプレート作成
    //    //単一要素表示用テンプレートに書き換える

    //    protected override void SetTemplate()
    //    {
    //        //Canvas
    //        //  Element
    //        FrameworkElementFactory waku = MakeWaku();
    //        FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyRootCanvas));

    //        baseCanvas.AppendChild(waku);
    //        ControlTemplate template = new();
    //        template.VisualTree = baseCanvas;

    //        this.Template = template;
    //        this.ApplyTemplate();
    //        MyRootCanvas = (Canvas)template.FindName(nameof(MyRootCanvas), this);
    //    }

    //    #endregion テンプレート作成


    //}
    //public class T2Group : TThumb2
    //{
    //    public bool IsNowEditing { get; private set; }
    //    private ItemsControl? MyItemsControl;
    //    private ObservableCollection<TThumb2> Children = new();//内部用
    //    public ReadOnlyObservableCollection<TThumb2> Items;//公開用
    //    #region コンストラクタ

    //    public T2Group()
    //    {
    //        SetTemplate();
    //        Items = new(Children);
    //    }
    //    public T2Group(Data2 data) : this()
    //    {
    //        if (data.ChildrenData == null)
    //        {
    //            throw new ArgumentNullException(nameof(data));
    //        }
    //        MyData = data;
    //        foreach (var item in data.ChildrenData)
    //        {
    //            if (item.DataType == DataType.Group)
    //            {
    //                T2Group group = new(item);
    //                this.Children.Add(group);
    //                group.MyParentGroup = this;
    //            }
    //            else if (item.DataType == DataType.Layer)
    //            {
    //                T2Group group = new(item);
    //                this.Children.Add(group);
    //                group.MyParentGroup = this;
    //                //group.MyLayer = this;
    //            }
    //            else
    //            {
    //                T2Item item1 = new(item);
    //                this.Children.Add(item1);
    //            }
    //        }

    //    }

    //    #endregion コンストラクタ

    //    public void SetEditing()
    //    {
    //        IsNowEditing = true;
    //    }

    //    #region テンプレート        
    //    //複数要素表示用テンプレートに書き換える

    //    protected override void SetTemplate()
    //    {
    //        FrameworkElementFactory itemsCanvas = new(typeof(Canvas));
    //        //canvas.SetValue(BackgroundProperty, Brushes.Transparent);
    //        itemsCanvas.SetValue(BackgroundProperty, Brushes.Beige);
    //        //アイテムズコントロール
    //        FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
    //        itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
    //        itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(itemsCanvas));
    //        //枠追加
    //        FrameworkElementFactory waku = MakeWaku();

    //        FrameworkElementFactory baseCanvas = new(typeof(Canvas));
    //        baseCanvas.AppendChild(itemsControl);
    //        baseCanvas.AppendChild(waku);

    //        ControlTemplate template = new();
    //        template.VisualTree = baseCanvas;

    //        this.Template = template;
    //        this.ApplyTemplate();
    //        MyItemsControl = (ItemsControl)template.FindName(nameof(MyItemsControl), this);

    //    }
    //    #endregion テンプレート

    //}
    //public class T2Layer : T2Group
    //{
    //    public T2Layer() : base()
    //    {

    //    }

    //}

    //[DataContract]
    //[KnownType(typeof(RectangleGeometry)),
    //    KnownType(typeof(MatrixTransform))]
    //public class Data2 : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler? PropertyChanged;
    //    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    //    }
    //    private double _x;
    //    private double _y;
    //    private string _text = "";
    //    public TThumb1? ParentGroup { get; set; }

    //    [DataMember]
    //    public ObservableCollection<Data2>? ChildrenData { get; set; }
    //    [DataMember]
    //    public DataType DataType { get; set; }
    //    [DataMember]
    //    public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
    //    [DataMember]
    //    public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
    //    [DataMember]
    //    public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
    //    [DataMember]
    //    public Brush? Brush { get; set; }
    //    [DataMember]
    //    public Geometry? Geometry { get; set; }
    //}
    //#endregion TThumb2


    #region TThumb3
    public class TThumb3 : Thumb, INotifyPropertyChanged
    {
        #region フィールド

        #region 共通
        public TThumb3? MyParentGroup;
        public TThumb3? MyLayer;
        public Data3 MyData { get; private set; }
        #endregion

        #region アイテム専用
        public Canvas MyItemCanvas = new();
        public FrameworkElement? MyItemElement;
        #endregion アイテム専用

        #region グループとレイヤー専用
        public ItemsControl? MyItemsControl;
        private ObservableCollection<TThumb3> Children { get; set; }
        public ReadOnlyObservableCollection<TThumb3> Items { get; set; }
        public bool IsEditing;
        #region レイヤー専用
        //編集状態のThumbの管理
        private TThumb3? _NowEditingThumb;
        public TThumb3? NowEditingThumb
        {
            get { return _NowEditingThumb; }
            set
            {
                if (value == _NowEditingThumb) { return; }
                //ドラッグ移動イベントの付け外し
                if (_NowEditingThumb != null)
                {
                    foreach (var item in _NowEditingThumb.Children)
                    {
                        DragEventRemove(item);
                    }
                }
                if (value != null)
                {
                    foreach (var item in value.Children)
                    {
                        DragEventAdd(item);
                    }
                }

                _NowEditingThumb = value;
            }
        }
        #endregion レイヤー専用

        #endregion グループとレイヤー専用

        #region 通知プロパティ
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set { if (value == isSelected) { return; } isSelected = value; OnPropertyChanged(); }
        }
        #endregion 通知プロパティ

        //public ContentControl? MyContent { get;private set; }
        #endregion フィールド

        #region コンストラクタ

        public TThumb3()
        {
            MyData = new(DataType.None);
            Children = new();
            Items = new(Children);
            Loaded += (a, b) =>
            {
                var w = this.ActualWidth; var h = this.ActualHeight;
                MyParentGroup?.AjustLocate3();
            };
            PreviewMouseDown += TThumb3_PreviewMouseDown;

        }

        private void TThumb3_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var neko = e.Source;
            var inu = e.OriginalSource;
            IsSelected = !IsSelected;
            //e.Handled = true;
        }

        public TThumb3(Data3 data) : this()
        {
            MyData = data ?? throw new ArgumentNullException(nameof(data));
            MyInitialize(data);

        }
        private void MyInitialize(Data3 data)
        {
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
            this.SetBinding(Panel.ZIndexProperty, new Binding(nameof(MyData.Z)));

            //Template
            var waku = MakeWaku(data.DataTypeMain);
            //グループタイプの場合
            if (data.DataTypeMain == DataTypeMain.Group)
            {
                Items = new(Children);
                SetGroupThumbTemplate(waku);
                if (MyItemsControl == null) { throw new Exception("neko"); }

                //Binding
                this.MyItemsControl.DataContext = this;//子要素コレクションは自身をソース
                this.DataContext = MyData;//自身はデータをソースにする                                          

                //子要素の作成、追加
                foreach (var item in data.ChildrenData)
                {
                    TThumb3 child = new(item);
                    Children.Add(child);
                }
                //Layerなら編集状態にする
                if (data.DataType == DataType.Layer)
                {
                    NowEditingThumb = this;
                    IsEditing = true;
                }
            }

            //アイテムタイプ
            else
            {
                this.DataContext = MyData;//アイテムタイプのBindingソースはデータ
                SetItemThumbTemplate(waku);
                MakeAndSetItem(data);
            }
        }
        #endregion コンストラクタ

        #region テンプレート
        protected FrameworkElementFactory MakeWaku(DataTypeMain dataType)
        {
            //枠表示            
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Rectangle.StrokeProperty, Brushes.Cyan);
            if (dataType == DataTypeMain.Group)
            {
                waku.SetValue(Rectangle.StrokeProperty, Brushes.MediumOrchid);
                waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);
            }
            //枠サイズは自身のサイズに合わせる
            Binding b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualWidthProperty);
            waku.SetBinding(Rectangle.WidthProperty, b);
            b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualHeightProperty);
            waku.SetValue(Rectangle.HeightProperty, b);
            //枠表示は選択状態のときだけ
            b = new(nameof(IsSelected));
            b.Source = this;
            b.Converter = new MyValueConverterVisible();
            waku.SetValue(VisibilityProperty, b);
            return waku;
        }
        //単一要素表示用テンプレートに書き換える
        private void SetItemThumbTemplate(FrameworkElementFactory waku)
        {
            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyItemCanvas));
            baseCanvas.AppendChild(waku);
            waku.SetValue(Panel.ZIndexProperty, 1);//枠表示前面

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            MyItemCanvas = (Canvas)template.FindName(nameof(MyItemCanvas), this) ?? throw new ArgumentNullException(nameof(MyItemCanvas));

        }
        //複数要素表示用テンプレートに書き換える
        private bool SetGroupThumbTemplate(FrameworkElementFactory waku)
        {
            //アイテムズコントロール
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));

            FrameworkElementFactory itemsCanvas = new(typeof(Canvas));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(itemsCanvas));

            FrameworkElementFactory baseCanvas = new(typeof(Canvas));
            baseCanvas.AppendChild(waku);//枠追加
            baseCanvas.AppendChild(itemsControl);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            MyItemsControl = (ItemsControl)template.FindName(nameof(MyItemsControl), this);
            if (MyItemsControl == null) { return false; }
            else return true;
        }

        #endregion テンプレート

        #region グループ化、グループ解除
        public void MakeGroup(List<TThumb3> thumbs)
        {
            //異なるParentのThumb同士はグループ化できない
            TThumb3 parentThumb = thumbs[0]?.MyParentGroup ?? throw new ArgumentNullException("");
            foreach (var item in thumbs)
            {
                if (item.MyParentGroup != parentThumb)
                {
                    throw new ArgumentException("異なるParentのThumb同士はグループ化できない");
                }
            }
            //外す            
            foreach (var item in thumbs)
            {
                item.MyParentGroup?.RemoveItem(item);
                //item.RemoveItem();
            }
            //GroupThumb新規作成
            Data3 data = new(DataType.Group);
            //var rect = GetThumbsRectValues(thumbs);
            var rect = GetThumbsRectValues(thumbs);
            data.X = rect.x; data.Y = rect.y;
            foreach (var item in thumbs)
            {
                data.ChildrenData.Add(item.MyData);
            }
            TThumb3 group = new TThumb3(data);
            parentThumb.AddItem(group);
        }
        ////解除
        //public void Groupkaijo()
        //{
        //    if (MyData.DataType != DataType.Group) { return; }
        //    if (MyParentGroup is not TThumb3 parentG) { return; }
        //    //処理順番
        //    //グループからアイテムを削除
        //    //アイテムをParentに追加する
        //    //Parentからグループ削除
        //    for (int i = 0; i < Items.Count; i++)
        //    {
        //        TThumb3 item = Items[i];
        //        item.MyData.X += MyData.X;
        //        item.MyData.Y += MyData.Y;
        //        item.MyData.Z += MyData.Z;
        //        DragEventRemove(item);
        //        var uma = parentG.Children;
        //        parentG.AddItemInsert(item, item.MyData.Z + MyData.Z);
        //        uma = parentG.Children;
        //    }
        //    Children.Clear();
        //    MyData.ChildrenData.Clear();
        //    parentG.RemoveItem(this);

        //    var neko = this.Items;
        //    var inu = this.MyData;
        //    var pItems = parentG.Children;
        //    var pData = parentG.MyData;
        //}
        //解除
        public void Groupkaijo(TThumb3? group)
        {
            if (group.MyData.DataType != DataType.Group) { return; }

            //処理順番

            int groupZ = group.MyData.Z;
            for (int i = 0; i < group.Items.Count; i++)
            {
                TThumb3 item = group.Items[i];
                item.MyData.X += group.MyData.X;
                item.MyData.Y += group.MyData.Y;
                item.MyData.Z += groupZ;
                group.DragEventRemove(item);
                AddItemInsert(item, item.MyData.Z);

            }
            //group.Children.Clear();
            //group.MyData.ChildrenData.Clear();
            RemoveItem(group);
            group = null;//要る？
        }

        #endregion グループ化、グループ解除

        #region ドラッグ移動

        private void DragEventAdd(TThumb3 thumb)
        {
            thumb.DragDelta += thumb.TThumb3_DragDelta;
            thumb.DragCompleted += thumb.TThumb3_DragCompleted;
        }

        private void DragEventRemove(TThumb3 thumb)
        {
            thumb.DragCompleted -= thumb.TThumb3_DragCompleted;
            thumb.DragDelta -= thumb.TThumb3_DragDelta;
        }
        private void TThumb3_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //AjustLocate3(this.MyParentGroup);
            this.MyParentGroup?.AjustLocate3();
        }

        private void TThumb3_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }

        #endregion ドラッグ移動

        #region その他
        public override string ToString()
        {
            string ss = MyData.Text;
            if (string.IsNullOrEmpty(ss)) { ss = this.Name; }

            return $"{MyData?.DataType}, {ss}";
        }
        #endregion その他

        #region アイテム専用
        private void MakeAndSetItem(Data3 data)
        {
            switch (data.DataType)
            {
                case DataType.Layer:
                    break;
                case DataType.Group:
                    break;
                case DataType.TextBlock:
                    MyItemElement = new TextBlock() { FontSize = 20, Background = Brushes.Orange, Opacity = 1.0 };
                    MyItemElement.SetBinding(TextBlock.TextProperty, new Binding(nameof(this.MyData.Text)));
                    break;
                case DataType.Path:
                    break;
                case DataType.Image:
                    break;
                default:
                    break;
            }
            if (MyItemElement == null) { throw new ArgumentNullException(nameof(data)); }
            MyItemCanvas.Children.Add(MyItemElement);

            //Canvasと自身のサイズを表示要素のサイズにバインドする
            Binding b = new() { Source = MyItemElement, Path = new PropertyPath(ActualWidthProperty) };
            MyItemCanvas.SetBinding(WidthProperty, b);
            this.SetBinding(WidthProperty, b);
            b = new() { Source = MyItemElement, Path = new PropertyPath(ActualHeightProperty) };
            MyItemCanvas.SetBinding(HeightProperty, b);
            this.SetBinding(HeightProperty, b);


            //SetContextMenu();
        }

        #endregion アイテム専用


        #region グループとレイヤー専用
        #region アイテム追加と削除

        public void AddItem(TThumb3 itemThumb)
        {
            if (this.MyData.DataTypeMain == DataTypeMain.Group)
            {
                itemThumb.MyData.Z = this.Children.Count;
                this.Children.Add(itemThumb);
                this.MyData.ChildrenData.Add(itemThumb.MyData);
                itemThumb.MyParentGroup = this;

                if (MyData.DataType == DataType.Layer && MyParentGroup == null)
                {
                    itemThumb.MyLayer = this;
                }
                else { itemThumb.MyLayer = this.MyParentGroup; }

                //ドラッグ移動イベント付加
                //Parentが編集状態なら追加アイテム自身をドラッグ移動可能にする
                if (itemThumb.MyParentGroup.IsEditing)
                {
                    DragEventAdd(itemThumb);
                }

            }
            else throw new Exception("Itemを追加できるのはグループだけ");
        }
        public void AddItemInsert(TThumb3 itemThumb, int z)
        {
            if (this.MyData.DataTypeMain == DataTypeMain.Group)
            {

                this.Children.Insert(z, itemThumb); //指定場所に挿入

                this.MyData.ChildrenData.Insert(z, itemThumb.MyData);
                for (int i = z + 1; i < Children.Count; i++)
                {
                    //this.Children[i].MyData.Z++;
                    this.MyData.ChildrenData[i].Z++;
                }

                itemThumb.MyParentGroup = this;

                if (MyData.DataType == DataType.Layer && MyParentGroup == null)
                {
                    itemThumb.MyLayer = this;
                }
                else { itemThumb.MyLayer = this.MyParentGroup; }

                //ドラッグ移動イベント付加
                //Parentが編集状態なら追加アイテム自身をドラッグ移動可能にする
                if (itemThumb.MyParentGroup.IsEditing)
                {
                    DragEventAdd(itemThumb);
                }

            }
            else throw new Exception("Itemを追加できるのはグループだけ");
        }

        //Item削除
        public void RemoveItem(TThumb3 thumb)
        {
            int itemsCount = this.Items.Count;
            if (itemsCount == 0) { return; }

            //ZOrder、削除するThumbより上にあるThumbのZを-1する
            int z = thumb.MyData.Z;
            var ol = this.MyData.ChildrenData.OrderBy(x => x.Z).ToList();
            for (int i = z + 1; i < itemsCount; i++)
            {
                this.MyData.ChildrenData[i].Z--;
            }
            //削除
            //残り2個だった場合はグループ解除するので
            if (Items.Count == 2 && MyData.DataType == DataType.Group)
            {
                //処理順番
                //グループからアイテムを削除
                //アイテムをParentに追加する
                //Parentからグループ削除

                //グループからアイテムを削除
                Children.Remove(thumb);
                TThumb3 lastItem = Children[0];
                Children.Remove(lastItem);
                DragEventRemove(lastItem);
                lastItem.MyData.X += MyData.X;
                lastItem.MyData.Y += MyData.Y;
                //アイテムをParentに追加する
                MyParentGroup?.AddItemInsert(lastItem, z);
                //Parentからグループ削除
                MyParentGroup?.RemoveItem(this);
            }

            else if (this.MyData.DataTypeMain == DataTypeMain.Group)
            {
                this.Children.Remove(thumb);
                this.MyData.ChildrenData.Remove(thumb.MyData);
            }

            AjustLocate3();
        }

        #endregion アイテム追加と削除

        #region サイズ修正、位置修正
        //アイテム移動後に実行
        //アイテム追加時に実行
        /// <summary>
        /// 自身とchildrenの位置とサイズ修正、
        /// 画面内に収まるように、余白ができないようにする
        /// ドラッグ移動後などに実行
        /// </summary>
        protected void AjustLocate3()
        {
            //新しいRect取得、Parentがnullの場合は0が返ってくる
            (double x, double y, double w, double h) = GetThumbRectValues();

            //位置とサイズともに変化無ければ終了
            if (w == 0 && h == 0) { return; }
            if (x == MyData.X &&
                y == MyData.Y &&
                w == Width &&
                h == Height) { return; }

            //位置が変化していた場合は自身とItemsの位置修正
            if (x != 0 || y != 0)
            {
                //自身がGroupタイプなら位置修正する
                if (MyData.DataType == DataType.Group)
                {
                    MyData.X -= x; MyData.Y -= y;
                }
                //Itemの位置修正
                foreach (var item in Items)
                {
                    item.MyData.X -= x; item.MyData.Y -= y;
                }
            }
            //自身のサイズが異なっていた場合は修正
            if (w != Width || h != Height)
            {
                Width = w; Height = h;
            }
            //Parentを辿り、再帰処理する
            if (MyParentGroup != null) { MyParentGroup.AjustLocate3(); };
        }

        /// <summary>
        /// 指定ThumbのParentの位置とサイズを返す、Parentがない場合は0を返す
        /// </summary>
        /// <param name="thumb">指定Thumb</param>
        /// <returns></returns>
        //private static (double x, double y, double w, double h) GetParentRectValues(TThumb3 thumb)
        //{
        //    if (thumb.MyParentGroup == null) { return (0, 0, 0, 0); }

        //    double x = double.MaxValue; double y = double.MaxValue;
        //    double width = double.MinValue; double height = double.MinValue;
        //    foreach (var item in thumb.MyParentGroup.Items)
        //    {
        //        x = Math.Min(x, item.MyData.X);
        //        y = Math.Min(y, item.MyData.Y);
        //        width = Math.Max(width, item.MyData.X + item.Width);
        //        height = Math.Max(height, item.MyData.Y + item.Height);
        //    }
        //    width -= x; height -= y;
        //    return (x, y, width, height);
        //}

        private static (double x, double y, double w, double h) GetThumbsRectValues(List<TThumb3> thumbs)
        {
            if (thumbs.Count == 0) { return (0, 0, 0, 0); }

            double x = double.MaxValue; double y = double.MaxValue;
            double width = double.MinValue; double height = double.MinValue;
            foreach (var item in thumbs)
            {
                x = Math.Min(x, item.MyData.X);
                y = Math.Min(y, item.MyData.Y);
                width = Math.Max(width, item.MyData.X + item.Width);
                height = Math.Max(height, item.MyData.Y + item.Height);
            }
            width -= x; height -= y;
            return (x, y, width, height);
        }

        private (double x, double y, double w, double h) GetThumbRectValues()
        {
            if (Children.Count == 0) { return (0, 0, 0, 0); }

            double x = double.MaxValue; double y = double.MaxValue;
            double width = double.MinValue; double height = double.MinValue;
            foreach (var item in Children)
            {
                x = Math.Min(x, item.MyData.X);
                y = Math.Min(y, item.MyData.Y);
                width = Math.Max(width, item.MyData.X + item.Width);
                height = Math.Max(height, item.MyData.Y + item.Height);
            }
            width -= x; height -= y;
            return (x, y, width, height);
        }




        #endregion サイズ修正、位置修正

        #endregion グループとレイヤー専用
    }

    #region Data3

    [DataContract]
    [KnownType(typeof(RectangleGeometry)),
     KnownType(typeof(MatrixTransform))]
    public class Data3 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _x;
        private double _y;
        private int _z;
        private string _text = "";


        [DataMember]
        public ObservableCollection<Data3> ChildrenData { get; set; } = new();
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public DataTypeMain DataTypeMain { get; set; }
        [DataMember]
        public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
        [DataMember]
        public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
        [DataMember]
        public int Z { get => _z; set { if (_z == value) { return; } _z = value; OnPropertyChanged(); } }

        [DataMember]
        public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
        [DataMember]
        public Brush? Brush { get; set; }
        [DataMember]
        public Geometry? Geometry { get; set; }

        //public Data3() { }
        public Data3(DataType type)
        {
            DataType = type;
            if (type == DataType.Group || type == DataType.Layer)
            {
                DataTypeMain = DataTypeMain.Group;
            }
            else DataTypeMain = DataTypeMain.Item;
        }
        public override string ToString()
        {
            return $"{X},{Y},{Z},{DataType},{Text}";
        }
    }
    #endregion Data3


    public class MyValueConverterVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if (b) { return Visibility.Visible; }
            else { return Visibility.Collapsed; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion TThumb3


    public abstract class TThumb4 : Thumb, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #region 共通
        private Group4Base? _myParentGroup;
        public Group4Base? MyParentGroup
        {
            get => _myParentGroup;
            set { if (value == _myParentGroup) { return; } _myParentGroup = value; OnPropertyChanged(); }
        }
        private Layer4? _myLayer;
        public Layer4? MyLayer
        {
            get => _myLayer;
            set { if (value == _myLayer) { return; } _myLayer = value; OnPropertyChanged(); }
        }

        //クリックされたThumbが属するグループの中で移動対象となるThumb
        //更新タイミングはクリックされたときにしたけど、
        //ホントはそれ以外にも編集状態Thumbが切り替わったときに全部のThumbに行いたい？
        private TThumb4? _myMovableThumb;
        public TThumb4? MyMovableThumb
        {
            get => _myMovableThumb;
            set { if (value == _myMovableThumb) { return; } _myMovableThumb = value; OnPropertyChanged(); }
        }
        public Data4 MyData { get; set; }
        public List<TThumb4> RegroupThumbs = new();//再グループ用
        private bool _IsMySelected;

        public bool IsMySelected
        {
            get => _IsMySelected;
            set { if (value == _IsMySelected) { return; } _IsMySelected = value; OnPropertyChanged(); }
        }
        //移動対象フラグ、編集状態のGroupThumbの直下のThumb全てが対象になる
        //Parentが編集状態(IsEditing)ならtrue
        private bool _isMyMoveTarget;
        public bool IsMyMoveTarget
        {
            get => _isMyMoveTarget;
            set
            {
                if (_isMyMoveTarget == value) { return; }
                _isMyMoveTarget = value; OnPropertyChanged();
            }
        }

        #endregion

        public TThumb4(Data4 data)
        {
            MyData = data;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
            this.SetBinding(Panel.ZIndexProperty, new Binding(nameof(MyData.Z)));

        }
        #region その他
        public override string ToString()
        {
            string ss = MyData.Text;
            if (string.IsNullOrEmpty(ss)) { ss = this.Name; }

            return $"{MyData?.DataType}, {ss}";
        }
        #endregion その他

        //テンプレートの枠
        protected FrameworkElementFactory MakeWaku1(DataTypeMain dataType)
        {
            //選択状態枠表示            
            FrameworkElementFactory waku = new(typeof(Rectangle));
            //waku.SetValue(Rectangle.StrokeProperty, Brushes.SkyBlue);
            waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);

            //枠サイズは自身のサイズに合わせる
            Binding b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualWidthProperty);
            waku.SetBinding(Rectangle.WidthProperty, b);
            b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualHeightProperty);
            waku.SetValue(Rectangle.HeightProperty, b);

            ////枠表示バインド、選択状態のときだけ表示する
            //b = new(nameof(IsMySelected)) { Source = this };
            //b.Converter = new MyValueConverterVisible();
            //waku.SetValue(VisibilityProperty, b);

            return waku;
        }
        //protected FrameworkElementFactory MakeWaku2()
        //{
        //    Binding b;
        //    //移動可能Thumbに表示する枠
        //    //枠サイズは自身のサイズに合わせる
        //    FrameworkElementFactory movableRect = new(typeof(Rectangle));
        //    movableRect.SetValue(Shape.StrokeProperty, Brushes.LightGray);

        //    b = new() { Source = this };
        //    b.Path = new PropertyPath(ActualWidthProperty);
        //    movableRect.SetBinding(Shape.WidthProperty, b);
        //    b = new() { Source = this };
        //    b.Path = new PropertyPath(ActualHeightProperty);
        //    movableRect.SetBinding(Shape.HeightProperty, b);
        //    //枠表示バインド、選択状態のときだけ表示する
        //    b = new(nameof(IsMyMoveTarget)) { Source = this };
        //    b.Converter = new MyValueConverterVisible();
        //    movableRect.SetValue(VisibilityProperty, b);

        //    //b = new(nameof(Group4Base.IsEditing)) { Source = MyParentGroup };
        //    //b.Converter = new MyValueConverterVisible();
        //    //movableRect.SetValue(VisibilityProperty, b);
        //    ////movableRect.SetBinding(VisibilityProperty, b);
        //    return movableRect;
        //}


        #region ドラッグ移動

        protected void DragEventAdd(TThumb4 thumb)
        {
            thumb.DragDelta += thumb.TThumb_DragDelta;
            thumb.DragCompleted += thumb.TThumb_DragCompleted;
            thumb.IsMyMoveTarget = true;//移動対象にする
        }

        protected void DragEventRemove(TThumb4 thumb)
        {
            thumb.DragCompleted -= thumb.TThumb_DragCompleted;
            thumb.DragDelta -= thumb.TThumb_DragDelta;
            thumb.IsMyMoveTarget = false;//移動対象から外す
        }
        private void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.MyParentGroup?.AjustLocate3();
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }

        #endregion ドラッグ移動

        #region メソッド

        //自身が属する移動可能状態のグループThumbを取得
        //編集状態のThumb直下のThumb群から自身が属するものを取得
        public Group4? GetMyMoveTargetThumb()
        {
            if (MyParentGroup is Group4 group)
            {
                if (group.MyParentGroup?.IsMyEditing == true)
                {
                    return group;
                }
                else
                {
                    return group.GetMyMoveTargetThumb();
                }
            }
            else { return null; }
        }
        //Layer直下のThumb群から自身に関連するThumbを取得            
        public Group4Base? GetMyUnderLayerThumb(TThumb4? thumb)
        {
            if (thumb == null) { return null; }
            if (thumb.MyParentGroup?.MyData.DataType == DataType.Layer)
            {
                return thumb.MyParentGroup;
            }
            else
            {
                return GetMyUnderLayerThumb(thumb.MyParentGroup);
            }
        }
        public void Regroup()
        {
            //選択状態のThumbを基準に再グループ
            var target = (TThumb4?)GetMyMoveTargetThumb() ?? this;
            if (target == null) { return; }
            if (target.IsMySelected == true && target.RegroupThumbs.Count >= 2)
            {
                target.MyParentGroup?.MakeGroupFromChildren2(target.RegroupThumbs);
            }
        }

        #endregion メソッド
    }

    public class Item4 : TThumb4
    {
        public Canvas MyTemplateCanvas;
        public FrameworkElement MyItemElement;
        private bool _IsMyLastClicked;//最後にクリックされたフラグ
        public bool IsMyLastClicked
        {
            get => _IsMyLastClicked;
            set
            {
                if (_IsMyLastClicked == value) { return; }
                _IsMyLastClicked = value;
                OnPropertyChanged();
            }
        }
        public Item4(Data4 data) : base(data)
        {
            DataContext = MyData;
            MyTemplateCanvas = InitializeTemplate();
            //表示する要素をDataから作成
            MyItemElement = MakeElement(data);
            MyTemplateCanvas.Children.Add(MyItemElement);
            //Canvasと自身のサイズを表示要素のサイズにバインドする
            Binding b = new() { Source = MyItemElement, Path = new PropertyPath(ActualWidthProperty) };
            MyTemplateCanvas.SetBinding(WidthProperty, b);
            this.SetBinding(WidthProperty, b);
            b = new() { Source = MyItemElement, Path = new PropertyPath(ActualHeightProperty) };
            MyTemplateCanvas.SetBinding(HeightProperty, b);
            this.SetBinding(HeightProperty, b);



            Loaded += Item4_Loaded;
            PreviewMouseDown += Item4_PreviewMouseDown;
        }

        #region イベント

        //表示された直後にサイズが決まるのでParentのサイズを修正する
        private void Item4_Loaded(object sender, RoutedEventArgs e)
        {
            var neko = e.Source;
            var inu = e.OriginalSource;
            var uma = sender;
            MyParentGroup?.AjustLocate3();
        }
        //クリックされたとき
        private void Item4_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyLayer == null) { return; }
            //最後にクリックされたThumbに自身を登録する
            MyLayer.LastClickedItem = this;

            //編集状態直下の自身が属するグループ
            TThumb4 topMovable = (TThumb4?)GetMyMoveTargetThumb() ?? this;
            MyMovableThumb = topMovable;
            //自身が編集状態Thumbの範囲外だった場合
            if (MyLayer.NowEditingThumb != topMovable.MyParentGroup)
            {
                //編集状態ThumbをLayer直下のThumb群から自身が属するThumbに切り替える、
                //同時に選択リストはリセットされる
                Group4Base? nextEdit = GetMyUnderLayerThumb(this);
                if (nextEdit == null) { MessageBox.Show("次の編集状態Thumbが見つからん"); }
                MyLayer.NowEditingThumb = nextEdit;
            }
            else
            {
                //編集状態直下の自身が属するグループを選択状態リストに登録する
                //クリックだけのときは入れ替え
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    MyLayer.SelectThumbReplace(topMovable);
                }
                //ctrlキーが押されていたら複数選択状態にするので、追加
                else if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    MyLayer.SelectThumbAdd(topMovable);
                }
            }
        }

        #endregion イベント

        //単一要素表示用テンプレートに書き換える
        private Canvas InitializeTemplate()
        {
            //共通枠
            Binding b1 = new(nameof(IsMyLastClicked)) { Source = this };
            Binding b2 = new(nameof(IsMySelected)) { Source = this };
            MultiBinding mb = new();
            mb.Bindings.Add(b1); mb.Bindings.Add(b2);
            mb.Converter = new MyConverterItemWaku4();

            List<Brush> brushes = new()
            {
                My2ColorDashBrush(5,Colors.Red,Colors.White),
                My2ColorDashBrush(5,Colors.Lime,Colors.White),
                My2ColorDashBrush(5,Colors.LightGray,Colors.White)
            };
            mb.ConverterParameter = brushes;
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Item);
            //waku.SetBinding(BorderBrushProperty, mb);
            waku.SetBinding(Rectangle.StrokeProperty, mb);
            //waku.SetValue(Rectangle.StrokeDashArrayProperty, new DoubleCollection() { 2.0, 2.0 });

            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyTemplateCanvas));
            baseCanvas.AppendChild(waku);
            waku.SetValue(Panel.ZIndexProperty, 1);//枠表示前面
            //baseCanvas.AppendChild(MakeWaku2());//枠2追加
            ////Item専用枠、最後にクリックされた識別用
            //FrameworkElementFactory lastClickWaku = new(typeof(Rectangle));
            //lastClickWaku.SetValue(Panel.ZIndexProperty, 2);
            //lastClickWaku.SetValue(Rectangle.StrokeProperty, Brushes.OrangeRed);
            //lastClickWaku.SetValue(Rectangle.StrokeDashArrayProperty, new DoubleCollection() { 5.0, 5.0 });
            //baseCanvas.AppendChild(lastClickWaku);
            //Binding b = new() { Source = this, Path = new PropertyPath(WidthProperty) };
            //lastClickWaku.SetBinding(WidthProperty, b);
            //b = new() { Source = this, Path = new PropertyPath(HeightProperty) };
            //lastClickWaku.SetBinding(HeightProperty, b);
            ////枠表示バインド
            //b = new()
            //{
            //    Source = this,
            //    Path = new PropertyPath(nameof(IsMyLastClicked)),
            //    Converter = new MyValueConverterVisible()
            //};
            //lastClickWaku.SetBinding(VisibilityProperty, b);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            return (Canvas)template.FindName(nameof(MyTemplateCanvas), this);

        }
        #region 枠線ブラシ作成
        //        WPF、Rectangleとかに2色の破線(点線)枠表示 - 午後わてんのブログ
        //https://gogowaten.hatenablog.com/entry/2022/05/29/140321

        private ImageBrush My2ColorDashBrush(int thickness, Color c1, Color c2)
        {
            WriteableBitmap bitmap = MakeCheckPattern(thickness, c1, c2);
            ImageBrush brush = new(bitmap)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                ViewportUnits = BrushMappingMode.Absolute
            };
            return brush;
        }
        /// <summary>
        /// 指定した2色から市松模様のbitmapを作成
        /// </summary>
        /// <param name="cellSize">1以上を指定、1指定なら2x2ピクセル、2なら4x4ピクセルの画像作成</param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private static WriteableBitmap MakeCheckPattern(int cellSize, Color c1, Color c2)
        {
            int width = cellSize * 2;
            int height = cellSize * 2;
            var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            int stride = wb.Format.BitsPerPixel / 8 * width;
            byte[] pixels = new byte[stride * height];
            Color iro;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((y < cellSize & x < cellSize) | (y >= cellSize & x >= cellSize))
                    {
                        iro = c1;
                    }
                    else { iro = c2; }

                    int p = y * stride + x * 4;
                    pixels[p] = iro.B;
                    pixels[p + 1] = iro.G;
                    pixels[p + 2] = iro.R;
                    pixels[p + 3] = iro.A;
                }
            }
            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return wb;
        }
        #endregion 枠線ブラシ作成


        //表示する要素をDataから作成
        private FrameworkElement MakeElement(Data4 data)
        {
            FrameworkElement? element = null;
            switch (data.DataType)
            {
                case DataType.None:
                    break;
                case DataType.Layer:
                    break;
                case DataType.Group:
                    break;
                case DataType.TextBlock:
                    element = new TextBlock() { FontSize = 20 };
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
                    element.SetBinding(TextBlock.BackgroundProperty, new Binding(nameof(MyData.Background)));
                    element.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(MyData.Foreground)));
                    break;
                case DataType.Path:
                    break;
                case DataType.Image:
                    break;
                default:
                    break;
            }

            return element ?? throw new ArgumentNullException($"{nameof(element)}", $"dataから要素が作れんかった");
        }


    }

    public abstract class Group4Base : TThumb4
    {
        public ItemsControl MyItemsControl;
        protected ObservableCollection<TThumb4> Children { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb4> Items { get; set; }
        private bool _IsMyEditing { get; set; }
        public bool IsMyEditing
        {
            get => _IsMyEditing;
            set
            {
                if (_IsMyEditing == value) { return; }
                _IsMyEditing = value; OnPropertyChanged();
            }
        }

        #region コンストラクタ、初期化

        public Group4Base(Data4 data) : base(data)
        {
            Items = new(Children);
            MyItemsControl = SetGroupThumbTemplate();
            //Binding
            this.MyItemsControl.DataContext = this;//子要素コレクションは自身をソース
            this.DataContext = MyData;//自身はデータをソースにする                                          

            //子要素の作成、追加
            foreach (var item in data.ChildrenData)
            {
                Children.Add(new Item4(item));
            }

            Children.CollectionChanged += Children_CollectionChanged;
        }


        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?[0] is TThumb4 addItem)
                {
                    //Dataの追加
                    if (addItem.MyData.Z == Children.Count)
                    {
                        MyData.ChildrenData.Add(addItem.MyData);
                    }
                    else
                    {
                        MyData.ChildrenData.Insert(addItem.MyData.Z, addItem.MyData);
                    }
                    //Parentの指定
                    addItem.MyParentGroup = this;
                    //Layerの指定
                    addItem.MyLayer = this.MyLayer;
                    //Parent(自身)が編集状態なら追加アイテムを
                    if (this.IsMyEditing)
                    {
                        DragEventAdd(addItem);//ドラッグ移動可能にする
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems?[0] is TThumb4 removeItem)
                {
                    MyData.ChildrenData.Remove(removeItem.MyData);//Dataの削除
                    DragEventRemove(removeItem);
                }
            }
        }

        //複数要素表示用テンプレートに書き換える
        private ItemsControl SetGroupThumbTemplate()
        {
            //アイテムズコントロール
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));

            FrameworkElementFactory itemsCanvas = new(typeof(Canvas));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(itemsCanvas));

            Binding b1 = new(nameof(IsMyEditing)) { Source = this };
            Binding b2 = new(nameof(IsMySelected)) { Source = this };
            MultiBinding mb = new();
            mb.Bindings.Add(b1); mb.Bindings.Add(b2);
            mb.Converter = new MyConverterGroupWaku4();
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Group);
            waku.SetBinding(Rectangle.StrokeProperty, mb);
            waku.SetValue(Rectangle.StrokeDashArrayProperty, new DoubleCollection() { 5.0, 5.0 });
            FrameworkElementFactory baseCanvas = new(typeof(Canvas));
            baseCanvas.AppendChild(waku);//枠追加
            //baseCanvas.AppendChild(MakeWaku2());//枠2追加
            //baseCanvas.AppendChild(MakeWaku3());//枠3追加
            baseCanvas.AppendChild(itemsControl);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();

            return (ItemsControl)template.FindName(nameof(MyItemsControl), this)
                ?? throw new ArgumentNullException(nameof(MyItemsControl));
        }
        //protected FrameworkElementFactory MakeWaku3()
        //{
        //    Binding b;
        //    //移動対象Thumbに表示する枠
        //    //枠サイズは自身のサイズに合わせる
        //    FrameworkElementFactory waku = new(typeof(Rectangle));
        //    waku.SetValue(Shape.StrokeProperty, Brushes.Green);
        //    b = new() { Source = this };
        //    b.Path = new PropertyPath(ActualWidthProperty);
        //    waku.SetBinding(Shape.WidthProperty, b);
        //    b = new() { Source = this };
        //    b.Path = new PropertyPath(ActualHeightProperty);
        //    waku.SetBinding(Shape.HeightProperty, b);
        //    //枠表示バインド、編集状態のときだけ表示する
        //    b = new(nameof(IsEditing)) { Source = this };
        //    b.Converter = new MyValueConverterVisible();
        //    waku.SetValue(VisibilityProperty, b);

        //    return waku;
        //}
        #endregion コンストラクタ、初期化

        #region publucメソッド

        /// <summary>
        /// 自身のRect(位置とサイズ)をchildrenから取得
        /// </summary>
        /// <returns></returns>
        public (double x, double y, double w, double h) GetUnionRectForChildren()
        {
            if (Children.Count == 0) { return (0, 0, 0, 0); }

            double x = double.MaxValue; double y = double.MaxValue;
            double width = double.MinValue; double height = double.MinValue;
            foreach (var item in Children)
            {
                x = Math.Min(x, item.MyData.X);
                y = Math.Min(y, item.MyData.Y);
                width = Math.Max(width, item.MyData.X + item.Width);
                height = Math.Max(height, item.MyData.Y + item.Height);
            }
            width -= x; height -= y;
            return (x, y, width, height);
        }
        public void AjustLocate3()
        {
            //新しいRect取得、Parentがnullの場合は0が返ってくる
            (double x, double y, double w, double h) = GetUnionRectForChildren();

            //位置とサイズともに変化無ければ終了
            if (w == 0 && h == 0) { return; }
            if (x == MyData.X &&
                y == MyData.Y &&
                w == Width &&
                h == Height) { return; }

            //位置が変化していた場合は自身とItemsの位置修正
            if (x != 0 || y != 0)
            {
                //自身がGroupタイプなら位置修正する
                if (MyData.DataType == DataType.Group)
                {
                    MyData.X += x; MyData.Y += y;
                }
                //Itemの位置修正
                foreach (var item in Items)
                {
                    item.MyData.X -= x; item.MyData.Y -= y;
                }
            }
            //自身のサイズが異なっていた場合は修正
            if (w != Width || h != Height)
            {
                Width = w; Height = h;
            }
            //Parentを辿り、再帰処理する
            if (MyParentGroup != null) { MyParentGroup.AjustLocate3(); };
        }




        /// <summary>
        /// グループ解除、自身を解除、子要素は親要素へ移動＋選択状態にする
        /// </summary>
        public void Ungroup2()
        {
            if (MyParentGroup == null) { return; }
            //上にあるThumbZの底上げ
            for (int i = MyData.Z + 1; i < MyParentGroup.Children.Count; i++)
            {
                //削除対象子要素数-1ぶん底上げする
                MyParentGroup.Children[i].MyData.Z += Children.Count - 1;
            }
            //子要素に対してx,y,zの調整
            foreach (var item in Children)
            {
                item.MyData.X += MyData.X;
                item.MyData.Y += MyData.Y;
                item.MyData.Z += MyData.Z;//Zを削除対象のZぶん底上げする
            }

            //追加してから削除、しないと削除時点で要素数が2未満になった場合にそれも解除対象になってしまうから
            //解除後は子要素群を選択状態にするので今のはクリアする
            MyLayer?.SelectThumbsCleal();
            //子要素の処理
            foreach (var item in Children)
            {
                MyLayer?.SelectThumbAdd(item);//選択状態にする、必要ない
                MyParentGroup.InsertThumb(item);//Parentの子要素に挿入

                //再グループ用の情報を付加
                item.RegroupThumbs = Children.ToList();
            }
            MyParentGroup.RemoveThumb(this);//削除

            //兄弟の再グループリストから自身を削除
            foreach (var brothers in MyParentGroup.Children)
            {
                brothers.RegroupThumbs.Remove(this);
                if (brothers.RegroupThumbs.Count <= 1)
                {
                    brothers.RegroupThumbs.Clear();
                }
            }
        }

        /// <summary>
        /// 指定要素群からグループ作成して自身に挿入する
        /// </summary>
        /// <param name="thumbs">要素群は自身のChildrenから指定</param>
        /// <returns></returns>
        public bool MakeGroupFromChildren2(List<TThumb4>? thumbs)
        {
            if (thumbs == null || thumbs.Count < 2) { return false; }
            //指定要素群のParentが自身ではなかった場合は作成失敗
            foreach (var item in thumbs) { if (item.MyParentGroup != this) { return false; } }

            //新グループのX、Y、Zを要素群から計算取得
            var (x, y, minZ, maxZ) = GetXYZForNewGroup(thumbs);
            //新グループのZ = 要素群の最上位Z - (要素数 - 1)
            maxZ -= thumbs.Count - 1;
            //新グループのData作成
            Data4 data = new(DataType.Group);
            data.X = x; data.Y = y; data.Z = maxZ;

            //新グループ作成            
            Group4 group = new(data);//作成
            group.MyLayer = this.MyLayer;
            //Z順にソートした要素群作成
            var sortedThumbs = thumbs.OrderBy(x => x.MyData.Z).ToList();
            //新グループに要素群追加
            foreach (var item in sortedThumbs)
            {
                group.AddThumb(item);
                //再グループ用の情報をクリア
                item.RegroupThumbs.Clear();
            }
            //新グループを作成挿入してから、元の要素群削除、順番大切
            //Childrenに新グループを挿入
            InsertThumb(group);
            //Childrenから要素群削除
            foreach (var item in sortedThumbs) { RemoveThumb(item); }

            //子要素全体のZ調整、歯抜けになっているので数値を詰める
            for (int i = minZ; i < Children.Count; i++) { Children[i].MyData.Z = i; }

            return true;
        }

        private static (double x, double y, int minZ, int maxZ) GetXYZForNewGroup(List<TThumb4> thumbs)
        {
            double x = 0; double y = 0;//左上座標
            int maxZ = 0; int minZ = int.MaxValue;
            foreach (var item in thumbs)
            {
                x = Math.Min(x, item.MyData.X);
                y = Math.Min(y, item.MyData.Y);
                minZ = Math.Min(minZ, item.MyData.Z);//最下位Z
                maxZ = Math.Max(maxZ, item.MyData.Z);//最上位Z
            }
            return (x, y, minZ, maxZ);
        }

        //指定ThumbをChildrenに追加
        public virtual void AddThumb(TThumb4 thumb)
        {
            //コレクションに追加
            thumb.MyData.Z = Children.Count;
            Children.Add(thumb);
        }
        //指定ThumbをChildrenに挿入
        public virtual void InsertThumb(TThumb4 thumb)
        {
            InsertThumb(thumb, thumb.MyData.Z);
        }
        public virtual void InsertThumb(TThumb4 thumb, int z)
        {
            //コレクションに挿入
            if (z >= Children.Count)
            {
                AddThumb(thumb);
            }
            else
            {
                Children.Insert(z, thumb);
            }

        }

        //指定ThumbをChildrenから削除
        public virtual void RemoveThumb(TThumb4 thumb)
        {
            if (Children.Contains(thumb))
            {
                //コレクションから削除
                Children.Remove(thumb);
                //2未満ならグループ解除
                if (Children.Count < 2)
                {
                    Ungroup2();
                    //Ungroup();
                }
            }
            else
            {
                throw new AggregateException("グループ内に対象Thumbが見つからない");
            }

        }
        #endregion publucメソッド

        //Childrenに対してドラッグ移動イベントの付加と削除
        internal void AddDragEventForChildren()
        {
            foreach (var item in Children)
            {
                DragEventAdd(item);
                //item.IsMoveTarget = true;
            }
        }
        internal virtual void RemoveDragEventForChildren()
        {
            foreach (var item in Children)
            {
                DragEventRemove(item);
            }
        }

        //最下層まで全てのThumbのMyLayerの指定
        internal void SetMyLayer2(Layer4 layer)
        {
            this.MyLayer = layer;
            foreach (var item in Children)
            {
                item.MyLayer = layer;
                if (item is Group4Base group)
                {
                    group.SetMyLayer2(layer);
                }
            }
        }

    }

    public class Group4 : Group4Base
    {
        public Group4(Data4 data) : base(data) { }

    }
    public class Layer4 : Group4Base
    {
        #region 通知プロパティ        
        //今編集状態(子要素の移動可能)のGroup
        private Group4Base? _NowEditingThumb;
        public Group4Base? NowEditingThumb
        {
            get { return _NowEditingThumb; }
            set
            {
                if (_NowEditingThumb == value) { return; }
                //選択リストのリセット
                if (_SelectedThumbs.Count > 0)
                {
                    foreach (var item in _SelectedThumbs)
                    {
                        item.IsMySelected = false;
                    }
                    _SelectedThumbs.Clear();
                }
                //ドラッグ移動イベントの付け外し
                if (_NowEditingThumb != null)
                {
                    _NowEditingThumb.IsMyEditing = false;
                    _NowEditingThumb.RemoveDragEventForChildren();
                }

                _NowEditingThumb = value;

                if (_NowEditingThumb != null)
                {
                    _NowEditingThumb.IsMyEditing = true;
                    _NowEditingThumb.AddDragEventForChildren();
                }

            }
        }

        //最後にクリックされたThumb
        private Item4? _lastClickedItem;
        public Item4? LastClickedItem
        {
            get => _lastClickedItem;
            set
            {
                var mod = Keyboard.Modifiers == ModifierKeys.Shift;

                //格納しているThumbと同じか、nullが来たら変更する必要なし、終了
                if (_lastClickedItem == value || value == null) { return; }

                //古い方のIsLastClickedをfalseに変更してから
                if (_lastClickedItem != null)
                {
                    _lastClickedItem.IsMyLastClicked = false;
                }
                //新しい方のIsLastClickedをtrue
                value.IsMyLastClicked = true;
                //入れ替え
                _lastClickedItem = value;
            }
        }

        //選択状態のThumb群
        private readonly ObservableCollection<TThumb4> _SelectedThumbs = new();
        public ReadOnlyObservableCollection<TThumb4> SelectedThumbs;
        #endregion 通知プロパティ

        public Layer4(Data4 data) : base(data)
        {
            //Layerなので編集状態にする
            NowEditingThumb = this;
            IsMyEditing = true;

            _SelectedThumbs.CollectionChanged += SelectedThumbs_CollectionChanged;
            PreviewMouseLeftButtonDown += Layer4_PreviewMouseLeftButtonDown;
            SelectedThumbs = new(_SelectedThumbs);
        }
        #region メソッド

        //選択状態Thumbの追加
        public void SelectThumbAdd(TThumb4? thumb)
        {
            //同じThumbがないかチェックしてから追加
            if (thumb == null) { return; }
            if (SelectedThumbs.Contains(thumb)) { return; }
            _SelectedThumbs.Add(thumb);
        }
        //選択状態Thumbの削除
        public void SelecthumbRemove(TThumb4 thumb)
        {
            if (!SelectedThumbs.Contains(thumb)) { return; }
            _SelectedThumbs.Remove(thumb);
        }
        //選択状態Thumbの入れ替え
        public void SelectThumbReplace(TThumb4? thumb)
        {
            if (thumb == null) { return; }
            if (SelectedThumbs.Count == 1)
            {
                if (SelectedThumbs[0] != thumb)
                {
                    _SelectedThumbs.Remove(SelectedThumbs[0]);
                    _SelectedThumbs.Add(thumb);
                }
            }
            else if (SelectedThumbs.Count > 1)
            {
                foreach (var item in SelectedThumbs)
                {
                    item.IsMySelected = false;
                }
                _SelectedThumbs.Clear();
                _SelectedThumbs.Add(thumb);
            }
            else
            {
                _SelectedThumbs.Add(thumb);
            }
        }
        //選択状態Thumbのクリア
        public void SelectThumbsCleal()
        {
            if (_SelectedThumbs.Count == 0) { return; }
            foreach (var item in _SelectedThumbs)
            {
                item.IsMySelected = false;
            }
            _SelectedThumbs.Clear();
        }

        //追加
        public override void AddThumb(TThumb4 thumb)
        {
            //基底クラスのメソッドを実行
            base.AddThumb(thumb);
            //すべてのThumbのMyLayerに自身を指定する
            if (thumb.MyData.DataType == DataType.Group)
            {
                SetMyLayer2(this);
            }
            else if (thumb.MyData.DataTypeMain == DataTypeMain.Item)
            {
                thumb.MyLayer = this;
            }
        }

        #endregion メソッド

        private void Layer4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var neko = sender;
            var inu = e.OriginalSource;
            var uma = e.Source;
            //SelectedThumbs.Add(LastClickedItem);
        }

        private void SelectedThumbs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            TThumb4? nn = e.NewItems?[0] as TThumb4;
            TThumb4? oo = e.OldItems?[0] as TThumb4;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (nn == null) { return; }
                    nn.IsMySelected = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (oo == null) { return; }
                    oo.IsMySelected = false;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (nn == null || oo == null) { return; }
                    oo.IsMySelected = false;
                    nn.IsMySelected = true;
                    var item0 = e.NewItems?[0];
                    var item1 = e.OldItems?[0];
                    break;
                case NotifyCollectionChangedAction.Move:
                    var item2 = e.NewItems?[0];
                    var item3 = e.OldItems?[0];
                    break;
                case NotifyCollectionChangedAction.Reset:

                    break;
                default:
                    break;
            }
        }
    }
    //    C# ObservableCollection<T>で大量の要素を追加したいとき - Qiita
    //https://qiita.com/Yuki4/items/0e73297db632376804dd

    public class ObsevableCollectionEX<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> addItems)
        {
            if (addItems == null)
            {
                throw new ArgumentNullException(nameof(addItems));
            }
            foreach (var item in addItems)
            {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Replace(T newItem, T oldItem)
        {
            int num = Items.IndexOf(oldItem);
            Items.Remove(oldItem);
            Items.Insert(num, newItem);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
        }
    }

    #region Data4

    [DataContract]
    [KnownType(typeof(RectangleGeometry)),
     KnownType(typeof(MatrixTransform))]
    public class Data4 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _x;
        private double _y;
        private int _z;
        private string _text = "";
        private Brush? _background;
        private Brush? _foreground;

        [DataMember]
        public ObservableCollection<Data4> ChildrenData { get; set; } = new();
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public DataTypeMain DataTypeMain { get; set; }
        [DataMember]
        public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
        [DataMember]
        public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
        [DataMember]
        public int Z { get => _z; set { if (_z == value) { return; } _z = value; OnPropertyChanged(); } }

        [DataMember]
        public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
        [DataMember]
        public Brush? Background
        {
            get => _background; set
            {
                if (_background == value) { return; }
                _background = value; OnPropertyChanged();
            }
        }
        public Brush? Foreground
        {
            get => _foreground;
            set
            {
                if (_foreground == value) { return; }
                _foreground = value; OnPropertyChanged();
            }
        }
        [DataMember]
        public Geometry? Geometry { get; set; }

        //public Data3() { }
        public Data4(DataType type)
        {
            DataType = type;
            if (type == DataType.Group || type == DataType.Layer)
            {
                DataTypeMain = DataTypeMain.Group;
            }
            else DataTypeMain = DataTypeMain.Item;
        }
        public override string ToString()
        {
            return $"{X},{Y},{Z},{DataType},{Text}";
        }
    }
    #endregion Data4

    public class WakuTest : Border, INotifyPropertyChanged
    {
        private bool _waku1;
        public bool Waku1
        {
            get => _waku1; set { if (value == _waku1) { return; } _waku1 = value; OnPropertyChanged(); }
        }
        private bool _waku2;
        public bool Waku2
        {
            get => _waku2; set { if (value == _waku2) { return; } _waku2 = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public WakuTest()
        {
            Binding b1 = new(nameof(Waku1)) { Source = this };
            Binding b2 = new(nameof(Waku2)) { Source = this };
            MultiBinding mb = new();
            mb.Bindings.Add(b1); mb.Bindings.Add(b2);
            mb.Converter = new MyConverterWakuTest();
            this.SetBinding(Border.BorderBrushProperty, mb);
            this.BorderThickness = new Thickness(1.0);
        }
    }
    public class MyConverterWakuTest : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool b1 = (bool)values[0];
            bool b2 = (bool)values[1];
            if (b2) { return Brushes.Red; }
            else if (b1) { return Brushes.Green; }
            else { return Brushes.Blue; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MyConverterItemWaku4 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<Brush> brushes = (List<Brush>)parameter;
            bool b1 = (bool)values[0];
            bool b2 = (bool)values[1];
            if (b1) { return brushes[0]; }
            else if (b2) { return brushes[1]; }
            else { return brushes[2]; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class MyConverterGroupWaku4 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool b1 = (bool)values[0];
            bool b2 = (bool)values[1];
            if (b1) { return Brushes.Purple; }
            else if (b2) { return Brushes.RoyalBlue; }
            else { return Brushes.LightGray; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

