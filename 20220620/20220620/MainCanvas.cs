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
using System.Xml;

namespace _20220620
{
    public class MainCanvas : Canvas
    {
        public ObservableCollection<Layer1> MyLayers = new();
        public Layer1? MyCurrentLayer;

        #region コンストラクタ
        public MainCanvas()
        {
            MyLayers.CollectionChanged += MyLayers_CollectionChanged;
        }
        
        public MainCanvas(Layer1 layer) : this()
        {
            MyLayers.Add(layer);
        }
        #endregion コンストラクタ

        private void MyLayers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?[0] is Layer1 layer)
                {
                    MyCurrentLayer = layer;
                }
            }
        }

      
    }
}
