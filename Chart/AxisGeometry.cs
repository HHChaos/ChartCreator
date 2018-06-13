using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace ChartCreator.Chart
{
    public class AxisGeometry:IDisposable
    {
        public CanvasGeometry AxisLine { get; set; }
        public CanvasGeometry AxisArrow { get; set; }
        public ICanvasImage AxisValueMarker { get; set; }
        public Rect DataRect { get; set; }

        public void Dispose()
        {
            AxisLine?.Dispose();
            AxisArrow?.Dispose();
            AxisValueMarker?.Dispose();
        }
    }
}
