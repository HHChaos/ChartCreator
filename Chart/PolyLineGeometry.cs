using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace ChartCreator.Chart
{
    public class PolyLineGeometry : IDisposable
    {
        public CanvasGeometry LineGeometry { get; set; }
        public List<CanvasGeometry> DotGeometries { get; set; }
        public ICanvasImage DataLabels { get; set; }
        public void Dispose()
        {
            LineGeometry?.Dispose();
            DataLabels?.Dispose();
            if (DotGeometries?.Count > 0)
                DotGeometries.ForEach(o => o?.Dispose());
        }
    }
}
