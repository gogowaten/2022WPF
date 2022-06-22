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

        #region フィールド
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
        public MainItemsControl MyMainItemsControl;

        ////クリックされたThumbが属するグループの中で移動対象となるThumb
        ////更新タイミングはクリックされたときにしたけど、
        ////ホントはそれ以外にも編集状態Thumbが切り替わったときに全部のThumbに行いたい？
        //private TThumb1? _myMovableTargetThumb;
        //public TThumb1? MyActiveMovableThumb
        //{
        //    get => _myMovableTargetThumb;
        //    set { if (value == _myMovableTargetThumb) { return; } _myMovableTargetThumb = value; OnPropertyChanged(); }
        //}

        public Data1 MyData { get; set; }
        public List<TThumb1>? RegroupThumbs;//再グループ用


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

        public TThumb1(MainItemsControl main)
        {
            MyMainItemsControl = main;
            MyData = new Data1(DataType.None);
        }
        public TThumb1(MainItemsControl main, Data1 data) : this(main)
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

            return $"{MyData?.DataType}, x{MyData?.X}, y{MyData?.Y}, z{MyData?.Z}";
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
        //ドラッグ移動終了時
        private void TThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //移動距離が0＆クリック対象が前回と同じだった場合は、編集状態グループをより近いグループに切り替える
            //移動がなかった
            if (e.HorizontalChange == 0.0 && e.VerticalChange == 0.0)
            {
                //if (MyLayer == null) { return; }
                //var thumb = e.OriginalSource as TThumb1;
                ////前回と同じThumbがクリックされた
                //if (MyLayer.LastPreviousClickedItem == thumb)
                //{
                //    //ParentThumbと編集状態Thumbが違う
                //    //if (MyLayer.NowEditingThumb != thumb?.MyParentGroup)
                //    if (MyMainItemsControl.MyEditingGroup != thumb?.MyParentGroup)
                //    {
                //        //アクティブThumbを編集状態Thumbに指定+アクティブThumbを更新
                //        if (MyMainItemsControl.MyActiveMovableThumb is Group4 ag)
                //        {
                //            MyMainItemsControl.MyEditingGroup = ag;
                //            MyMainItemsControl.MyActiveMovableThumb = thumb?.GetMyActiveMoveThumb();
                //        }
                //        //if (thumb?.GetMyActiveMoveThumb() is Group4 activeParent)
                //        //{
                //        //    MyLayer.SetNowEditingThumb(activeParent, thumb);
                //        //}
                //    }
                //}
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
        public Group4? GetMyActiveGroup()
        {
            if (MyParentGroup is Group4 group)
            {
                //if (group.MyParentGroup?.IsMyEditing == true)
                if (group.MyParentGroup == MyMainItemsControl.MyEditingGroup)
                {
                    return group;
                }
                else
                {
                    return group.GetMyActiveGroup();
                }

            }
            else { return null; }
        }
        /// <summary>
        /// 編集状態Thumb直下のChildrenから自身が属するGroupを取得して、見つからない場合は自身を返す
        /// </summary>
        /// <returns></returns>

        public TThumb1 GetMyActiveThumb()
        {
            Group4? item = GetMyActiveGroup();
            if (item == null)
                return this;
            else
                return item;
        }
        //Layer直下にある関連グループを取得
        public Group1Base? GetMyTopParentGroup(TThumb1? thumb)
        {
            if (thumb == null) { return null; }
            if (thumb.MyParentGroup?.MyData.DataType == DataType.Layer)
            {
                return thumb.MyParentGroup;
            }
            else
            {
                return GetMyTopParentGroup(thumb.MyParentGroup);
            }
        }
        public void Regroup()
        {
            //選択状態のThumbを基準に再グループ
            var target = (TThumb1?)GetMyActiveGroup() ?? this;
            if (target == null) { return; }

            if (target.IsMySelected == true && target.RegroupThumbs.Count >= 2)
            {
                target.MyParentGroup?.MakeGroupFromChildren3(target.RegroupThumbs);
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




    //public class ReGroupData
    //{
    //    //public static List<TTextBox> TTextBoxList = new();
    //    public static List<TThumb1> RegroupThumbs { get; }//再グループ用
    //    public static int Reg = 1;
    //    public ReGroupData(List<TThumb1> thumbs)
    //    {   
    //        foreach (var item in thumbs)
    //        {
    //            RegroupThumbs.Add(item);
    //            item.MyReGroupData = this;
    //        }
    //    }
    //    internal void RemoveItem(TThumb1 thumb)
    //    {
    //        RegroupThumbs.Remove(thumb);
    //    }

    //}
    //public class TTextBox : TextBox
    //{
    //    public ReGroupData? ReGroupData;
    //    public TTextBox(string text)
    //    {
    //        Text = text;            
    //    }
    //    public void RemoveItem()
    //    {
    //        this.ReGroupData?.RemoveItem(this);
    //    }
    //}
}
