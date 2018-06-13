using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using ChartCreator.Chart.Style;

namespace ChartCreator.Chart
{
    public abstract class Chart
    {
        protected Chart(ChartType chartType)
        {
            ChartType = chartType;
        }

        public ChartType ChartType { get; }
        public ChartValues Values { get; set; }
        public ChartStyle Style { get; set; }
        public abstract Task<SoftwareBitmap> GetChartBitmapAsync();
    }
}
