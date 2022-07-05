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
        public TThumb(Data myData)
        {
            MyData = myData;
            DataContext = this;


            SetDragDeltaBinding();
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
    //public abstract class GroupBaseThumb : TThumb
    //{
    //    public new DataGroupBase MyData { get; protected set; }
    //    private ObservableCollection<TThumb> Children { get; } = new();
    //    public ReadOnlyObservableCollection<TThumb> MyChildren { get; set; }
    //    protected GroupBaseThumb(DataGroupBase myData) : base(myData)
    //    {

    //        MyData = myData;
    //        MyChildren = new(Children);
    //        SetTemplate();
    //        if (myData.ChildrenData.Count > 0)
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }
    //    protected override void SetTemplate()
    //    {
    //        FrameworkElementFactory panel = new(typeof(Grid));

    //        FrameworkElementFactory content = new(typeof(ItemsControl));
    //        content.SetValue(ItemsControl.ItemsPanelProperty,
    //            new ItemsPanelTemplate(
    //                new FrameworkElementFactory(typeof(Canvas))));
    //        content.SetValue(ItemsControl.ItemsSourceProperty,
    //            new Binding(nameof(MyChildren)));
    //        panel.AppendChild(content);

    //        FrameworkElementFactory waku = new(typeof(Rectangle));
    //        panel.AppendChild(waku);


    //        ControlTemplate template = new();
    //        template.VisualTree = panel;
    //        this.Template = template;

    //    }
    //    public void AddChild(TThumb thumb)
    //    {
    //        Children.Add(thumb);
    //    }       
    //}

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

        public void AddChild(TThumb thumb)
        {
            Children.Add(thumb);
        }
    }

    public class GroupThumb : GroupAndLayerBase
    {
        public GroupThumb(DataGroup myData) : base(myData)
        {

        }

    }
    public class LayerThumb : GroupAndLayerBase
    {
        public LayerThumb(DataLayer myData) : base(myData)
        {
        }
    }

    public class CanvasThumb : GroupBase
    {
        private new ObservableCollection<LayerThumb> Children { get; } = new();
        public new ReadOnlyObservableCollection<LayerThumb> MyChildren { get; set; }

        public CanvasThumb(DataCanvas myData) : base(myData)
        {
            MyChildren = new(Children);

        }
        public void AddChild(LayerThumb layer)
        {
            Children.Add(layer);
        }


    }

    #endregion グループ用Thumb



}
