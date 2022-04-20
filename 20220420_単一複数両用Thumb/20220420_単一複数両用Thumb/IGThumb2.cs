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

//単一要素表示用のContentControl
//複数要素表示用にはItemsControl
//この2つを入れたCanvasをテンプレートにしてみた
//結果、良くなったのは空の要素でも表示できるようになった
//良くないのは、作成時やグループに要素追加時の処理がわかりにくい
//基本クラスを作って、そこから単一と複数用クラスを派生させた方がいい？
//単一表示したものに追加データすると療法が表示されてしまう、回避させる処理がめんどくさい
namespace _20220420_単一複数両用Thumb
{
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
            //if (data.Children == null)
            //{
            //}
            //else
            //{

            //}
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
            if (Data2.Children == null) { Data2.Children = new(); }
            Data2.Children.Add(data);
            Children.Add(new IGThumb2(data));
        }

    }
}

