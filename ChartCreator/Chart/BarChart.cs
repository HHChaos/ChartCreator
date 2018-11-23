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
    public class BarChart : Chart
    {
        public BarChart() : base(ChartType.Bar)
        {
        }

        public static List<BarGroupGeometry> GetBarChart(ICanvasResourceCreator resourceCreator,
            List<float[]> valuesList, float height, float barWidth, float barBorderWidth,
            float barGap, float groupGap, ChartTextFormat dataLabelFormat)
        {
            var geoList = new List<BarGroupGeometry>();
            var max = valuesList.Select(o => o.Max()).Max();
            var barNum = valuesList.Count;
            var totalGroupGap = (barWidth + barBorderWidth + barGap) * barNum + groupGap - barGap;
            var offset = 0f;
            foreach (var list in valuesList)
            {
                var builder = new CanvasPathBuilder(resourceCreator);
                var heightList = list.Select(o => (float) (o / max * height)).ToArray();
                var startPoint = new Vector2();
                var markerPoints = new List<Vector2>();
                var markerStrs = new List<string>();
                for (int i = 0; i < heightList.Count(); i++)
                {
                    builder.BeginFigure(startPoint);
                    builder.AddLine(new Vector2(startPoint.X, startPoint.Y - heightList[i]));
                    builder.AddLine(new Vector2(startPoint.X + barWidth, startPoint.Y - heightList[i]));
                    builder.AddLine(new Vector2(startPoint.X + barWidth, startPoint.Y));
                    builder.EndFigure(CanvasFigureLoop.Open);

                    var markerPoint = new Vector2(startPoint.X + barWidth / 2, startPoint.Y - heightList[i]);
                    markerPoints.Add(markerPoint);
                    markerStrs.Add($"{list[i]}");

                    startPoint = new Vector2(startPoint.X + totalGroupGap, startPoint.Y);
                }
                geoList.Add(new BarGroupGeometry
                {
                    ShapeGeometry = CanvasGeometry.CreatePath(builder),
                    DataLabels = ChartDrawHelper.CreateValueMarker(resourceCreator, dataLabelFormat, markerPoints,
                        markerStrs,
                        MarkerPosition.Bottom),
                    Offset = offset
                });
                builder.Dispose();
                offset += barGap + barWidth + barBorderWidth;
            }
            return geoList;
        }

        public override CanvasCommandList GetChartImage()
        {
            if (Values == null || Values.GroupCount == 0 ||
                Style?.ChartType != ChartType.Bar)
            {
                return null;
            }
            var style = (BarChartStyle) Style;
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
                var barWidth = style.BarMaxNumber > 1
                    ? style.BarMaxWidth - (style.BarMaxWidth - style.BarMinWidth) / (style.BarMaxNumber - 1) *
                      (Values.LegendItemCount - 1)
                    : style.BarMaxWidth;
                var groupLength = (barWidth + style.BarBorderWidth + style.BarSpace) * Values.LegendItemCount +
                                  style.GroupSpace - style.BarSpace;
                var totalWidth = Values.GroupCount * groupLength - style.GroupSpace;

                var maxValue = Values.Data.Select(o => o.Max()).Max();
                var maxMarkerValue = (float) ValueFormatHelper.Ceiling(maxValue, 2);

                var axisGeo = ChartDrawHelper.GetAxis(device, style.AxisStyle, totalWidth, maxMarkerValue);
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
                var barGroupGeos = GetBarChart(device, Values.Data, (float) maxValueHeight, barWidth,
                    style.BarBorderWidth,
                    style.BarSpace, style.GroupSpace, style.DataLabelFormat);
                var dataOffset = axisOffset +
                                 new Point(axisGeo.DataRect.X, axisGeo.DataRect.Y + axisGeo.DataRect.Height)
                                     .ToVector2();
                using (var dataCommandList = new CanvasCommandList(session))
                {
                    using (var drawSession = dataCommandList.CreateDrawingSession())
                    {
                        drawSession.Transform = Matrix3x2.CreateTranslation(dataOffset);
                        for (int i = 0; i < barGroupGeos.Count; i++)
                        {
                            drawSession.FillGeometry(barGroupGeos[i].ShapeGeometry,
                                new Vector2(barGroupGeos[i].Offset, 0),
                                colorSpace[i % colorSpace.Count]);
                            drawSession.DrawGeometry(barGroupGeos[i].ShapeGeometry,
                                new Vector2(barGroupGeos[i].Offset, 0),
                                ColorConverter.ConvertHexToColor(style.BarBorderColor), style.BarBorderWidth);
                            if (Values.IsShowDataValues)
                                drawSession.DrawImage(barGroupGeos[i].DataLabels,
                                    new Vector2(barGroupGeos[i].Offset, 0));
                            barGroupGeos[i].Dispose();
                        }
                    }
                    session.DrawImage(dataCommandList);
                }
                if (Values.IsShowGroupLabels)
                {
                    var markerPoints = new List<Vector2>();
                    var markerStrs = new List<string>();
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
                                                style.AxisStyle.StartOffset + (groupLength - style.GroupSpace) / 2, 0);
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
