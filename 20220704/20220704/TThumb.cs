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

namespace _20220704
{
    public abstract class TThumb : Thumb
    {
        public Data MyData { get; protected set; }
        public CanvasThumb? MyCanvas { get; set; }
        //public LayerThumb? MyLayerThumb { get; set; }
        //public GroupAndLayerBase? MyGroupThumb { get; set; }
        public bool IsMyCurrentItem { get; private set; }
        public bool IsMyActiveThumb { get; private set; }
        public bool IsMySelected { get; private set; }
        public TThumb? MyParentThumb { get; set; }

        public TThumb(Data myData, string name = "")
        {
            MyData = myData;
            Name = name;
            DataContext = this;

            SetMyDataBinding();
            //PreviewMouseDown += TThumb_PreviewMouseDown;
        }

        //private void TThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (MyCanvas is not CanvasThumb ct)
        //    {
        //        MyCanvas = GetMyCanvas();
        //    }
        //    MyCanvas?.MySelectedThumbs.Add(this);
        //}

        protected static void SetIsMySelected(TThumb thumb, bool b)
        {
            thumb.IsMySelected = b;
        }


        protected TThumb? GetActiveThumb(TThumb? thumb)
        {
            if (thumb == null) { return null; }
            else if (thumb.MyParentThumb is GroupAndLayerBase group)
            {
                if (group.IsEditingThumb) { return thumb; }
                else { return GetActiveThumb(group); }
            }
            return null;
        }
        protected static void SetIsMyActiveThumb(TThumb? oldAct, TThumb? newAct)
        {
            if (oldAct != null) oldAct.IsMyActiveThumb = false;
            if (newAct != null) newAct.IsMyActiveThumb = true;
        }

        protected static void SetIsMyCurrentItem(ItemThumb? oldItem, ItemThumb? newItem)
        {
            if (oldItem != null) oldItem.IsMyCurrentItem = false;
            if (newItem != null) newItem.IsMyCurrentItem = true;
        }

        protected CanvasThumb SetMyCanvas()
        {
            if (GetMyCanvas() is not CanvasThumb cc)
            {
                throw new ArgumentNullException(nameof(cc));
            }
            else
            {
                MyCanvas = cc;
                return cc;
            }
        }
        protected CanvasThumb? GetMyCanvas()
        {
            if (MyParentThumb is CanvasThumb canvas)
            {
                return canvas;
            }
            else
            {
                return MyParentThumb?.GetMyCanvas();
            }
        }
        protected abstract void SetTemplate();



        public void AddDragEvents()
        {
            DragDelta += TThumb_DragDelta;
        }
        public void RemoveDragEvents()
        {
            DragDelta -= TThumb_DragDelta;
        }
        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }
        private void SetMyDataBinding()
        {
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            Binding b;
            b = new(nameof(MyData.X)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.LeftProperty, b);
            b = new(nameof(MyData.Y)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.TopProperty, b);
            b = new(nameof(MyData.Z)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Panel.ZIndexProperty, b);

        }

