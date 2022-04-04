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
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace _20220401_ThumbDragAndDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   
        public partial class Pane : Canvas
        {
            private void OnDragDelta(object sender, DragDeltaEventArgs e)
            {
                //Move the Thumb to the mouse position during the drag operation
                var yadjust = myCanvasStretch.Height + e.VerticalChange;
                var xadjust = myCanvasStretch.Width + e.HorizontalChange;
                if ((xadjust >= 0) && (yadjust >= 0))
                {
                    myCanvasStretch.Width = xadjust;
                    myCanvasStretch.Height = yadjust;
                    SetLeft(myThumb, GetLeft(myThumb) +
                                     e.HorizontalChange);
                    SetTop(myThumb, GetTop(myThumb) +
                                    e.VerticalChange);
                    changes.Text = "Size: " +
                                   myCanvasStretch.Width.ToString(CultureInfo.InvariantCulture) +
                                   ", " +
                                   myCanvasStretch.Height.ToString(CultureInfo.InvariantCulture);
                }
            }

            private void OnDragStarted(object sender, DragStartedEventArgs e)
            {
                myThumb.Background = Brushes.Orange;
            }

            private void OnDragCompleted(object sender, DragCompletedEventArgs e)
            {
                myThumb.Background = Brushes.Blue;
            }
        }
    
}
