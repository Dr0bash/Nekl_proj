namespace Nekl_proj
{
    public class Point2D
    {
        public readonly double X;
        public readonly double Y;

        public Point2D(double x, double y) { X = x; Y = y; }

        // Находит точку на пересечнии прямой, образованной p1 и p2, и прямой y = y0
        public static Point2D LineXForY(Point2D p1, Point2D p2, double y0)
            => new Point2D(((p2.X - p1.X) * y0 + p1.X * p2.Y - p2.X * p1.Y) / (p2.Y - p1.Y), y0);

        // Находит точку пересечения линии, заданной p11 и p12, и линии, заданной p21 и p22
        public static Point2D LinesIntersection(Point2D p11, Point2D p12, Point2D p21, Point2D p22)
        {
            var a1 = p12.Y - p11.Y;
            var b1 = p11.X - p12.X;
            var c1 = -a1 * p11.X - b1 * p11.Y;
            
            var a2 = p22.Y - p21.Y;
            var b2 = p21.X - p22.X;
            var c2 = -a2 * p21.X - b2 * p21.Y;
            
            var z1 = a1 * b2 - a2 * b1;
            var z2 = a1 * b2 - a2 * b1;
            
            if (z1 != 0 && z2 != 0)
                return new Point2D(-(c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1), -(a1 * c2 - a2 * c1) / (a1 * b2 - a2 * b1));

            return null;
        }
    }

    class LogicTrapeze
    {
        public readonly double BottomLeft;
        public readonly double BottomRight;
        public readonly double TopLeft;
        public readonly double TopRight;

        public LogicTrapeze(double bottomLeft, double topLeft, double topRight, double bottomRight)
        {
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            TopLeft = topLeft;
            TopRight = topRight;
        }
        
        //  
        public Trapeze Cut(double  affiliationDegree)
            => new Trapeze(
                new Point2D(BottomLeft, 0),
                new Point2D(BottomLeft + (TopLeft - BottomLeft) * affiliationDegree, affiliationDegree),
                new Point2D(BottomRight - (BottomRight - TopRight) * affiliationDegree, affiliationDegree),
                new Point2D(BottomRight, 0)
            );
    }
    
    class Trapeze
    {
        public readonly Point2D BottomLeft;
        public readonly Point2D BottomRight;
        public readonly Point2D TopLeft;
        public readonly Point2D TopRight;

        public Trapeze(Point2D bottomLeft, Point2D topLeft, Point2D topRight, Point2D bottomRight)
        {
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            TopLeft = topLeft;
            TopRight = topRight;
        }
    }
}