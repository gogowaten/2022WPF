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

namespace _20220111
{
    internal class BaseThumb : Thumb
    {
        private Canvas RootCanvas;
        private readonly string ROOT_CANVAS_NAME = "root";
        public Canvas GropuCanvas = new();
        public Canvas SurfaceCanvas = new();

        public BaseThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), ROOT_CANVAS_NAME);
            this.Template = template;
            ApplyTemplate();
            RootCanvas = this.Template.FindName(ROOT_CANVAS_NAME, this) as Canvas;


            RootCanvas.Children.Add(GropuCanvas);
            RootCanvas.Children.Add(SurfaceCanvas);
        }
    }
    class TThumb : BaseThumb
    {
    }
}
