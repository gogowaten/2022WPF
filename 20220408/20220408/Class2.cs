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

namespace _20220408
{

    class Class2 { }
    public class IThumb : Thumb
    {
        public ContentControl MyContentControl;
        public Data2 Data2;


        public IThumb()
        {
            FrameworkElementFactory content = new(typeof(ContentControl), nameof(MyContentControl));
            ControlTemplate template = new();
            template.VisualTree = content;
            this.Template = template;
            this.ApplyTemplate();
            MyContentControl = (ContentControl)template.FindName(nameof(MyContentControl), this);

        }


        public IThumb(Data2 data) : this()
        {
            this.Data2 = data;
            this.MyContentControl.DataContext = data;
            Canvas.SetLeft(this, data.X);
            Canvas.SetTop(this, data.Y);

            FrameworkElement element = null;
            switch (data.ItemType)
            {
                case ThumbType.Path:
                    element = new Path();
                    element.SetBinding(Path.DataProperty, new Binding(nameof(data.Geometry)));
                    element.SetBinding(Path.StrokeProperty, new Binding(nameof(data.Stroke)));
                    break;
                case ThumbType.TextBlock:
                    element = new TextBlock();
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(data.Text)));
                    break;
                case ThumbType.Group:
                    element = new IThumb();

                    break;
            }
            this.MyContentControl.Content = element;

        }
    }

    //アイテムとグループをまとめた
    //データの種類によって作成する要素を変えるようにした
    //グループも種類の一つにした
    //基本的にはContentテンプレートにして、グループのときはItemsテンプレートに書き換えるようにした
    //これは良くないのは、空のグループを作って表示しておく方法がない
    //基本をItemsテンプレートにして、グループ以外のときにテンプレートを書き換えるようにしたほうがマシ？
    public class IGThumb : Thumb
    {
        public ObservableCollection<IGThumb> MyChildren { get; set; }
        public ContentControl MyContent { get; set; }
        public Data2 MyData2 { get; set; }
        public IGThumb()
        {
            FrameworkElementFactory content = new(typeof(ContentControl), nameof(MyContent));
            ControlTemplate template = new();
            template.VisualTree = content;
            this.Template = template;
            this.ApplyTemplate();
            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);

        }
        public IGThumb(Data2 data) : this()
        {
            this.MyData2 = data;
            this.MyContent.DataContext = data;
            Canvas.SetLeft(this, data.X);
            Canvas.SetTop(this, data.Y);

            FrameworkElement element = null;
            switch (data.ItemType)
            {
                case ThumbType.Path:
                    element = new Path();
                    element.SetBinding(Path.DataProperty, new Binding(nameof(data.Geometry)));
                    element.SetBinding(Path.StrokeProperty, new Binding(nameof(data.Stroke)));
                    break;
                case ThumbType.TextBlock:
                    element = new TextBlock();
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(data.Text)));
                    break;
                case ThumbType.Group:
                    //テンプレート書き換え
                    FrameworkElementFactory canvas = new(typeof(Canvas));
                    canvas.SetValue(BackgroundProperty, Brushes.Bisque);
                    FrameworkElementFactory itemsControl = new(typeof(ItemsControl));
                    itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(MyChildren)));
                    itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvas));
                    ControlTemplate template = new();
                    template.VisualTree = itemsControl;
                    this.Template = template;
                    this.ApplyTemplate();

                    this.DataContext = this;
                    MyChildren = new();
                    foreach (Data2 item in data.Children)
                    {
                        MyChildren.Add(new IGThumb(item));
                    }
                    break;
            }
            this.MyContent.Content = element;
        }
    }


    //単一要素表示用のContentControl
    //複数要素表示用にはItemsControl
    //この2つを入れたCanvasをテンプレートにしてみた
    //結果、良くなったのは空の要素でも表示できるようになった
    //良くないのは、作成時やグループに要素追加時の処理がわかりにくい
    //基本クラスを作って、そこから単一と複数用クラスを派生させた方がいい？
    public class IGThumb2 : Thumb
    {
        public ObservableCollection<IGThumb2> Children { get; set; } = new();
        public ContentControl Content { get; private set; }
        public Data2 Data2 { get; set; }
        public IGThumb2()
        {
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);

            //テンプレート書き換え
            //単一要素表示用
            FrameworkElementFactory content = new(typeof(ContentControl), nameof(Content));

            //複数要素表示用
            FrameworkElementFactory canvas = new(typeof(Canvas));
            canvas.SetValue(BackgroundProperty, Brushes.Bisque);
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvas));

            //2つをまとめる
            FrameworkElementFactory base_panel = new(typeof(Canvas));
            base_panel.AppendChild(content);
            base_panel.AppendChild(itemsControl);

            //テンプレート適用
            ControlTemplate template = new();
            template.VisualTree = base_panel;
            this.Template = template;

            //単一要素表示用は取り出しておく
            this.ApplyTemplate();
            Content = (ContentControl)template.FindName(nameof(Content), this);

            this.DataContext = this;
        }
        public IGThumb2(Data2 data) : this()
        {

            MySetContent(data);

        }

        public void MySetContent(Data2 data)
        {
            this.Data2 = data;
            this.Content.DataContext = Data2;
            Canvas.SetLeft(this, Data2.X);
            Canvas.SetTop(this, Data2.Y);

            FrameworkElement element = null;
            switch (Data2.ItemType)
            {
                case ThumbType.Path:
                    element = new Path();
                    element.SetBinding(Path.DataProperty, new Binding(nameof(Data2.Geometry)));
                    element.SetBinding(Path.StrokeProperty, new Binding(nameof(Data2.Stroke)));
                    break;
                case ThumbType.TextBlock:
                    element = new TextBlock();
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(Data2.Text)));
                    break;
                case ThumbType.Group:

                    Children = new();
                    foreach (Data2 item in Data2.Children)
                    {
                        Children.Add(new IGThumb2(item));
                    }
                    break;
            }
            this.Content.Content = element;
        }

        public void MyAddChildren(Data2 data)
        {
            Data2.Children.Add(data);
            Children.Add(new IGThumb2(data));
        }
    }


    //基本クラスを作って、そこから単一と複数用クラスを派生させてみたけど
    //中途半端な気がする
    //データも基本クラスを作って、種類ごとに派生クラスを作る？
    public abstract class IGThumb3 : Thumb
    {
        public Data2 MyData { get; set; }
        public IGThumb3()
        {
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }

    }
    public class ItemThumb3 : IGThumb3
    {
        public ContentControl Content { get; private set; }

        public ItemThumb3()
        {
            FrameworkElementFactory content = new(typeof(ContentControl), nameof(Content));
            ControlTemplate template = new();
            template.VisualTree = content;
            this.Template = template;
            this.ApplyTemplate();
            Content = (ContentControl)template.FindName(nameof(Content), this);

        }
        public ItemThumb3(Data2 data) : this()
        {
            SetData(data);
        }
        public void SetData(Data2 data)
        {
            this.MyData = data;
            this.DataContext = MyData;
            Canvas.SetLeft(this, data.X); Canvas.SetTop(this, data.Y);

            FrameworkElement element = null;
            switch (data.ItemType)
            {
                case ThumbType.Path:
                    element = new Path();
                    element.SetBinding(Path.DataProperty, new Binding(nameof(MyData.Geometry)));
                    element.SetBinding(Path.StrokeProperty, new Binding(nameof(MyData.Stroke)));
                    break;
                case ThumbType.TextBlock:
                    element = new TextBlock();
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
                    break;

            }
            this.Content.Content = element;
        }
    }
    public class GroupThumb3 : IGThumb3
    {
        public ObservableCollection<IGThumb3> MyChildren { get; set; } = new();
        public GroupThumb3()
        {
            FrameworkElementFactory canvas = new(typeof(Canvas));
            canvas.SetValue(BackgroundProperty, Brushes.Bisque);
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(MyChildren)));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvas));
            ControlTemplate template = new();
            template.VisualTree = itemsControl;
            this.Template = template;
            this.ApplyTemplate();

            this.DataContext = this;

            MyData = new Data2
            {
                Children = new(),
                ItemType = ThumbType.Group
            };
        }
        public GroupThumb3(double x, double y) : this()
        {
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);
        }

        //public void AddData(Data2 data)
        //{
        //    ItemThumb3 itemThumb3 = new(data);
        //    MyChildren.Add(itemThumb3);
        //}
        public void AddItem(ItemThumb3 item)
        {
            MyChildren.Add(item);
            MyData.Children.Add(item.MyData);
        }
        public void AddItem(List<ItemThumb3> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }
    }


    //データも種類ごとに別クラスにしてみた
    //Thumbはアイテム用途グループ用の2種類は前回と同じだけど内容は少し変更
    //良くなった
    public class TThumb4 : Thumb
    {
        public Data3Base MyData { get; set; }
        public TThumb4()
        {
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }
        public override string ToString()
        {
            //return base.ToString();
            return $"x, y = ({MyData.X}, {MyData.Y}), type = {MyData.ItemType}";
        }
    }
    public class ItemTThumb4 : TThumb4
    {
        public ContentControl MyContent { get; private set; }

        public ItemTThumb4()
        {
            FrameworkElementFactory content = new(typeof(ContentControl), nameof(MyContent));
            ControlTemplate template = new();
            template.VisualTree = content;
            this.Template = template;
            this.ApplyTemplate();
            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);

        }
        public ItemTThumb4(Data3Base data) : this()
        {
            SetData(data);
        }
        public void SetData(Data3Base data)
        {
            this.MyData = data;
            this.DataContext = MyData;
            Canvas.SetLeft(this, data.X); Canvas.SetTop(this, data.Y);

            FrameworkElement element = null;
            switch (data.ItemType)
            {
                case ThumbType.Path:
                    element = new Path();
                    element.SetBinding(Path.DataProperty, new Binding(nameof(Data3Path.Geometry)));
                    element.SetBinding(Path.StrokeProperty, new Binding(nameof(Data3Path.Fill)));
                    break;
                case ThumbType.TextBlock:
                    element = new TextBlock();
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(Data3TextBlock.Text)));
                    break;

            }
            this.MyContent.Content = element;
        }
    }
    public class GroupTThumb4 : TThumb4
    {
        public new Data3Group MyData { get; set; }
        public ObservableCollection<TThumb4> MyChildrenItems { get; set; } = new();
        public GroupTThumb4()
        {
            FrameworkElementFactory MyCanvas = new(typeof(Canvas));
            MyCanvas.SetValue(BackgroundProperty, Brushes.Bisque);
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(MyChildrenItems)));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(MyCanvas));
            ControlTemplate template = new();
            template.VisualTree = itemsControl;
            this.Template = template;
            this.ApplyTemplate();

            MyData = new Data3Group();
            this.DataContext = this;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }
        public GroupTThumb4(List<ItemTThumb4> items) : this()
        {
            foreach (var item in items)
            {
                MyData.ChildrenData.Add(item.MyData);
                MyChildrenItems.Add(item);
            }
        }
        public GroupTThumb4(List<Data3Base> datas) : this()
        {
            foreach (var data in datas)
            {
                ItemTThumb4 itemTThumb4 = new(data);
                MyChildrenItems.Add(itemTThumb4);
                MyData.ChildrenData.Add(data);
            }
        }
        public GroupTThumb4(Data3Group data3Group) : this()
        {
            foreach (var item in data3Group.ChildrenData)
            {

                ThumbType type = item.ItemType;
                if (type == ThumbType.Group)
                {
                    GroupTThumb4 groupTThumb4 = new((Data3Group)item);
                    MyChildrenItems.Add(groupTThumb4);
                    MyData.ChildrenData.Add(item);
                }
                else
                {
                    ItemTThumb4 itemTThumb4 = new(item);
                    MyChildrenItems.Add(itemTThumb4);
                    MyData.ChildrenData.Add(item);
                }
            }
        }
        public void AddItem(ItemTThumb4 item)
        {
            MyChildrenItems.Add(item);
            MyData.ChildrenData.Add(item.MyData);
        }
    }


    //Thumb2の派生、全部まとめた
    //ContentControlをContentPresenterに変更した、
    //初期テンプレートはItemsControl(複数用)にしておいて
    //単体アイテム用は作成時にテンプレートを書き換えるようにした
    public class TThumb5 : Thumb
    {
        public ItemsControl MyItemsControl;
        public ContentPresenter ContentPresenter;
        private Canvas RootCanvas;
        public bool IsMoveItems;
        private bool isEditing;

        public bool IsEditing
        {
            get => isEditing;
            set
            {
                if (value != isEditing)
                {
                    //var pastnext = GetPastAndNextThumb(this);

                    //TThumb5 editing = GetEditingThumb();
                    ////編集中にする
                    //if (value && editing != null)
                    //{
                    //    editing.IsEditing = false;
                    //    //編集中グループのアイテムのドラッグイベントを削除
                    //    foreach (var item in editing.Items)
                    //    {
                    //        RemoveDragEvent(item);
                    //    }
                    //    //新たに編集中にするグループを取得、そのアイテムにドラッグ移動イベント付加
                    //    var next = GetNextEditingThumb(this);
                    //    foreach (var item in next.Items)
                    //    {
                    //        AddDragEvent(item);
                    //    }
                    //}

                    ////編集終了にする
                    //else
                    //{

                    //}
                    isEditing = value;
                }


            }
        }


        //Childrenは外部に公開しないで、リンクした読み取り専用Itemsを公開する
        //Thumbの追加や削除は別メソッドにした
        private ObservableCollection<TThumb5> Children { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb5> Items { get; set; }
        public TThumb5 ParentGroup { get; private set; }

        public Data4 MyData { get; set; } = new();
        public TThumb5()
        {
            Items = new(Children);
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);

            SetGroupThumbTemplate();
            SetContextMenu();

            this.DataContext = this;

            //子要素のParentを自身に指定する
            Children.CollectionChanged += (a, b) =>
            {
                if (b.NewItems != null && b.NewItems[0] is TThumb5 t) { t.ParentGroup = this; }
            };
            //子要素サイズ変更時にParentのサイズも変更する
            this.SizeChanged += (x, y) =>
            {
                if (this.ParentGroup != null) { AjustParentSize(this); }
            };
        }


        #region ドラッグイベント
        protected void TThumb5_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
            var neko = e.Source;
            var inu = e.OriginalSource;
            var tt = (TThumb5)e.OriginalSource;
            //var uma = tt.MyData.Parent;
        }
        private void TThumb5_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            TThumb5 thumb = (TThumb5)sender;
            AjustLocate2(thumb.ParentGroup);//位置調整
            //Parentのサイズ再計算、設定
            AjustParentSize(thumb);
        }
        private static void AddDragEvent(TThumb5 thumb)
        {
            thumb.DragDelta += thumb.TThumb5_DragDelta;
            thumb.DragCompleted += thumb.TThumb5_DragCompleted;
        }
        private static void RemoveDragEvent(TThumb5 thumb)
        {
            thumb.DragDelta -= thumb.TThumb5_DragDelta;
            thumb.DragCompleted -= thumb.TThumb5_DragCompleted;

        }

        #endregion ドラッグイベント


        public TThumb5(Data4 data) : this()
        {
            this.MyData = data;
            this.DataContext = MyData;
            this.MyItemsControl.DataContext = this;
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(Data4.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(Data4.Y)));


            //グループなら子要素を作成追加
            if (data.DataType == ThumbType.Group || data.DataType == ThumbType.Layer)
            {
                data.ChildrenData.ToList()
                    .ForEach(x => Children.Add(new TThumb5(x)));
                //AjustLocate2(this);



            }
            //グループ以外はそれぞれの要素を作成
            else
            {
                //this.DragDelta += TThumb5_DragDelta;

                SetItemThumbTemplate();//テンプレート書き換え
                FrameworkElement element = null;

                switch (data.DataType)
                {
                    case ThumbType.Layer:
                        break;
                    case ThumbType.Path:
                        element = new Path() { Fill = Brushes.MediumAquamarine };
                        element.SetBinding(Path.DataProperty, new Binding(nameof(Data4.Geometry)));
                        break;
                    case ThumbType.TextBlock:
                        element = new TextBlock();
                        //element.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
                        element.SetBinding(TextBlock.TextProperty, new Binding(nameof(Data4.Text)));
                        break;
                    case ThumbType.Image:
                        break;
                    case ThumbType.Group:
                        break;
                    default:
                        break;
                }
                this.ContentPresenter.Content = element;


            }
            //
            if (data.DataType == ThumbType.Layer) { isEditing = true; }

        }

        //複数要素表示用テンプレートに書き換える
        private void SetGroupThumbTemplate()
        {
            FrameworkElementFactory itemsCanvas = new(typeof(Canvas));
            //canvas.SetValue(BackgroundProperty, Brushes.Transparent);
            itemsCanvas.SetValue(BackgroundProperty, Brushes.Beige);
            //アイテムズコントロール
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(itemsCanvas));
            //枠表示
            FrameworkElementFactory rect = MakeWaku();

            FrameworkElementFactory baseCanvas = new(typeof(Canvas));
            baseCanvas.AppendChild(itemsControl);
            baseCanvas.AppendChild(rect);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            MyItemsControl = (ItemsControl)template.FindName(nameof(MyItemsControl), this);

        }
        private FrameworkElementFactory MakeWaku()
        {
            //枠表示
            FrameworkElementFactory rect = new(typeof(Rectangle));
            rect.SetValue(Rectangle.StrokeProperty, Brushes.MediumBlue);
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
        private void SetItemThumbTemplate()
        {
            //FrameworkElementFactory contentPresenter = new(typeof(ContentPresenter), nameof(ContentPresenter));
            //ControlTemplate template = new();
            //template.VisualTree = contentPresenter;

            //this.Template = template;
            //this.ApplyTemplate();
            //ContentPresenter = (ContentPresenter)template.FindName(nameof(ContentPresenter), this);
            ////ContentPresenter.SetValue(ContentPresenter.ContentProperty, new Binding(nameof(MyContet)));

            FrameworkElementFactory waku = MakeWaku();
            FrameworkElementFactory contentPresenter = new(typeof(ContentPresenter), nameof(ContentPresenter));
            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(RootCanvas));

            baseCanvas.AppendChild(contentPresenter);
            baseCanvas.AppendChild(waku);
            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            ContentPresenter = (ContentPresenter)template.FindName(nameof(ContentPresenter), this);
            //ContentPresenter.SetValue(ContentPresenter.ContentProperty, new Binding(nameof(MyContet)));
            RootCanvas = (Canvas)template.FindName(nameof(RootCanvas), this);

            //要素のサイズとCanvasのサイズのBinding
            //Template構築中のFrameworkElementFactoryでは反応しないので
            //Template構築後に取り出してBindingした
            Binding b = new();
            b.Source = ContentPresenter;
            b.Path = new PropertyPath(ActualWidthProperty);
            RootCanvas.SetBinding(WidthProperty, b);
            b = new();
            b.Source = ContentPresenter;
            b.Path = new PropertyPath(ActualHeightProperty);
            RootCanvas.SetBinding(HeightProperty, b);

        }

        public void AddItem(TThumb5 thumb)
        {
            MyData.ChildrenData.Add(thumb.MyData);
            Children.Add(thumb);
            //thumb.MyData.Parent = this;

            //Layer直下に追加するものだけドラッグ移動イベントを追加する
            if (this.MyData.DataType == ThumbType.Layer)
            {
                AddDragEvent(thumb);
                //thumb.DragDelta += thumb.TThumb5_DragDelta;
                //thumb.DragCompleted += thumb.TThumb5_DragCompleted;                
            }
        }


        public static void AjustParentSize(TThumb5 thumb)
        {
            (double w, double y) = GetParentSize(thumb);
            thumb.ParentGroup.Width = w;
            thumb.ParentGroup.Height = y;
        }
        private static (double w, double y) GetParentSize(TThumb5 thumb)
        {
            double w = double.MinValue;
            double h = double.MinValue;
            foreach (var item in thumb.ParentGroup.Items)
            {
                w = Math.Max(w, item.MyData.X + item.ActualWidth);
                h = Math.Max(h, item.MyData.Y + item.ActualHeight);
            }
            return (w, h);
        }
        /// <summary>
        /// 位置調整、画面内に収まるように、余白ができないようにする
        /// 子要素を処理したあとに親要素を遡って処理、Layerまでたどる
        /// ドラッグ移動後や要素追加後などに実行
        /// </summary>
        /// <param name="groupThumb">グループ型Thumb</param>       
        private void AjustLocate2(TThumb5 groupThumb)
        {
            //子要素の左端と上端を取得
            double left = double.MaxValue;
            double top = double.MaxValue;
            foreach (var item in groupThumb.Items)
            {
                if (item.MyData.X < left) { left = item.MyData.X; }
                if (item.MyData.Y < top) { top = item.MyData.Y; }
            }
            //0以外なら位置調整
            if (left != 0 || top != 0)
            {
                foreach (var item in groupThumb.Items)
                {
                    item.MyData.X -= left;
                    item.MyData.Y -= top;
                }
                //自身がレイヤー型ではなければ自身も調整して、さらに親要素も処理する
                if (groupThumb.MyData.DataType != ThumbType.Layer)
                {
                    groupThumb.MyData.X += left;
                    groupThumb.MyData.Y += top;
                    //親要素をたどる
                    AjustLocate2(groupThumb.ParentGroup);
                }
            }
        }

        //public TThumb5 GetLayerThumb(TThumb5 thumb)
        //{
        //    //Parentを遡ってLayer型のThumbを返す
        //    TThumb5 t = thumb.ParentGroup;
        //    if (t == null) { return null; }
        //    if (t.MyData.DataType == ThumbType.Layer) { return t; }
        //    else { GetLayerThumb(t); }
        //    return null;
        //}
        public void RemoveItem(TThumb5 thumb)
        {
            MyData.ChildrenData.Remove(thumb.MyData);
            Children.Remove(thumb);
        }
        public void RemoveAllItem()
        {
            Children.Clear();
            MyData.ChildrenData.Clear();
        }
        public override string ToString()
        {
            return $"{MyData.X}, {MyData.Y}, {MyData.Text}";
        }

        private void SetContextMenu()
        {
            ContextMenu cm = new();
            this.ContextMenu = cm;
            MenuItem item = new() { Header = "BeginEdit" };
            item.Click += Item_ClickBeginEdit;
            cm.Items.Add(item);
            item = new() { Header = "EndEdit" };
            item.Click += Item_ClickEndEdit;
            cm.Items.Add(item);
            item.Initialized += Item_Initialized;
            item.ContextMenuOpening += Item_ContextMenuOpening;
            グループThumbには付けないようにする
        }

        private void Item_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            mi.IsEnabled = !this.ParentGroup.IsEnabled;
            //if (this.ParentGroup.IsEditing) { mi.IsEnabled = false; }
        }

        private void Item_Initialized(object sender, EventArgs e)
        {
            
        }

        private void Item_ClickEndEdit(object sender, RoutedEventArgs e)
        {
            //編集状態解除


        }

        private void Item_ClickBeginEdit(object sender, RoutedEventArgs e)
        {
            //編集状態(グループ内Thumbにドラッグ移動イベント付加)にする
            //編集状態のグループと次に編集状態にするグループを取得
            (TThumb5 now, TThumb5 next) = GetEditingGroupThumbToLeaf22(this);

            if (now != next && next != null)
            {
                now.IsEditing = false;
                next.IsEditing = true;
                foreach (var item in now.Items)
                {
                    RemoveDragEvent(item);
                }
                foreach (var item in next.Items)
                {
                    AddDragEvent(item);
                }
            }

        }
        //自身が所属するLayerを取得
        private TThumb5 GetLayer(TThumb5 thumb)
        {
            if (thumb.MyData.DataType == ThumbType.Layer) { return thumb; }
            else { return GetLayer(thumb.ParentGroup); }
        }
        /// <summary>
        /// 指定ThumbからParentを辿ってLayerと直下のGroupを返す、このときのGroupは指定Thumbの系統のもの
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns></returns>
        private (TThumb5 layer, TThumb5 group) GetLayerAndRelationalGroup(TThumb5 thumb)
        {
            TThumb5 parent = thumb.ParentGroup;
            if (parent.MyData.DataType == ThumbType.Layer)
            {
                TThumb5 child = null;
                if (thumb.MyData.DataType == ThumbType.Group)
                {
                    child = thumb;
                }
                return (parent, child);//直下がItemだった場合はchildはnullで返す
            }
            else
            {
                return GetLayerAndRelationalGroup(parent);
            }
        }



        /// <summary>
        /// 編集状態のグループThumbと次に編集状態にするグループThumbを返す
        /// 指定ThumbからParentを辿る同系統探索なので、別系統にある場合はnullを返す
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns></returns>
        private (TThumb5 now, TThumb5 next) GetEditingGroupThumbToRoot2(TThumb5 thumb)
        {
            //ルート方向へ探索(Parentをたどる同系統探索)
            TThumb5 parent = thumb.ParentGroup;
            if (parent == null) { return (null, null); }//同系統にはなかった
            else
            {
                if (parent.isEditing)
                {
                    if (thumb.MyData.DataType == ThumbType.Group)
                    {
                        return (parent, thumb);//今と次の両方見つかった
                    }
                    else return (parent, null);//今のグループが先端だった(変更する必要なし)
                }
                else
                {
                    return GetEditingGroupThumbToRoot2(parent);
                }
            }
        }


        /// <summary>
        /// 編集状態のグループThumbを返す、探索方向は指定Thumbからリーフに行う
        /// 見つからない場合はnullを返す
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns></returns>
        private TThumb5 GetEditingGroupThumbToLeaf(TThumb5 thumb)
        {
            //リーフ方向へ探索
            TThumb5 result = null;
            if (thumb.isEditing) { return thumb; }
            else if (thumb.Items.Count == 0) { return null; }
            else
            {
                foreach (TThumb5 item in thumb.Items)
                {
                    result = GetEditingGroupThumbToLeaf(item);
                    if (result != null) { break; }
                }
            }
            return result;
        }

        /// <summary>
        /// 編集状態のグループThumbと、次に変状態にするグループThumbを返す
        /// 指定Thumbの同系統から探索する、見つからない場合は全体から探索する
        /// nextがnullの場合は変更する必要なしの意味になる
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns></returns>
        private (TThumb5 now, TThumb5 next) GetEditingGroupThumbToLeaf22(TThumb5 thumb)
        {
            //同系統から探索
            var tt = GetEditingGroupThumbToRoot2(thumb);
            if (tt.now != null && tt.next != null) { return tt; }//同系統にあった、完了            
            else if (tt.now != null && tt.next == null) { return tt; }//変更の必要なし、完了
            //同系統になかった場合、全体から今を探索、次はルートの直下になる
            else
            {
                //Layerと同系統直下のGroup取得
                (TThumb5 layer, TThumb5 group) = GetLayerAndRelationalGroup(thumb);
                //直下がItemだった場合は次をLayerにする
                if (group == null) { tt.next = layer; }
                else { tt.next = group; }
                tt.now = GetEditingGroupThumbToLeaf(layer);
                return tt;
            }

        }


    }


    public class BBH : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var children = (ReadOnlyObservableCollection<TThumb5>)parameter;
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (var item in children)
            {
                min = Math.Min(min, item.MyData.Y);
                max = Math.Max(max, item.MyData.Y + item.ActualHeight);
            }
            return max - min;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BBWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var children = (ReadOnlyObservableCollection<TThumb5>)parameter;
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (var item in children)
            {
                min = Math.Min(min, item.MyData.X);
                max = Math.Max(max, item.MyData.X + item.ActualWidth);
            }
            return max - min;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class AA : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
