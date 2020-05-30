using System;

namespace RandomShapeCreate
{
    public enum Type
    {
        Rectangle,
        Square,
        Triangle
    }
    class Program
    {
        static void Main(string[] args)
        {
            int num;
            Type nowType;
            Type[] typeCreated;
            IShape[] shapes;
            double scale, areaSum = 0.0;
            Console.WriteLine("PLS input the num of random shape you want to creat.");
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.WriteLine("Invalid input,do it again.");
            }
            shapes = new IShape[num];
            typeCreated = new Type[num];
            Console.WriteLine("PLS input the lagestnum of your random shape's edge you want to creat.");
            while (!double.TryParse(Console.ReadLine(), out scale))
            {
                Console.WriteLine("Invalid input,do it again.");
            }
            Random rd = new Random();
            for (int i = 0; i < num; i++)
            {
                nowType = (Type)rd.Next(0, 3);
                shapes[i] = ShapeFactory.RandomCreate(nowType, scale);
                typeCreated[i] = nowType;
                areaSum += shapes[i].CalculateArea();
                Console.WriteLine("A " + typeCreated[i].ToString() + " is created and its area is "
                    + shapes[i].CalculateArea());
            }
            Console.WriteLine("Your sharpes' total area is " + areaSum);
        }
    }

    public interface IShape
    {
        public double CalculateArea();
        public bool JudgeValid();
    }

    public class Rectangle : IShape
    {
        protected double height;
        protected double width;

        public Rectangle(double inHeight, double inWidth)
        {
            height = inHeight;
            width = inWidth;
        }
        public double Length
        {
            get
            {
                return height;
            }
        }
        public double Width
        {
            get
            {
                return width;
            }
        }

        public double CalculateArea()
        {
            if (JudgeValid())
            {
                return height * width;
            }
            return -1;

        }
        public bool JudgeValid()
        {
            return height > 0 && width > 0;
        }

    }

    public class Square : IShape
    {
        private double edge;
        public Square(double inEdge)
        {
            edge = inEdge;
        }
        public double Edge
        {
            get
            {
                return edge;
            }
        }
        public bool JudgeValid()
        {
            return edge > 0;
        }
        public double CalculateArea()
        {
            if (JudgeValid())
            {
                return edge * edge;
            }
            return -1;
        }
    }

    public class Triangle : IShape
    {
        protected double[] edge;

        public double[] Edge { get; }

        public Triangle(double first, double second, double third)
        {
            edge = new double[3] { first, second, third };
        }

        public bool JudgeValid()
        {
            return (edge[0] > 0 && edge[1] > 0 && edge[2] > 0)
                && (edge[0] + edge[1] > edge[2])
                && (edge[0] + edge[2] > edge[1])
                && (edge[2] + edge[1] > edge[0]);
        }
        public double CalculateArea()
        {
            if (!JudgeValid())
            {
                return -1;
            }
            double p = (edge[0] + edge[1] + edge[2]) / 2;
            return Math.Sqrt(p * (p - edge[0]) * (p - edge[1]) * (p - edge[2]));
        }
    }


    public class ShapeFactory
    {

        public static IShape RandomCreate(Type type, double scale = 10.0)
        {
            if (scale <= 0)
            {
                return null;
            }
            Random random = new Random();
            IShape temp;
            switch(type)
            {
                case Type.Rectangle:
                    while(!(temp = 
                        new Rectangle(random.NextDouble() * scale, random.NextDouble() * scale)).JudgeValid());
                    return temp;
                case Type.Square:
                    while (!(temp =
                       new Square(random.NextDouble() * scale)).JudgeValid()) ;
                    return temp;
                case Type.Triangle:
                    while (!(temp =
                       new Triangle(random.NextDouble() * scale,
                                random.NextDouble() * scale,
                                random.NextDouble() * scale)).JudgeValid()) ;
                    return temp;
                default:
                    return null;
            }
        }

    }

}
