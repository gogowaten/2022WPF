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
namespace _20220107
{
    public class SelectThumb : Thumb, System.ComponentModel.INotifyPropertyChanged
    {
        private Grid MyRootPanel;
        public SelectThumb ParentSelectThumb;
        public SelectThumb RootSelectThumb;//動かすThumb

        Rectangle MyRectangle = new();

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
        //重なり順番、大きいほうが上、
        //下に装飾用のRectangleとか置く予定だから
        //実質のZIndexはConverterで+10している、10から開始、
        public int ZetIndex { get => zetIndex; set { zetIndex = value; OnPropertyChanged(); } }

        private double myWidth;
        public double MyWidth
        {
            get => myWidth;
            set { if (myWidth == value) { return; } myWidth = value; OnPropertyChanged(); }
        }

        public SelectThumb()
        {
            //Thumb
            // ┗Grid
            //      ┗Rectangle

            //Thumb
            // ┗Grid
            //      ┗RectangleGeometry





            ControlTemplate template = new();
            template.VisualTree = new(typeof(Grid), "rootGrid");
            this.Template = template;
            ApplyTemplate();
            MyRootPanel = template.FindName("rootGrid", this) as Grid;


            MyRootPanel.Children.Add(MyRectangle);
            MyRectangle.Fill = Brushes.Red;
            MyRectangle.Width = 100;
            MyRectangle.Height = 100;

            this.DragDelta += SelectThumb_DragDelta;

            BindingOperations.SetBinding(this, Canvas.LeftProperty, MakeBind("Left"));
            BindingOperations.SetBinding(this, Canvas.TopProperty, MakeBind("Top"));
        }

        private void SelectThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }
        private Binding MakeBind(string path)
        {
            Binding b = new();
            b.Source = this;
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath(path);
            return b;
        }
        //----------------------------------------------------------------------------------------

    }

    public class FlatThumb : Thumb, System.ComponentModel.INotifyPropertyChanged
    {
        public Canvas MyRootPanel;
        private double left;
        private double top;
        private double right;
        private double bottom;
        private double myWidth;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double Left { get => left; set { left = value; OnPropertyChanged(); } }
        public double Top { get => top; set { top = value; OnPropertyChanged(); } }
        public double Right { get => right; set { right = value; OnPropertyChanged(); } }
        public double Bottom { get => bottom; set { bottom = value; OnPropertyChanged(); } }
        public double MyWidth { get => myWidth; set { myWidth = value; OnPropertyChanged(); } }

        public FlatThumb()
        {
            SnapsToDevicePixels = true;
            ControlTemplate template = new();
            template.VisualTree = new(typeof(Canvas), "rootPanel");
            this.Template = template;
            ApplyTemplate();
            MyRootPanel = template.FindName("rootPanel", this) as Canvas;
            MyWidth = 100; Height = 100; MyRootPanel.Background = Brushes.Red;

            this.DragDelta += SelectThumb_DragDelta;

            BindingOperations.SetBinding(this, Canvas.LeftProperty, MakeBind("Left"));
            BindingOperations.SetBinding(this, Canvas.TopProperty, MakeBind("Top"));
            BindingOperations.SetBinding(this, WidthProperty, MakeBind("MyWidth"));
        }
        protected Binding MakeBind(string path)
        {
            Binding b = new();
            b.Source = this;
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath(path);
            return b;
        }
        public void SelectThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }
    }

    public class FlatThumb2 : FlatThumb
    {
        public FlatThumb TopLeft;
        public FlatThumb LeftT;
        public FlatThumb TRight = new() { Width = 10, Height = 10 };
        public FlatThumb2()
        {
            TopLeft = new() { Left = Left, Top = Top, Width = 10, Height = 10 };
            TopLeft.MyRootPanel.Background = Brushes.Black;
            MyRootPanel.Children.Add(TopLeft);
            TRight.MyRootPanel.Background = Brushes.Black;
            MyRootPanel.Children.Add(TRight);
            MyRootPanel.Background = Brushes.AliceBlue;

            this.DragDelta -= SelectThumb_DragDelta;

            Binding b1 = new();
            b1.Source = this;
            b1.Path = new PropertyPath(Canvas.LeftProperty);
            b1.Mode = BindingMode.TwoWay;
            Binding b2 = new();
            b2.Source = this;
            b2.Path = new PropertyPath(WidthProperty);
            b2.Mode = BindingMode.TwoWay;
            MultiBinding mb = new();
            mb.Converter = new MyConverter();
            mb.Bindings.Add(b1);
            mb.Bindings.Add(b2);
            mb.Mode = BindingMode.TwoWay;

            TRight.SetBinding(Canvas.LeftProperty, mb);
        }

    }

    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int left = (int)values[0];
            double width = (double)values[1];
            return left + width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            //double right = (double)value;
            //object[] obj = new object[1];
            //obj[0] = 20;
            //obj[1] =(double)30;
            //return obj;
        }
    }
}
