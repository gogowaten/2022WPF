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
        private Grid MyRootPanel;
        private double left;
        private double top;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double Left { get => left; set { left = value; OnPropertyChanged(); } }
        public double Top { get => top; set { top = value; OnPropertyChanged(); } }

        public FlatThumb()
        {
            ControlTemplate template = new();
            template.VisualTree = new(typeof(Grid), "rootGrid");
            this.Template = template;
            ApplyTemplate();
            MyRootPanel = template.FindName("rootGrid", this) as Grid;
            Width = 100; Height = 100; MyRootPanel.Background = Brushes.Red;

            this.DragDelta += SelectThumb_DragDelta;

            BindingOperations.SetBinding(this, Canvas.LeftProperty, MakeBind("Left"));
            BindingOperations.SetBinding(this, Canvas.TopProperty, MakeBind("Top"));
        }
        private Binding MakeBind(string path)
        {
            Binding b = new();
            b.Source = this;
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath(path);
            return b;
        }
        private void SelectThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }
    }

    public class FlatThumb2 : FlatThumb
    {
        public FlatThumb2()
        {

        }

    }

}
