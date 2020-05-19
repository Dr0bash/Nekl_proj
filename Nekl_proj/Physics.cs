using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nekl_proj
{
    class Physics
    {
        public struct Point3D
        {
            public double x;
            public double y;
            public double z;
            public Point3D(double xx, double yy, double zz)
            {
                x = xx;
                y = yy;
                z = zz;
            }
        }
        public static Point3D Container_Movement(PointF wind, double mass, double rope_len)
        {
            Point3D answer = new Point3D(wind.X,wind.Y,mass);

            double vec_len = Math.Sqrt(Math.Pow(wind.X,2) + Math.Pow(wind.Y, 2) + Math.Pow(mass,2));

            answer.x = answer.x / vec_len * rope_len;
            answer.y = answer.y / vec_len * rope_len;
            answer.z = answer.z / vec_len * rope_len;

            return answer;
        }
        public static PointF Wind_to_destination(PointF wind_now, PointF speed)
        {
            PointF answer = new PointF();

            answer.X = wind_now.X + speed.X;
            answer.Y = wind_now.Y + speed.Y;

            return answer;
        }
    }
}
