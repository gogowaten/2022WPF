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
            get => _myActiveMovableThumb;
            set
            {
                if (value == _myActiveMovableThumb) { return; }
                _myActiveMovableThumb = value; OnPropertyChanged();
            }
        }

        //注目ItemThumb、直前にクリックされたor追加された
        private Item4? _myCurrentItem;
        public Item4? MyCurrentItem
        {
            get => _myCurrentItem;
            set
            {
                //新旧入れ替え前処理
                //古い方を1つ前注目にコピー
                MyPreviousCurrentItem = _myCurrentItem;

                if (value == null) MyActiveMovableThumb = null;
                if (value == _myCurrentItem) { return; }

                //古い方のIsLastClickedをfalseに変更してから
                if (_myCurrentItem != null)
                    _myCurrentItem.IsMyLastClicked = false;
                //新しい方のIsLastClickedをtrue
                if (value != null)
                    value.IsMyLastClicked = true;

                //新旧入れ替え
                _myCurrentItem = value; OnPropertyChanged();

                //Activeの更新
                MyActiveMovableThumb = value?.GetMyActiveGroup();
                if (MyActiveMovableThumb == null)
                {
                    MyActiveMovableThumb = value;
                }
                else
                {
                    MyActiveMovableThumb.IsMySelected = true;
                }
            }
        }

        //1つ前に注目itemThumbだったもの
        private Item4? _myPreviousCurrentItem;
        public Item4? MyPreviousCurrentItem
        {
            get => _myPreviousCurrentItem;
            set
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

        //選択Thumb群コレクション要素の変更時
        //追加時、フラグ変更
        //削除時、フラグ変更とCurrentのクリア
        private void SelectedThumbs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //追加時
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            {
                if (e.NewItems?[0] is TThumb1 tt) tt.IsMySelected = true;
            }
            //削除時(Remove, Clear)
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems?[0] is TThumb1 tt)
                {
                    tt.IsMySelected = false;
                    //if (MyCurrentItem == tt) MyCurrentItem = null;
                    //if (MyActiveMovableThumb == tt) MyActiveMovableThumb = null;
                }
            }
            //else if (e.Action == NotifyCollectionChangedAction.Reset)
            //{
            //    if (e.OldItems is null) { return; }
            //    //ResetのoldItemsは必ずnullなので↓に到達することはない
            //    foreach (var item in e.OldItems)
            //    {
            //        if (item is TThumb1 tt) tt.IsMySelected = false;
            //    }
            //}
        }


        #region publicメソッド
        //編集範囲を狭くする(1つ近づける)
        public void EditingNarrow(TThumb1? thumb)
        {
            //十分狭い場合はなにもしない
            if (thumb == null || thumb.MyParentGroup == MyEditingGroup) { return; }

            Group4? n = thumb.GetMyActiveGroup();
            if (n != null)
            {
                MyEditingGroup = n;
            }
        }
        public void EditingSetMyCurrentLayer()
        {
            MyEditingGroup = MyCurrentLayer;
        }

        //編集グループ内に属しているかの判定
        public bool IsInEditingGroup(TThumb1? thumb)
        {
            if (thumb == null) return false;
            if (thumb?.MyParentGroup == MyEditingGroup) return true;
            else { IsInEditingGroup(thumb?.MyParentGroup); }
            return false;
        }
        //同系統判定
        public bool IsSameSystem(TThumb1? t1,TThumb1? t2)
        {
            var tt1 = t1?.GetMyTopThumb();
            var tt2 = t2?.GetMyTopThumb();
            if (tt1 == tt2) return true;
            else return false;
        }


        #region 追加系
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
        #endregion 追加系

        #region 削除系        
        //指定Thumb
        public void RemoveThumb(TThumb1? thumb, bool isAjustoLocate)
        {
            if (thumb == null) return;
            MyEditingGroup?.RemoveThumb2(thumb, isAjustoLocate);
            //再グループ化リストと選択リストからも削除
            thumb.RegroupThumbs?.Remove(thumb);
            MySelectedThumbs.Remove(thumb);
            //Currentを削除した場合はCurrentとPreCurrentをnull
            if (MyCurrentItem == thumb)
            {
                MyCurrentItem = null;
                MyPreviousCurrentItem = null;
            }
        }
        //CurrentThumb
        public void RemoveActiveThumb()
        {
            if (MyActiveMovableThumb == null) return;
            RemoveThumb(MyActiveMovableThumb, true);
        }
        //選択Thumbs
        public void RemoveSelectedThumbs()
        {
            Group1Base? parent = MySelectedThumbs[0].MyParentGroup;
            for (int i = MySelectedThumbs.Count - 1; i >= 0; i--)
            {
                RemoveThumb(MySelectedThumbs[i], false);
            }
            parent?.AjustLocate3();
        }

        #endregion 削除系

        #region グループ化系
        /// <summary>
        /// 選択リスト要素をグループ化
        /// </summary>
        public void MakeGroup()
        {
            MyEditingGroup?.MakeGroupFromChildren3(MySelectedThumbs);
        }
        private static (double x, double y, int minZ, int maxZ) GetXYZForNewGroup(IEnumerable<TThumb1> thumbs)
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

        /// <summary>
        /// ActiveなGroupを解除する
        /// </summary>
        public void Ungroup()
        {
            if (MyActiveMovableThumb is Group4 group)
            {
                Ungroup(group);
                MyActiveMovableThumb = null;
            }
        }
        public void Ungroup(Group4 group)
        {
            group.Ungroup3();            
        }

        
        public void Regroup(TThumb1? thumb)
        {
            if (thumb == null) return;
            thumb.Regroup();
        }
        #endregion グループ化系

        #endregion publicメソッド

    }
}
