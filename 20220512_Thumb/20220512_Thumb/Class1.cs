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

namespace _20220512_Thumb
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

    #region TThumb3
    public class TThumb3 : Thumb
    {
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
        #endregion グループとレイヤー専用
        //public ContentControl? MyContent { get;private set; }


        public TThumb3()
        {
            MyData = new(DataType.None);
            Children = new();
            Items = new(Children);
            Loaded += (a, b) =>
            {
                var w = this.ActualWidth; var h = this.ActualHeight;
                AjustLocate2(this);
                //AjustSize(this.MyParentGroup);
            };

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
            }

            //アイテムタイプ
            else
            {
                this.DataContext = MyData;//アイテムタイプのBindingソースはデータ
                SetItemThumbTemplate(waku);
                MakeAndSetItem(data);
            }
        }


        #region テンプレート
        protected FrameworkElementFactory MakeWaku(DataTypeMain dataType)
        {
            //枠表示            
            FrameworkElementFactory rect = new(typeof(Rectangle));
            rect.SetValue(Rectangle.StrokeProperty, Brushes.OrangeRed);
            if (dataType == DataTypeMain.Group)
            {
                rect.SetValue(Rectangle.StrokeProperty, Brushes.MediumOrchid);
                rect.SetValue(Rectangle.StrokeThicknessProperty, 4.0);
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
        //単一要素表示用テンプレートに書き換える
        private void SetItemThumbTemplate(FrameworkElementFactory waku)
        {
            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyItemCanvas));
            baseCanvas.AppendChild(waku);
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
            AjustLocate2(this);
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
                    MyItemElement = new TextBlock() { FontSize = 20, Background = Brushes.Orange, Opacity = 0.5 };
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
        #region アイテム追加

        public void AddItem(TThumb3 itemThumb)
        {
            if (this.MyData.DataTypeMain == DataTypeMain.Group)
            {
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
        #endregion アイテム追加

        #region サイズ修正、位置修正
        //アイテム移動後に実行
        //アイテム追加時に実行
        /// <summary>
        /// 指定Thumbと同レベルThumbとParentの位置とサイズ修正、
        /// 画面内に収まるように、余白ができないようにする
        /// ドラッグ移動後などに実行
        /// </summary>
        /// <param name="thumb">指定Thumb</param>
        protected void AjustLocate2(TThumb3 thumb)
        {
            if (thumb == null) { return; }
            if (thumb.MyParentGroup == null) { return; }
            TThumb3 parentThumb = thumb.MyParentGroup;
            //Parentの位置とサイズを取得、Parentがnullの場合は0が返ってくる
            (double x, double y, double w, double h) = GetParentRectValues(thumb);
            //位置とサイズともに変化無ければ終了
            if (w == 0 && h == 0) { return; }
            if (x == parentThumb.MyData.X &&
                y == parentThumb.MyData.Y &&
                w == parentThumb.Width &&
                h == parentThumb.Height) { return; }

            //位置が変化していた場合はParentとItemsの位置修正
            if (x != 0 || y != 0)
            {
                //Layerは位置修正しない
                if (parentThumb.MyData.DataType == DataType.Group)
                {
                    parentThumb.MyData.X -= x; parentThumb.MyData.Y -= y;
                }
                foreach (var item in parentThumb.Items)
                {
                    item.MyData.X -= x; item.MyData.Y -= y;
                }
            }
            //Parentのサイズが異なっていた場合は修正
            if (w != parentThumb.Width || h != parentThumb.Height)
            {
                parentThumb.Width = w; parentThumb.Height = h;
            }
            //Parentを辿り、再帰処理する
            AjustLocate2(parentThumb);
        }

        /// <summary>
        /// 指定ThumbのParentの位置とサイズを返す、Parentがない場合は0を返す
        /// </summary>
        /// <param name="thumb">指定Thumb</param>
        /// <returns></returns>
        private (double x, double y, double w, double h) GetParentRectValues(TThumb3 thumb)
        {
            if (thumb.MyParentGroup == null) { return (0, 0, 0, 0); }

            double x = double.MaxValue; double y = double.MaxValue;
            double width = double.MinValue; double height = double.MinValue;
            foreach (var item in thumb.MyParentGroup.Items)
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
    }
    #endregion Data3
    #endregion TThumb3

}