        public override string ToString()
        {
            string str = $"X{MyData.X}, Y{MyData.Y}, Z{MyData.Z}, {Name}";
            return str;
        }
    }

    public abstract class ItemThumb : TThumb
    {
        protected FrameworkElementFactory waku;
        protected FrameworkElementFactory panel;



        public ItemThumb(Data myData) : base(myData)
        {
            waku = new(typeof(Rectangle));
            waku.SetValue(Panel.ZIndexProperty, 1);
            panel = new(typeof(Grid));
            panel.AppendChild(waku);

            SetTemplate();

            PreviewMouseDown += ItemThumb_PreviewMouseDown;
        }

        //クリックしたとき、CurrentItemを更新、ActiveThumbを更新
        //ctrlキーが押されていたら選択Collectionに追加を試みる
        private void ItemThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyCanvas is not CanvasThumb canvas)
            {
                canvas = SetMyCanvas();
            }

            canvas.MyCurrentItem = this;
            canvas.MyActiveThumb = GetActiveThumb(this);
            if (Keyboard.Modifiers == ModifierKeys.Control
                && canvas.MyActiveThumb is TThumb actThumb)
            {
                canvas.MySelectedThumbs.Add(actThumb);
            }
            //if (canvas.MyActiveThumb is not TThumb active) return;
            //canvas.MySelectedThumbs.Add(active);


            //if (MyCanvas is null)
            //{
            //    AddSelectedThumb2(SetMyCanvas(), this);
            //}
            //else
            //{
            //    AddSelectedThumb2(MyCanvas, this);
            //}

            //AddSelectedThumb3(MyCanvas, GetActiveThumb(this));
        }
        //private static void AddSelectedThumb(CanvasThumb canvas, TThumb addItem)
        //{
        //    if (canvas.MySelectedThumbs.Count > 0
        //        && addItem.MyParentThumb != canvas.MySelectedThumbs[0]?.MyParentThumb)
        //    {
        //        canvas.MySelectedThumbs.Clear();
        //    }
        //    canvas.MySelectedThumbs.Add(addItem);
        //}
        private void AddSelectedThumb2(CanvasThumb canvas, ItemThumb item)
        {

            canvas.MyCurrentItem = item;
            if (GetActiveThumb(item) is TThumb active)
            {
                if (canvas.MySelectedThumbs.Count > 0
                    && active.MyParentThumb != canvas.MySelectedThumbs[0].MyParentThumb)
                {
                    canvas.MySelectedThumbs.Clear();
                }
                canvas.MySelectedThumbs.Add(active);
            }
        }
        private void AddSelectedThumb3(CanvasThumb? canvas, TThumb? active)
        {
            if (active is null) return;
            if (canvas is null) canvas = SetMyCanvas();
            if (canvas.MySelectedThumbs.Count > 0
                && active.MyParentThumb != canvas.MySelectedThumbs[0].MyParentThumb)
            {
                canvas.MySelectedThumbs.Clear();
            }
            canvas.MySelectedThumbs.Add(active);
        }


        protected static Binding MakeTwoWayBinding(string path, object source)
        {
            return new(path) { Source = source, Mode = BindingMode.TwoWay };
        }
        protected void MySetBinding(FrameworkElementFactory elem, DependencyProperty dp, string path)
        {
            elem.SetBinding(dp, MakeTwoWayBinding(path, MyData));
        }


    }
    public class TTextBlock : ItemThumb
    {
        public new DataTextBlock MyData { get; protected set; }
        public TTextBlock(DataTextBlock data) : base(data)
        {
            MyData = data;

        }

        protected override void SetTemplate()
        {
            FrameworkElementFactory content = new(typeof(TextBlock));
            MySetBinding(content, TextBlock.TextProperty, nameof(MyData.Text));
            MySetBinding(content, TextBlock.ForegroundProperty, nameof(MyData.ColorFont));
            MySetBinding(content, TextBlock.BackgroundProperty, nameof(MyData.ColorBack));
            MySetBinding(content, TextBlock.FontSizeProperty, nameof(MyData.FontSize));
            panel.AppendChild(content);
            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;
        }
        public override string ToString()
        {
            return $" {base.ToString()}, {MyData.Text}";
        }

    }
    public class TRectangle : ItemThumb
    {
        public new DataRectangle MyData { get; protected set; }
        public TRectangle(DataRectangle myData) : base(myData)
        {
            MyData = myData;
        }

        protected override void SetTemplate()
        {
            FrameworkElementFactory content = new(typeof(Rectangle));
            MySetBinding(content, Rectangle.WidthProperty, nameof(MyData.Width));
            MySetBinding(content, Rectangle.HeightProperty, nameof(MyData.Height));
            MySetBinding(content, Rectangle.FillProperty, nameof(MyData.ColorFill));
            MySetBinding(content, Rectangle.StrokeProperty, nameof(MyData.ColorStroke));
            MySetBinding(content, Rectangle.StrokeThicknessProperty, nameof(MyData.StrokeThickness));
            panel.AppendChild(content);
            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;
        }
    }



    #region グループ用Thumb


    //GroupとLayer、Canvasの基本クラス
    public abstract class GroupBase : TThumb
    {
        public new DataGroupBase MyData { get; protected set; }
        protected ObservableCollection<TThumb> Children { get; } = new();
        public ReadOnlyObservableCollection<TThumb> MyChildren { get; set; }
        protected GroupBase(DataGroupBase myData, string name = "") : base(myData, name)
        {
            MyData = myData;
            MyChildren = new(Children);
            SetTemplate();
            if (myData.ChildrenData.Count > 0)
            {
                throw new NotImplementedException();
            }
        }
        protected override void SetTemplate()
        {
            FrameworkElementFactory panel = new(typeof(Grid));

            FrameworkElementFactory content = new(typeof(ItemsControl));
            content.SetValue(ItemsControl.ItemsPanelProperty,
                new ItemsPanelTemplate(
                    new FrameworkElementFactory(typeof(Canvas))));
            content.SetValue(ItemsControl.ItemsSourceProperty,
                new Binding(nameof(MyChildren)));
            panel.AppendChild(content);

            FrameworkElementFactory waku = new(typeof(Rectangle));
            panel.AppendChild(waku);


            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;

        }
        public bool IsEditingThumb { get; private set; }
        protected static void SetIsEditingThumb(GroupAndLayerBase? oldItem, GroupAndLayerBase? newItem)
        {
            if (oldItem != null) oldItem.IsEditingThumb = false;
            if (newItem != null) newItem.IsEditingThumb = true;
        }

    }

    //GroupとLayerの基本クラス
    public abstract class GroupAndLayerBase : GroupBase
    {

        protected GroupAndLayerBase(DataGroupBase myData, string name = "") : base(myData, name)
        {

        }

        public void AddThumb(TThumb thumb, int z = -1)
        {
            //ZIndex指定なしは末尾(最上位)に追加
            if (z == -1)
            {
                thumb.MyData.Z = Children.Count;
                Children.Add(thumb);
            }
            else
            {
                thumb.MyData.Z = z;
                Children.Insert(z, thumb);
                for (int i = z; i < Children.Count; i++)
                {
                    Children[i].MyData.Z = i;
                }
            }
            thumb.MyParentThumb = this;
        }
    }

    public class GroupThumb : GroupAndLayerBase
    {
        public GroupThumb(DataGroup myData, string name = "") : base(myData, name)
        {

        }
        public void AddThumb(TThumb thumb)
        {
            base.AddThumb(thumb);

        }
    }
    public class LayerThumb : GroupAndLayerBase
    {
        public LayerThumb(DataLayer myData, string name = "") : base(myData, name)
        {
        }
        public void AddThumb(TThumb thumb)
        {
            base.AddThumb(thumb);

        }
    }


    public class CanvasThumb : GroupBase
    {
        protected new ObservableCollection<LayerThumb> Children { get; } = new();
        public new ReadOnlyObservableCollection<LayerThumb> MyChildren { get; set; }

        //最後にクリック(選択)したItem、今のItem、操作対象とは限らない
        public ItemThumb? MyCurrentItem
        {
            get { return (ItemThumb?)GetValue(MyCurrentItemProperty); }
            //set { SetValue(MyActiveItemProperty, value); }
            set
            {
                if (MyCurrentItem != value)
                {
                    SetIsMyCurrentItem(MyCurrentItem, value);
                    SetValue(MyCurrentItemProperty, value);

                    //MyActiveThumb = GetActiveThumb(value);
                    //UpdateActiveThumb();
                    //if (MyActiveThumb != null) MySelectedThumbs.Add(MyActiveThumb);
                }
            }
        }
        public static readonly DependencyProperty MyCurrentItemProperty =
            DependencyProperty.Register("MyCurrentItem", typeof(ItemThumb), typeof(CanvasThumb), new PropertyMetadata(null));

        //操作対象、移動、削除
        public TThumb? MyActiveThumb
        {
            get { return (TThumb?)GetValue(MyActiveThumbProperty); }
            set
            {
                if (MyActiveThumb != value)
                {
                    SetIsMyActiveThumb(MyActiveThumb, value);
                    SetValue(MyActiveThumbProperty, value);
                }
            }
        }
        public static readonly DependencyProperty MyActiveThumbProperty =
            DependencyProperty.Register(nameof(MyActiveThumb), typeof(TThumb), typeof(CanvasThumb), new PropertyMetadata(null));

        //編集中のグループ
        //変更時は直下Thumbのドラッグ移動イベント付け替えをする、ActiveThumbを変更
        public GroupAndLayerBase? MyEditingThumb
        {
            get { return (GroupAndLayerBase?)GetValue(MyEditingThumbProperty); }
            //set { SetValue(MyEditingThumbProperty, value); }
            set
            {
                if (MyEditingThumb != value)
                {
                    if (MyEditingThumb != null)
                    {
                        foreach (var item in MyEditingThumb.MyChildren)
                        {
                            item.RemoveDragEvents();
                        }
                    }
                    if (value != null)
                    {
                        foreach (var item in value.MyChildren)
                        {
                            item.AddDragEvents();
                        }
                    }

                    SetIsEditingThumb(MyEditingThumb, value);
                    SetValue(MyEditingThumbProperty, value);

                    MyActiveThumb = GetActiveThumb(MyCurrentItem);
                    //UpdateActiveThumb();
                }
            }
        }

        public static readonly DependencyProperty MyEditingThumbProperty =
            DependencyProperty.Register("MyEditingThumb", typeof(GroupAndLayerBase), typeof(CanvasThumb), new PropertyMetadata(null));


        public LayerThumb? MyActiveLayer
        {
            get { return (LayerThumb?)GetValue(MyActiveLayerProperty); }
            set { SetValue(MyActiveLayerProperty, value); }
        }

        public static readonly DependencyProperty MyActiveLayerProperty =
            DependencyProperty.Register(nameof(MyActiveLayer), typeof(LayerThumb), typeof(CanvasThumb), new PropertyMetadata(null));

        //選択Thumb、グループ化などに使用
        //public SelectedCollection<TThumb> MySelectedThumbs { get; private set; } = new();
        public SelectedCollection2 MySelectedThumbs { get; private set; } = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="myData"></param>
        public CanvasThumb(DataCanvas myData, string name = "") : base(myData, name)
        {
            MyChildren = new(Children);
            MySelectedThumbs.CollectionChanged += MySelectedThumbs_CollectionChanged;
        }

        private void MySelectedThumbs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems?[0] is TThumb nItem)
            {
                SetIsMySelected(nItem, true);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems?[0] is TThumb oItem)
            {
                SetIsMySelected(oItem, false);
            }
        }


        public void AddLayer(LayerThumb layer)
        {
            Children.Add(layer);
            layer.MyParentThumb = this;
        }

        //private void UpdateActiveThumb()
        //{
        //    MyActiveThumb = GetActiveThumb(MyCurrentItem);
        //}
    }

    #endregion グループ用Thumb


    //選択Thumb保持用Collection
    public class SelectedCollection<T> : ObservableCollection<T>
    {
        //protected override void InsertItem(int index, T item)
        //{
        //    if (Items.Contains(item))
        //    {
        //        Remove(item);

        //    }
        //    else base.InsertItem(index, item);
        //}
        //protected override void SetItem(int index, T item)
        //{
        //    if (Items.Contains(item)) Remove(item);
        //    else base.SetItem(index, item);
        //}
        //protected override void ClearItems()
        //{
        //    for (int i = Items.Count - 1; i >= 0; i--)
        //    {
        //        base.RemoveAt(i);
        //    }
        //    //base.ClearItems();
        //}
    }

    //選択Thumb保持用Collection
    //トグルにしたいので追加Itemがすでにある場合は追加せず逆に削除する
    //同一ParentThumbのChildrenだけの要素群にしたいので、
    //違うものが追加されてきたときはClear(リセット)する
    public class SelectedCollection2 : ObservableCollection<TThumb>
    {
        protected override void InsertItem(int index, TThumb item)
        {
            if (Items.Contains(item))
            {
                Remove(item);
                return;
            }
            else if (Items.Count > 0 && item.MyParentThumb != Items[0].MyParentThumb)
            {
                Clear();
                index = 0;
            }
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, TThumb item)
        {
            if (Items.Contains(item))
                Remove(item);
            else if (Items.Count > 0 &&
                item.MyParentThumb != Items[0].MyParentThumb)
                Clear();
            base.InsertItem(index, item);
        }
        protected override void ClearItems()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                base.RemoveAt(i);
            }
            //base.ClearItems();
        }
    }
}
