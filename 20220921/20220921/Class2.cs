using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;


//Composite
//https://refactoring.guru/ja/design-patterns/composite
//より

namespace _20220921
{
    internal class Class2
    {
        public Class2()
        {
            Debug.WriteLine("Class2");
            Circle circle = new(1, 2, 3);
            circle.Draw();
        }
    }

    public interface IGraphic
    {
        void Move(double x, double y);
        void Draw();
    }
    public class Dot : IGraphic
    {
        private double X;
        private double Y;

        public Dot(double x, double y)
        {
            X = x; Y = y;
        }
        public void Draw()
        {
            Debug.WriteLine("Dot描画");
        }

        public void Move(double x, double y)
        {
            this.X += x; this.Y += y;
            Debug.WriteLine($"({x}, {y})に移動");
        }
    }
    public class Circle : Dot
    {
        private double Radius;

        public Circle(double x, double y, double radius) : base(x, y)
        {
            Radius = radius;
        }

        public new void Draw()
        {
            Debug.WriteLine($"半径{Radius}の円を描画");
        }
    }
    public class CompoundGraphic : IGraphic
    {
        private List<IGraphic> children = new();
        public CompoundGraphic() { }
        public void Add(IGraphic graphic) { children.Add(graphic); }
        public void Remove(IGraphic graphic) { children.Remove(graphic); }

        public void Draw()
        {
            foreach (IGraphic item in children) { item.Draw(); }
        }

        public void Move(double x, double y)
        {
            foreach (IGraphic item in children) { item.Move(x, y); }
        }
    }
    public class ImageEditor
    {
        CompoundGraphic All = new();
        public void Test1()
        {
            All.Add(new Dot(1, 2));
            All.Add(new Circle(3, 4, 5));
        }
        public void GroupSelected(List<IGraphic> graphics)
        {

        }
    }
}
