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
            // TODO 
            return res;
        }

        public List<Point2D> GetPolygon(List<double> distribution)
        {
            var res = new List<Point2D>();
            if (distribution.Count != _list.Count)
                return null;
            // TODO
            
            return res;
        }
    }
}