using System;
using System.Collections.Generic;
using System.Linq;

namespace Nekl_proj
{
    // Вызывать как-то так
    // private var logic = new Logic(
    //     WorldCharacteristics.MaxHorizontalCraneSpeed * WorldCharacteristics.TimeDimension,
    //     WorldCharacteristics.MaxRopeDownSpeed * WorldCharacteristics.TimeDimension);

    // Наружу торчат методы которые говорят должное перемещение по вертикали и горизонтали

    public class Logic
    {
        private readonly double _maxDeviationSpeedPerTick;
        private readonly double _maxHeightSpeedPerTick;

        private readonly LogicGraph _deviationGraph = new LogicGraph(2);
        private readonly LogicGraph _heightGraph = new LogicGraph(3);
        private readonly LogicGraph _speedGraph = new LogicGraph(3);

        private enum DeviationType
        {
            Controlled,
            Uncontrolled
        }

        private enum HeightType
        {
            VeryClose,
            Close,
            Far
        }

        private enum SpeedType
        {
            Up,
            DownSlow,
            DownFast
        }

        public Logic(double maxDeviationSpeedPerTick, double maxHeightSpeedPerTick)
        {
            _maxDeviationSpeedPerTick = maxDeviationSpeedPerTick;
            _maxHeightSpeedPerTick = maxHeightSpeedPerTick;

            _deviationGraph.AddLogicTrapeze((int) DeviationType.Controlled, new LogicTrapeze(0, 0, 3, 5));
            _deviationGraph.AddLogicTrapeze((int) DeviationType.Uncontrolled, new LogicTrapeze(3, 5, 100, 100));

            _heightGraph.AddLogicTrapeze((int) HeightType.VeryClose, new LogicTrapeze(-100, -100, 2, 3));
            _heightGraph.AddLogicTrapeze((int) HeightType.Close, new LogicTrapeze(2, 3, 5, 6));
            _heightGraph.AddLogicTrapeze((int) HeightType.Far, new LogicTrapeze(5, 6, 100, 100));

            _speedGraph.AddLogicTrapeze((int) SpeedType.Up, new LogicTrapeze(-1, -1, -1, 0));
            _speedGraph.AddLogicTrapeze((int) SpeedType.DownSlow, new LogicTrapeze(-1, 0, 1, 2));
            _speedGraph.AddLogicTrapeze((int) SpeedType.DownFast, new LogicTrapeze(1, 2, 2, 2));
        }

        public double DeviationCompensation(double horizontalMove)
        {
            if (Math.Abs(horizontalMove) < _maxDeviationSpeedPerTick)
                return -horizontalMove;

            return horizontalMove < 0 ? _maxDeviationSpeedPerTick : -_maxDeviationSpeedPerTick;
        }

        private int[,] rules =
        {
            {0, 1, 2},
            {0, 0, 1}
        };

        public double HeightCompensation(double horizontalMove, double distance)
        {
            if (distance <= _maxHeightSpeedPerTick)
                return distance;

            var deviationDistribution = _deviationGraph.GetDistribution(Math.Abs(horizontalMove));
            var heightDistribution = _heightGraph.GetDistribution(distance);

            var speedDistribution = new List<double>(3);
            for (var i = 0; i < deviationDistribution.Count; ++i)
            for (var j = 0; j < heightDistribution.Count; ++j)
                speedDistribution[rules[i, j]] =
                    Math.Max(speedDistribution[rules[i, j]], Math.Min(deviationDistribution[i], heightDistribution[j]));

            var polygon = _speedGraph.GetPolygon(speedDistribution);
            var centerOfMass = CenterOfMass(polygon);

            return centerOfMass.X;
        }

        private Point2D CenterOfMass(List<Point2D> points)
        {
            var point = new Point2D(0, 0);
            double x = 0, y = 0, s = 0;

            for (var i = 0; i < points.Count - 1; i++)
            {
                var signArea = SignArea(point, points[i], points[i + 1]);
                var centroid = Centroid(point, points[i], points[i + 1]);
                x += centroid.X * signArea;
                y += centroid.Y * signArea;
                s += signArea;
            }

            if (s != 0)
                return new Point2D(x / s, y / s);

            // прямая => среднее по Х
            x = points.Sum(t => t.X);
            return new Point2D(x / points.Count, 0);
        }

        private double SignArea(Point2D p1, Point2D p2, Point2D p3)
        {
            return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X)) / 2;
        }

        private Point2D Centroid(Point2D p1, Point2D p2, Point2D p3)
        {
            return new Point2D((p1.X + p2.X + p3.X) / 3, (p1.Y + p2.Y + p3.Y) / 3);
        }
    }
}