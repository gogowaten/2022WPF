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

//アイテムとグループをまとめた
//データの種類によって作成する要素を変えるようにした
//グループも種類の一つにした
//基本的にはContentテンプレートにして、グループのときはItemsテンプレートに書き換えるようにした
//これは良くないのは、空のグループを作って表示しておく方法がない
//基本をItemsテンプレートにして、グループ以外のときにテンプレートを書き換えるようにしたほうがマシ？
namespace _20220419_タイプ別Thumb
{
    class IGThumb : Thumb
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
        public override string ToString()
        {
            //return base.ToString();
            return MyData2.ItemType.ToString();
        }
    }
}
