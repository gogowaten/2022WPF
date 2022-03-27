using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace _20220325_クリックイベント順番
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test();
        }
        private void Test()
        {
            MyTT.AddElement(MakeLabel("t 1",Brushes.Gray), 10, 10);

            TThumb thumb = new("text1");
            MyTT.AddElement(thumb, 70, 20);
            thumb.Width = 50; thumb.Height = 50;thumb.Background = Brushes.Silver;
            thumb.AddElement(MakeLabel("text", Brushes.Gray), 10, 10);

            thumb = new("text2");
            MyTT.AddElement(thumb, 170, 120);
            thumb.Width = 50; thumb.Height = 50; thumb.Background = Brushes.Silver;
            thumb.AddElement(MakeLabel("text", Brushes.Gray), 10, 10);
        }
        private Label MakeLabel(string text, Brush brush)
        {
            Label label = new() { Content = text, Background = brush, Padding = new Thickness(10) };            
            //label.PreviewMouseLeftButtonDown += Label_PreviewMouseLeftButtonDown;
            return label;
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender;
            var source = e.Source;
            var origin = e.OriginalSource;
        }


        private void Label_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var source = e.Source;
            var origin = e.OriginalSource;
        }
    }

    public class TThumb : System.Windows.Controls.Primitives.Thumb, System.ComponentModel.INotifyPropertyChanged
    {
        private string PANEL_NAME = "panel";
        private Canvas MyCanvas;
        private Brush bGColor;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Brush BGColor
        {
            get => bGColor;
            set
            {
                if(value != bGColor)
                {
                    bGColor = value;
                    //MyCanvas.Background = bGColor;
                }
                

            }
        }

        public TThumb()
        {
            ControlTemplate template = new(typeof(TThumb));
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), PANEL_NAME);
            this.Template = template;
            this.ApplyTemplate();
            MyCanvas = (Canvas)template.FindName(PANEL_NAME, this);

            Binding b = new();
            b.Source = this;
            b.Path = new PropertyPath(BackgroundProperty);
            MyCanvas.SetBinding(Canvas.BackgroundProperty, b);

            this.PreviewMouseLeftButtonUp += TThumb_PreviewMouseLeftButtonUp;
        }
        public TThumb(string name) : this()
        {

            this.Name = name;
        }

        private void TThumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender;
            var source = e.Source;
            var origin = e.OriginalSource;
        }

        public void AddElement(UIElement element, int left, int top)
        {
            MyCanvas.Children.Add(element);
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
        }
        public override string ToString()
        {
            //return base.ToString();
            return base.Name.ToString();
        }
    }
}
