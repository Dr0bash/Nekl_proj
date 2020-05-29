using System;
using System.Collections.Generic;

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
        
        private readonly LogicGraph _deviationGraph = new LogicGraph(3); // тут наверное не 3 будет
        private readonly LogicGraph _heightGraph = new LogicGraph(3);
        private readonly LogicGraph _speedGraph = new LogicGraph(3);
        
        private enum DeviationType { /* TODO: пока непонятно */ }
        private enum HeightType { VeryClose, Close, Far }
        private enum SpeedType { Up, DownSlow, DownFast }

        public Logic(double maxDeviationSpeedPerTick, double maxHeightSpeedPerTick)
        {
            _maxDeviationSpeedPerTick = maxDeviationSpeedPerTick;
            _maxHeightSpeedPerTick = maxHeightSpeedPerTick;

            // правила для height
            _heightGraph.AddLogicTrapeze((int) HeightType.VeryClose, new LogicTrapeze(-100, -100, 2, 3));
            _heightGraph.AddLogicTrapeze((int) HeightType.Close, new LogicTrapeze(2, 3, 5, 6));
            _heightGraph.AddLogicTrapeze((int) HeightType.Far, new LogicTrapeze(5, 6, 100, 100));
        
            // правила для speed
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
        
        public double HeightCompensation(double horizontalMove, double distance)
        {
            return 0;
        }
        
        private Point2D CenterOfMass(List<Point2D> points)
        {
            return null;
        }
    }
}