namespace Nekl_proj
{
    public class Point2D
    {
        public readonly double X;
        public readonly double Y;

        public Point2D(double x, double y) { X = x; Y = y; }

        // TODO: координата точки на прямой по У
        public static Point2D LineXForY(Point2D p1, Point2D p2, double y)
        {
            return null;
        }

        // TODO: точка пересечения прямых заданных концами
        public static Point2D Intersection(Point2D p11, Point2D p12, Point2D p21, Point2D p22)
        {
            return null; 
        }
    }

    class LogicTrapeze
    {
        public double bottomLeft, bottomRight;
        public double topLeft, topRight;

        public LogicTrapeze(double bottomLeft, double topLeft, double topRight, double bottomRight)
        {
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.topLeft = topLeft;
            this.topRight = topRight;
        }
    }
}