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

namespace _20220704
{
    public abstract class TThumb : Thumb
    {
        public Data MyData { get; protected set; }
        public CanvasThumb? MyCanvas { get; set; }
        //public LayerThumb? MyLayerThumb { get; set; }
        //public GroupAndLayerBase? MyGroupThumb { get; set; }
        public bool IsCurrentItem { get; private set; }
        public TThumb? MyParentThumb { get; set; }

        public TThumb(Data myData)
        {
            MyData = myData;
            DataContext = this;


            SetDragDeltaBinding();
            Binding bind = new(nameof(MyData.Z)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Panel.ZIndexProperty, bind);
        }

        protected static void SetIsCurrentItem(ItemThumb? oldItem, ItemThumb? newItem)
        {
            if (oldItem != null) oldItem.IsCurrentItem = false;
            if (newItem != null) newItem.IsCurrentItem = true;
        }
        protected void SetMyCanvas()
        {
            MyCanvas = GetMyCanvas();
        }
        protected CanvasThumb? GetMyCanvas()
        {
            if(MyParentThumb is CanvasThumb canvas)
            {
                return canvas;
            }
            else
            {
               return MyParentThumb?.GetMyCanvas();
            }
        }
        protected abstract void SetTemplate();

      

        public void SetDragDelta()
        {
            DragDelta += TThumb_DragDelta;
        }
        private void TThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MyData.X += e.HorizontalChange;
            MyData.Y += e.VerticalChange;
        }
        private void SetDragDeltaBinding()
        {
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
            Binding b;
            b = new(nameof(MyData.X)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.LeftProperty, b);
            b = new(nameof(MyData.Y)) { Source = MyData, Mode = BindingMode.TwoWay };
            this.SetBinding(Canvas.TopProperty, b);
        }

