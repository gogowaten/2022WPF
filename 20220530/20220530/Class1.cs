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

namespace _20220530
{
    public enum DataType
    {
        None = 0,
        Root,
        Layer,
        Group,
        TextBlock,
        Path,
        Image,

    }
    public enum DataTypeMain
    {
        Item = 1,
        Group
    }
    public abstract class TThumb1 : Thumb, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #region 共通
        private Group1Base? _myParentGroup;
        public Group1Base? MyParentGroup
        {
            get => _myParentGroup;
            set { if (value == _myParentGroup) { return; } _myParentGroup = value; OnPropertyChanged(); }
        }
        private Layer1? _myLayer;
        public Layer1? MyLayer
        {
            get => _myLayer;
            set { if (value == _myLayer) { return; } _myLayer = value; OnPropertyChanged(); }
        }

        //クリックされたThumbが属するグループの中で移動対象となるThumb
        //更新タイミングはクリックされたときにしたけど、
        //ホントはそれ以外にも編集状態Thumbが切り替わったときに全部のThumbに行いたい？
        private TThumb1? _myMovableTargetThumb;
        public TThumb1? MyActiveMovableThumb
        {
            get => _myMovableTargetThumb;
            set { if (value == _myMovableTargetThumb) { return; } _myMovableTargetThumb = value; OnPropertyChanged(); }
        }

        public Data1 MyData { get; set; }
        public List<TThumb1> RegroupThumbs = new();//再グループ用

        private bool _IsMySelected;
        public bool IsMySelected
        {
            get => _IsMySelected;
            set { if (value == _IsMySelected) { return; } _IsMySelected = value; OnPropertyChanged(); }
        }
        //移動対象フラグ、編集状態のGroupThumbの直下のThumb全てが対象になる
        //Parentが編集状態(IsEditing)ならtrue
        private bool _isMyMoveTarget;
        public bool IsMyMoveTarget
        {
            get => _isMyMoveTarget;
            set
            {
                if (_isMyMoveTarget == value) { return; }
                _isMyMoveTarget = value; OnPropertyChanged();
            }
        }

        #endregion
        public TThumb1() { MyData = new Data1(DataType.None); }
        public TThumb1(Data1 data)
        {
            MyData = data;
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            this.SetBinding(Canvas.LeftProperty, new Binding(nameof(MyData.X)));
            this.SetBinding(Canvas.TopProperty, new Binding(nameof(MyData.Y)));
            this.SetBinding(Panel.ZIndexProperty, new Binding(nameof(MyData.Z)));

        }
        #region その他
        public override string ToString()
        {
            string? ss = MyData?.Text;
            if (string.IsNullOrEmpty(ss)) { ss = this.Name; }

            return $"{MyData?.DataType}, {ss}, x{MyData?.X}, y{MyData?.Y}, z{MyData?.Z}";
        }
        #endregion その他

        //テンプレートの枠
        protected FrameworkElementFactory MakeWaku1(DataTypeMain dataType)
        {
            //選択状態枠表示            
            FrameworkElementFactory waku = new(typeof(Rectangle));
            //waku.SetValue(Rectangle.StrokeProperty, Brushes.SkyBlue);
            waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);

