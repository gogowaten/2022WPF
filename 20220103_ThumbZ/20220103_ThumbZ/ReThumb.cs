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

namespace _20220103_ThumbZ
{
    public class ReThumb : Thumb, System.ComponentModel.INotifyPropertyChanged
    {
        private Canvas RootCanvas;
        //public bool IsRoot;
        public bool IsGroup;
        public ReThumb ParentReThumb;
        public ReThumb RootReThumb;//動かすThumb

        //public ObservableCollection<ReThumb> ChildrenOld { get; set; } = new();
        //        読み取り専用に公開するパターン - Qiita
        //https://qiita.com/Azleep/items/299fafdf51f260bbecb2

        protected ObservableCollection<ReThumb> children = new();
        public ReadOnlyObservableCollection<ReThumb> Children { get; private set; }
        //public ReadOnlyObservableCollection<ReThumb> Children1 => new(Children);

        private double left;
        private double top;
        private string idName;
        private int zetIndex;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double Left { get => left; set { left = value; OnPropertyChanged(); } }
        public double Top { get => top; set { top = value; OnPropertyChanged(); } }
        public string IdName { get => idName; set { idName = value; OnPropertyChanged(); } }
        //重なり順番、大きいほうが上、
        //下に装飾用のRectangleとか置く予定だから
        //実質のZIndexはConverterで+10している、10から開始、
        public int ZetIndex { get => zetIndex; set { zetIndex = value; OnPropertyChanged(); } }

        protected ReThumb()
        {
            Children = new(children);


            ControlTemplate template = new();
            FrameworkElementFactory fCanvas = new(typeof(Canvas), "rootcanvas");
            template.VisualTree = fCanvas;
            this.Template = template;
            ApplyTemplate();
            RootCanvas = template.FindName("rootcanvas", this) as Canvas;

            children.CollectionChanged += Children_CollectionChanged;
            DragDelta += ReThumb_DragDelta;

            this.SetBinding(Canvas.LeftProperty, MakeBind(nameof(Left)));
            this.SetBinding(Canvas.TopProperty, MakeBind(nameof(Top)));
            //ZIndexの実際の値は+10したいのでBindingじゃなくてsetのほうで指定
            var z = MakeBind(nameof(ZetIndex));
            z.Converter = new ZIndexConverter();
            this.SetBinding(Panel.ZIndexProperty, z);

            this.Focusable = true;
            //IsRoot = true;
            RootReThumb = this;

        }


        public void SetGotFocus(ReThumb focus)
        {
            this.GotFocus += (a, b) => ReThumb_GotFocus1(a, b, focus);
        }

        private void ReThumb_GotFocus1(object sender, RoutedEventArgs e, ReThumb focus)
        {
            focus = this;
        }

        public ReThumb(UIElement element, string name = "", double x = 0, double y = 0) : this()
        {
            //AddElement(element);
            RootCanvas.Children.Add(element);
            IdName = string.IsNullOrEmpty(name) ? DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") : name;
            Left = x;
            Top = y;
        }

        public ReThumb(UIElement element, double x, double y) : this(element, "")
        {
            Left = x;
            Top = y;
        }


        public ReThumb(UIElement element) : this(element, "", 0, 0)
        {

        }

        //グループ化は要素群(Thumb)を元に新規作成する
        //要素群のParentがある場合は、そこに追加する
        public ReThumb(IEnumerable<ReThumb> reThumbs, string name = "") : this()
        {
            //要素群からParent取得
            ReThumb reParent = reThumbs.First().ParentReThumb;
            //if (reParent == null)
            //{
            //    throw new ArgumentNullException(nameof(reParent), "Parentが無いものはグループ化できない");
            //}
            int ziMax = reThumbs.Max(a => a.ZetIndex);
            int ziMin = reThumbs.Min(a => a.ZetIndex);
            IdName = string.IsNullOrEmpty(name) ? DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") : name;
            //要素群全体を含むRectの左上座標
            double left = reThumbs.Min(a => a.Left);
            double top = reThumbs.Min(a => a.Top);
            //自身(新規ブループ)の座標調整
            Left = left; Top = top;

            //要素群をZIndex順にソートする
            var items = reThumbs.OrderBy(i => i.zetIndex).ToList();
            for (int i = 0; i < items.Count; i++)
            {
                ReThumb item = items[i];
                //Parentがある場合は、そこから削除
                if (reParent != null)
                {
                    reParent.children.Remove(item);//削除
                }
                //自身(新規ブループ)に要素を追加してZIndexも指定
                children.Add(item);
                item.ZetIndex = i;
                //要素の座標調整
                item.Left -= left; item.Top -= top;
            }

            if (reParent != null)
            {
                //Parentに自身(新規ブループ)を挿入、挿入Index = 最上位Index - (グループ要素数 - 1)
                reParent.children.Insert(ziMax - (items.Count() - 1), this);


                //ParentChildrenのZIndex調整、グループ追加位置より後ろの要素が対象
                for (int i = ziMin; i < reParent.children.Count; i++)
                {
                    int zi = reParent.children[i].ZetIndex;
                    if (zi != i) { reParent.children[i].ZetIndex = i; }
                }
            }

        }

