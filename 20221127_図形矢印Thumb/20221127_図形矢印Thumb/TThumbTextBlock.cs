using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;

namespace _20221127_図形矢印Thumb
{
    internal class TThumbTextBlock : Thumb
    {
        public string MyText { get; set; } = "";
        public TThumbTextBlock()
        {
            SetTemplate();
            DataContext = this;
        }
        public void SetTemplate()
        {
            FrameworkElementFactory text = new(typeof(TextBlock));
            Binding b = new(nameof(MyText));
            b.Mode = BindingMode.TwoWay;
            text.SetBinding(TextBlock.TextProperty, b);

            ControlTemplate template = new();
            template.VisualTree = text;
            this.Template = template;
        }
    }

}
