using System.Collections.Generic;

namespace Nekl_proj
{
    class LogicGraph
    {
        private readonly List<LogicTrapeze> _list;

        public LogicGraph(int count)
        {
            _list = new List<LogicTrapeze>(new LogicTrapeze[count]);
        }

        public void AddLogicTrapeze(int type, LogicTrapeze trapeze)
        {
            _list[type] = trapeze;
        }

        public List<double> GetDistribution(double x)
        {
            var res = new List<double>(new double[_list.Count]);
            for (var i = 0; i < _list.Count; ++i)
            {
                var c = _list[i];
                if (x < c.BottomLeft || c.BottomRight < x)
                {
                    res[i] = 0;
                    continue;
                }

                if (c.TopLeft <= x && x <= c.TopRight)
                {
                    res[i] = 1;
                    continue;
                }

                if (c.BottomLeft <= x && x < c.TopLeft)
                {
                    res[i] = (x - c.BottomLeft) / (c.TopLeft - c.BottomLeft);
                    continue;
                }

                res[i] = 1 - (x - c.TopRight) / (c.BottomRight - c.TopRight);
            }

            return res;
        }

        public List<Point2D> GetPolygon(List<double> distribution)
        {
            var res = new List<Point2D>();
            if (distribution.Count != _list.Count)
                return null;

            var j = 0;
            while (distribution[j] == 0)
                j++;

            var t1 = _list[j].Cut(distribution[j]);
            res.Add(t1.BottomLeft);
            res.Add(t1.TopLeft);

            for (var i = j + 1; i < _list.Count; i++)
            {
                if (distribution[i] == 0)
                    continue;

                var t2 = _list[i].Cut(distribution[i]);
                var intersection = Point2D.LinesIntersection(t1.TopRight, t1.BottomRight, t2.BottomLeft, t2.TopLeft);
                if (intersection != null)
                {
                    if (t1.TopRight.Y >= intersection.Y)
                    {
                        res.Add(t1.TopRight);
                        if (t2.TopLeft.Y > intersection.Y)
                        {
                            res.Add(t2.TopLeft);
                            res.Add(intersection);
                        }
                        else
                        {
                            res.Add(Point2D.LineXForY(t1.TopRight, t1.BottomRight, distribution[i]));
                        }
                    }
                    else
                    {
                        if (t2.TopLeft.Y > intersection.Y)
                        {
                            res.Add(Point2D.LineXForY(t2.BottomLeft, t2.TopLeft, distribution[i - 1]));
                            res.Add(t2.TopLeft);
                        }
                        else
                        {
                            if (t1.TopRight.Y > t2.TopLeft.Y)
                            {
                                res.Add(t1.TopRight);
                                res.Add(Point2D.LineXForY(t1.TopRight, t1.BottomRight, distribution[i]));
                            }
                            else if (t1.TopRight.Y < t2.TopLeft.Y)
                            {
                                res.Add(t2.TopLeft);
                                res.Add(Point2D.LineXForY(t2.BottomLeft, t2.TopLeft, distribution[i - 1]));
                            }
                        }
                    }
                }

                t1 = t2;
            }

            res.Add(t1.TopRight);
            res.Add(t1.BottomRight);

            return res;
        }
    }
}