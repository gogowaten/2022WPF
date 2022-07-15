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


        public bool IsMyCurrentItem
        {
            get { return (bool)GetValue(IsMyCurrentItemProperty); }
            private set { SetValue(IsMyCurrentItemProperty, value); }
        }
        public static readonly DependencyProperty IsMyCurrentItemProperty =
            DependencyProperty.Register(nameof(IsMyCurrentItem), typeof(bool), typeof(TThumb), new PropertyMetadata(false));

        public bool IsMyActiveThumb
        {
            get { return (bool)GetValue(IsMyActiveThumbProperty); }
            private set { SetValue(IsMyActiveThumbProperty, value); }
        }
        public static readonly DependencyProperty IsMyActiveThumbProperty =
            DependencyProperty.Register(nameof(IsMyActiveThumb), typeof(bool), typeof(TThumb), new PropertyMetadata(false));

        public bool IsMySelected
        {
            get { return (bool)GetValue(IsMySelectedProperty); }
            private set { SetValue(IsMySelectedProperty, value); }
        }
        public static readonly DependencyProperty IsMySelectedProperty =
            DependencyProperty.Register(nameof(IsMySelected), typeof(bool), typeof(TThumb), new PropertyMetadata(false));

        //選択リストに追加削除のフラグ
        public bool IsAddedJustBefore { get; set; }
        public GroupBase? MyParentThumb { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="myData"></param>
        /// <param name="name"></param>
        public TThumb(Data myData, string name = "")
        {
            MyData = myData;
            Name = name;
            DataContext = this;

            SetMyDataBinding();

            //サイズ変更があった場合はParentのサイズと位置を調整
            //基本的にサイズ変更は最初に表示されるときだけ発生
            SizeChanged += (a, b) => { MyParentThumb?.AjustSizeAndLocate3(); };
        }

        protected static void SetIsMySelected(TThumb thumb, bool b)
        {
            thumb.IsMySelected = b;
        }


        protected TThumb? GetActiveThumb(TThumb? thumb)
        {
            if (thumb == null) { return null; }
            else if (thumb.MyParentThumb is GroupAndLayerBase group)
            {
                if (group.IsMyEditingGroup) { return thumb; }
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
        private CanvasThumb? GetMyCanvas()
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
            DragStarted += TThumb_DragStarted;
            DragDelta += TThumb_DragDelta;
            DragCompleted += TThumb_DragCompleted;
        }

        private void TThumb_DragStarted(object sender, DragStartedEventArgs e)
        {

            CanvasThumb canvas = MyCanvas ?? SetMyCanvas();
            canvas.MyActiveThumb = this;

            //selected
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                canvas.MySelectedThumbs.Clear();
                canvas.MySelectedThumbs.Add(this);
                this.IsAddedJustBefore = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                canvas.MySelectedThumbs.Add(this);
                this.IsAddedJustBefore = true;
            }



        }

        private void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            MyParentThumb?.AjustSizeAndLocate3();

            if (IsAddedJustBefore)
                IsAddedJustBefore = false;
            else if (IsAddedJustBefore == false && e.HorizontalChange == 0 && e.VerticalChange == 0)
            {
                CanvasThumb ct = MyCanvas ?? SetMyCanvas();
                if (ct.MySelectedThumbs.Contains(this))
                {
                    ct.MySelectedThumbs.Remove(this);
                }
            }

        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            CanvasThumb canvas = MyCanvas ?? SetMyCanvas();
            foreach (var item in canvas.MySelectedThumbs)
            {
                item.MyData.X += e.HorizontalChange;
                item.MyData.Y += e.VerticalChange;
            }

            //MyData.X += e.HorizontalChange;
            //MyData.Y += e.VerticalChange;
        }
        public void RemoveDragEvents()
        {
            DragDelta -= TThumb_DragDelta;
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

            //b = new() { Source = this, Path = new PropertyPath(ActualWidthProperty) };
            //this.SetBinding(WidthProperty, b);
            //b = new() { Source = this, Path = new PropertyPath(ActualHeightProperty) };
            //this.SetBinding(HeightProperty, b);
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


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="myData"></param>
        public ItemThumb(Data myData) : base(myData)
        {
            waku = new(typeof(Rectangle));
            waku.SetValue(Panel.ZIndexProperty, 1);
            MultiBinding mb = new();
            mb.Converter = new MyConverterForItemWaku();
            mb.Bindings.Add(MakeBind(TThumb.IsMyCurrentItemProperty));
            mb.Bindings.Add(MakeBind(TThumb.IsMyActiveThumbProperty));
            mb.Bindings.Add(MakeBind(TThumb.IsMySelectedProperty));
            waku.SetBinding(Rectangle.StrokeProperty, mb);

            panel = new(typeof(Grid));
            panel.AppendChild(waku);

            SetTemplate();

            PreviewMouseDown += ItemThumb_PreviewMouseDown;
            PreviewMouseUp += ItemThumb_PreviewMouseUp;
            Binding MakeBind(DependencyProperty dp)
            {
                Binding b = new();
                b.Source = this;
                //b.Mode = BindingMode.TwoWay;
                b.Path = new PropertyPath(dp);
                return b;
            }
        }

        //マウスアップ時
        //選択状態の解除、条件は
        //選択状態が複数＋クリック対象が選択状態
        private void ItemThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        //クリックしたとき、CurrentItemを更新、ActiveThumbを更新
        //ctrlキーが押されていたら選択Collectionに追加を試みる
        private void ItemThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (MyCanvas is not CanvasThumb canvas)
            //{
            //    canvas = SetMyCanvas();
            //}
            CanvasThumb canvas = MyCanvas ?? SetMyCanvas();
            canvas.MyCurrentItem = this;
            //TThumb? act = GetActiveThumb(this);
            //canvas.MyActiveThumb = act;

            ////selected
            //if (act is not null)
            //{
            //    if (Keyboard.Modifiers == ModifierKeys.None)
            //    {
            //        canvas.MySelectedThumbs.Clear();
            //        canvas.MySelectedThumbs.Add(act);
            //    }
            //    else if (Keyboard.Modifiers == ModifierKeys.Control)
            //    {
            //        canvas.MySelectedThumbs.Add(act);
            //    }

            //}
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
            return $"{base.ToString()}, {MyData.Text}";
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
            //Grid
            // ┣ItemsControl
            // ┗Rectangle
            FrameworkElementFactory panel = new(typeof(Grid));

            FrameworkElementFactory content = new(typeof(ItemsControl));
            content.SetValue(ItemsControl.ItemsPanelProperty,
                new ItemsPanelTemplate(
                    new FrameworkElementFactory(typeof(Canvas))));
            content.SetValue(ItemsControl.ItemsSourceProperty,
                new Binding(nameof(MyChildren)));
            panel.AppendChild(content);

            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Panel.ZIndexProperty, 1);
            MultiBinding mb = new();
            mb.Converter = new MyConverterForGroupWaku();
            mb.Bindings.Add(MakeBinding(IsMyActiveThumbProperty));
            mb.Bindings.Add(MakeBinding(IsMySelectedProperty));
            mb.Bindings.Add(MakeBinding(IsMyEditingGroupProperty));
            waku.SetBinding(Rectangle.StrokeProperty, mb);
            panel.AppendChild(waku);

            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;

            Binding MakeBinding(DependencyProperty dp)
            {
                return new() { Source = this, Path = new PropertyPath(dp) };
            }
        }


        public bool IsMyEditingGroup
        {
            get { return (bool)GetValue(IsMyEditingGroupProperty); }
            set { SetValue(IsMyEditingGroupProperty, value); }
        }
        public static readonly DependencyProperty IsMyEditingGroupProperty =
            DependencyProperty.Register(nameof(IsMyEditingGroup), typeof(bool), typeof(GroupBase), new PropertyMetadata(false));

        protected static void SetIsEditingThumb(GroupAndLayerBase? oldItem, GroupAndLayerBase? newItem)
        {
            if (oldItem != null) oldItem.IsMyEditingGroup = false;
            if (newItem != null) newItem.IsMyEditingGroup = true;
        }

        /// <summary>
        /// 自身のRect(位置とサイズ)をThumbのコレクションから取得
        /// </summary>
        /// <param name="thumbs">Thumbのコレクション</param>
        /// <returns></returns>
        public (double x, double y, double w, double h) GetRectUnionChildren(IEnumerable<TThumb> thumbs)
        {
            if (Children.Count == 0) { return (0, 0, 0, 0); }

            double x = double.MaxValue; double y = double.MaxValue;
            double width = double.MinValue; double height = double.MinValue;
            foreach (var item in thumbs)
            {
                x = Math.Min(x, item.MyData.X);
                y = Math.Min(y, item.MyData.Y);
                width = Math.Max(width, item.MyData.X + item.ActualWidth);
                height = Math.Max(height, item.MyData.Y + item.ActualHeight);
            }
            width -= x; height -= y;
            return (x, y, width, height);
        }

        //サイズと位置の更新
        //使用場面は、追加、削除、グループ化、グループ解除
        public void AjustSizeAndLocate3()
        {
            //新しいRect取得、Parentがnullの場合は0が返ってくる
            (double x, double y, double w, double h) = GetRectUnionChildren(Children);

            //位置とサイズともに変化無ければ終了
            if (w == 0 && h == 0) { return; }
            if (x == MyData.X &&
                y == MyData.Y &&
                w == Width &&
                h == Height) { return; }

            //位置が変化していた場合は自身と子要素の位置修正
            if (x != 0 || y != 0)
            {
                //自身がGroupタイプなら位置修正する(LayerとCanvasは修正しない常に0,0)
                if (MyData.DataGroupType == DataGroupType.Group)
                { MyData.X += x; MyData.Y += y; }
                //子要素の位置修正
                foreach (var item in Children)
                { item.MyData.X -= x; item.MyData.Y -= y; }
            }
            //自身のサイズが異なっていた場合は修正
            if (w != Width || h != Height)
            { Width = w; Height = h; }
            //Parentを辿り、再帰処理する
            if (MyParentThumb != null) { MyParentThumb.AjustSizeAndLocate3(); };
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
            AjustSizeAndLocate3();
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
    //トグルにしたいので追加Itemがすでにある場合は追加せず逆に削除する、だたし
    //Countが1で、それが追加Itemと同じだった場合は何もしない。
    //同一ParentThumbのChildrenだけの要素群にしたいので、
    //違うものが追加されてきたときはClear(リセット)する
    public class SelectedCollection2 : ObservableCollection<TThumb>
    {
        protected override void InsertItem(int index, TThumb item)
        {
            if (Items.Contains(item))
            {
                if (Items.Count == 1)
                    return;
                else
                {

                    Remove(item);
                    return;
                }
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


    public class MyConverterForItemWaku : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0]) { return Brushes.Red; }
            else if ((bool)values[1]) { return Brushes.Green; }
            else if ((bool)values[2]) { return Brushes.Blue; }
            else return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MyConverterForGroupWaku : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0]) { return Brushes.Red; }
            else if ((bool)values[1]) { return Brushes.Green; }
            else if ((bool)values[2]) { return Brushes.Blue; }
            else return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
