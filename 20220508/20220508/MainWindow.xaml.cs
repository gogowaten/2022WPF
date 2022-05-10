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

namespace _20220508
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
           
          

            #region T1
            TThumb1 thumb1 = new TThumb1();
            thumb1.Text = "serialTest";
            thumb1.X = 100;
            thumb1.Y = 100;
            thumb1.Geometry = new RectangleGeometry(new Rect(0, 20, 10, 40));
            thumb1.DataSave();
            #endregion T1
            T2Layer t2l = new();
            t2l.SetEditing();

        }
    }



}
