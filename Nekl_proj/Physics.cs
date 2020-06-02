using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

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

        public static double WaveFun1(double t)
        {
            return Math.Sin(t);
        }
        public static double WaveFun2(double t)
        {
            return Math.Sin(2 * t) * Math.Pow(Math.Sin(0.5 * t), 2);
        }
        public static double WaveFun3(double t)
        {
            return (Math.Sin(t) + 0.4*Math.Pow(Math.Sin(2*t),2)) / 2;
        }
        public static double WaveFun4(double t)
        {
            return 1-Math.Abs(Math.Sin(t));
        }


        public static PointF WindChangeByContainers(int[,] topContsHeights, PointF wind, Point contGridLoc)
        {
            PointF finWind = new PointF(wind.X, wind.Y);
            //PointF answer = new PointF();
            double x = wind.X;
            double y = -wind.Y;
            int cX = contGridLoc.X;
            int cY = contGridLoc.Y;
            int xmax = 3 - 1;
            int ymax = 5 - 1;            
            double contLocZ = Form1.ContainerLocation.z;
            double contHeight = Form1.PcontHeight;
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
                    if (cX < xmax)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX+1,cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1,Math.Max(0, ((Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY] - contLocZ) /contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                case 1:
                    if (cX < xmax && cY > 0)
                    {
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY - 1] - contLocZ) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY - 1] - contLocZ) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY] && contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX + 1, cY], topContsHeights[cX, cY - 1])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX + 1, cY], topContsHeights[cX, cY - 1])) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    }
                    if (cY < ymax && cX == 0)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    if (cX > 0 && cY == ymax)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                case 2:
                    if (cY > 0)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                case 3:
                    if (cY > 0 && cX > 0)
                    {
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY - 1]) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY - 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1] && contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX, cY - 1], topContsHeights[cX - 1, cY])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX, cY - 1], topContsHeights[cX - 1, cY])) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    }
                    if (cY > 0 && cX == 0)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY - 1]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    if (cX > 0 && cY == 0)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                case 4:
                    if (cX > 0)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                case 5:
                    if (cX > 0 && cY < ymax)
                    {
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY + 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY + 1]) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY + 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY] && contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX - 1, cY], topContsHeights[cX, cY + 1])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX - 1, cY], topContsHeights[cX, cY + 1])) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    }
                    if (cX > 0 && cY == ymax)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX - 1, cY]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    if (cY < xmax && cX == 0)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                case 6:
                    if (cY < ymax)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((-contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                case 7:
                    if (cY < ymax && cX < xmax)
                    {
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY + 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY + 1]) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY + 1]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1] && contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX, cY + 1], topContsHeights[cX + 1, cY])) / contHeight))));
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * Math.Max(topContsHeights[cX, cY + 1], topContsHeights[cX + 1, cY])) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY] && contLocZ + contHeight <= Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    }
                    if (cY < ymax && cX == xmax)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1])
                        {
                            wind.Y = (float)(wind.Y * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX, cY + 1]) / contHeight))));
                            if (wind.Y == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    if (cX < xmax && cY == ymax)
                        if (contLocZ + contHeight > Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY])
                        {
                            wind.X = (float)(wind.X * Math.Min(1, Math.Max(0, ((- contLocZ + Form1.ShipLeft.Location.Y - contHeight * topContsHeights[cX + 1, cY]) / contHeight))));
                            if (wind.X == 0)
                                Form1.speedAlarm = false;
                            else
                            {
                                Form1.speedAlarm = true;
                                break;
                            }
                        }
                    Form1.speedAlarm = false;
                    break;
                default:
                    throw new Exception("what");
            }
            //if (finWind.X != wind.X || finWind.Y != wind.Y)
            //    Form1.speedAlarm = true;
            //else Form1.speedAlarm = false;
            return wind;
        }
    }
}
