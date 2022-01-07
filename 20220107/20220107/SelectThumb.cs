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

            //this.DragDelta += SelectThumb_DragDelta;

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
        //public void SelectThumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    Left += e.HorizontalChange;
        //    Top += e.VerticalChange;
        //}
        public override string ToString()
        {
            //return base.ToString();
            return base.Name;
        }
    }

    public class FlatThumb2 : FlatThumb
    {
        public FlatThumb TRight = new() { Width = 10, Height = 10, Name = "Right" };
        public FlatThumb2()
        {
            MyRootPanel.Background = Brushes.AliceBlue;

            TRight.MyRootPanel.Background = Brushes.Gold;
            MyRootPanel.Children.Add(TRight);
            TRight.DragDelta += TRight_DragDelta;
            //TRight.DragDelta -= TRight.SelectThumb_DragDelta;

            //this.DragDelta -= SelectThumb_DragDelta;

            //Binding b1 = new();
            //b1.Source = this;
            //b1.Path = new PropertyPath(Canvas.LeftProperty);
            //b1.ConverterParameter = TRight;
            //b1.Converter = new ConverterWidth();
            ////b1.Mode = BindingMode.TwoWay;
            //this.SetBinding(WidthProperty, b1);


            //Binding b1 = new();
            //b1.Path = new PropertyPath(Canvas.LeftProperty);
            //b1.Source = this;
            //b1.Mode = BindingMode.TwoWay;

            //Binding b2 = new();
            //b2.Source = this;
            //b2.Path = new PropertyPath(WidthProperty);
            //b2.Mode = BindingMode.TwoWay;

            //MultiBinding mb = new();
            //mb.Converter = new MyConverter();
            //mb.ConverterParameter = this;
            //mb.Bindings.Add(b1);
            //mb.Bindings.Add(b2);
            //mb.Mode = BindingMode.TwoWay;

            //TRight.SetBinding(Canvas.LeftProperty, mb);


            //Binding b = new();
            //b.Source = this;
            //b.Path = new PropertyPath(WidthProperty);
            //b.Mode = BindingMode.TwoWay;
            //TRight.SetBinding(Canvas.LeftProperty, b);


            Binding b = new("Left");
            b.Source = TRight;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new ConverterWidth2();
            this.SetBinding(WidthProperty, b);


        }

        private void TRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //double left = Left + e.HorizontalChange;
            //if (left < 1) { left = 1; }
            //Left = left;
            //double top = Top + e.VerticalChange;
            //if (top < 1) { Top = 1; }
            //Top = top;
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }
    }


    public class ConverterWidth2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double left = (double)value;
            if (left < 0) { left = 0; }
            return left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)value;
            return width;
            //throw new NotImplementedException();
        }
    }
    public class ConverterWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double left = (double)value;
            FlatThumb ft = (FlatThumb)parameter;
            double right = left + ft.Left;
            return right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double left = (double)values[0];
            double width = (double)values[1];
            double right = width;
            //double right = left + width;
            return right;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            double right = (double)value;
            FlatThumb2 ft = (FlatThumb2)parameter;
            //double left = right;
            double width = right - ft.Left;
            object[] obj = new object[1];
            obj[0] = ft.Left;
            obj[1] = width;
            return obj;
        }
    }
}
