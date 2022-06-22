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
using System.Xml;

namespace _20220620
{
    //public class MainCanvas : Canvas
    //{


    //}
    public class MainItemsControl : ItemsControl, INotifyPropertyChanged
    {
        #region 通知プロパティ
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ObservableCollection<Layer1> MyLayers { get; set; } = new();

        //選択状態のThumb群
        public ObservableCollectionToggleCollection<TThumb1> MySelectedThumbs { get; set; } = new();

        //操作対象のLayer
        private Layer1? _myCurrentLayer;
        public Layer1? MyCurrentLayer
        {
            get => _myCurrentLayer;
            set { if (value == _myCurrentLayer) { return; } _myCurrentLayer = value; OnPropertyChanged(); }
        }
        //public Group1Base? MyCurrentGroup;

        //編集状態Group、中のThumbが移動可能状態
        private Group1Base? _myEditingGroup;
        public Group1Base? MyEditingGroup
        {
            get => _myEditingGroup;
            set
            {
                if (value == _myEditingGroup) { return; }
                //選択状態のThumb群のクリア
                MySelectedThumbs.Clear();
                //旧グループItemsからドラッグ移動イベントを削除
                MyEditingGroup?.RemoveDragEventForChildren();
                //入れ替え＆通知
                _myEditingGroup = value; OnPropertyChanged();
                //新グループItemsへのドラッグ移動イベント追加
                MyEditingGroup?.AddDragEventForChildren();

                //移動対象Thumbのクリア
                MyActiveMovableThumb = null;

            }
        }

        //移動対象Thumb
        private TThumb1? _myActiveMovableThumb;
        public TThumb1? MyActiveMovableThumb
        {
            get => _myActiveMovableThumb; set
            { if (value == _myActiveMovableThumb) { return; } _myActiveMovableThumb = value; OnPropertyChanged(); }
        }

        //注目ItemThumb、直前にクリックされたor追加された
        private Item4? _myCurrentItem;
        public Item4? MyCurrentItem
        {
            get => _myCurrentItem; set
            {
                //新旧入れ替え前処理
                //古い方を1つ前注目にコピー
                MyPreviousCurrentItem = _myCurrentItem;

                //新旧が同じitemならEditingとActiveの更新
                //EditingをCurrentItemに近づける(ActiveをEditingに指定)
                if (value == _myCurrentItem && _myCurrentItem != null)
                {
                    if (MyActiveMovableThumb?.MyData.DataTypeMain == DataTypeMain.Group)
                    {
                        MyEditingGroup = (Group1Base)MyActiveMovableThumb;
                        MyActiveMovableThumb = _myCurrentItem.GetMyActiveMoveThumb();
                        return;
                    }
                    return;
                }

                //以下はクリックitemが前回のもととは違った場合の処理
                //編集グループ外だった場合は編集グループにLayerを指定
                if (IsInEditingGroup(value) == false)
                {
                    MyEditingGroup = MyCurrentLayer;
                }

                //古い方のIsLastClickedをfalseに変更してから
                if (_myCurrentItem != null)
                {
                    _myCurrentItem.IsMyLastClicked = false;
                }
                //新しい方のIsLastClickedをtrue
                if (value != null)
                {
                    value.IsMyLastClicked = true;
                }

                //新旧入れ替え
                _myCurrentItem = value; OnPropertyChanged();

                //Activeの更新
                MyActiveMovableThumb = value?.GetMyActiveMoveThumb();
                if (MyActiveMovableThumb == null)
                {
                    MyActiveMovableThumb = value;
                }

            }
        }

        //1つ前に注目itemThumbだったもの
        private Item4? _myPreviousCurrentItem;
        public Item4? MyPreviousCurrentItem
        {
            get => _myPreviousCurrentItem; set
            {
                if (value == _myPreviousCurrentItem) { return; }
                _myPreviousCurrentItem = value; OnPropertyChanged();
            }
        }



        #endregion 通知プロパティ


