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

namespace _20220408
{
    public enum ThumbType
    {
        Layer = 0,
        Path,
        TextBlock,
        Image,
        Group,

    }
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
        private ItemsControl MyItemsControl;
        private ContentPresenter ContentPresenter;

        //Childrenは外部に公開しないで、リンクした読み取り専用Itemsを公開する
        //Thumbの追加や削除は別メソッドにした
        private ObservableCollection<TThumb5> Children { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb5> Items { get; set; }

        public Data4 MyData { get; set; } = new();
        public TThumb5()
        {
            Items = new(Children);
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);

            SetGroupThumbTemplate();

            this.DataContext = this;


        }

        protected void TThumb5_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }

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

        }

        //複数要素表示用テンプレートに書き換える
        private void SetGroupThumbTemplate()
        {
            FrameworkElementFactory canvas = new(typeof(Canvas));
            canvas.SetValue(BackgroundProperty, Brushes.Bisque);
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(canvas));
            ControlTemplate template = new();
            template.VisualTree = itemsControl;

            this.Template = template;
            this.ApplyTemplate();
            MyItemsControl = (ItemsControl)template.FindName(nameof(MyItemsControl), this);
        }

        //単一要素表示用テンプレートに書き換える
        private void SetItemThumbTemplate()
        {
            FrameworkElementFactory contentPresenter = new(typeof(ContentPresenter), nameof(ContentPresenter));
            ControlTemplate template = new();
            template.VisualTree = contentPresenter;

            this.Template = template;
            this.ApplyTemplate();
            ContentPresenter = (ContentPresenter)template.FindName(nameof(ContentPresenter), this);
            //ContentPresenter.SetValue(ContentPresenter.ContentProperty, new Binding(nameof(MyContet)));
        }

        public void AddItem(TThumb5 thumb)
        {
            MyData.ChildrenData.Add(thumb.MyData);
            Children.Add(thumb);
            //Layer直下に追加するものだけドラッグ移動イベントを追加する
            if (this.MyData.DataType == ThumbType.Layer)
            {
                thumb.DragDelta += thumb.TThumb5_DragDelta;
            }
        }
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
            return $"{MyData.Text}";
        }
    }
}
