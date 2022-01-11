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

namespace _20220107_Thumb選択グループ
{
    public class NeoThumb : Thumb, System.ComponentModel.INotifyPropertyChanged
    {
        private Canvas RootCanvas;
        //public bool IsRoot;
        public bool IsGroup;
        public NeoThumb ParentNeoThumb;
        public NeoThumb RootNeoThumb;//動かすThumb

        //public ObservableCollection<NeoThumb> ChildrenOld { get; set; } = new();
        //        読み取り専用に公開するパターン - Qiita
        //https://qiita.com/Azleep/items/299fafdf51f260bbecb2

        protected ObservableCollection<NeoThumb> children = new();
        public ReadOnlyObservableCollection<NeoThumb> Children { get; private set; }
        //public ReadOnlyObservableCollection<NeoThumb> Children1 => new(Children);

        private double myLeft;
        private double myTop;
        private string myName;
        private int myZIndex;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double MyLeft { get => myLeft; set { myLeft = value; OnPropertyChanged(); } }
        public double MyTop { get => myTop; set { myTop = value; OnPropertyChanged(); } }
        public string MyName { get => myName; set { myName = value; OnPropertyChanged(); } }
        //重なり順番、大きいほうが上、
        //下に装飾用のRectangleとか置く予定だから
        //実質のZIndexはConverterで+10している、10から開始、
        public int MyZIndex { get => myZIndex; set { myZIndex = value; OnPropertyChanged(); } }

        protected NeoThumb()
        {
            Children = new(children);


            ControlTemplate template = new();
            FrameworkElementFactory fCanvas = new(typeof(Canvas), "rootcanvas");
            template.VisualTree = fCanvas;
            this.Template = template;
            ApplyTemplate();
            RootCanvas = template.FindName("rootcanvas", this) as Canvas;

            children.CollectionChanged += Children_CollectionChanged;
            DragDelta += NeoThumb_DragDelta;

            this.SetBinding(Canvas.LeftProperty, MakeBind(nameof(MyLeft)));
            this.SetBinding(Canvas.TopProperty, MakeBind(nameof(MyTop)));
            //ZIndexの実際の値は+10したいのでBindingじゃなくてsetのほうで指定
            var z = MakeBind(nameof(MyZIndex));
            z.Converter = new ZIndexConverter();
            this.SetBinding(Panel.ZIndexProperty, z);

            this.Focusable = true;
            //IsRoot = true;
            RootNeoThumb = this;

        }


        //public void SetGotFocus(NeoThumb focus)
        //{
        //    this.GotFocus += (a, b) => NeoThumb_GotFocus1(a, b, focus);
        //}

        private void NeoThumb_GotFocus1(object sender, RoutedEventArgs e, NeoThumb focus)
        {
            focus = this;
        }

        public NeoThumb(UIElement element, string name = "", double x = 0, double y = 0) : this()
        {
            //AddElement(element);
            RootCanvas.Children.Add(element);
            MyName = string.IsNullOrEmpty(name) ? DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") : name;
            MyLeft = x;
            MyTop = y;
        }

        public NeoThumb(UIElement element, double x, double y) : this(element, "")
        {
            MyLeft = x;
            MyTop = y;
        }


        public NeoThumb(UIElement element) : this(element, "", 0, 0)
        {

        }

