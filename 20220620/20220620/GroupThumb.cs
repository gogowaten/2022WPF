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

namespace _20220620
{
    internal class GroupThumb
    {
    }

    public abstract class Group1Base : TThumb1
    {
        #region フィールド

        public ItemsControl? MyItemsControl;//必要ない
        protected ObservableCollection<TThumb1> Children { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb1> Items { get; set; }

        //自身の編集状態の正否、枠表示の選択で使用
        private bool _IsMyEditing;
        public bool IsMyEditing
        {
            get => _IsMyEditing;
            set
            {
                if (_IsMyEditing == value) { return; }
                _IsMyEditing = value; OnPropertyChanged();
            }
        }
        #endregion フィールド

        #region コンストラクタ、初期化
        protected Group1Base(MainItemsControl main) : base(main)
        {
            Items = new(Children);
        }
        public Group1Base(MainItemsControl main, Data1 data) : base(main, data)
        {
            ////Layerなら編集状態にする
            //if (data.DataType == DataType.Layer) { IsMyEditing = true; }

            Items = new(Children);
            MyItemsControl = SetGroupThumbTemplate();

            //Binding
            MyItemsControl.DataContext = this;//子要素コレクションは自身をソース
            this.DataContext = MyData;//自身はデータをソースにする                                          

            //子要素の作成、追加
            Children.CollectionChanged += Children_CollectionChanged;
            foreach (Data1 childData in data.ChildrenData)
            {
                if (childData.DataTypeMain == DataTypeMain.Item)
                {
                    Item4 thumb = new(main, childData);
                    thumb.MyParentGroup = this;
                    AddThumb(thumb);
                    //Children.Add(thumb);
                }
                else
                {
                    Group4 thumb = new(MyMainItemsControl, childData);
                    thumb.MyParentGroup = this;
                    AddThumb(thumb);
                    //Children.Add(thumb);
                }
            }
            //Children.CollectionChanged += Children_CollectionChanged;



        }
        //public Group1Base(Data1 data) : base(data)
        //{

        //    Items = new(Children);
        //    MyItemsControl = SetGroupThumbTemplate();

        //    //Binding
        //    MyItemsControl.DataContext = this;//子要素コレクションは自身をソース
        //    this.DataContext = MyData;//自身はデータをソースにする                                          

        //    //子要素の作成、追加
        //    foreach (var item in data.ChildrenData)
        //    {
        //        Children.Add(new Item4(item));
        //    }

        //    Children.CollectionChanged += Children_CollectionChanged;


        //}


        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?[0] is TThumb1 addItem)
                {
                    //ChildrenDataへData追加、同じのがあれば追加しない
                    if (MyData.ChildrenData.Contains(addItem.MyData) == false)
                    {
                        if (addItem.MyData.Z == Children.Count)
                        {
                            MyData.ChildrenData.Add(addItem.MyData);
                        }
                        else
                        {
                            MyData.ChildrenData.Insert(addItem.MyData.Z, addItem.MyData);
                        }
                    }

                    //Parentの指定
                    addItem.MyParentGroup = this;
                    //Layerの指定
                    if (MyData.DataType == DataType.Layer)
                    {
                        addItem.MyLayer = (Layer1)this;
                    }
                    else
                    {
                        addItem.MyLayer = this.MyLayer;
                    }

                    //Parent(自身)が編集状態なら追加アイテムを
                    if (this == MyMainItemsControl.MyEditingGroup)
                    {
                        DragEventAdd(addItem);//ドラッグ移動可能にする
                    }
                    //if (this.IsMyEditing)
                    //{
                    //    DragEventAdd(addItem);//ドラッグ移動可能にする
                    //}
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems?[0] is TThumb1 removeItem)
                {
                    MyData.ChildrenData.Remove(removeItem.MyData);//Dataの削除
                    DragEventRemove(removeItem);
                }
            }
        }

        //複数要素表示用テンプレートに書き換える
        private ItemsControl SetGroupThumbTemplate()
        {
            //アイテムズコントロール
            FrameworkElementFactory itemsControl = new(typeof(ItemsControl), nameof(MyItemsControl));
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Items)));

            FrameworkElementFactory itemsCanvas = new(typeof(Canvas));
            itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(itemsCanvas));

            Binding b1 = new(nameof(IsMyEditing)) { Source = this };
            Binding b2 = new(nameof(IsMySelected)) { Source = this };
            MultiBinding mb = new();
            mb.Bindings.Add(b1); mb.Bindings.Add(b2);
            mb.Converter = new MyConverterGroupWaku4();
            List<Brush> brushes = new()
            {
                My2ColorDashBrush(4,Color.FromArgb(255,255,0,200),Colors.White),
                My2ColorDashBrush(4,Colors.DodgerBlue,Colors.White),
                //My2ColorDashBrush(4,Color.FromArgb(255,0,255,255),Colors.White),
                My2ColorDashBrush(4,Color.FromArgb(255,200,200,200),Colors.White),
            };
            mb.ConverterParameter = brushes;
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Group);
            waku.SetBinding(Rectangle.StrokeProperty, mb);
            waku.SetValue(Panel.ZIndexProperty, 1);//枠は前面表示
            FrameworkElementFactory baseCanvas = new(typeof(Canvas));
            baseCanvas.AppendChild(waku);//枠追加
            baseCanvas.AppendChild(itemsControl);

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();

            return (ItemsControl)template.FindName(nameof(MyItemsControl), this)
                ?? throw new ArgumentNullException(nameof(MyItemsControl));
        }

        #endregion コンストラクタ、初期化

        #region publucメソッド

        /// <summary>
        /// 自身のRect(位置とサイズ)をchildrenから取得
        /// </summary>
        /// <returns></returns>
        public (double x, double y, double w, double h) GetUnionRectForChildren()
        {
            if (Children.Count == 0) { return (0, 0, 0, 0); }

            double x = double.MaxValue; double y = double.MaxValue;
            double width = double.MinValue; double height = double.MinValue;
            foreach (var item in Children)
            {
                x = Math.Min(x, item.MyData.X);
                y = Math.Min(y, item.MyData.Y);
                width = Math.Max(width, item.MyData.X + item.Width);
                height = Math.Max(height, item.MyData.Y + item.Height);
            }
            width -= x; height -= y;
            return (x, y, width, height);
        }

        //サイズと位置の更新
        //使用：削除時
        public void AjustLocate3()
        {
            //新しいRect取得、Parentがnullの場合は0が返ってくる
            (double x, double y, double w, double h) = GetUnionRectForChildren();

            //位置とサイズともに変化無ければ終了
            if (w == 0 && h == 0) { return; }
            if (x == MyData.X &&
                y == MyData.Y &&
                w == Width &&
                h == Height) { return; }

            //位置が変化していた場合は自身とItemsの位置修正
            if (x != 0 || y != 0)
            {
                //自身がGroupタイプなら位置修正する
                if (MyData.DataType == DataType.Group)
                {
                    MyData.X += x; MyData.Y += y;
                }
                //Itemの位置修正
                foreach (var item in Items)
                {
                    item.MyData.X -= x; item.MyData.Y -= y;
                }
            }
            //自身のサイズが異なっていた場合は修正
            if (w != Width || h != Height)
            {
                Width = w; Height = h;
            }
            //Parentを辿り、再帰処理する
            if (MyParentGroup != null) { MyParentGroup.AjustLocate3(); };
        }




        /// <summary>
        /// グループ解除、自身を解除、子要素は親要素へ移動＋選択状態にする
        /// </summary>
        public void Ungroup2()
        {
            //if (MyParentGroup == null) { return; }
            ////上にあるThumbZの底上げ
            //for (int i = MyData.Z + 1; i < MyParentGroup.Children.Count; i++)
            //{
            //    //削除対象子要素数-1ぶん底上げする
            //    MyParentGroup.Children[i].MyData.Z += Children.Count - 1;
            //}
            ////子要素に対してx,y,zの調整
            //foreach (var item in Children)
            //{
            //    item.MyData.X += MyData.X;
            //    item.MyData.Y += MyData.Y;
            //    item.MyData.Z += MyData.Z;//Zを削除対象のZぶん底上げする
            //}

            ////追加してから削除、しないと削除時点で要素数が2未満になった場合にそれも解除対象になってしまうから
            ////解除後は子要素群を選択状態にするので今のはクリアする
            //MyLayer?.SelectThumbsCleal();
            ////子要素の処理
            //foreach (var item in Children)
            //{
            //    MyLayer?.SelectThumbAdd(item);//選択状態にする、必要ない
            //    MyParentGroup.InsertThumb(item);//Parentの子要素に挿入

            //    //再グループ用の情報を付加
            //    item.RegroupThumbs = Children.ToList();

            //}
            //MyParentGroup.RemoveThumb2(this);//削除

            ////兄弟の再グループリストから自身を削除
            //foreach (var brothers in MyParentGroup.Children)
            //{
            //    brothers.RegroupThumbs.Remove(this);
            //    if (brothers.RegroupThumbs.Count <= 1)
            //    {
            //        brothers.RegroupThumbs.Clear();
            //    }
            //}
        }

        /// <summary>
        /// 指定要素群からグループ作成して自身に挿入する
        /// </summary>
        /// <param name="thumbs">要素群は自身のChildrenから指定</param>
        /// <returns></returns>
        public bool MakeGroupFromChildren3(ReadOnlyObservableCollection<TThumb1>? thumbs)
        {
            return MakeGroupFromChildren3(thumbs?.ToList());
        }

        public bool MakeGroupFromChildren3(List<TThumb1>? thumbs)
        {
            if (thumbs == null || thumbs.Count < 2) { return false; }
            //指定要素群のParentが自身ではなかった場合は作成失敗
            foreach (var item in thumbs)
            { if (item.MyParentGroup != this) { return false; } }

            //新グループのX、Y、Zを要素群から計算取得
            var (x, y, minZ, maxZ) = GetXYZForNewGroup(thumbs);
            ////新グループのZ = 要素群の最上位Z - (要素数 - 1)
            //maxZ -= thumbs.Count - 1;
            //新グループのData作成
            Data1 data = new(DataType.Group);
            data.X = x; data.Y = y; data.Z = maxZ;

            //新グループ作成            
            Group4 group = new(this.MyMainItemsControl, data);//作成
            group.MyLayer = this.MyLayer;
            //Z順にソートした要素群作成
            var sortedThumbs = thumbs.OrderBy(x => x.MyData.Z).ToList();

            //新グループを作成挿入してから、元の要素群削除、新グループに要素追加、順番大切
            //Childrenに新グループを挿入
            InsertThumb(group);
            //Childrenから要素群削除
            foreach (var item in sortedThumbs) { RemoveThumb2(item); }
            //新グループに要素群追加
            foreach (var item in sortedThumbs)
            {
                group.AddThumb(item);
                //再グループ用の情報をクリア
                item.RegroupThumbs.Clear();
            }

            //子要素全体のZ調整、歯抜けになっているので数値を詰める
            for (int i = minZ; i < Children.Count; i++) { Children[i].MyData.Z = i; }

            return true;
        }

        private static (double x, double y, int minZ, int maxZ) GetXYZForNewGroup(List<TThumb1> thumbs)
        {
            double x = 0; double y = 0;//左上座標
            int maxZ = 0; int minZ = int.MaxValue;
            foreach (var item in thumbs)
            {
                x = Math.Min(x, item.MyData.X);
                y = Math.Min(y, item.MyData.Y);
                minZ = Math.Min(minZ, item.MyData.Z);//最下位Z
                maxZ = Math.Max(maxZ, item.MyData.Z);//最上位Z
            }
            return (x, y, minZ, maxZ);
        }

        #region Childrenの操作

        //指定ThumbをChildrenに追加
        public virtual void AddThumb(TThumb1 thumb)
        {
            //コレクションに追加
            thumb.MyData.Z = Children.Count;
            Children.Add(thumb);
        }
        //指定ThumbをChildrenに挿入
        public virtual void InsertThumb(TThumb1 thumb)
        {
            InsertThumb(thumb, thumb.MyData.Z);
        }
        public virtual void InsertThumb(TThumb1 thumb, int z)
        {
            //コレクションに挿入
            if (z >= Children.Count)
            {
                AddThumb(thumb);
            }
            else
            {
                Children.Insert(z, thumb);
            }

        }

        //指定ThumbをChildrenから削除
        public virtual void RemoveThumb2(TThumb1 thumb)
        {
            if (Children.Contains(thumb))
            {
                int z = thumb.MyData.Z;
                ////LastClickedをクリア
                //MyMainItemsControl.MyCurrentItem = null;

                //選択リストから削除
                MyMainItemsControl.MySelectedThumbs.Remove(thumb);

                //コレクションから削除
                Children.Remove(thumb);
                //2未満ならグループ解除
                if (Children.Count < 2) { Ungroup2(); }
                else
                {
                    for (int i = z; i < Children.Count; i++)
                    {
                        Children[i].MyData.Z = i;
                    }
                }
                //位置とサイズの修正
                AjustLocate3();
            }
            else
            {
                throw new AggregateException("グループ内に対象Thumbが見つからない");
            }
        }
        ////指定ThumbをChildrenから削除
        //public virtual void RemoveThumb(TThumb1 thumb)
        //{
        //    if (Children.Contains(thumb))
        //    {
        //        int z = thumb.MyData.Z;
        //        //LastClickedをクリア
        //        if (MyLayer != null) MyLayer.LastClickedItem = null;
        //        //選択リストから削除
        //        MyLayer?.SelectThumbRemove(thumb);
        //        //コレクションから削除
        //        Children.Remove(thumb);
        //        //2未満ならグループ解除
        //        if (Children.Count < 2)
        //        {
        //            Ungroup2();
        //            //Ungroup();
        //        }
        //        else
        //        {
        //            for (int i = z; i < Children.Count; i++)
        //            {
        //                Children[i].MyData.Z = i;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        throw new AggregateException("グループ内に対象Thumbが見つからない");
        //    }
        //}
        public virtual void MoveThumbIndexWithZIndex(int oldIndex, int newIndex)
        {
            Children.Move(oldIndex, newIndex);
            int min; int max;
            if (oldIndex < newIndex) { min = oldIndex; max = newIndex; }
            else { min = newIndex; max = oldIndex; }
            for (int i = min; i <= max; i++)
            {
                Children[i].MyData.Z = i;
            }
        }
        #endregion Childrenの操作
        #endregion publucメソッド

        //Childrenに対してドラッグ移動イベントの付加と削除
        internal void AddDragEventForChildren()
        {
            foreach (var item in Children)
            {
                DragEventAdd(item);
            }
        }
        internal virtual void RemoveDragEventForChildren()
        {
            foreach (var item in Children)
            {
                DragEventRemove(item);
            }
        }

        //最下層まで全てのThumbのMyLayerの指定
        internal void SetMyLayer2(Layer1 layer)
        {
            this.MyLayer = layer;
            foreach (var item in Children)
            {
                item.MyLayer = layer;
                if (item is Group1Base group)
                {
                    group.SetMyLayer2(layer);
                }
            }
        }

    }

    public class Group4 : Group1Base
    {
        public Group4(MainItemsControl main) : this(main, new Data1(DataType.Group)) { }
        public Group4(MainItemsControl main, Data1 data) : base(main, data) { }

    }
    public class Layer1 : Group1Base
    {
        #region 通知プロパティ
        //今編集状態(子要素の移動可能)のGroup
        //private Group1Base? _NowEditingThumb;
        //public Group1Base? NowEditingThumb
        //{
        //    get { return _NowEditingThumb; }
        //    private set
        //    {
        //        if (_NowEditingThumb == value) { return; }
        //        _NowEditingThumb = value; OnPropertyChanged();
        //    }
        //}
        //public void SetNowEditingThumb(Group1Base? newEditing, TThumb1 lastClicked)
        //{
        //    if (NowEditingThumb == newEditing) { return; }
        //    //選択リストのリセット
        //    if (SelectedThumbs.Count > 0)
        //    {
        //        foreach (var item in _SelectedThumbs)
        //        {
        //            item.IsMySelected = false;
        //        }
        //        _SelectedThumbs.Clear();
        //    }
        //    //ドラッグ移動イベントの付け外し
        //    if (NowEditingThumb != null)
        //    {
        //        NowEditingThumb.IsMyEditing = false;
        //        NowEditingThumb.RemoveDragEventForChildren();
        //    }

        //    NowEditingThumb = newEditing;//切り替え

        //    if (NowEditingThumb != null)
        //    {
        //        NowEditingThumb.IsMyEditing = true;
        //        NowEditingThumb.AddDragEventForChildren();
        //    }

        //    TThumb1? active = lastClicked.GetMyActiveMoveThumb();
        //    if (active == null) { active = lastClicked; }
        //    SelectThumbAdd(active);
        //}



        //最後の一個前に選択されたItem
        //public Item4? LastPreviousClickedItem;
        //最後にクリックされたItem
        //private Item4? _lastClickedItem;
        //public Item4? LastClickedItem
        //{
        //    get => _lastClickedItem;
        //    set
        //    {
        //        //古い方を記録
        //        LastPreviousClickedItem = _lastClickedItem;
        //        //格納しているThumbと同じなら必要なし、終了
        //        if (_lastClickedItem == value) { return; }

        //        //古い方のIsLastClickedをfalseに変更してから
        //        if (_lastClickedItem != null)
        //        {
        //            _lastClickedItem.IsMyLastClicked = false;
        //        }
        //        //新しい方のIsLastClickedをtrue
        //        if (value != null)
        //        {
        //            value.IsMyLastClicked = true;
        //        }

        //        //新旧入れ替え
        //        _lastClickedItem = value;
        //        OnPropertyChanged();
        //    }
        //}

        //選択状態のThumb群
        //private readonly ObservableCollection<TThumb1> _SelectedThumbs = new();
        //public ReadOnlyObservableCollection<TThumb1> SelectedThumbs;
        #endregion 通知プロパティ

        public Layer1(MainItemsControl main) : this(main, new Data1(DataType.Layer)) { }
        public Layer1(MainItemsControl main, Data1 data) : base(main, data)
        {
            ////Layerなので編集状態にする
            //NowEditingThumb = this;
            //IsMyEditing = true;

            //_SelectedThumbs.CollectionChanged += SelectedThumbs_CollectionChanged;
            //SelectedThumbs = new(_SelectedThumbs);
            SetMyLayer2(this);
        }


        #region メソッド

        #region 選択状態Thumb群の操作
        //private void SelectedThumbs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        //{
        //    TThumb1? nn = e.NewItems?[0] as TThumb1;
        //    TThumb1? oo = e.OldItems?[0] as TThumb1;
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            if (nn == null) { return; }
        //            nn.IsMySelected = true;
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            if (oo == null) { return; }
        //            oo.IsMySelected = false;
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            if (nn == null || oo == null) { return; }
        //            oo.IsMySelected = false;
        //            nn.IsMySelected = true;
        //            var item0 = e.NewItems?[0];
        //            var item1 = e.OldItems?[0];
        //            break;
        //        case NotifyCollectionChangedAction.Move:
        //            var item2 = e.NewItems?[0];
        //            var item3 = e.OldItems?[0];
        //            break;
        //        case NotifyCollectionChangedAction.Reset:

        //            break;
        //        default:
        //            break;
        //    }
        //}

        ////選択状態Thumbの追加
        //public void SelectThumbAdd(TThumb1? thumb)
        //{
        //    //同じThumbがないかチェックしてから追加
        //    if (thumb == null) { return; }
        //    if (SelectedThumbs.Contains(thumb)) { return; }
        //    _SelectedThumbs.Add(thumb);
        //}
        ////選択状態Thumbの削除
        //public void SelectThumbRemove(TThumb1 thumb)
        //{
        //    if (!SelectedThumbs.Contains(thumb)) { return; }
        //    _SelectedThumbs.Remove(thumb);
        //}
        ////選択状態Thumbの入れ替え
        //public void SelectThumbReplace(TThumb1? thumb)
        //{
        //    if (thumb == null) { return; }
        //    if (SelectedThumbs.Count == 1)
        //    {
        //        if (SelectedThumbs[0] != thumb)
        //        {
        //            _SelectedThumbs.Remove(SelectedThumbs[0]);
        //            _SelectedThumbs.Add(thumb);
        //        }
        //    }
        //    else if (SelectedThumbs.Count > 1)
        //    {
        //        foreach (var item in SelectedThumbs)
        //        {
        //            item.IsMySelected = false;
        //        }
        //        _SelectedThumbs.Clear();
        //        _SelectedThumbs.Add(thumb);
        //    }
        //    else
        //    {
        //        _SelectedThumbs.Add(thumb);
        //    }
        //}
        ////選択状態Thumbのクリア
        //public void SelectThumbsCleal()
        //{
        //    if (_SelectedThumbs.Count == 0) { return; }
        //    foreach (var item in _SelectedThumbs)
        //    {
        //        item.IsMySelected = false;
        //    }
        //    _SelectedThumbs.Clear();
        //}
        #endregion 選択状態Thumb群の操作
        //追加
        public override void AddThumb(TThumb1 thumb)
        {
            //基底クラスのメソッドを実行
            base.AddThumb(thumb);
            //すべてのThumbのMyLayerに自身を指定する
            if (thumb.MyData.DataType == DataType.Group)
            {
                SetMyLayer2(this);
            }
            else if (thumb.MyData.DataTypeMain == DataTypeMain.Item)
            {
                thumb.MyLayer = this;
            }
        }

        #endregion メソッド



    }
    //    C# ObservableCollection<T>で大量の要素を追加したいとき - Qiita
    //https://qiita.com/Yuki4/items/0e73297db632376804dd

    public class ObsevableCollectionEX<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> addItems)
        {
            if (addItems == null)
            {
                throw new ArgumentNullException(nameof(addItems));
            }
            foreach (var item in addItems)
            {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Replace(T newItem, T oldItem)
        {
            int num = Items.IndexOf(oldItem);
            Items.Remove(oldItem);
            Items.Insert(num, newItem);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
        }
    }


    //    c＃-WPFを使用したObservableCollectionでのHashSetの使用-スタックオーバーフロー
    //https://stackoverflow.com/questions/1793109/using-hashsets-with-observablecollection-with-wpf

    //重複チェックしてから追加するObservableCollection
    public class ObservableCollectionSetCollection<T> : ObservableCollection<T>
    {
        //Add, Insert
        protected override void InsertItem(int index, T item)
        {
            if (Items.Contains(item)) return;
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, T item)
        {
            if (Items.Contains(item)) return;
            base.SetItem(index, item);
        }
    }

    //追加しようとしたitemがリストに存在した場合は、削除する
    //トグルでアイテム選択する選択リストで使用
    public class ObservableCollectionToggleCollection<T> : ObservableCollection<T>
    {
        //Add, Insert
        protected override void InsertItem(int index, T item)
        {
            if (Items.Contains(item)) base.Remove(item);
            else base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
        }
        protected override void SetItem(int index, T item)
        {
            if (Items.Contains(item)) Remove(item);
            else base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

}
