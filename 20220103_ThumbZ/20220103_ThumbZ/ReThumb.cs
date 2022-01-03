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
        //重なり順番、大きいほうが上、0から始まるけど実質のZIndexは+10して10から開始、これは下に装飾用のRectangleとか置く予定だから
        public int ZetIndex
        {
            get => zetIndex; set
            {
                zetIndex = value;
                Panel.SetZIndex(this, value + 10);
                OnPropertyChanged();
            }
        }

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
            AddElement(element);
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

        //グループ化は複数Thumbを元に新規作成する→やめた、どこにも追加されていないThumb同士でグループ化するとZIndex処理がめんどくさそう
        //代わりにメソッドにする、どこかに追加されたThumbのみ対象にする
        public ReThumb(IEnumerable<ReThumb> reThumbs, string name = "") : this()
        {
            int ziMax = reThumbs.Max(a => a.ZetIndex);
            int ziMin = reThumbs.Min(a => a.ZetIndex);
            IdName = string.IsNullOrEmpty(name) ? DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") : name;
            double left = reThumbs.Min(a => a.Left);
            double top = reThumbs.Min(a => a.Top);
            //Parent取得、すべて同じはずなので先頭から取得
            ReThumb reParent = reThumbs.First().ParentReThumb;

            //元の親から削除して、新しい親のChildrenに追加する
            //ZIndex順にソートされている(前提の)はず
            foreach (ReThumb item in reThumbs)
            {
                item.Left -= left;
                item.Top -= top;
                //元の所属先がある場合は、そこから削除
                if (reParent != null)
                {
                    reParent.children.Remove(item);//削除
                }
                this.children.Add(item);//追加
            }

            Left = left;
            Top = top;
        }



        //こっちでのGotFocusはやめた
        //private void ReThumb_GotFocus(MainWindow mainWindow)
        //{
        //    mainWindow.FocusThumb = this;
        //}
        //public ReThumb(UIElement element, MainWindow mainWindow) : this(element, null, 0, 0)
        //{
        //    this.GotFocus += (a, b) => ReThumb_GotFocus(mainWindow);
        //}
        //public ReThumb(UIElement element, MainWindow mainWindow, string name = null) : this(element, mainWindow)
        //{
        //    idName = name;
        //}
        //public ReThumb(UIElement element, MainWindow mainWindow, string name = null, double x = 0, double y = 0) : this(element, mainWindow, name)
        //{
        //    Left = x;
        //    Top = y;
        //}

        //グループ解除
        //子要素を開放して親の要素にする
        //親がLayerならDragDeltaイベント付着
        public void UnGroup()
        {
            if (this.IsGroup == false) { return; }
            foreach (ReThumb item in children.ToList())
            {
                children.Remove(item);
                //Parentに移動、ParentがLayerだったばあいはドラッグ移動できるようにする
                Layer layer = this.ParentReThumb as Layer;
                if (layer != null)
                {
                    item.DragDelta += item.ReThumb_DragDelta;
                    layer.AddChildren(item);
                }
                else
                {
                    this.ParentReThumb.AddElement(item);
                }
                //座標修正
                item.Left += this.left;
                item.Top += this.top;
                //RootReThumbの更新
                ReplaceRootReThumb(item, item);
            }
            //グループ(自身)を削除
            this.ParentReThumb.children.Remove(this);
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

            //追加された場合
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
                        //ZIndex
                        re.ZetIndex = this.children.Count - 1;
                    }

                }
            }

            //削除された場合、ParentのChildrenに移動する
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    ReThumb re = item as ReThumb;
                    RootCanvas.Children.Remove(re);//グループから切り離し
                    //ZIndexの調整、削除対象のZIndexより大きいものだけが対象、-1する
                    foreach (ReThumb child in children)
                    {
                        if (child.zetIndex > re.zetIndex)
                        {
                            child.ZetIndex--;
                        }
                    }
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var nn = e.NewItems[0];
                var oo = e.OldItems[0];
            }
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

        public void SetZIndex(int newIndex)
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

    public class Layer : ReThumb
    {
        public Layer()
        {
            DragDelta -= ReThumb_DragDelta;
        }

        public void AddChildren(ReThumb thumb)
        {
            children.Add(thumb);
            thumb.ZetIndex = children.Count - 1;
        }
        //protected override void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{

        //}

        //private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{

        //}
    }
}
