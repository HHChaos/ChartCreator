using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Geometry;

namespace ChartCreator.Chart
{
    public class PiePartGeometry:IDisposable
    {
        public CanvasGeometry ShapeGeometry { get; set; }
        public CanvasGeometry TextGeometry { get; set; }
        public Rect DataRect { get; set; }

        public void Dispose()
        {
            ShapeGeometry?.Dispose();
            TextGeometry?.Dispose();
        }
    }
}
