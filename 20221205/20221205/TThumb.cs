using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace _20221205
{
    internal class TThumb : Thumb
    {

        public TThumb(Data data)
        {
            SetLocate(data.X, data.Y);
            //this.Template = MakeTemplate(data.DataType);
            switch (data.DataType)
            {
                case DataType.None:
                    break;
                case DataType.TextBlock:
                    break;
                default:
                    break;
            }
            switch (data.Type)
            {
                case nameof(TextBlock):
                    break;
                default:
                    break;
            }
        }
        private void SetLocate(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }



    }
    public abstract class Product : Thumb { }
    public class IdTextblock : Product
    {
        public Data Data { get; private set; }
        internal IdTextblock(Data data)
        {
            this.Data = data;
        }
    }
    public abstract class Factory
    {
        public Product Create(Data data)
        {
            Product p = CreateProduct(data);
            RegisterProduct(p);
            return p;
        }
        protected abstract Product CreateProduct(Data data);
        protected abstract void RegisterProduct(Product product);
    }
    public class IdFactory : Factory
    {
        protected override Product CreateProduct(Data data)
        {
            
        }

        protected override void RegisterProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
