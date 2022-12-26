using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace _20221224
{
    public class TThumb : Thumb, INotifyPropertyChanged
    {
        #region 依存プロパティ、通知プロパティ

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private double _myLeft;
        public double MyLeft { get => _myLeft; set => SetProperty(ref _myLeft, value); }


        //依存プロパティじゃなくても、通知プロパティでおｋ
        //public double MyLeft
        //{
        //    get { return (double)GetValue(MyLeftProperty); }
        //    set
        //    {
        //        SetValue(MyLeftProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty MyLeftProperty =
        //    DependencyProperty.Register(nameof(MyLeft), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));

        public double MyTop
        {
            get { return (double)GetValue(MyTopProperty); }
            set { SetValue(MyTopProperty, value); }
        }
        public static readonly DependencyProperty MyTopProperty =
            DependencyProperty.Register(nameof(MyTop), typeof(double), typeof(TThumb), new PropertyMetadata(0.0));


        #endregion 依存プロパティ

        public TTGroup? TTParent { get; set; }
        public TThumb()
        {
            SetBinding(Canvas.LeftProperty, nameof(MyLeft));
            SetBinding(Canvas.TopProperty, nameof(MyTop));
        }

        public override string ToString()
        {
            //return base.ToString();
            return Name;
        }
        //ドラッグ移動終了時に親要素のサイズと位置の更新
        private void TT_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is TThumb tt)
            {
                tt.TTParent?.TTGroupUpdateLayout();
                //this.TTGroupUpdateLayout();
            }
        }
        //マウスドラッグ移動
        private void TT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;
        }
        /// <summary>
        /// ドラッグ移動系イベント登録
        /// </summary>
        /// <param name="addTarget">登録するThumb</param>
        /// <param name="moveTarget">実際に移動させるThumb</param>
        public void AddDragEvent(TThumb addTarget, TThumb moveTarget)
        {
            addTarget.DragDelta += moveTarget.TT_DragDelta;
            addTarget.DragCompleted += moveTarget.TT_DragCompleted;
        }

        public void RemoveDragEvent(TThumb addTarget, TThumb moveTarget)
        {
            addTarget.DragDelta -= moveTarget.TT_DragDelta;
            addTarget.DragCompleted -= moveTarget.TT_DragCompleted;
        }

    }


    public abstract class TTItemThumb : TThumb
    {
        public TTItemThumb()
        {
            SizeChanged += TThumb_SizeChanged;
        }
        private void TThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TTParent?.TTGroupUpdateLayout();
        }
    }


    public class TTTextBlock : TTItemThumb
    {
        private string? _myText;
        public string? MyText { get => _myText; set => SetProperty(ref _myText, value); }

        public TTTextBlock()
        {
            DataContext = this;
            //Template構築、適用
            FrameworkElementFactory text = new(typeof(TextBlock));
            FrameworkElementFactory waku = new(typeof(Rectangle));
            FrameworkElementFactory panel = new(typeof(Grid));
            waku.SetValue(Shape.StrokeProperty, Brushes.Red);
            waku.SetValue(Shape.StrokeThicknessProperty, 1.0);
            text.SetValue(TextBlock.TextProperty, new Binding(nameof(MyText)));
            panel.AppendChild(text);
            panel.AppendChild(waku);
            this.Template = new() { VisualTree = panel };
            this.ApplyTemplate();

        }


    }



    [ContentProperty(nameof(Children))]
    public class TTGroup : TThumb
    {
        public ObservableCollection<TThumb> Children { get; set; } = new();
        public TTGroup()
        {
            DataContext = this;
            Children.CollectionChanged += Children_CollectionChanged;

            FrameworkElementFactory waku = new(typeof(Rectangle));
            waku.SetValue(Rectangle.StrokeProperty, Brushes.Blue);
            waku.SetValue(Rectangle.StrokeThicknessProperty, 1.0);
            FrameworkElementFactory grid = new(typeof(Grid));
            FrameworkElementFactory ic = new(typeof(ItemsControl));
            //ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            FrameworkElementFactory icPanel = new(typeof(Canvas));
            ic.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(icPanel));
            ic.SetValue(ItemsControl.ItemsSourceProperty, new Binding(nameof(Children)));
            grid.AppendChild(ic);
            grid.AppendChild(waku);
            this.Template = new() { VisualTree = grid };

            Loaded += TTGroup_Loaded;
        }


        //TTGroupのRect取得
        private static (double x, double y, double w, double h) GetRect(TTGroup? group)
        {
            double x = double.MaxValue, y = double.MaxValue;
            double w = 0, h = 0;
            if (group != null)
            {
                foreach (var item in group.Children)
                {
                    var left = item.MyLeft; if (x > left) x = left;
                    var top = item.MyTop; if (y > top) y = top;
                    var width = left + item.ActualWidth;
                    var height = top + item.ActualHeight;
                    if (w < width) w = width;
                    if (h < height) h = height;
                }
            }
            return (x, y, w, h);
        }

        public void TTGroupUpdateLayout()
        {
            //TTGroup target = this;
            (double x, double y, double w, double h) = GetRect(this);

            //子要素位置修正
            foreach (var item in Children)
            {
                item.MyLeft -= x;
                item.MyTop -= y;
            }
            //自身がRoot以外なら自身の位置を更新
            if(this.GetType() != typeof(TTRoot))
            {
                MyLeft += x;
                MyTop += y;
            }

            //自身のサイズ更新
            w -= x; h -= y;
            if (w >= 0) Width = w;
            if (h >= 0) Height = h;

            //必要、これがないと見た目が変化しない、SizeChangedで確認できる
            UpdateLayout();

            //親要素Groupがあれば遡って更新
            if (TTParent is TTGroup parent)
            {
                parent.TTGroupUpdateLayout();
            }
        }

        //起動直後にサイズ更新実行、
        //これでXamlで子要素が置かれていても親要素のサイズが決定される
        //デザイン画面でも反映される
        private void TTGroup_Loaded(object sender, RoutedEventArgs e)
        {
            TTParent?.TTGroupUpdateLayout();
        }


        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                //子要素追加時、
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is TThumb thumb)
                    {
                        //子要素のParentプロパティに自身を入力しておく
                        thumb.TTParent = this;
                        //子要素がItemThumb系ならドラッグ移動系イベント登録
                        if (thumb is TTItemThumb item)
                        {
                            AddDragEvent(item, item);//移動はItemThumb
                            //AddDragEvent(item, this);//移動はグループ単位になる
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    //削除時はサイズと位置の更新
                    TTGroupUpdateLayout();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }

    public class TTRoot : TTGroup
    {
        public TThumb? ActiveThumb { get; set; }
        //クリックされたThumb
        public TThumb? ClickedThumb { get; set; }
        //フォーカスされているThumb、カーソルキーで移動させるThumb
        public TThumb? FocusThumb { get; set; }
        //子要素が移動対象になっているグループThumb
        public TTGroup EnableGroup { get; set; }
        
        public TTRoot()
        {
            if (EnableGroup == null) { EnableGroup = this; }
        }

    }
}
