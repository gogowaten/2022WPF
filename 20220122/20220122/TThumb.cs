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
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;


namespace _20220122
{
    public enum TType
    {
        Layer = 1,
        Group,
        Image,
        TextBox,
        TextBlock,
        PathGeometry,
    }

    public abstract class AbstractBaseThumb : Thumb
    {
        protected Canvas RootCanvas;
        private readonly string ROOT_CANVAS_NAME = "root";
        protected Canvas GroupCanvas = new();
        public Canvas SurfaceCanvas = new();//neko

        protected AbstractBaseThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), ROOT_CANVAS_NAME);
            this.Template = template;
            ApplyTemplate();
            RootCanvas = (Canvas)this.Template.FindName(ROOT_CANVAS_NAME, this);
            RootCanvas.Children.Add(GroupCanvas);//グループ内要素用
            RootCanvas.Children.Add(SurfaceCanvas);//枠表示用、最前面

            //RootCanvas
            //┣GroupCanvas
            //┗SurfaceCanvas
        }

    }
    public abstract class BaseThumb : AbstractBaseThumb, System.ComponentModel.INotifyPropertyChanged
    {
        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Notify

        #region 保存するプロパティ、依存プロパティ
        public TType Type;//外から変更できないようにしたけどわからん
        private double x;
        private double y;
        private int z;
        private double width;
        private double height;
        private Visibility visibleFrame = Visibility.Collapsed;
        private Visibility visibleFrame2 = Visibility.Visible;
        private string name = "";
        private Rect bounds;
        public Rect Bounds
        {
            get => bounds;
            set
            {
                if (value != bounds)
                {
                    bounds = value; OnPropertyChanged();
                    X = value.X;
                    Y = value.Y;
                    TTWidth = value.Width;
                    TTHeight = value.Height;
                }
            }
        }

        public double X { get => x; set { if (value != x) { x = value; OnPropertyChanged(); Bounds = new Rect(value, y, width, height); } } }
        public double Y { get => y; set { if (value != y) { y = value; OnPropertyChanged(); Bounds = new Rect(x, value, width, height); } } }
        public int Z { get => z; set { if (value != z) { z = value; OnPropertyChanged(); } } }
        public double TTWidth { get => width; set { if (value != width) { width = value; OnPropertyChanged(); Bounds = new Rect(x, y, value, height); } } }
        //public double Width { get => width; set { if (value != width) { width = value; OnpropertyChanged(); } } }
        public double TTHeight { get => height; set { if (value != height) { height = value; OnPropertyChanged(); Bounds = new Rect(x, y, width, value); } } }
        public Visibility VisibleFrame { get => visibleFrame; set { if (value != visibleFrame) { visibleFrame = value; OnPropertyChanged(); } } }
        public Visibility VisibleFrame2 { get => visibleFrame2; set { if (value != visibleFrame2) { visibleFrame2 = value; OnPropertyChanged(); } } }
        public string TTName { get => name; set { if (value != name) { name = value; OnPropertyChanged(); } } }

        #endregion プロパティ


        #region フィールド
        public BaseThumb ParentThumb { get; set; }//所属するGroupやLayer
        public BaseThumb TopParentThumb { get; set; }//一番上のGroupThumb
        public Layer ParentLayer { get; set; }//所属するLayer


        #endregion フィールド

        public BaseThumb()
        {
            DataContext = this;
            MySetTwoWayModeBinding(Thumb.WidthProperty, nameof(TTWidth));
            MySetTwoWayModeBinding(Thumb.HeightProperty, nameof(TTHeight));
            MySetTwoWayModeBinding(Thumb.NameProperty, nameof(TTName));
            MySetTwoWayModeBinding(Canvas.LeftProperty, nameof(X));
            MySetTwoWayModeBinding(Canvas.TopProperty, nameof(Y));

            this.PreviewMouseLeftButtonUp += TThumb_PreviewMouseLeftButtonUp;
            this.PreviewMouseLeftButtonDown += TThumb_PreviewMouseLeftButtonDown;
            this.DragDelta += TThumbDragDelta;
            this.GotFocus += TThumb_GotFocus;
            this.LostFocus += TThumb_LostFocus;
            this.PreviewMouseDown += TThumbPreviewMouseDown;
            //枠表示用Rectangle
            Rectangle rectangle = new();
            rectangle.SetBinding(Rectangle.WidthProperty, nameof(TTWidth));
            rectangle.SetBinding(Rectangle.HeightProperty, nameof(TTHeight));
            rectangle.Stroke = Brushes.Cyan;
            rectangle.StrokeThickness = 4;
            SurfaceCanvas.Children.Add(rectangle);
            rectangle.SetBinding(VisibilityProperty, nameof(VisibleFrame));

            //枠表示用Rectangle2
            Rectangle rectangle2 = new();
            rectangle2.SetBinding(Rectangle.WidthProperty, nameof(TTWidth));
            rectangle2.SetBinding(Rectangle.HeightProperty, nameof(TTHeight));
            rectangle2.Stroke = Brushes.MediumBlue;
            rectangle2.StrokeThickness = 1;
            SurfaceCanvas.Children.Add(rectangle2);
            rectangle2.SetBinding(VisibilityProperty, nameof(VisibleFrame2));


            void MySetTwoWayModeBinding(DependencyProperty property, string path)
            {
                Binding b = new(path);
                b.Mode = BindingMode.TwoWay;
                this.SetBinding(property, b);
            }

        }

        #region イベント


        protected void TThumbPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        protected void TThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsFocused)
            {
                var neko = 0;
            }
        }

        protected void TThumb_LostFocus(object sender, RoutedEventArgs e)
        {
            BaseThumb thumb = sender as BaseThumb;
            thumb.VisibleFrame = Visibility.Collapsed;
        }

        protected void TThumb_GotFocus(object sender, RoutedEventArgs e)
        {
            TThumb thumb = sender as TThumb;
            if (thumb.ParentThumb.visibleFrame == Visibility.Visible)
            {
                var neko = 0;
            }

            //枠表示
            if (e.OriginalSource == e.Source)
            {
                BaseThumb top = GetTopGroupThumb(thumb);
                top.VisibleFrame = Visibility.Visible;


            }

            //thumb.VisibleFrame = Visibility.Visible;
        }

        protected void TThumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is BaseThumb target && sender is Layer thumb)
            {
                thumb.LastClickedThumb = target;
            }
        }

        ///// <summary>
        ///// ParentThumbのbounds取得
        ///// </summary>
        ///// <param name="baseThumb"></param>
        ///// <returns></returns>
        //        protected Rect GetGroupRect(BaseThumb baseThumb)
        //        {
        //            Rect r;
        //            if (baseThumb.ParentThumb is TTGroup group)
        //            {
        //                r = baseThumb.bounds;
        //                foreach (var item in group.ChildrenList)
        //                {
        //                    r.Union(item.bounds);
        //                }
        //            }
        //            else { r = baseThumb.bounds; }
        //            return r;
        //        }
        //子要素の移動終了時に自身のサイズ変更、一番上までサイズ変更？
        public void TThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            BaseThumb currentT = (BaseThumb)sender;
            if (currentT.ParentThumb == null) { return; }
            if (currentT.ParentThumb is TTGroup group)
            {
                //兄弟要素が収まるRect取得
                Rect unionRect = currentT.Bounds;
                foreach (var item in group.ChildrenList)
                {
                    unionRect.Union(item.Bounds);
                }

                //Parentの座標変更
                //Layerだった場合
                if (group.Type == TType.Layer)
                {
                    double xOffset = 0;
                    double yOffset = 0;
                    //兄弟要素の座標変更
                    xOffset = -unionRect.X;
                    group.ChildrenList.ForEach(i => i.X += xOffset);
                    yOffset = -unionRect.Y;
                    group.ChildrenList.ForEach(i => i.Y += yOffset);
                    //Layerはサイズだけ変更
                    group.Width = unionRect.Width + unionRect.X + xOffset;
                    group.Height = unionRect.Height + unionRect.Y + yOffset;

                }
                else
                {
                    //Parentのサイズと位置変更
                    group.X += unionRect.X;
                    group.Y += unionRect.Y;
                    group.Width = unionRect.Width;
                    group.Height = unionRect.Height;

                    //兄弟要素の座標変更
                    foreach (BaseThumb item in group.ChildrenList)
                    {
                        item.X -= unionRect.X;
                        item.Y -= unionRect.Y;
                    }
                }

            }

            //移動していないクリックの場合、編集状態にする
            //同じトップグループ内の要素全部を編集(移動可能)状態にする
            if (e.HorizontalChange + e.VerticalChange == 0)
            {

            }

        }


        //子要素の移動開始時、移動しない要素全体のRect取得しておく
        //protected void TThumb_DragStarted(object sender, DragStartedEventArgs e)
        //{

        //}

        public void TThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            X += e.HorizontalChange;
            Y += e.VerticalChange;
        }

        protected void ElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is not FrameworkElement element) { return; }
            TTWidth = element.ActualWidth;
            TTHeight = element.ActualHeight;
        }

        #endregion イベント


        /// <summary>
        /// 対象Thumbが所属する一番上のグループthumbを返す(Layer以外、なければそのまま返す)
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns></returns>
        protected BaseThumb GetTopGroupThumb(BaseThumb thumb)
        {
            BaseThumb temp = thumb.ParentThumb;
            if (temp != null && temp.Type != TType.Layer)
            {
                return GetTopGroupThumb(temp);
            }
            else
            {
                return thumb;
            }
        }

        public override string ToString()
        {
            //return base.ToString();
            return $"{Name} x,y,z({X},{Y},{Z}) 幅,高({width:0.0}, {height:0.0})";
        }
    }


    public abstract class TThumb : BaseThumb
    {
        public TThumb()
        {
            this.Focusable = true;
        }

        //protected TThumb GetVisibleFrameThumb(TThumb thumb)
        //{
        //    TThumb parent = thumb.ParentGroupThumb;
        //    if (parent.visibleFrame != Visibility.Visible && parent.Type == TType.Group)
        //    {
        //        return GetVisibleFrameThumb(parent);
        //    }
        //    else
        //    {
        //        return thumb;
        //    }
        //}



    }



    public class Layer : TTGroup
    {
        public BaseThumb LastClickedThumb { get; set; }//最後にクリックされたChildThumb
        public Layer()
        {
            Type = TType.Layer;
            Focusable = false;
            DragDelta -= TThumbDragDelta;
            GotFocus -= TThumb_GotFocus;
            LostFocus -= TThumb_LostFocus;
            //PreviewMouseLeftButtonUp -= TThumb_PreviewMouseLeftButtonUp;
            PreviewMouseLeftButtonDown -= TThumb_PreviewMouseLeftButtonDown;
        }


    }
    public class TTGroup : TThumb
    {
        public List<BaseThumb> ChildrenList { get; set; } = new();


        private bool isMovableChildrenMode;

        public bool IsMovableChildrenMode
        {
            get => isMovableChildrenMode;
            set
            {
                if (value != isMovableChildrenMode)
                {
                    isMovableChildrenMode = value;
                    if (value)
                    {
                        this.DragDelta -= TThumbDragDelta;
                        this.DragCompleted -= TThumbDragCompleted;
                        //this.ContextMenu = null;
                        //AddDragEvent(this);
                    }
                    else
                    {
                        this.DragDelta += TThumbDragDelta;
                        this.DragCompleted += TThumbDragCompleted;
                        //this.AddContextMenu(this);
                        //RemoveDragEvent(this);
                    }
                }
            }
        }

        //private void RemoveDragEvent(TTGroup group)
        //{
        //    foreach (TThumb item in group.ChildrenList)
        //    {
        //        item.DragDelta -= item.TThumbDragDelta;
        //        item.DragCompleted -= item.TThumbDragCompleted;
        //        if (item.Type == TType.Group)
        //        {
        //            TTGroup g = item as TTGroup;
        //            g.AddContextMenu(g);
        //        }
        //    }
        //}
        //private void AddDragEvent(TTGroup group)
        //{
        //    foreach (TThumb item in group.ChildrenList)
        //    {
        //        item.DragDelta += item.TThumbDragDelta;
        //        item.DragCompleted += item.TThumbDragCompleted;
        //        if (item.Type == TType.Group)
        //        {
        //            TTGroup g = item as TTGroup;
        //            g.AddContextMenu(g);
        //            //AddDragEvent(item as TTGroup);
        //        }
        //    }
        //}


        public TTGroup()
        {
            Type = TType.Group;

            //フォーカスはバブルで上がってくるので停止できないので、ここで無効にしても意味が薄い
            GotFocus -= TThumb_GotFocus;
            //this.Focusable = false;
            PreviewMouseLeftButtonUp -= TThumb_PreviewMouseLeftButtonUp;
        }




        public void AddThumb(BaseThumb thumb)
        {
            thumb.Z = ChildrenList.Count;
            ChildrenList.Add(thumb);
            GroupCanvas.Children.Add(thumb);
            thumb.ParentThumb = this;
            Panel.SetZIndex(thumb, thumb.Z);
            //レイアウト更新してからサイズ変更(調整)
            thumb.UpdateLayout();
            Rect r = this.Bounds;
            r.Union(thumb.Bounds);
            this.Bounds = r;

            thumb.DragCompleted += thumb.TThumbDragCompleted;

            //Layerに登録と、Layerを登録
            Layer layer1 = this as Layer;
            if(layer1 != null)
            {
                thumb.ParentLayer = layer1;
                layer1.LastClickedThumb = thumb;
            }
            
            //Layer layer = this as Layer ?? this.ParentLayer ?? thumb.ParentLayer;
            //thumb.ParentLayer = layer;
            //layer.LastClickedThumb = thumb;

            //if (this is Layer layer)
            //{
            //    thumb.ParentLayer = layer;
            //    layer.LastClickedThumb = thumb;
            //}
            //else
            //{
            //    thumb.ParentLayer = this.ParentLayer;
            //    this.ParentLayer.LastClickedThumb = thumb;
            //}


            //if (thumb.ContextMenu == null && thumb.ParentGroupThumb.Type == TType.Group)
            //{
            //    //右クリックメニュー            
            //    ContextMenu contextMenu = new();
            //    thumb.ContextMenu = contextMenu;

            //    MenuItem menuItem = new();
            //    contextMenu.Items.Add(menuItem);
            //    menuItem.Header = "編集開始";
            //    menuItem.Click += MenuItem_Click;

            //    MenuItem editEnd = new();
            //    contextMenu.Items.Add(editEnd);
            //    editEnd.Header = "編集終了";
            //    editEnd.Click += EditEnd_Click;

            //}
        }

        private void EditEnd_Click(object sender, RoutedEventArgs e)
        {
            this.IsMovableChildrenMode = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.IsMovableChildrenMode = true;
        }

        //private BaseThumb GetLastClickedThumb(BaseThumb thumb)
        //{
        //    //最後にクリックしたThumbを記録
        //    BaseThumb thumb = sender as BaseThumb;
        //    if (thumb.ParentThumb != null)
        //    {
        //        thumb.ParentThumb.LastClickedThumb = thumb;
        //    }

        //    return
        //}

        public TTGroup MakeGroupFromChildren(List<BaseThumb> thumbs, string name = "Group")
        {
            if (thumbs.Count < 2) { throw new ArgumentException("Thumb数が2未満"); }
            //グループThumb作成
            TTGroup group = new();

            group.ParentThumb = this;
            group.Name = name;
            group.DragCompleted += group.TThumbDragCompleted;

            //ZIndex
            int maxZ = thumbs.Max(a => a.Z);
            int minZ = thumbs.Min(a => a.Z);
            //追加
            this.ChildrenList.Insert(maxZ, group);
            this.GroupCanvas.Children.Add(group);

            //要素群をZIndex順に並べ替えたリスト作成
            List<BaseThumb> items = thumbs.OrderBy(i => i.Z).ToList();
            //要素群を元の親から外してから、グループThumbの子要素にする
            for (int i = 0; i < items.Count; i++)
            {
                BaseThumb tt = items[i];
                //Layerから外す
                this.ChildrenList.Remove(tt);
                this.GroupCanvas.Children.Remove(tt);

                //グループThumbに追加
                tt.ParentThumb = group;
                tt.Z = i;
                group.AddThumb(tt);

                //ドラッグイベント
                tt.DragDelta -= tt.TThumbDragDelta;
                tt.DragCompleted -= tt.TThumbDragCompleted;

                //test
                tt.ContextMenu = null;

                //test
                tt.PreviewMouseLeftButtonUp += Tt_PreviewMouseLeftButtonUp;
            }

            //グループThumb
            //ZIndexを指定する、前に詰める
            for (int i = minZ; i < this.ChildrenList.Count; i++)
            {
                this.ChildrenList[i].Z = i;
                Panel.SetZIndex(this.ChildrenList[i], i);
            }
            //サイズと座標
            Rect rect = items[0].Bounds;
            for (int i = 1; i < items.Count; i++)
            {
                rect.Union(items[i].Bounds);
            }
            group.Bounds = rect;

            //要素群の座標調整
            foreach (var item in items)
            {
                item.X -= rect.X;
                item.Y -= rect.Y;
            }

            //AddContextMenu(group);

            return group;
        }

        //private void AddContextMenu(TTGroup thumb)
        //{
        //    //右クリックメニュー            
        //    ContextMenu contextMenu = new();
        //    thumb.ContextMenu = contextMenu;

        //    MenuItem menuItem = new();
        //    contextMenu.Items.Add(menuItem);
        //    menuItem.Header = "編集開始";
        //    menuItem.Click += thumb.MenuItem_Click;

        //    MenuItem editEnd = new();
        //    contextMenu.Items.Add(editEnd);
        //    editEnd.Header = "編集終了";
        //    editEnd.Click += thumb.EditEnd_Click;

        //}

        private void Tt_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //TThumb tt = sender as TThumb;
            //TTGroup parent = tt.ParentGroupThumb;
            //if (parent != null)
            //{
            //    parent.DragDelta -= parent.TThumbDragDelta;
            //    parent.DragCompleted -= parent.TThumbDragCompleted;
            //    foreach (var item in parent.ChildrenList)
            //    {
            //        item.DragDelta += item.TThumbDragDelta;
            //        item.DragCompleted += item.TThumbDragCompleted;
            //    }
            //}
        }
    }

    public class TTTextBlock : TThumb
    {
        private string text;
        private double tTFontSize = 30;

        public string Text { get => text; set { if (value != text) { text = value; OnPropertyChanged(); } } }
        public double TTFontSize { get => tTFontSize; set { if (value != tTFontSize) { tTFontSize = value; OnPropertyChanged(); } } }

        public TTTextBlock()
        {
            Type = TType.TextBlock;
            TextBlock tb = new() { Text = text, FontSize = tTFontSize };
            tb.SizeChanged += ElementSizeChanged;
            tb.SetBinding(TextBlock.TextProperty, nameof(Text));
            this.RootCanvas.Children.Add(tb);
        }
        public TTTextBlock(string str) : this()
        {
            Text = str; X = 10; Y = 10; TTName = "youso1";
        }
        public TTTextBlock(string str, double x = 0, double y = 0, string name = "ななし") : this()
        {
            Text = str; X = x; Y = y; TTName = name;
        }
    }



}
