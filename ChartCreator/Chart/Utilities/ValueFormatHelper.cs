using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCreator.Chart.Utilities
{
    public static class ValueFormatHelper
    {
        public static double Ceiling(double value,int significand)
        {
            var str = string.Format($"{{0:E{significand}}}", value);
            var strs = str.Split('E');
            var signValue = double.Parse(strs[0]);
            signValue = Math.Ceiling(signValue * Math.Pow(10,significand-1)) / Math.Pow(10, significand-1);
            return double.Parse($"{signValue}E{strs[1]}");
        }
        public static double Round(double value, int significand)
        {
            var str = string.Format($"{{0:E{significand}}}", value);
            var strs = str.Split('E');
            var signValue = double.Parse(strs[0]);
            signValue = Math.Round(signValue * Math.Pow(10, significand - 1)) / Math.Pow(10, significand - 1);
            return double.Parse($"{signValue}E{strs[1]}");
        }
    }
}
