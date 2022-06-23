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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace _20220620
{
    internal class ItemThumb
    {
    }

    public class Item4 : TThumb1
    {
        public Canvas MyTemplateCanvas;
        public FrameworkElement MyItemElement;
        private bool _IsMyLastClicked;//最後にクリックされたフラグ
        public bool IsMyLastClicked
        {
            get => _IsMyLastClicked;
            set
            {
                if (_IsMyLastClicked == value) { return; }
                _IsMyLastClicked = value;
                OnPropertyChanged();
            }
        }
        public Item4(MainItemsControl main, Data1 data) : base(main, data)
        {
            DataContext = MyData;
            MyTemplateCanvas = InitializeTemplate();
            //表示する要素をDataから作成
            MyItemElement = MakeElement(data);
            MyTemplateCanvas.Children.Add(MyItemElement);
            //サイズのBinding
            //表示する要素のサイズに自身を合わせる
            Binding b = new() { Source = MyItemElement, Path = new PropertyPath(ActualWidthProperty) };
            MyTemplateCanvas.SetBinding(WidthProperty, b);
            this.SetBinding(WidthProperty, b);
            b = new() { Source = MyItemElement, Path = new PropertyPath(ActualHeightProperty) };
            MyTemplateCanvas.SetBinding(HeightProperty, b);
            this.SetBinding(HeightProperty, b);



            Loaded += Item4_Loaded;
            
            PreviewMouseLeftButtonDown += Item4_PreviewMouseLeftButtonDown;
            PreviewMouseUp += Item4_PreviewMouseUp;

        }



        #region イベント

        //表示された直後にサイズが決まるのでParentのサイズを修正する
        private void Item4_Loaded(object sender, RoutedEventArgs e)
        {
            MyParentGroup?.AjustLocate3();
        }
        //クリックでボタンが離されたときされたとき
        private void Item4_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //編集状態Thumbの切り替え

        }
        //左クリック時、クリックitem更新と選択リスト更新
        private void Item4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MyMainItemsControl.MyCurrentItem != this)
            {
                //別系統のitemがクリックされた場合は、EditingをLayerにリセットする
                if( MyMainItemsControl.IsSameSystem(this, MyMainItemsControl.MyCurrentItem) == false)
                {
                    MyMainItemsControl.EditingSetMyCurrentLayer();
                }

                //if (MyMainItemsControl.IsInEditingGroup(this) == false)
                //{
                //    MyMainItemsControl.EditingSetMyCurrentLayer();
                //}
                MyMainItemsControl.MyCurrentItem = this;
                
            }

            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                MyMainItemsControl.MySelectedThumbs.Clear();
                MyMainItemsControl.MySelectedThumbs.Add(this.GetMyActiveThumb());
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                MyMainItemsControl.MySelectedThumbs.Add(this.GetMyActiveThumb());
            }

        }

     

        #endregion イベント

        //単一要素表示用テンプレートに書き換える
        private Canvas InitializeTemplate()
        {
            //共通枠
            Binding b1 = new(nameof(IsMyLastClicked)) { Source = this };
            Binding b2 = new(nameof(IsMySelected)) { Source = this };
            MultiBinding mb = new();
            mb.Bindings.Add(b1); mb.Bindings.Add(b2);
            mb.Converter = new MyConverterItemWaku1();
            List<Brush> brushes = new()
            {
                My2ColorDashBrush(4,Colors.Red,Colors.White),
                My2ColorDashBrush(4,Colors.DodgerBlue,Colors.MintCream),
                My2ColorDashBrush(4,Colors.LightGray,Colors.White)
            };
            mb.ConverterParameter = brushes;
            FrameworkElementFactory waku = MakeWaku1(DataTypeMain.Item);
            waku.SetBinding(Rectangle.StrokeProperty, mb);
            FrameworkElementFactory baseCanvas = new(typeof(Canvas), nameof(MyTemplateCanvas));
            baseCanvas.AppendChild(waku);
            waku.SetValue(Panel.ZIndexProperty, 1);//枠表示前面

            ControlTemplate template = new();
            template.VisualTree = baseCanvas;

            this.Template = template;
            this.ApplyTemplate();
            return (Canvas)template.FindName(nameof(MyTemplateCanvas), this);

        }


        //表示する要素をDataから作成
        private FrameworkElement MakeElement(Data1 data)
        {
            FrameworkElement? element = null;
            switch (data.DataType)
            {
                case DataType.None:
                    break;
                case DataType.Layer:
                    break;
                case DataType.Group:
                    break;
                case DataType.TextBlock:
                    element = new TextBlock() { FontSize = 20 };
                    element.SetBinding(TextBlock.TextProperty, new Binding(nameof(MyData.Text)));
                    element.SetBinding(TextBlock.BackgroundProperty, new Binding(nameof(MyData.Background)));
                    element.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(MyData.Foreground)));
                    element.SetBinding(TextBlock.PaddingProperty, new Binding(nameof(MyData.Padding)));
                    break;
                case DataType.Path:
                    break;
                case DataType.Image:
                    break;
                default:
                    break;
            }

            return element ?? throw new ArgumentNullException($"{nameof(element)}", $"dataから要素が作れんかった");
        }


    }

}
