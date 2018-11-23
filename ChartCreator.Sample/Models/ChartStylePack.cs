using ChartCreator.Chart;
using ChartCreator.Chart.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCreator.Sample.Models
{
    public class ChartStylePack
    {
        public BarChartStyle BarChartStyle { get; set; }
        public LineChartStyle LineChartStyle { get; set; }
        public PieChartStyle PieChartStyle { get; set; }

        public ChartStyle GetStyle(ChartType type)
        {
            switch (type)
            {
                case ChartType.Bar:
                    return BarChartStyle;
                case ChartType.Pie:
                    return PieChartStyle;
                case ChartType.Line:
                    return LineChartStyle;
            }
            return null;
        }

        public bool MatchStyle(ChartStyle style)
        {
            return style == BarChartStyle
                   || style == LineChartStyle
                   || style == PieChartStyle;
        }
    }
}
