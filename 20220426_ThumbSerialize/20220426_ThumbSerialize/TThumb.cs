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

namespace _20220426_ThumbSerialize
{


    //20220426_Thumbの派生、シリアル化してファイルに保存、ファイルからの再現できるようにした
    //
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
