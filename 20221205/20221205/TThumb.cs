using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace _20221205
{
    public abstract class TThumb : Thumb
    {
        public Data MyData;
        public DataType MyDataType { get; private set; }
        public TThumb(Data data)
        {
            this.MyData = data;
            MyDataType = data.DataType;
            SetLocate(data.X, data.Y);
        }
        private void SetLocate(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
        protected abstract void SetTemplate();


    }
    public class TTTextBlock : TThumb
    {
        public new DDTextBlock MyData;
        public TTTextBlock(DDTextBlock data) : base(data)
        {
            MyData = data;
            this.DataContext = MyData;
            SetTemplate();
        }



        protected override void SetTemplate()
        {
            FrameworkElementFactory elem = new(typeof(TextBlock));
            elem.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
            elem.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(MyData.FontColor)));
            elem.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(MyData.FontSize)));
            ControlTemplate template = new();
            template.VisualTree = elem;
            this.Template = template;
        }
    }


}
