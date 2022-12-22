using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

//参照したところ
//c# - WPF:キャンバスのサイズを変更して子を含める
//https://stackoverflow.com/questions/55101074/wpf-sizing-canvas-to-contain-its-children
//ArrangeOverride と MeasureOverride
//http://www.kanazawa-net.ne.jp/~pmansato/wpf/wpf_ctrl_arrange.htm

namespace _20221221_子要素のサイズと位置でサイズ更新するCanvas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MyExCanvas.Width = 200; MyExCanvas.Height = 100;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Width = 200; MyCanvas.Height = 100;
        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            MyExCanvas.Width = double.NaN; MyExCanvas.Height = double.NaN;
        }

        private void Button21_Click(object sender, RoutedEventArgs e)
        {
            
            MyCanvas.Width = double.NaN; MyCanvas.Height = double.NaN;
        }
    }

    public class ExCanvas : Canvas
    {

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    if (double.IsNaN(Width) && double.IsNaN(Height))
        //    {
        //        base.MeasureOverride(constraint);//これを実行してから計算
        //        Size size = new();
        //        foreach (var item in Children.OfType<FrameworkElement>())
        //        {
        //            double x = GetLeft(item) + item.DesiredSize.Width;//ActualWidthは更新されていない
        //            double y = GetTop(item) + item.DesiredSize.Height;
        //            if (size.Width < x) size.Width = x;
        //            if (size.Height < y) size.Height = y;
        //        }
        //        return size;
        //    }
        //    else { return base.MeasureOverride(constraint); }
        //}

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);
            Size size = new();
            foreach (var item in Children.OfType<FrameworkElement>())
            {
                double x = GetLeft(item) + item.DesiredSize.Width;//ActualWidthは更新されていない
                double y = GetTop(item) + item.DesiredSize.Height;
                if (size.Width < x) size.Width = x;
                if (size.Height < y) size.Height = y;
            }
            return size;
            //return base.ArrangeOverride(arrangeSize);

        }

    }


    ///// <summary>
    ///// 子要素のサイズor位置変更で自身のサイズ(Actual)を自動変更するCanvas
    ///// もしWidthやHeightを指定されていた場合は自動更新しない
    ///// </summary>
    //public class ExCanvas : Canvas
    //{
    //    //MeasureOverrideは
    //    //子要素のサイズ変更で実行される
    //    //子要素の位置変更では実行されない
    //    //CanvasのActualサイズが更新される(よくやった！)
    //    protected override Size MeasureOverride(Size constraint)
    //    {
    //        if (double.IsNaN(Width) && double.IsNaN(Height))
    //        {
    //            base.MeasureOverride(constraint);
    //            double w = 0; double h = 0;
    //            foreach (var item in Children.OfType<FrameworkElement>())
    //            {
    //                var desiredW = GetLeft(item) + item.DesiredSize.Width;
    //                if (desiredW > w) w = desiredW;
    //                var desiredH = GetTop(item) + item.DesiredSize.Height;
    //                if (desiredH > h) h = desiredH;
    //            }
    //            return new Size(w, h);
    //        }
    //        else return base.MeasureOverride(constraint);

    //    }

    //    //ArrangeOverrideは
    //    //子要素のサイズ変更で実行される
    //    //子要素の位置変更でも実行される(よくやった！)
    //    //CanvasのActualサイズは更新されない、のでMeasureを実行する
    //    protected override Size ArrangeOverride(Size arrangeSize)
    //    {

    //        if (double.IsNaN(Width) && double.IsNaN(Height))
    //        {
    //            base.ArrangeOverride(arrangeSize);
    //            //子要素全体が収まるサイズを計算
    //            //子要素の幅サイズ取得にWidthやActualWidthを使わずにDesiredSize.Widthを使っているのは、
    //            //例えばTextBlockのようにフォントサイズなどの変更でサイズが自動変更される要素の
    //            //Widthは常にNaNだし、
    //            //ActualWidthはフォントサイズ変更前の値
    //            //DesiredSizeはフォントサイズ変更適用済みの値
    //            double w = 0; double h = 0;
    //            foreach (var item in Children.OfType<FrameworkElement>())
    //            {
    //                var desiredW = GetLeft(item) + item.DesiredSize.Width;
    //                if (desiredW > w) w = desiredW;
    //                var desiredH = GetTop(item) + item.DesiredSize.Height;
    //                if (desiredH > h) h = desiredH;
    //            }
    //            Size size = new(w, h);
    //            //差異があればサイズ更新(Measure実行)
    //            if (arrangeSize.Width != w || arrangeSize.Height != h)
    //            {
    //                Measure(size);
    //            }
    //            return size;
    //        }
    //        else
    //        {
    //            return base.ArrangeOverride(arrangeSize);
    //        }

    //        //base.ArrangeOverride(arrangeSize);
    //        ////子要素全体が収まるサイズを計算
    //        ////子要素の幅サイズ取得にWidthやActualWidthを使わずにDesiredSize.Widthを使っているのは、
    //        ////例えばTextBlockのようにフォントサイズなどの変更でサイズが自動変更される要素の
    //        ////Widthは常にNaNだし、
    //        ////ActualWidthはフォントサイズ変更前の値
    //        ////DesiredSizeはフォントサイズ変更適用済みの値
    //        //double w = 0; double h = 0;
    //        //foreach (var item in Children.OfType<FrameworkElement>())
    //        //{
    //        //    var desiredW = GetLeft(item) + item.DesiredSize.Width;
    //        //    if (desiredW > w) w = desiredW;
    //        //    var desiredH = GetTop(item) + item.DesiredSize.Height;
    //        //    if (desiredH > h) h = desiredH;
    //        //}
    //        //Size size = new(w, h);
    //        ////差異があればサイズ更新(Measure実行)
    //        //if (arrangeSize.Width != w || arrangeSize.Height != h)
    //        //{
    //        //    Measure(size);
    //        //}
    //        //return size;

    //    }

    //}



}
