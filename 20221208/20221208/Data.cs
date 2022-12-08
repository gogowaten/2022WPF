using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _20221208
{
   public class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Z { get; set; }
        public string Text { get; set; } = "";
        public Brush? ForeColor { get; set; }
        public Brush? BackColor { get; set; }
        public double FontSize { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
