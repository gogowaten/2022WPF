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
            if (data.Children.Count == 0)
            {
                MySetContent(data);
            }
            else
            {

            }
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



    public enum ThumbType
    {
        Path,
        TextBlock,
        Image,
        Group,

    }
}
