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
    //public class MainCanvas : Canvas
    //{


    //}
    public class MainItemsControl : ItemsControl
    {
        public ObservableCollection<Layer1> MyLayers { get; set; } = new();
        public Layer1 MyCurrentLayer;
        //public Group1Base? MyCurrentGroup;
        public Group1Base MyEditingGroup;//編集状態Group、中のThumbが移動可能状態
        public TThumb1? MyActiveMovableThumb;//移動対象Thumb
        public Item4? MyCurrentItem;//直前にクリックされたThumb

        #region コンストラクタ
        public MainItemsControl()
        {
            DataContext = this;
            SetTemplate();
            MyLayers.CollectionChanged += MyLayers_CollectionChanged;
            Layer1 l = new(this, new Data1(DataType.Layer));
            MyLayers.Add(l);
            MyCurrentLayer = l;
            MyEditingGroup = l;
        }

        public MainItemsControl(Layer1 layer) : this()
        {
            MyLayers.Add(layer);
            MyCurrentLayer = layer;
            MyEditingGroup = layer;

        }
        private void SetTemplate()
        {
            //FrameworkElementFactory panel = new(typeof(Canvas));
            ItemsPanelTemplate panelTemplate = new(new(typeof(Canvas)));
            this.SetBinding(ItemsSourceProperty, new Binding(nameof(MyLayers)));
            this.ItemsPanel = panelTemplate;
        }

        #endregion コンストラクタ

        private void MyLayers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //if (e.NewItems?[0] is Layer1 layer)
                //{
                //    MyCurrentLayer = layer;
                //}
            }
        }

        public void AddItem(Data1 data)
        {
            if (data.DataTypeMain == DataTypeMain.Item)
            {
                Item4 item = new(this, data);
                MyEditingGroup.AddThumb(item);
            }
            else
            {

            }
        }
    }
}