        public override string ToString()
        {
            string str = $"X{MyData.X}, Y{MyData.Y}, Z{MyData.Z}";
            return str;
        }
    }

    public abstract class ItemThumb : TThumb
    {
        protected FrameworkElementFactory waku;
        protected FrameworkElementFactory panel;



        public ItemThumb(Data myData) : base(myData)
        {
            waku = new(typeof(Rectangle));
            waku.SetValue(Panel.ZIndexProperty, 1);
            panel = new(typeof(Grid));
            panel.AppendChild(waku);

            SetTemplate();

            PreviewMouseDown += ItemThumb_PreviewMouseDown;
        }

        private void ItemThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyCanvas is not null)
            {
                MyCanvas.MyCurrentItem = this;
            }
            else
            {
                SetMyCanvas();
                if(MyCanvas is not null)
                {
                    MyCanvas.MyCurrentItem = this;
                }
            }
        }

        protected static Binding MakeTwoWayBinding(string path, object source)
        {
            return new(path) { Source = source, Mode = BindingMode.TwoWay };
        }
        protected void MySetBinding(FrameworkElementFactory elem, DependencyProperty dp, string path)
        {
            elem.SetBinding(dp, MakeTwoWayBinding(path, MyData));
        }

        
    }
    public class TTextBlock : ItemThumb
    {
        public new DataTextBlock MyData { get; protected set; }
        public TTextBlock(DataTextBlock data) : base(data)
        {
            MyData = data;

        }

        protected override void SetTemplate()
        {
            FrameworkElementFactory content = new(typeof(TextBlock));
            MySetBinding(content, TextBlock.TextProperty, nameof(MyData.Text));
            MySetBinding(content, TextBlock.ForegroundProperty, nameof(MyData.ColorFont));
            MySetBinding(content, TextBlock.BackgroundProperty, nameof(MyData.ColorBack));
            MySetBinding(content, TextBlock.FontSizeProperty, nameof(MyData.FontSize));
            panel.AppendChild(content);
            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;
        }
        public override string ToString()
        {
            return $" {base.ToString()}, {MyData.Text}";
        }

    }
    public class TRectangle : ItemThumb
    {
        public new DataRectangle MyData { get; protected set; }
        public TRectangle(DataRectangle myData) : base(myData)
        {
            MyData = myData;
        }

        protected override void SetTemplate()
        {
            FrameworkElementFactory content = new(typeof(Rectangle));
            MySetBinding(content, Rectangle.WidthProperty, nameof(MyData.Width));
            MySetBinding(content, Rectangle.HeightProperty, nameof(MyData.Height));
            MySetBinding(content, Rectangle.FillProperty, nameof(MyData.ColorFill));
            MySetBinding(content, Rectangle.StrokeProperty, nameof(MyData.ColorStroke));
            MySetBinding(content, Rectangle.StrokeThicknessProperty, nameof(MyData.StrokeThickness));
            panel.AppendChild(content);
            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;
        }
    }



    #region グループ用Thumb


    //GroupとLayer、Canvasの基本クラス
    public abstract class GroupBase : TThumb
    {
        public new DataGroupBase MyData { get; protected set; }
        protected ObservableCollection<TThumb> Children { get; } = new();
        public ReadOnlyObservableCollection<TThumb> MyChildren { get; set; }
        protected GroupBase(DataGroupBase myData) : base(myData)
        {
            MyData = myData;
            MyChildren = new(Children);
            SetTemplate();
            if (myData.ChildrenData.Count > 0)
            {
                throw new NotImplementedException();
            }
        }
        protected override void SetTemplate()
        {
            FrameworkElementFactory panel = new(typeof(Grid));

            FrameworkElementFactory content = new(typeof(ItemsControl));
            content.SetValue(ItemsControl.ItemsPanelProperty,
                new ItemsPanelTemplate(
                    new FrameworkElementFactory(typeof(Canvas))));
            content.SetValue(ItemsControl.ItemsSourceProperty,
                new Binding(nameof(MyChildren)));
            panel.AppendChild(content);

            FrameworkElementFactory waku = new(typeof(Rectangle));
            panel.AppendChild(waku);


            ControlTemplate template = new();
            template.VisualTree = panel;
            this.Template = template;

        }
    }

    //GroupとLayerの基本クラス
    public abstract class GroupAndLayerBase : GroupBase
    {
        protected GroupAndLayerBase(DataGroupBase myData) : base(myData)
        {

        }

        public void AddChild(TThumb thumb, int z = -1)
        {
            //ZIndex指定なしは末尾(最上位)に追加
            if (z == -1)
            {
                thumb.MyData.Z = Children.Count;
                Children.Add(thumb);
            }
            else
            {
                thumb.MyData.Z = z;
                Children.Insert(z, thumb);
                for (int i = z; i < Children.Count; i++)
                {
                    Children[i].MyData.Z = i;
                }
            }
            thumb.MyParentThumb = this;
        }
        //public void AddChild(TThumb thumb)
        //{
        //    int z = thumb.MyData.Z;
        //    AddChild(thumb, z);
        //    thumb.MyParentThumb = this;
        //}
    }

    public class GroupThumb : GroupAndLayerBase
    {
        public GroupThumb(DataGroup myData) : base(myData)
        {

        }
        public new void AddChild(TThumb thumb)
        {
            base.AddChild(thumb);

        }
    }
    public class LayerThumb : GroupAndLayerBase
    {
        public LayerThumb(DataLayer myData) : base(myData)
        {
        }
        public new void AddChild(TThumb thumb)
        {
            base.AddChild(thumb);

        }
    }


    public class CanvasThumb : GroupBase
    {
        protected new ObservableCollection<LayerThumb> Children { get; } = new();
        public new ReadOnlyObservableCollection<LayerThumb> MyChildren { get; set; }


        public ItemThumb MyCurrentItem
        {
            get { return (ItemThumb)GetValue(MyCurrentItemProperty); }
            //set { SetValue(MyCurrentItemProperty, value); }
            set
            {
                if (MyCurrentItem != value)
                {
                    SetIsCurrentItem(MyCurrentItem, value);
                    SetValue(MyCurrentItemProperty, value);
                }
            }
        }
        public static readonly DependencyProperty MyCurrentItemProperty =
            DependencyProperty.Register("MyCurrentItem", typeof(ItemThumb), typeof(CanvasThumb), new PropertyMetadata(null));


        public TThumb CurrentThumb
        {
            get { return (TThumb)GetValue(CurrentThumbProperty); }
            set { SetValue(CurrentThumbProperty, value); }
        }
        public static readonly DependencyProperty CurrentThumbProperty =
            DependencyProperty.Register("CurrentThumb", typeof(TThumb), typeof(CanvasThumb), new PropertyMetadata(null));


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="myData"></param>
        public CanvasThumb(DataCanvas myData) : base(myData)
        {
            MyChildren = new(Children);

        }
        public void AddLayer(LayerThumb layer)
        {
            Children.Add(layer);
            layer.MyParentThumb = this;
        }


    }

    #endregion グループ用Thumb



}
