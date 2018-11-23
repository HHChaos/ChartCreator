using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;
using ChartCreator.Chart.Style;
using ChartCreator.Chart.Utilities;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace ChartCreator.Chart
{
    public class LineChart : Chart
    {
        public LineChart() : base(ChartType.Line)
        {
        }

        public static List<PolyLineGeometry> GetLineChart(ICanvasResourceCreator resourceCreator, LineChartStyle style,
            List<float[]> valuesList, float height, float width, ChartTextFormat dataLabelFormat,
            List<string> colorSpace)
        {
            var geoList = new List<PolyLineGeometry>();
            var max = valuesList.Select(o => o.Max()).Max();
            var pointNum = valuesList[0].Length;
            var pointGap = width / (pointNum - 1);
            for (int i = 0; i < valuesList.Count; i++)
            {
                var list = valuesList[i];
                var markerPoints = new List<Vector2>();
                var markerStrs = new List<string>();
                var dotList = new List<CanvasGeometry>();
                var builder = new CanvasPathBuilder(resourceCreator);
                var heightList = list.Select(o => (float) (o / max * height)).ToList();
                var offset = 0f;
                var first = heightList.First();
                var startPoint = new Vector2(offset, -first);
                builder.BeginFigure(startPoint);
                dotList.Add(GetDotGeometry(resourceCreator, style.DotShape, startPoint, style.DotSizeWidth,
                    style.DotSizeHeight));
                markerPoints.Add(startPoint);
                markerStrs.Add($"{list[0]}");
                heightList.Remove(first);
                for (int j = 0; j < heightList.Count; j++)
                {
                    offset += pointGap;
                    var point = new Vector2(offset, -heightList[j]);
                    builder.AddLine(point);

                    dotList.Add(GetDotGeometry(resourceCreator, style.DotShape, point, style.DotSizeWidth,
                        style.DotSizeHeight));

                    markerPoints.Add(point);
                    markerStrs.Add($"{list[j + 1]}");
                }
                builder.EndFigure(CanvasFigureLoop.Open);
                var format = dataLabelFormat.Clone();
                //format.Foreground = colorSpace[i % colorSpace.Count];
                format.Thickness = new ChartThickness(0, format.Thickness.Top + style.DotSizeHeight / 2, 0, 0);
                var polyLineGeometry = new PolyLineGeometry
                {
                    LineGeometry = CanvasGeometry.CreatePath(builder),
                    DotGeometries = dotList,
                    DataLabels = ChartDrawHelper.CreateValueMarker(resourceCreator, format, markerPoints,
                        markerStrs,
                        MarkerPosition.Bottom),
                };
                geoList.Add(polyLineGeometry);
            }
            return geoList;
        }

        public static CanvasGeometry GetDotGeometry(ICanvasResourceCreator resourceCreator, DotShape shape,
            Vector2 point, float dotWidth, float dotHeight)
        {
            return CanvasGeometry.CreateCircle(resourceCreator, point, dotWidth / 2);
        }

        public override CanvasCommandList GetChartImage()
        {
            if (Values == null || Values.GroupCount == 0 ||
                Style?.ChartType != ChartType.Line)
            {
                return null;
            }
            var style = (LineChartStyle) Style;
            var colorSpace = Style.ColorSpace.Select(ColorConverter.ConvertHexToColor).ToList();
            var device = CanvasDevice.GetSharedDevice();
            var commandList = new CanvasCommandList(device);
            using (var session = commandList.CreateDrawingSession())
            {
                var titleLayout = ChartDrawHelper.CreateCanvasText(device, Values.Title, style.TitleFormat);
                var titleRect = titleLayout.LayoutBounds;
                if (Values.IsShowTitle)
                    session.DrawTextLayout(titleLayout, new Vector2(),
                        ColorConverter.ConvertHexToColor(style.TitleFormat.Foreground));
                titleLayout.Dispose();

                var maxValue = Values.Data.Select(o => o.Max()).Max();
                var maxMarkerValue = (float) ValueFormatHelper.Ceiling(maxValue, 2);

                var axisGeo = ChartDrawHelper.GetAxis(device, style.AxisStyle, style.DataAreaWidth, maxMarkerValue);
                var axisRect = axisGeo.AxisLine.ComputeBounds();
                axisRect.Union(axisGeo.AxisValueMarker.GetBounds(device));
                var axisOffset =
                    new Point((titleRect.Width - axisRect.Width) / 2 - axisRect.X,
                        titleRect.Height + style.TitleFormat.Thickness.Bottom - axisRect.Y).ToVector2();
                using (var axisCommandList = new CanvasCommandList(session))
                {
                    using (var axisDrawSession = axisCommandList.CreateDrawingSession())
                    {
                        axisDrawSession.Transform = Matrix3x2.CreateTranslation(axisOffset);
                        axisDrawSession.DrawGeometry(axisGeo.AxisLine,
                            ColorConverter.ConvertHexToColor(style.AxisStyle.LineColor), style.AxisStyle.LineWidth);
                        axisDrawSession.FillGeometry(axisGeo.AxisArrow,
                            ColorConverter.ConvertHexToColor(style.AxisStyle.LineColor));
                        axisDrawSession.DrawImage(axisGeo.AxisValueMarker);
                    }
                    session.DrawImage(axisCommandList);
                }
                axisGeo.Dispose();

                var maxValueHeight = maxValue / maxMarkerValue * axisGeo.DataRect.Height;
                var lineGeos = GetLineChart(device, style, Values.Data, (float) maxValueHeight, style.DataAreaWidth,
                    style.DataLabelFormat, Style.ColorSpace);
                var dataOffset = axisOffset +
                                 new Point(axisGeo.DataRect.X, axisGeo.DataRect.Y + axisGeo.DataRect.Height)
                                     .ToVector2();
                using (var dataCommandList = new CanvasCommandList(session))
                {
                    using (var drawSession = dataCommandList.CreateDrawingSession())
                    {
                        drawSession.Transform = Matrix3x2.CreateTranslation(dataOffset);
                        for (int i = 0; i < lineGeos.Count; i++)
                        {
                            drawSession.DrawGeometry(lineGeos[i].LineGeometry, colorSpace[i % colorSpace.Count],
                                style.LineWidth);
                            foreach (var dot in lineGeos[i].DotGeometries)
                            {
                                drawSession.FillGeometry(dot, colorSpace[i % colorSpace.Count]);
                            }
                            if (Values.IsShowDataValues)
                                drawSession.DrawImage(lineGeos[i].DataLabels);
                            lineGeos[i].Dispose();
                        }
                    }
                    session.DrawImage(dataCommandList);
                }
                if (Values.IsShowGroupLabels)
                {
                    var markerPoints = new List<Vector2>();
                    var markerStrs = new List<string>();
                    var groupLength = style.DataAreaWidth / (Values.GroupCount - 1);
                    for (int i = 0; i < Values.GroupCount; i++)
                    {
                        var markerPoint = new Vector2(i * groupLength, 0);
                        markerPoints.Add(markerPoint);
                        markerStrs.Add(Values.GroupLabels[i]);
                    }
                    var groupLabels = ChartDrawHelper.CreateValueMarker(device, style.GroupLabelFormat, markerPoints,
                        markerStrs,
                        MarkerPosition.Bottom);
                    var groupLabelsOffset = axisOffset +
                                            new Vector2(
                                                style.AxisStyle.StartOffset, 0);
                    using (var dataCommandList = new CanvasCommandList(session))
                    {
                        using (var drawSession = dataCommandList.CreateDrawingSession())
                        {
                            drawSession.Transform = Matrix3x2.CreateTranslation(groupLabelsOffset);
                            drawSession.DrawImage(groupLabels);
                        }
                        session.DrawImage(dataCommandList);
                    }
                    groupLabels.Dispose();
                }
                if (Values.IsShowLegend)
                {
                    var legendLabels = Values.LegendItemLabels.Take(Values.LegendItemCount).ToList();
                    var legend = ChartDrawHelper.CreateLegend(device, style.LegendStyle,
                        legendLabels, colorSpace, Values.LegendPosition);
                    var legendRect = legend.GetBounds(device);
                    switch (Values.LegendPosition)
                    {
                        case LegendPosition.Right:
                            var rightOffset =
                                new Point(
                                        axisOffset.X + axisRect.Width + style.LegendStyle.Thickness.Left - legendRect.X,
                                        axisOffset.Y - legendRect.Height - legendRect.Y)
                                    .ToVector2();
                            session.DrawImage(legend, rightOffset);
                            break;
                        case LegendPosition.Bottom:
                            var bottomOffset =
                                new Point(
                                        (titleRect.Width - legendRect.Width) / 2 - legendRect.X,
                                        axisOffset.Y + style.LegendStyle.Thickness.Top - legendRect.Y)
                                    .ToVector2();
                            session.DrawImage(legend, bottomOffset);
                            break;
                    }
                    legend.Dispose();
                }
                return commandList;
            }
        }
    }
}
