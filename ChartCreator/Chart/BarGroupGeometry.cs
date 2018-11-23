using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace ChartCreator.Chart
{
    public class BarGroupGeometry : IDisposable
    {
        public CanvasGeometry ShapeGeometry { get; set; }
        public ICanvasImage DataLabels { get; set; }
        public float Offset { get; set; }
        public void Dispose()
        {
            ShapeGeometry?.Dispose();
            DataLabels?.Dispose();
        }
    }
}