        //グループ解除
        //子要素を開放して親の要素にする
        //親がLayerならDragDeltaイベント付着
        //ZIndexの変更もここで行う
        public void UnGroup()
        {
            if (this.IsGroup == false) { return; }

            //最初にグループ(自身)をParentから削除
            this.ParentReThumb.children.Remove(this);

            int oldIndex = this.zetIndex;
            int oldItemsCount = this.ParentReThumb.children.Count;
            //ZIndex順にソート
            List<ReThumb> items = children.OrderBy(a => a.zetIndex).ToList();
            //要素群をグループから削除
            for (int i = 0; i < items.Count; i++)
            {
                ReThumb item = items[i];
                children.Remove(item);
                //Parentに移動、ParentがLayerだった場合はドラッグ移動できるようにする                
                if ((this.ParentReThumb as Layer) != null)
                {
                    item.DragDelta += item.ReThumb_DragDelta;
                }
                //Parentに挿入
                this.ParentReThumb.children.Insert(oldIndex + i, item);

                //座標修正
                item.Left += this.left; item.Top += this.top;
                //RootReThumbの更新
                ReplaceRootReThumb(item, item);
                //ZIndex
                item.ZetIndex += oldIndex;
            }


            //グループ解除対象以外の要素のZIndex修正
            for (int i = oldIndex + items.Count; i < this.ParentReThumb.children.Count; i++)
            {
                this.ParentReThumb.children[i].ZetIndex = i;
            }

        }


        protected virtual void ReThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }

        protected virtual void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (children.Count < 2 && IsGroup) { IsGroup = false; }
            else { IsGroup = true; }

            //追加された場合、Insertもここに来る
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    ReThumb re = item as ReThumb;
                    RootCanvas.Children.Add(re);
                    re.ParentReThumb = this;
                    //Layerではなく新規グループに追加された場合
                    if (typeof(Layer) != this.GetType())
                    {
                        //RootThumb変更
                        ReplaceRootReThumb(re, this.RootReThumb);
                        //DragDeltaを外す
                        re.DragDelta -= re.ReThumb_DragDelta;
                    }

                }
            }

            //削除された場合
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    ReThumb re = item as ReThumb;
                    RootCanvas.Children.Remove(re);//グループから切り離し
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var nn = e.NewItems[0];
                var oo = e.OldItems[0];
            }
            //Moveだけ特別にZIndexを変更するようにしているけど、これでいい？
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                int newId = e.NewStartingIndex;//移動先Index
                int oldId = e.OldStartingIndex;//元のIndex
                children[newId].ZetIndex = newId;//自身のZIndex変更

                //自身が上がった場合、元の位置から新しい位置の間にある要素全てを-1する
                if (newId > oldId)
                {
                    for (int i = oldId; i < newId; i++)
                    {
                        children[i].ZetIndex--;
                    }
                }
                //自身が下がった場合はあいだの要素全てを+1
                else
                {
                    for (int i = newId + 1; i < oldId + 1; i++)
                    {
                        children[i].ZetIndex++;
                    }
                }

            }
        }

        private void ReplaceRootReThumb(ReThumb current, ReThumb root)
        {
            current.RootReThumb = root;
            if (current.children.Count < 1) { return; }
            foreach (var item in current.children)
            {
                item.RootReThumb = root;
                ReplaceRootReThumb(item, root);
            }

        }
        private Binding MakeBind(string path)
        {
            Binding b = new();
            b.Source = this;
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath(path);
            return b;
        }
        private void AddElement(UIElement element)
        {
            RootCanvas.Children.Add(element);
        }

        /// <summary>
        /// ZIndexの変更、実際の動作は移動になるので、移動元から移動先の間の全要素のZIndexも変更になる
        /// </summary>
        /// <param name="newIndex"></param>
        public void ChangeZIndex(int newIndex)
        {
            int oldIndex = this.ZetIndex;
            if (newIndex >= ParentReThumb.children.Count
                || newIndex < 0
                || newIndex == oldIndex)
            {
                return;
            }

            this.ParentReThumb.children.Move(oldIndex, newIndex);
        }

        public override string ToString()
        {
            //return base.ToString();
            return $"{IdName}, x={Left}, y={Top}, z={zetIndex}";
        }
    }

    //----------------------------------------------------------------------------------------


    public class Layer : ReThumb
    {
        public Layer()
        {
            DragDelta -= ReThumb_DragDelta;
        }

        public void AddChildren(ReThumb thumb, int z = int.MinValue)
        {
            if (z == int.MinValue) { z = children.Count; }
            thumb.ZetIndex = z;
            children.Insert(z, thumb);

        }
        //protected override void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{

        //}

        //private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{

        //}
    }

    public class ZIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            int z = (int)value;
            return z + 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            int z = (int)value;
            return z - 10;
        }
    }
}
