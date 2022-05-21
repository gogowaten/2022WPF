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
        public Group4Base? MyParentGroup;
        public Layer4? MyLayer;
        public Data4 MyData { get; set; }
        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set { if (value == isSelected) { return; } isSelected = value; OnPropertyChanged(); }
        }
        //移動対象フラグ、編集状態のGroupThumbの直下のThumb全てが対象になる
        //Parentが編集状態(IsEditing)ならtrue
        private bool _IsMoveTarget { get; set; }
        public bool IsMoveTarget
        {
            get => _IsMoveTarget;
            set
            {
                if (_IsMoveTarget == value) { return; }
                _IsMoveTarget = value; OnPropertyChanged();
            }
        }
        #endregion

        public TThumb4(Data4 data)
        {
            MyData = data;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
            this.SetBinding(Panel.ZIndexProperty, new Binding(nameof(MyData.X)));

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
            waku.SetValue(Rectangle.StrokeProperty, Brushes.MediumOrchid);

            //枠サイズは自身のサイズに合わせる
            Binding b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualWidthProperty);
            waku.SetBinding(Rectangle.WidthProperty, b);
            b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualHeightProperty);
            waku.SetValue(Rectangle.HeightProperty, b);

            //枠表示バインド、選択状態のときだけ表示する
            b = new(nameof(IsSelected)) { Source = this };
            b.Converter = new MyValueConverterVisible();
            waku.SetValue(VisibilityProperty, b);

            return waku;
        }
        protected FrameworkElementFactory MakeWaku2()
        {
            Binding b;
            //移動可能Thumbに表示する枠
            //枠サイズは自身のサイズに合わせる
            FrameworkElementFactory movableRect = new(typeof(Rectangle));
            movableRect.SetValue(Shape.StrokeProperty, Brushes.LightGray);
            movableRect.SetValue(Shape.OpacityProperty, 0.5);
            b = new() { Source = this };
            b.Path = new PropertyPath(ActualWidthProperty);
            movableRect.SetBinding(Shape.WidthProperty, b);
            b = new() { Source = this };
            b.Path = new PropertyPath(ActualHeightProperty);
            movableRect.SetBinding(Shape.HeightProperty, b);
            //枠表示バインド、選択状態のときだけ表示する
            b = new(nameof(IsMoveTarget)) { Source = this };
            b.Converter = new MyValueConverterVisible();
            movableRect.SetValue(VisibilityProperty, b);

            //b = new(nameof(Group4Base.IsEditing)) { Source = MyParentGroup };
            //b.Converter = new MyValueConverterVisible();
            //movableRect.SetValue(VisibilityProperty, b);
            ////movableRect.SetBinding(VisibilityProperty, b);
            return movableRect;
        }


        #region ドラッグ移動

        protected void DragEventAdd(TThumb4 thumb)
        {
            thumb.DragDelta += thumb.TThumb_DragDelta;
            thumb.DragCompleted += thumb.TThumb_DragCompleted;
        }

        protected void DragEventRemove(TThumb4 thumb)
        {
            thumb.DragCompleted -= thumb.TThumb_DragCompleted;
            thumb.DragDelta -= thumb.TThumb_DragDelta;
        }
        private void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //AjustLocate3(this.MyParentGroup);
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
        public Group4? GetMyMovableTopGroup()
        {
            if (MyParentGroup is Group4 group)
            {
                if (group.MyParentGroup?.IsEditing == true)
                {
                    return group;
                }
                else
                {
                    return group.GetMyMovableTopGroup();
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

        public virtual void RemoveThumb()
        {
            if (MyParentGroup is Group4 group)
            {
                //コレクションから削除
                group.Children.Remove(this);
                group.MyData.ChildrenData.Remove(this.MyData);
            }

        }
        #endregion メソッド
    }

    public class Item4 : TThumb4
    {
        public Canvas MyTemplateCanvas;
        public FrameworkElement MyItemElement;
        private bool _IsLastClicked;//最後にクリックされたフラグ
        public bool IsLastClicked
        {
            get => _IsLastClicked;
            set
            {
                if (_IsLastClicked == value) { return; }
                _IsLastClicked = value;
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
            TThumb4 topMovable = (TThumb4?)GetMyMovableTopGroup() ?? this;
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
                    MyLayer.AddSelectThumb(topMovable);
                }
            }
        }

        #endregion イベント  
        //単一要素表示用テンプレートに書き換える
        private Canvas InitializeTemplate()
        {
            //共通枠
            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyTemplateCanvas));
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Item);
            baseCanvas.AppendChild(waku);
            waku.SetValue(Panel.ZIndexProperty, 1);//枠表示前面
            baseCanvas.AppendChild(MakeWaku2());//枠2追加
            //Item専用枠、最後にクリックされた識別用
            FrameworkElementFactory lastClickWaku = new(typeof(Rectangle));
            lastClickWaku.SetValue(Panel.ZIndexProperty, 2);
            lastClickWaku.SetValue(Rectangle.StrokeProperty, Brushes.OrangeRed);
            lastClickWaku.SetValue(Rectangle.StrokeDashArrayProperty, new DoubleCollection() { 5.0, 5.0 });
            baseCanvas.AppendChild(lastClickWaku);
            Binding b = new() { Source = this, Path = new PropertyPath(WidthProperty) };
            lastClickWaku.SetBinding(WidthProperty, b);
            b = new() { Source = this, Path = new PropertyPath(HeightProperty) };
            lastClickWaku.SetBinding(HeightProperty, b);
            //枠表示バインド
            b = new()
            {
                Source = this,
                Path = new PropertyPath(nameof(IsLastClicked)),
                Converter = new MyValueConverterVisible()
            };
            lastClickWaku.SetBinding(VisibilityProperty, b);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            //MyTemplateCanvas = (Canvas)template.FindName(nameof(MyTemplateCanvas), this) ?? throw new ArgumentNullException(nameof(MyTemplateCanvas));
            return (Canvas)template.FindName(nameof(MyTemplateCanvas), this);
        }

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
                    element = new TextBlock() { FontSize = 20, Background = Brushes.Transparent };
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
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

        //グループ解除
        public void UngroupMyTopGroup()
        {
            //解除対象にするのはクリックしたThumbのGetMyMovableTopGroup
            Group4? myGroup = GetMyMovableTopGroup();
            if (myGroup == null) { return; }
            myGroup.Ungroup();

        }
    }

    public abstract class Group4Base : TThumb4
    {
        public ItemsControl MyItemsControl;
        internal ObservableCollection<TThumb4> Children { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb4> Items { get; set; }
        private bool _IsEditing { get; set; }
        public bool IsEditing
        {
            get => _IsEditing;
            set
            {
                if (_IsEditing == value) { return; }
                _IsEditing = value; OnPropertyChanged();
            }
        }
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

        }
        //複数要素表示用テンプレートに書き換える
        private ItemsControl SetGroupThumbTemplate()
        {
            //アイテムズコントロール
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));

            FrameworkElementFactory itemsCanvas = new(typeof(Canvas));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(itemsCanvas));

            FrameworkElementFactory baseCanvas = new(typeof(Canvas));
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Group);
            baseCanvas.AppendChild(waku);//枠追加
            baseCanvas.AppendChild(MakeWaku2());//枠2追加
            baseCanvas.AppendChild(MakeWaku3());//枠3追加
            baseCanvas.AppendChild(itemsControl);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();

            return (ItemsControl)template.FindName(nameof(MyItemsControl), this)
                ?? throw new ArgumentNullException(nameof(MyItemsControl));
        }
        protected FrameworkElementFactory MakeWaku3()
        {
            Binding b;
            //移動対象Thumbに表示する枠
            //枠サイズは自身のサイズに合わせる
            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Shape.StrokeProperty, Brushes.Green);
            b = new() { Source = this };
            b.Path = new PropertyPath(ActualWidthProperty);
            waku.SetBinding(Shape.WidthProperty, b);
            b = new() { Source = this };
            b.Path = new PropertyPath(ActualHeightProperty);
            waku.SetBinding(Shape.HeightProperty, b);
            //枠表示バインド、編集状態のときだけ表示する
            b = new(nameof(IsEditing)) { Source = this };
            b.Converter = new MyValueConverterVisible();
            waku.SetValue(VisibilityProperty, b);

            return waku;
        }

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


        public virtual void AddThumb(TThumb4 thumb)
        {
            //コレクションに追加
            thumb.MyData.Z = Children.Count;
            Children.Add(thumb);
            MyData.ChildrenData.Add(thumb.MyData);
            //Parentの指定
            thumb.MyParentGroup = this;
            //Layerの指定
            thumb.MyLayer = this.MyLayer;
            //ドラッグ移動イベント付加
            //Parentが編集状態なら追加アイテム自身をドラッグ移動可能にする
            if (thumb.MyParentGroup.IsEditing)
            {
                DragEventAdd(thumb);
            }
            //IsEditingなら移動対象にする
            if (IsEditing) { thumb.IsMoveTarget = true; }
        }

    }
    public class Group4 : Group4Base
    {
        public Group4(Data4 data) : base(data) { }

        //グループ解除
        public void Ungroup()
        {
            if (this.MyData.DataType == DataType.Layer) { return; }


            //解除後はThumb群を選択状態にする
            //Z、元のグループのZをChildrenThumbに足し算する、さらに
            //元グループと同レベルで元グループZより上にあるThumbZには
            //ChildrenThumbの個数を足し算する
            int motoZ = this.MyData.Z;
            var neko = MyLayer?.NowEditingThumb;
            MyLayer.item

        }

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
                        item.IsSelected = false;
                    }
                    _SelectedThumbs.Clear();
                }
                //ドラッグ移動イベントの付け外し
                if (_NowEditingThumb != null)
                {
                    _NowEditingThumb.IsEditing = false;
                    foreach (var item in _NowEditingThumb.Children)
                    {
                        item.IsMoveTarget = false;
                        DragEventRemove(item);
                    }
                }
                if (value != null)
                {
                    value.IsEditing = true;
                    foreach (var item in value.Children)
                    {
                        item.IsMoveTarget = true;
                        DragEventAdd(item);
                    }
                }

                _NowEditingThumb = value;
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
                    _lastClickedItem.IsLastClicked = false;
                }
                //新しい方のIsLastClickedをtrue
                value.IsLastClicked = true;
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
            IsEditing = true;

            _SelectedThumbs.CollectionChanged += SelectedThumbs_CollectionChanged;
            PreviewMouseLeftButtonDown += Layer4_PreviewMouseLeftButtonDown;
            SelectedThumbs = new(_SelectedThumbs);
        }
        #region メソッド

        //選択状態Thumbの追加
        public void AddSelectThumb(TThumb4? thumb)
        {
            //同じThumbがないかチェックしてから追加
            if (thumb == null) { return; }
            if (SelectedThumbs.Contains(thumb)) { return; }
            _SelectedThumbs.Add(thumb);
        }
        public void RemoveSelecthumb(TThumb4 thumb)
        {
            if (!SelectedThumbs.Contains(thumb)) { return; }
            _SelectedThumbs.Remove(thumb);
        }
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
                    item.IsSelected = false;
                }
                _SelectedThumbs.Clear();
                _SelectedThumbs.Add(thumb);
            }
            else
            {
                _SelectedThumbs.Add(thumb);
            }
        }
        //追加
        public override void AddThumb(TThumb4 thumb)
        {
            //基底クラスのメソッドを実行
            base.AddThumb(thumb);
            //すべてのThumbのMyLayerに自身を指定する
            if (thumb is Group4 group)
                SetMyLayer(group);
            else if (thumb is Item4 item)
                item.MyLayer = this;
        }
        //すべてのThumbのMyLayerに自身を指定する
        private void SetMyLayer(Group4 group)
        {
            group.MyLayer = this;
            foreach (var item in group.Children)
            {
                item.MyLayer = this;
                if (item is Group4 group4)
                    SetMyLayer(group4);
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
                    nn.IsSelected = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (oo == null) { return; }
                    oo.IsSelected = false;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (nn == null || oo == null) { return; }
                    oo.IsSelected = false;
                    nn.IsSelected = true;
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
        public Brush? Brush { get; set; }
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
}

