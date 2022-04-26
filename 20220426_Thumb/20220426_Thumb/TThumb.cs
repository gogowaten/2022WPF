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

namespace _20220426_Thumb
{
    public enum ThumbType
    {
        None = 0,
        Path,
        TextBlock,
        Image,
        Group,

    }
    //Thumb2の派生、全部まとめた
    //ContentControlをContentPresenterに変更した、
    //初期テンプレートはItemsControl(複数用)にしておいて
    //単体アイテム用は作成時にテンプレートを書き換えるようにした
    public class TThumb : Thumb
    {
        private ItemsControl MyItemsControl;
        private ContentPresenter ContentPresenter;

        //Childrenは外部に公開しないで、リンクした読み取り専用Itemsを公開する
        //Thumbの追加や削除は別メソッドにした
        private ObservableCollection<TThumb> Children { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb> Items { get; set; }

        public Data MyData { get; set; } = new();
        public TThumb()
        {
            Items = new(Children);
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);

            SetGroupThumbTemplate();

            this.DataContext = this;
        }


        public TThumb(Data data) : this()
        {
            MyData = data;
            Canvas.SetLeft(this, data.X); Canvas.SetTop(this, data.Y);

            //グループなら子要素を作成追加
            if (data.DataType == ThumbType.Group || data.DataType == ThumbType.None)
            {
                data.ChildrenData.ToList()
                    .ForEach(x => Children.Add(new TThumb(x)));
            }
            //グループ以外はそれぞれの要素を作成
            else
            {
                SetItemThumbTemplate();//テンプレート書き換え
                UIElement element = null;
                switch (data.DataType)
                {
                    case ThumbType.None:
                        break;
                    case ThumbType.Path:
                        break;
                    case ThumbType.TextBlock:
                        element = new TextBlock() { Text = data.Text };
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

        public void AddItem(TThumb thumb)
        {
            MyData.ChildrenData.Add(thumb.MyData);
            Children.Add(thumb);
        }
        public void RemoveItem(TThumb thumb)
        {
            MyData.ChildrenData.Remove(thumb.MyData);
            Children.Remove(thumb);
        }
    }
}