        #region コンストラクタ
        public MainItemsControl()
        {
            DataContext = this;
            SetTemplate();
            MyLayers.CollectionChanged += MyLayers_CollectionChanged;
            MySelectedThumbs.CollectionChanged += SelectedThumbs_CollectionChanged;
            Layer1 l = new(this, new Data1(DataType.Layer));
            MyLayers.Add(l);
            MyCurrentLayer = l;
            MyEditingGroup = l;
        }



        public MainItemsControl(Layer1 layer) : this()
        {
            MyLayers.Add(layer);
            MyCurrentLayer = layer;
            MyEditingGroup = layer;

        }
        private void SetTemplate()
        {
            //FrameworkElementFactory panel = new(typeof(Canvas));
            ItemsPanelTemplate panelTemplate = new(new(typeof(Canvas)));
            this.SetBinding(ItemsSourceProperty, new Binding(nameof(MyLayers)));
            this.ItemsPanel = panelTemplate;
        }

        #endregion コンストラクタ

        private void MyLayers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //if (e.NewItems?[0] is Layer1 layer)
                //{
                //    MyCurrentLayer = layer;
                //}
            }
        }
        private void SelectedThumbs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //削除時、フラグ解除とCurrentのクリア
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems?[0] is TThumb1 tt)
                {
                    tt.IsMySelected = false;
                    if (MyCurrentItem == tt) MyCurrentItem = null;
                    if (MyActiveMovableThumb == tt) MyActiveMovableThumb = null;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (e.OldItems is null) { return; }
                foreach (var item in e.OldItems)
                {
                    if (item is TThumb1 tt) tt.IsMySelected = false;
                }
            }
        }
        #region publicメソッド
        //編集グループ内に属しているかの判定
        public bool IsInEditingGroup(TThumb1? thumb)
        {
            if (thumb == null) return false;
            if (thumb?.MyParentGroup == MyEditingGroup) return true;
            else { IsInEditingGroup(thumb?.MyParentGroup); }
            return false;
        }
        public void AddItem(Group1Base group, TThumb1 thumb)
        {
            group.AddThumb(thumb);
        }
        public void AddItem(Data1 data)
        {
            if (MyEditingGroup == null) { return; }
            if (data.DataTypeMain == DataTypeMain.Item)
            {
                Item4 item = new(this, data);
                MyEditingGroup.AddThumb(item);
                MyCurrentItem = item;
            }
            else
            {

            }
        }
        public void RemoveThumb(TThumb1 thumb)
        {
            MyEditingGroup?.RemoveThumb2(thumb);
        }
        public void RemoveActiveThumb()
        {
            if (MyActiveMovableThumb == null) return;
            MyEditingGroup?.RemoveThumb2(MyActiveMovableThumb);
            MyActiveMovableThumb = null;
        }
        //public void ChangeEditingGroup(Group1Base? newGroup, TThumb1 lastClicked)
        //{
        //    if (MyEditingGroup == newGroup) { return; }
        //    //選択リストのリセット
        //    if (MySelectedThumbs.Count > 0)
        //    {
        //        foreach (var item in MySelectedThumbs)
        //        {
        //            item.IsMySelected = false;
        //        }
        //        MySelectedThumbs.Clear();
        //    }
        //    //ドラッグ移動イベントの付け外し
        //    if (MyEditingGroup != null)
        //    {
        //        MyEditingGroup.IsMyEditing = false;
        //        MyEditingGroup.RemoveDragEventForChildren();
        //    }

        //    NowEditingThumb = newGroup;//切り替え

        //    if (NowEditingThumb != null)
        //    {
        //        NowEditingThumb.IsMyEditing = true;
        //        NowEditingThumb.AddDragEventForChildren();
        //    }

        //    TThumb1? active = lastClicked.GetMyActiveMoveThumb();
        //    if (active == null) { active = lastClicked; }
        //    SelectThumbAdd(active);
        //}

        #endregion publicメソッド

    }
}