        //グループ化は要素群(Thumb)を元に新規作成する
        //要素群のParentがある場合は、そこに追加する
        public NeoThumb(IEnumerable<NeoThumb> NeoThumbs, string name = "") : this()
        {
            if (NeoThumbs.Count() < 2) { return; }
            //要素群からParent取得
            NeoThumb reParent = NeoThumbs.First().ParentNeoThumb;
            int ziMax = NeoThumbs.Max(a => a.MyZIndex);
            int ziMin = NeoThumbs.Min(a => a.MyZIndex);
            MyName = string.IsNullOrEmpty(name) ? DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") : name;
            //要素群全体を含むRectの左上座標
            double left = NeoThumbs.Min(a => a.MyLeft);
            double top = NeoThumbs.Min(a => a.MyTop);
            //自身(新規ブループ)の座標調整
            MyLeft = left; MyTop = top;

            //要素群をZIndex順にソートする
            var items = NeoThumbs.OrderBy(i => i.myZIndex).ToList();
            for (int i = 0; i < items.Count; i++)
            {
                NeoThumb item = items[i];
                //Parentがある場合は、そこから削除
                if (reParent != null)
                {
                    reParent.children.Remove(item);//削除
                }
                //自身(新規ブループ)に要素群追加
                children.Add(item);
                item.MyZIndex = i;
                //要素の座標調整
                item.MyLeft -= left; item.MyTop -= top;
            }

            if (reParent != null)
            {
                //Parentに自身(新規ブループ)を挿入、挿入Index = 最上位Index - (グループ要素数 - 1)
                reParent.children.Insert(ziMax - (items.Count - 1), this);


                //ParentChildrenのZIndex調整、グループ追加位置より後ろの要素が対象
                for (int i = ziMin; i < reParent.children.Count; i++)
                {
                    int zi = reParent.children[i].MyZIndex;
                    if (zi != i) { reParent.children[i].MyZIndex = i; }
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
            this.ParentNeoThumb.children.Remove(this);

            int oldIndex = this.myZIndex;
            int oldItemsCount = this.ParentNeoThumb.children.Count;
            //ZIndex順にソート
            List<NeoThumb> items = children.OrderBy(a => a.myZIndex).ToList();
            //要素群をグループから削除
            for (int i = 0; i < items.Count; i++)
            {
                NeoThumb item = items[i];
                children.Remove(item);
                //Parentに移動、ParentがLayerだった場合はドラッグ移動できるようにする                
                if ((this.ParentNeoThumb as Layer) != null)
                {
                    item.DragDelta += item.NeoThumb_DragDelta;
                }
                //Parentに挿入
                this.ParentNeoThumb.children.Insert(oldIndex + i, item);

                //座標修正
                item.MyLeft += this.myLeft; item.MyTop += this.myTop;
                //RootNeoThumbの更新
                ReplaceRootNeoThumb(item, item);
                //ZIndex
                item.MyZIndex += oldIndex;
            }


            //グループ解除対象以外の要素のZIndex修正
            for (int i = oldIndex + items.Count; i < this.ParentNeoThumb.children.Count; i++)
            {
                this.ParentNeoThumb.children[i].MyZIndex = i;
            }

        }


        protected virtual void NeoThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyLeft += e.HorizontalChange;
            MyTop += e.VerticalChange;
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
                    NeoThumb re = item as NeoThumb;
                    RootCanvas.Children.Add(re);
                    re.ParentNeoThumb = this;
                    //Layerではなく新規グループに追加された場合
                    if (typeof(Layer) != this.GetType())
                    {
                        //RootThumb変更
                        ReplaceRootNeoThumb(re, this.RootNeoThumb);
                        //DragDeltaを外す
                        re.DragDelta -= re.NeoThumb_DragDelta;
                    }

                }
            }

            //削除された場合
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    NeoThumb re = item as NeoThumb;
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
                children[newId].MyZIndex = newId;//自身のZIndex変更

                //自身が上がった場合、元の位置から新しい位置の間にある要素全てを-1する
                if (newId > oldId)
                {
                    for (int i = oldId; i < newId; i++)
                    {
                        children[i].MyZIndex--;
                    }
                }
                //自身が下がった場合はあいだの要素全てを+1
                else
                {
                    for (int i = newId + 1; i < oldId + 1; i++)
                    {
                        children[i].MyZIndex++;
                    }
                }

            }
        }

        private void ReplaceRootNeoThumb(NeoThumb current, NeoThumb root)
        {
            current.RootNeoThumb = root;
            if (current.children.Count < 1) { return; }
            foreach (var item in current.children)
            {
                item.RootNeoThumb = root;
                ReplaceRootNeoThumb(item, root);
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
            int oldIndex = this.MyZIndex;
            if (newIndex >= ParentNeoThumb.children.Count
                || newIndex < 0
                || newIndex == oldIndex)
            {
                return;
            }

            this.ParentNeoThumb.children.Move(oldIndex, newIndex);
        }

        public void Test()
        {
            var neko = RootCanvas.ActualHeight;
            Rect range = new();
            foreach (var item in children)
            {
                FrameworkElement element = item as FrameworkElement;
                Rect r = new(item.MyLeft, item.MyTop, element.ActualWidth, element.ActualHeight);
                range.Union(r);
            }

        }
        public override string ToString()
        {
            //return base.ToString();
            return $"{MyName}, x={MyLeft}, y={MyTop}, z={myZIndex}";
        }
    }

    //----------------------------------------------------------------------------------------


    public class Layer : NeoThumb
    {
        public Layer()
        {
            DragDelta -= NeoThumb_DragDelta;
        }

        public void AddChildren(NeoThumb thumb, int z = int.MinValue)
        {
            if (z == int.MinValue) { z = children.Count; }
            if (z >= children.Count) { z = children.Count; }
            thumb.MyZIndex = z;
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
