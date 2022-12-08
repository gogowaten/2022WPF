using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;


namespace _20221208
{
    class TThumb
    {
    }
    public class TTTextBlock : Thumb
    {
        #region 依存プロパティ

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TTTextBlock), new PropertyMetadata(""));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(nameof(X), typeof(double), typeof(TTTextBlock), new PropertyMetadata(0.0));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(nameof(Y), typeof(double), typeof(TTTextBlock), new PropertyMetadata(0.0));

        public int Z
        {
            get { return (int)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }
        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register(nameof(Z), typeof(int), typeof(TTTextBlock), new PropertyMetadata(0));

        #endregion
        public TTTextBlock()
        {


        }

    }
}
