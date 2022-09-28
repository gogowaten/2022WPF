using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _20220921
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PointCollection MyPoints = new();
        public MainWindow()
        {
            InitializeComponent();


            var c2 = new Class2();
            StrategyStructure strategy = new();
            strategy.Start();
            

            MyPoints = new() { new Point(0, 30), new Point(100, 40) };
            Path path = new();
            Polyline polyline = new()
            {
                Points = MyPoints,
                StrokeThickness = 3.0,
                Stroke = Brushes.Red
            };
            MyCanvas.Children.Add(polyline);


            TThumb thumb = new();
            MyCanvas.Children.Add(thumb);
            thumb.DragDelta += Thumb_DragDelta;

        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is TThumb tt)
            {
                Canvas.SetLeft(tt, Canvas.GetLeft(tt) + e.HorizontalChange);
                Canvas.SetTop(tt, Canvas.GetTop(tt) + e.VerticalChange);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            MyPoints.Add(new Point(200, 0));
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class TThumb : Thumb
    {
        public TThumb()
        {
            this.Template = MakeTemplate(typeof(Polyline));
            Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
        }
        private ControlTemplate MakeTemplate(Type type)
        {
            FrameworkElementFactory element = new(type);
            element.SetValue(Polyline.StrokeProperty, Brushes.Blue);
            element.SetValue(Polyline.StrokeThicknessProperty, 10.0);
            element.SetValue(Polyline.PointsProperty, new PointCollection() { new Point(), new Point(100, 10) });
            ControlTemplate template = new(typeof(Thumb));
            template.VisualTree = element;
            return template;
        }
    }
}
