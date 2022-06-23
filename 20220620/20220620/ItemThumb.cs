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
            PreviewMouseDown += Item4_PreviewMouseDown;
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
            MyMainItemsControl.MyCurrentItem = this;
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

        //クリックされたとき
        private void Item4_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ////CurrentItemを自身に変更
            //MyMainItemsControl.MyCurrentItem = this;
            ////選択リストの更新
            ////通常クリックなら、リストをクリアしてから自身を追加
            ////ctrlキー押しながらのクリックされた場合
            ////    自身がリストにない場合は、自身を追加
            ////    リストにある場合は、リストから自身を削除
            //if (Keyboard.Modifiers == ModifierKeys.None)
            //{
            //    MyMainItemsControl.MySelectedThumbs.Clear();
            //    MyMainItemsControl.MySelectedThumbs.Add(this.GetMyActiveThumb());
            //}
            //else if (Keyboard.Modifiers == ModifierKeys.Control)
            //{
            //    MyMainItemsControl.MySelectedThumbs.Add(this.GetMyActiveThumb());
            //}

        }
        //  //クリックされたとき
        //private void Item4_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    if (MyLayer == null) { return; }
        //    //前回と同じThumbをクリックされたのかチェック
        //    bool isEqual = false;
        //    if (MyLayer.LastClickedItem == this) isEqual = true;
        //    //最後にクリックされたThumbに自身を登録する
        //    MyLayer.LastClickedItem = this;

        //    //編集状態直下の自身が属するグループ
        //    //TThumb1 activeThumb = (TThumb1?)GetMyActiveMoveThumb2() ?? this;
        //    TThumb1 activeThumb = (TThumb1?)GetMyActiveMoveThumb() ?? this;
        //    //MyActiveMovableThumb = activeThumb;
        //    MyMainItemsControl.MyActiveMovableThumb = activeThumb;
        //    //自身が編集状態Thumbの範囲外だった場合

        //    if (MyLayer.NowEditingThumb != activeThumb.MyParentGroup)
        //    {
        //        //編集状態Thumbの切り替え
        //        //Layer直下のThumb群から自身が属するThumbに切り替える
        //        Group1Base? nextEdit = GetMyUnderLayerThumb(this) as Group1Base;
        //        if (nextEdit == null) { MessageBox.Show("次の編集状態Thumbが見つからん"); }
        //        //MyLayer.NowEditingThumb = nextEdit;
        //        MyLayer.SetNowEditingThumb(nextEdit, this);
        //    }
        //    else
        //    {
        //        //編集状態直下の自身が属するグループを選択状態リストに登録する
        //        //通常クリック(修飾キーなし)のときは入れ替え
        //        if (Keyboard.Modifiers == ModifierKeys.None)
        //        {
        //            //違うThumbクリックなら選択リストの入れ替え
        //            if (isEqual == false)
        //            {
        //                MyLayer.SelectThumbReplace(activeThumb);
        //            }
        //        }
        //        //ctrlキーが押されていたら複数選択状態にするので、選択リストに追加
        //        else if (Keyboard.Modifiers == ModifierKeys.Control)
        //        {
        //            //すでに選択中だった場合は選択解除
        //            if (MyLayer.SelectedThumbs.Contains(activeThumb))
        //            {
        //                MyLayer.SelectThumbRemove(activeThumb);
        //            }
        //            else
        //            {
        //                MyLayer.SelectThumbAdd(activeThumb);
        //            }
        //        }
        //    }
        //}

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
