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

namespace _20220425_Thumb
{
    //データも種類ごとに別クラスにしてみた
    //Thumbはアイテム用途グループ用の2種類は前回と同じだけど内容は少し変更
    //良くなった？イマイチのような気がする
    //データはⅠ種類にしたほうが良いかな？Thumbも？
    public class TThumb : Thumb
    {
        public Data MyData { get; set; }
        public TThumb()
        {
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }
        public override string ToString()
        {
            //return base.ToString();
            return $"x, y = ({MyData.X}, {MyData.Y}), type = {MyData.ItemType}";
        }
    }

    public class ItemTThumb : TThumb
    {
        public ContentControl MyContent { get; private set; }

        public ItemTThumb()
        {
            FrameworkElementFactory content = new(typeof(ContentControl), nameof(MyContent));
            ControlTemplate template = new();
            template.VisualTree = content;
            this.Template = template;
            this.ApplyTemplate();
            MyContent = (ContentControl)template.FindName(nameof(MyContent), this);

        }
        public ItemTThumb(Data data) : this()
        {
            SetData(data);
        }
        public void SetData(Data data)
        {
            this.MyData = data;
            this.DataContext = MyData;
            Canvas.SetLeft(this, data.X); Canvas.SetTop(this, data.Y);

            FrameworkElement element = null;
            switch (data.ItemType)
            {
                case ThumbType.Path:
                    element = new Path();
                    element.SetBinding(Path.DataProperty, new Binding(nameof(DataPath.Geometry)));
                    element.SetBinding(Path.StrokeProperty, new Binding(nameof(DataPath.Fill)));
                    break;
                case ThumbType.TextBlock:
                    element = new TextBlock();
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(DataTextBlock.Text)));
                    break;

            }
            this.MyContent.Content = element;
        }
    }
    public class GroupTThumb : TThumb
    {
        public new DataGroup MyData { get; set; } = new();
        public ObservableCollection<TThumb> MyChildrenItems { get; set; } = new();
        public GroupTThumb()
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

            this.DataContext = this;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }
        public GroupTThumb(List<ItemTThumb> items) : this()
        {
            foreach (var item in items)
            {
                MyData.ChildrenData.Add(item.MyData);
                MyChildrenItems.Add(item);
            }
        }
        public GroupTThumb(List<Data> datas) : this()
        {
            foreach (var data in datas)
            {
                ItemTThumb itemTThumb = new(data);
                MyChildrenItems.Add(itemTThumb);
                MyData.ChildrenData.Add(data);
            }
        }
       //グループデータから含まれる要素を再構築するところ
       //ここがいまいち、アイテムタイプがグループかどうかを判定してる
        public GroupTThumb(DataGroup dataGroup) : this()
        {
            Canvas.SetLeft(this, dataGroup.X); Canvas.SetTop(this, dataGroup.Y);

            foreach (var item in dataGroup.ChildrenData)
            {

                ThumbType type = item.ItemType;
                if (type == ThumbType.Group)
                {
                    GroupTThumb groupTThumb = new((DataGroup)item);
                    MyChildrenItems.Add(groupTThumb);
                    MyData.ChildrenData.Add(item);
                }
                else
                {
                    ItemTThumb itemTThumb = new(item);
                    MyChildrenItems.Add(itemTThumb);
                    MyData.ChildrenData.Add(item);
                }
            }
        }
        public void AddItem(TThumb item)
        {
            MyChildrenItems.Add(item);
            if (typeof(GroupTThumb) == item.GetType())
            {
                MyData.ChildrenData.Add(((GroupTThumb)item).MyData);
            }
            else
            {
                MyData.ChildrenData.Add(item.MyData);
            }


        }
        public void AddData(Data data)
        {
            ItemTThumb item = new(data);
            AddItem(item);
        }
    }
}