            //枠サイズは自身のサイズに合わせる
            Binding b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualWidthProperty);
            waku.SetBinding(Rectangle.WidthProperty, b);
            b = new();
            b.Source = this;
            b.Path = new PropertyPath(ActualHeightProperty);
            waku.SetValue(Rectangle.HeightProperty, b);

            return waku;
        }



        #region ドラッグ移動

        protected void DragEventAdd(TThumb1 thumb)
        {
            thumb.DragDelta += thumb.TThumb_DragDelta;
            thumb.DragCompleted += thumb.TThumb_DragCompleted;
            thumb.IsMyMoveTarget = true;//移動対象にする
        }

        protected void DragEventRemove(TThumb1 thumb)
        {
            thumb.DragCompleted -= thumb.TThumb_DragCompleted;
            thumb.DragDelta -= thumb.TThumb_DragDelta;
            thumb.IsMyMoveTarget = false;//移動対象から外す
        }
        private void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //移動がなかった
            if (e.HorizontalChange == 0.0 && e.VerticalChange == 0.0)
            {
                if (MyLayer == null) { return; }
                var thumb = e.OriginalSource as TThumb1;
                //前回と同じThumbがクリックされた
                if (MyLayer.LastPreviousClickedItem == thumb)
                {
                    //ParentThumbと編集状態Thumbが違う
                    if (MyLayer.NowEditingThumb != thumb?.MyParentGroup)
                    {
                        //アクティブThumbを編集状態Thumbに指定
                        if (thumb?.GetMyActiveMoveThumb() is Group4 activeParent)
                        {
                            MyLayer.SetNowEditingThumb(activeParent, thumb);
                        }
                    }
                }
            }
            else
            {
                this.MyParentGroup?.AjustLocate3();
            }
        }

        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }

        #endregion ドラッグ移動

        #region メソッド
        #region 枠線ブラシ作成
        //        WPF、Rectangleとかに2色の破線(点線)枠表示 - 午後わてんのブログ
        //https://gogowaten.hatenablog.com/entry/2022/05/29/140321

        public static ImageBrush My2ColorDashBrush(int thickness, Color c1, Color c2)
        {
            WriteableBitmap bitmap = MakeCheckPattern(thickness, c1, c2);
            ImageBrush brush = new(bitmap)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                ViewportUnits = BrushMappingMode.Absolute
            };
            return brush;
        }
        /// <summary>
        /// 指定した2色から市松模様のbitmapを作成
        /// </summary>
        /// <param name="cellSize">1以上を指定、1指定なら2x2ピクセル、2なら4x4ピクセルの画像作成</param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private static WriteableBitmap MakeCheckPattern(int cellSize, Color c1, Color c2)
        {
            int width = cellSize * 2;
            int height = cellSize * 2;
            var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            int stride = wb.Format.BitsPerPixel / 8 * width;
            byte[] pixels = new byte[stride * height];
            Color iro;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((y < cellSize & x < cellSize) | (y >= cellSize & x >= cellSize))
                    {
                        iro = c1;
                    }
                    else { iro = c2; }

                    int p = y * stride + x * 4;
                    pixels[p] = iro.B;
                    pixels[p + 1] = iro.G;
                    pixels[p + 2] = iro.R;
                    pixels[p + 3] = iro.A;
                }
            }
            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return wb;
        }
        #endregion 枠線ブラシ作成

        /// <summary>
        /// 編集状態Thumb直下のChildrenから自身が属するGroupを取得、見つからない場合はnull
        /// </summary>
        /// <returns></returns>
        public Group4? GetMyActiveMoveThumb()
        {
            if (MyParentGroup is Group4 group)
            {
                if (group.MyParentGroup?.IsMyEditing == true)
                {
                    return group;
                }
                else
                {
                    return group.GetMyActiveMoveThumb();
                }
            }
            else { return null; }
        }

        //Layer直下のThumb群から自身に関連するThumbを取得            
        public Group1Base? GetMyUnderLayerThumb(TThumb1? thumb)
        {
            if (thumb == null) { return null; }
            if (thumb.MyParentGroup?.MyData.DataType == DataType.Layer)
            {
                return thumb.MyParentGroup;
            }
            else
            {
                return GetMyUnderLayerThumb(thumb.MyParentGroup);
            }
        }
        public void Regroup()
        {
            //選択状態のThumbを基準に再グループ
            var target = (TThumb1?)GetMyActiveMoveThumb() ?? this;
            if (target == null) { return; }
            if (target.IsMySelected == true && target.RegroupThumbs.Count >= 2)
            {
                target.MyParentGroup?.MakeGroupFromChildren2(target.RegroupThumbs);
            }
        }
        public void SetZIndex(int z)
        {
            if (MyParentGroup is null) { return; }
            int count = MyParentGroup.Items.Count;
            if (count == 0 || z < 0 || z >= count) { return; }

            MyParentGroup.MoveThumbIndexWithZIndex(MyData.Z, z);
        }
        #endregion メソッド
    }


    public class Item4 : TThumb1
    {
        public Canvas MyTemplateCanvas;
        public FrameworkElement MyItemElement;
        private bool _IsMyLastClicked;//最後にクリックされたフラグ
        public bool IsMyLastClicked
        {
            get => _IsMyLastClicked;
            set
            {
                if (_IsMyLastClicked == value) { return; }
                _IsMyLastClicked = value;
                OnPropertyChanged();
            }
        }
        public Item4(Data1 data) : base(data)
        {
            DataContext = MyData;
            MyTemplateCanvas = InitializeTemplate();
            //表示する要素をDataから作成
            MyItemElement = MakeElement(data);
            MyTemplateCanvas.Children.Add(MyItemElement);
            //Canvasと自身のサイズを表示要素のサイズにバインドする
            Binding b = new() { Source = MyItemElement, Path = new PropertyPath(ActualWidthProperty) };
            MyTemplateCanvas.SetBinding(WidthProperty, b);
            this.SetBinding(WidthProperty, b);
            b = new() { Source = MyItemElement, Path = new PropertyPath(ActualHeightProperty) };
            MyTemplateCanvas.SetBinding(HeightProperty, b);
            this.SetBinding(HeightProperty, b);



            Loaded += Item4_Loaded;
            PreviewMouseDown += Item4_PreviewMouseDown;
            PreviewMouseUp += Item4_PreviewMouseUp;
        }


        #region イベント

        //表示された直後にサイズが決まるのでParentのサイズを修正する
        private void Item4_Loaded(object sender, RoutedEventArgs e)
        {
            var neko = e.Source;
            var inu = e.OriginalSource;
            var uma = sender;
            MyParentGroup?.AjustLocate3();
        }
        //クリックでボタンが離されたときされたとき
        private void Item4_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //編集状態Thumbの切り替え

        }

        //クリックされたとき
        private void Item4_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyLayer == null) { return; }
            //前回と同じThumbをクリックされたのかチェック
            bool isEqual = false;
            if (MyLayer.LastClickedItem == this) isEqual = true;
            //最後にクリックされたThumbに自身を登録する
            MyLayer.LastClickedItem = this;

            //編集状態直下の自身が属するグループ
            //TThumb1 activeThumb = (TThumb1?)GetMyActiveMoveThumb2() ?? this;
            TThumb1 activeThumb = (TThumb1?)GetMyActiveMoveThumb() ?? this;
            MyActiveMovableThumb = activeThumb;
            //自身が編集状態Thumbの範囲外だった場合
            if (MyLayer.NowEditingThumb != activeThumb.MyParentGroup)
            {
                //編集状態Thumbの切り替え
                //Layer直下のThumb群から自身が属するThumbに切り替える
                Group1Base? nextEdit = GetMyUnderLayerThumb(this) as Group1Base;
                if (nextEdit == null) { MessageBox.Show("次の編集状態Thumbが見つからん"); }
                //MyLayer.NowEditingThumb = nextEdit;
                MyLayer.SetNowEditingThumb(nextEdit, this);
            }
            else
            {
                //編集状態直下の自身が属するグループを選択状態リストに登録する
                //通常クリック(修飾キーなし)のときは入れ替え
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    //違うThumbクリックなら選択リストの入れ替え
                    if (isEqual == false)
                    {
                        MyLayer.SelectThumbReplace(activeThumb);
                    }
                    ////同じThumbがクリックされた
                    //else
                    //{
                    //    //ParentThumbと編集状態Thumbが違う
                    //    if (MyLayer.NowEditingThumb != MyParentGroup)
                    //    {
                    //        //アクティブThumbを編集状態Thumbに指定
                    //        if (activeThumb is Group4 activeParent)
                    //        {
                    //            if (isEqual) { MyLayer.NowEditingThumb = activeParent; }

                    //        }
                    //    }
                    //}

                }
                //ctrlキーが押されていたら複数選択状態にするので、選択リストに追加
                else if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    //すでに選択中だった場合は選択解除
                    if (MyLayer.SelectedThumbs.Contains(activeThumb))
                    {
                        MyLayer.SelectThumbRemove(activeThumb);
                    }
                    else
                    {
                        MyLayer.SelectThumbAdd(activeThumb);
                    }
                }
            }
        }

        #endregion イベント

        //単一要素表示用テンプレートに書き換える
        private Canvas InitializeTemplate()
        {
            //共通枠
            Binding b1 = new(nameof(IsMyLastClicked)) { Source = this };
            Binding b2 = new(nameof(IsMySelected)) { Source = this };
            MultiBinding mb = new();
            mb.Bindings.Add(b1); mb.Bindings.Add(b2);
            mb.Converter = new MyConverterItemWaku1();
            List<Brush> brushes = new()
            {
                My2ColorDashBrush(5,Colors.Red,Colors.White),
                My2ColorDashBrush(5,Colors.DeepSkyBlue,Colors.PaleTurquoise),
                My2ColorDashBrush(5,Colors.LightGray,Colors.White)
            };
            mb.ConverterParameter = brushes;
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Item);
            waku.SetBinding(Rectangle.StrokeProperty, mb);
            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyTemplateCanvas));
            baseCanvas.AppendChild(waku);
            waku.SetValue(Panel.ZIndexProperty, 1);//枠表示前面

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            return (Canvas)template.FindName(nameof(MyTemplateCanvas), this);

        }


        //表示する要素をDataから作成
        private FrameworkElement MakeElement(Data1 data)
        {
            FrameworkElement? element = null;
            switch (data.DataType)
            {
                case DataType.None:
                    break;
                case DataType.Layer:
                    break;
                case DataType.Group:
                    break;
                case DataType.TextBlock:
                    element = new TextBlock() { FontSize = 20 };
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
                    element.SetBinding(TextBlock.BackgroundProperty, new Binding(nameof(MyData.Background)));
                    element.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(MyData.Foreground)));
                    element.SetBinding(TextBlock.PaddingProperty, new Binding(nameof(MyData.Padding)));
                    break;
                case DataType.Path:
                    break;
                case DataType.Image:
                    break;
                default:
                    break;
            }

            return element ?? throw new ArgumentNullException($"{nameof(element)}", $"dataから要素が作れんかった");
        }


    }

    public abstract class Group1Base : TThumb1
    {
        public ItemsControl? MyItemsControl;
        protected ObservableCollection<TThumb1> Children { get; set; } = new();
        public ReadOnlyObservableCollection<TThumb1> Items { get; set; }
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

        #region コンストラクタ、初期化
        protected Group1Base() { Items = new(Children); }
        public Group1Base(Data1 data) : base(data)
        {

            Items = new(Children);
            MyItemsControl = SetGroupThumbTemplate();

            //Binding
            MyItemsControl.DataContext = this;//子要素コレクションは自身をソース
            this.DataContext = MyData;//自身はデータをソースにする                                          

            //子要素の作成、追加
            foreach (var item in data.ChildrenData)
            {
                Children.Add(new Item4(item));
            }

            Children.CollectionChanged += Children_CollectionChanged;
        }


        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?[0] is TThumb1 addItem)
                {
                    //Dataの追加
                    if (addItem.MyData.Z == Children.Count)
                    {
                        MyData.ChildrenData.Add(addItem.MyData);
                    }
                    else
                    {
                        MyData.ChildrenData.Insert(addItem.MyData.Z, addItem.MyData);
                    }
                    //Parentの指定
                    addItem.MyParentGroup = this;
                    //Layerの指定
                    addItem.MyLayer = this.MyLayer;
                    //Parent(自身)が編集状態なら追加アイテムを
                    if (this.IsMyEditing)
                    {
                        DragEventAdd(addItem);//ドラッグ移動可能にする
                    }
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
                My2ColorDashBrush(3,Color.FromArgb(255,255,0,200),Colors.White),
                My2ColorDashBrush(3,Color.FromArgb(255,0,255,255),Colors.LightGray),
                My2ColorDashBrush(3,Color.FromArgb(255,200,200,200),Colors.White),
            };
            mb.ConverterParameter = brushes;
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Group);
            waku.SetBinding(Rectangle.StrokeProperty, mb);
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
            if (MyParentGroup == null) { return; }
            //上にあるThumbZの底上げ
            for (int i = MyData.Z + 1; i < MyParentGroup.Children.Count; i++)
            {
                //削除対象子要素数-1ぶん底上げする
                MyParentGroup.Children[i].MyData.Z += Children.Count - 1;
            }
            //子要素に対してx,y,zの調整
            foreach (var item in Children)
            {
                item.MyData.X += MyData.X;
                item.MyData.Y += MyData.Y;
                item.MyData.Z += MyData.Z;//Zを削除対象のZぶん底上げする
            }

            //追加してから削除、しないと削除時点で要素数が2未満になった場合にそれも解除対象になってしまうから
            //解除後は子要素群を選択状態にするので今のはクリアする
            MyLayer?.SelectThumbsCleal();
            //子要素の処理
            foreach (var item in Children)
            {
                MyLayer?.SelectThumbAdd(item);//選択状態にする、必要ない
                MyParentGroup.InsertThumb(item);//Parentの子要素に挿入

                //再グループ用の情報を付加
                item.RegroupThumbs = Children.ToList();
            }
            MyParentGroup.RemoveThumb(this);//削除

            //兄弟の再グループリストから自身を削除
            foreach (var brothers in MyParentGroup.Children)
            {
                brothers.RegroupThumbs.Remove(this);
                if (brothers.RegroupThumbs.Count <= 1)
                {
                    brothers.RegroupThumbs.Clear();
                }
            }
        }

        /// <summary>
        /// 指定要素群からグループ作成して自身に挿入する
        /// </summary>
        /// <param name="thumbs">要素群は自身のChildrenから指定</param>
        /// <returns></returns>
        public bool MakeGroupFromChildren2(List<TThumb1>? thumbs)
        {
            if (thumbs == null || thumbs.Count < 2) { return false; }
            //指定要素群のParentが自身ではなかった場合は作成失敗
            foreach (var item in thumbs) { if (item.MyParentGroup != this) { return false; } }

            //新グループのX、Y、Zを要素群から計算取得
            var (x, y, minZ, maxZ) = GetXYZForNewGroup(thumbs);
            ////新グループのZ = 要素群の最上位Z - (要素数 - 1)
            //maxZ -= thumbs.Count - 1;
            //新グループのData作成
            Data1 data = new(DataType.Group);
            data.X = x; data.Y = y; data.Z = maxZ;

            //新グループ作成            
            Group4 group = new(data);//作成
            group.MyLayer = this.MyLayer;
            //Z順にソートした要素群作成
            var sortedThumbs = thumbs.OrderBy(x => x.MyData.Z).ToList();
            //新グループに要素群追加
            foreach (var item in sortedThumbs)
            {
                group.AddThumb(item);
                //再グループ用の情報をクリア
                item.RegroupThumbs.Clear();
            }
            //新グループを作成挿入してから、元の要素群削除、順番大切
            //Childrenに新グループを挿入
            InsertThumb(group);
            //Childrenから要素群削除
            foreach (var item in sortedThumbs) { RemoveThumb(item); }

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
        public virtual void RemoveThumb(TThumb1 thumb)
        {
            if (Children.Contains(thumb))
            {
                int z = thumb.MyData.Z;
                //LastClickedをクリア
                if (MyLayer != null) MyLayer.LastClickedItem = null;
                //選択リストから削除
                MyLayer?.SelectThumbRemove(thumb);
                //コレクションから削除
                Children.Remove(thumb);
                //2未満ならグループ解除
                if (Children.Count < 2)
                {
                    Ungroup2();
                    //Ungroup();
                }
                else
                {
                    for (int i = z; i < Children.Count; i++)
                    {
                        Children[i].MyData.Z = i;
                    }
                }
            }
            else
            {
                throw new AggregateException("グループ内に対象Thumbが見つからない");
            }
        }
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
        public Group4() : this(new Data1(DataType.Group))
        {
        }
        public Group4(Data1 data) : base(data) { }

    }
    public class Layer1 : Group1Base
    {
        #region 通知プロパティ
        //今編集状態(子要素の移動可能)のGroup
        private Group1Base? _NowEditingThumb;
        public Group1Base? NowEditingThumb
        {
            get { return _NowEditingThumb; }
            private set
            {
                if (_NowEditingThumb == value) { return; }
                _NowEditingThumb = value; OnPropertyChanged();
            }
        }
        public void SetNowEditingThumb(Group1Base? newEditing, TThumb1 lastClicked)
        {
            if (NowEditingThumb == newEditing) { return; }
            //選択リストのリセット
            if (SelectedThumbs.Count > 0)
            {
                foreach (var item in _SelectedThumbs)
                {
                    item.IsMySelected = false;
                }
                _SelectedThumbs.Clear();
            }
            //ドラッグ移動イベントの付け外し
            if (NowEditingThumb != null)
            {
                NowEditingThumb.IsMyEditing = false;
                NowEditingThumb.RemoveDragEventForChildren();
            }

            NowEditingThumb = newEditing;//切り替え

            if (NowEditingThumb != null)
            {
                NowEditingThumb.IsMyEditing = true;
                NowEditingThumb.AddDragEventForChildren();
            }

            TThumb1? active = lastClicked.GetMyActiveMoveThumb();
            if (active == null) { active = lastClicked; }
            SelectThumbAdd(active);
        }

        //最後の一個前に選択されたItem
        public Item4? LastPreviousClickedItem;
        //最後にクリックされたItem
        private Item4? _lastClickedItem;
        public Item4? LastClickedItem
        {
            get => _lastClickedItem;
            set
            {
                //古い方を記録
                LastPreviousClickedItem = _lastClickedItem;
                //格納しているThumbと同じなら必要なし、終了
                if (_lastClickedItem == value) { return; }

                //古い方のIsLastClickedをfalseに変更してから
                if (_lastClickedItem != null)
                {
                    _lastClickedItem.IsMyLastClicked = false;
                }
                //新しい方のIsLastClickedをtrue
                if (value != null)
                {
                    value.IsMyLastClicked = true;
                }

                //新旧入れ替え
                _lastClickedItem = value;
                OnPropertyChanged();
            }
        }

        //選択状態のThumb群
        private readonly ObservableCollection<TThumb1> _SelectedThumbs = new();
        public ReadOnlyObservableCollection<TThumb1> SelectedThumbs;
        #endregion 通知プロパティ

        public Layer1() : this(new Data1(DataType.Layer)) { }
        public Layer1(Data1 data) : base(data)
        {
            //Layerなので編集状態にする
            NowEditingThumb = this;
            IsMyEditing = true;

            _SelectedThumbs.CollectionChanged += SelectedThumbs_CollectionChanged;
            PreviewMouseLeftButtonDown += Layer4_PreviewMouseLeftButtonDown;
            SelectedThumbs = new(_SelectedThumbs);
        }


        #region メソッド

        #region 選択状態Thumb群の操作
        private void SelectedThumbs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            TThumb1? nn = e.NewItems?[0] as TThumb1;
            TThumb1? oo = e.OldItems?[0] as TThumb1;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (nn == null) { return; }
                    nn.IsMySelected = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (oo == null) { return; }
                    oo.IsMySelected = false;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (nn == null || oo == null) { return; }
                    oo.IsMySelected = false;
                    nn.IsMySelected = true;
                    var item0 = e.NewItems?[0];
                    var item1 = e.OldItems?[0];
                    break;
                case NotifyCollectionChangedAction.Move:
                    var item2 = e.NewItems?[0];
                    var item3 = e.OldItems?[0];
                    break;
                case NotifyCollectionChangedAction.Reset:

                    break;
                default:
                    break;
            }
        }

        //選択状態Thumbの追加
        public void SelectThumbAdd(TThumb1? thumb)
        {
            //同じThumbがないかチェックしてから追加
            if (thumb == null) { return; }
            if (SelectedThumbs.Contains(thumb)) { return; }
            _SelectedThumbs.Add(thumb);
        }
        //選択状態Thumbの削除
        public void SelectThumbRemove(TThumb1 thumb)
        {
            if (!SelectedThumbs.Contains(thumb)) { return; }
            _SelectedThumbs.Remove(thumb);
        }
        //選択状態Thumbの入れ替え
        public void SelectThumbReplace(TThumb1? thumb)
        {
            if (thumb == null) { return; }
            if (SelectedThumbs.Count == 1)
            {
                if (SelectedThumbs[0] != thumb)
                {
                    _SelectedThumbs.Remove(SelectedThumbs[0]);
                    _SelectedThumbs.Add(thumb);
                }
            }
            else if (SelectedThumbs.Count > 1)
            {
                foreach (var item in SelectedThumbs)
                {
                    item.IsMySelected = false;
                }
                _SelectedThumbs.Clear();
                _SelectedThumbs.Add(thumb);
            }
            else
            {
                _SelectedThumbs.Add(thumb);
            }
        }
        //選択状態Thumbのクリア
        public void SelectThumbsCleal()
        {
            if (_SelectedThumbs.Count == 0) { return; }
            foreach (var item in _SelectedThumbs)
            {
                item.IsMySelected = false;
            }
            _SelectedThumbs.Clear();
        }
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

        private void Layer4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var neko = sender;
            var inu = e.OriginalSource;
            var uma = e.Source;
            //SelectedThumbs.Add(LastClickedItem);
        }


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

    public class TThumb1Manage
    {

    }

    #region Data4

    [DataContract]
    [KnownType(typeof(RectangleGeometry)),
     KnownType(typeof(MatrixTransform))]
    public class Data1 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double _x;
        private double _y;
        private int _z;
        private string _text = "";
        private Brush? _background;
        private Brush _foreground = Brushes.Black;
        private double _padding = 0;
        [DataMember]
        public ObservableCollection<Data1> ChildrenData { get; set; } = new();
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public DataTypeMain DataTypeMain { get; set; }
        [DataMember]
        public double X { get => _x; set { if (_x == value) { return; } _x = value; OnPropertyChanged(); } }
        [DataMember]
        public double Y { get => _y; set { if (_y == value) { return; } _y = value; OnPropertyChanged(); } }
        [DataMember]
        public int Z { get => _z; set { if (_z == value) { return; } _z = value; OnPropertyChanged(); } }

        [DataMember]
        public string Text { get => _text; set { if (_text == value) { return; } _text = value; OnPropertyChanged(); } }
        [DataMember]
        public Brush? Background
        {
            get => _background; set
            {
                if (_background == value) { return; }
                _background = value; OnPropertyChanged();
            }
        }
        [DataMember]
        public Brush Foreground
        {
            get => _foreground;
            set
            {
                if (_foreground == value) { return; }
                _foreground = value; OnPropertyChanged();
            }
        }
        [DataMember]
        public double Padding
        {
            get => _padding;
            set { if (value == _padding) { return; } _padding = value; OnPropertyChanged(); }
        }
        [DataMember]
        public Geometry? Geometry { get; set; }


        public Data1(DataType type)
        {
            DataType = type;
            if (type == DataType.Group || type == DataType.Layer)
            {
                DataTypeMain = DataTypeMain.Group;
            }
            else DataTypeMain = DataTypeMain.Item;
        }
        public override string ToString()
        {
            return $"{X},{Y},{Z},{DataType},{Text}";
        }
    }
    #endregion Data4


    public class MyConverterItemWaku1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //List<Brush> brushes = (List<Brush>)parameter;
            //bool b1 = (bool)values[0];
            //bool b2 = (bool)values[1];
            //if (b1) { return brushes[0]; }
            //else if (b2) { return brushes[1]; }
            //else { return Brushes.Transparent; }

            List<Brush> brushes = (List<Brush>)parameter;
            bool b2 = (bool)values[1];
            if (b2) { return brushes[1]; }
            else { return Brushes.Transparent; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class MyConverterGroupWaku4 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<Brush> brushes = (List<Brush>)parameter;
            bool b1 = (bool)values[0];
            bool b2 = (bool)values[1];
            if (b1) { return brushes[0]; }
            else if (b2) { return brushes[1]; }
            //else { return Brushes.Transparent; }
            else { return brushes[2]; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
