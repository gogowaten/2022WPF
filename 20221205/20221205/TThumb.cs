using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace _20221205
{
    internal class TThumb : Thumb
    {

        public TThumb(Data data)
        {
            SetLocate(data.X, data.Y);
            //this.Template = MakeTemplate(data.DataType);
        }
        private void SetLocate(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }

        private ControlTemplate MakeTemplate(DataType type)
        {
            switch (type)
            {
                case DataType.None:
                    break;
                case DataType.TextBlock:
                    break;
                default:
                    break;
            }
            FrameworkElementFactory element = new(typeof(TextBlock));
            element.SetValue(TextBlock.TextProperty, "Text");
            element.SetValue(TextBlock.FontSizeProperty, 20.0);
            element.SetValue(TextBlock.ForegroundProperty, Brushes.MediumAquamarine);

            ControlTemplate template = new();
            template.VisualTree = element;
            return template;
        }


    }
}
