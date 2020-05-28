using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nekl_proj
{
    public class Physics
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

        public static PointF WindChangeByContainers(int[,] topContsHeights, PointF wind, Point contGridLoc)
        {
            PointF answer = new PointF();
            double x = wind.X;
            double y = wind.Y;
            int cX = contGridLoc.X;
            int cY = contGridLoc.Y;
            int ymax = 3;
            int xmax = 5;
            double contLocZ = Form1.ContainerLocation.z;
            int contHeight = Form1.PcontHeight;
            int dir = -1; // 0-n, 1-ne, 2-e, 3-se, 4-s, 5-sw, 6-w, 7-nw
            if (y >= 2 * x && y >= -2 * x) dir = 0;
            else if (y <= 2 * x && y >= 0.5 * x) dir = 1;
            else if (y <= 0.5 * x && y >= -0.5 * x) dir = 2;
            else if (y <= -0.5 * x && y >= -2 * x) dir = 3;
            else if (y <= -2 * x && y <= 2 * x) dir = 4;
            else if (y >= 2 * x && y <= 0.5 * x) dir = 5;
            else if (y >= 0.5 * x && y <= -0.5 * x) dir = 6;
            else if (y <= -2 * x && y >= -0.5 * x) dir = 7;

            switch (dir)
            {
                case 0:
                    if (cY < ymax)
                        if (contLocZ > topContsHeights[cX,cY+1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1,Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX, cY + 1])/contHeight))));
                        }
                    break;
                case 1:
                    if (cY < ymax && cX > 0)
                    {
                        if (contLocZ > topContsHeights[cX - 1, cY + 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX - 1, cY + 1]) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX - 1, cY + 1]) / contHeight))));
                        }
                        if (contLocZ > topContsHeights[cX, cY + 1] && contLocZ > topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY + 1], topContsHeights[cX - 1, cY])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY + 1], topContsHeights[cX - 1, cY])) / contHeight))));
                        }
                    }
                    if (cY < ymax)
                        if (contLocZ > topContsHeights[cX, cY + 1] && contLocZ <= topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX, cY + 1]) / contHeight))));
                        }
                    if (cX > 0)
                        if (contLocZ > topContsHeights[cX - 1, cY] && contLocZ <= topContsHeights[cX, cY + 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX - 1, cY]) / contHeight))));
                        }
                    break;
                case 2:
                    if (cX > 0)
                        if (contLocZ > topContsHeights[cX - 1, cY])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX - 1, cY]) / contHeight))));
                        }
                    break;
                case 3:
                    if (cY > 0 && cX > 0)
                    {
                        if (contLocZ > topContsHeights[cX - 1, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX - 1, cY - 1]) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX - 1, cY - 1]) / contHeight))));
                        }
                        if (contLocZ > topContsHeights[cX, cY - 1] && contLocZ > topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY - 1], topContsHeights[cX - 1, cY])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY - 1], topContsHeights[cX - 1, cY])) / contHeight))));
                        }
                    }
                    if (cY > 0)
                        if (contLocZ > topContsHeights[cX, cY - 1] && contLocZ <= topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX, cY - 1]) / contHeight))));
                        }
                    if (cX > 0)
                        if (contLocZ > topContsHeights[cX - 1, cY] && contLocZ <= topContsHeights[cX, cY - 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX - 1, cY]) / contHeight))));
                        }
                    break;
                case 4:
                    if (cY > 0)
                        if (contLocZ > topContsHeights[cX, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX, cY - 1]) / contHeight))));
                        }
                    break;
                case 5:
                    if (cY > 0 && cX < xmax)
                    {
                        if (contLocZ > topContsHeights[cX + 1, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX + 1, cY - 1]) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX + 1, cY - 1]) / contHeight))));
                        }
                        if (contLocZ > topContsHeights[cX, cY - 1] && contLocZ > topContsHeights[cX + 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY - 1], topContsHeights[cX + 1, cY])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY - 1], topContsHeights[cX + 1, cY])) / contHeight))));
                        }
                    }
                    if (cY > 0)
                        if (contLocZ > topContsHeights[cX, cY - 1] && contLocZ <= topContsHeights[cX + 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX, cY - 1]) / contHeight))));
                        }
                    if (cX < xmax)
                        if (contLocZ > topContsHeights[cX + 1, cY] && contLocZ <= topContsHeights[cX, cY - 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX + 1, cY]) / contHeight))));
                        }
                    break;
                case 6:
                    if (cX < xmax)
                        if (contLocZ > topContsHeights[cX + 1, cY])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX + 1, cY]) / contHeight))));
                        }
                    break;
                case 7:
                    if (cY < ymax && cX < xmax)
                    {
                        if (contLocZ > topContsHeights[cX + 1, cY + 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX + 1, cY + 1]) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX + 1, cY + 1]) / contHeight))));
                        }
                        if (contLocZ > topContsHeights[cX, cY + 1] && contLocZ > topContsHeights[cX + 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY + 1], topContsHeights[cX + 1, cY])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + Math.Max(topContsHeights[cX, cY + 1], topContsHeights[cX + 1, cY])) / contHeight))));
                        }
                    }
                    if (cY < ymax)
                        if (contLocZ > topContsHeights[cX, cY + 1] && contLocZ <= topContsHeights[cX + 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX, cY + 1]) / contHeight))));
                        }
                    if (cX < xmax)
                        if (contLocZ > topContsHeights[cX + 1, cY] && contLocZ <= topContsHeights[cX, cY + 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((contHeight - contLocZ + topContsHeights[cX + 1, cY]) / contHeight))));
                        }
                    break;
                default:
                    throw new Exception("what");
            }
            
            return answer;
        }
    }
}
