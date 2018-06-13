using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using ChartCreator.Chart.Style;
using ChartCreator.Chart.Utilities;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;

namespace ChartCreator.Chart
{
    public class PieChart : Chart
    {
        public PieChart() : base(ChartType.Pie)
        {
        }

        public static List<PiePartGeometry> GetPieChart(ICanvasResourceCreator resourceCreator,
            float[] valueList, ChartTextFormat textFormat, float radius, bool isShowData)
        {
            var centerPoint = new Vector2(radius, radius);
            var geoList = new List<PiePartGeometry>();
            var sum = valueList.Sum();
            var startAngle = 0f;
            var percentList = valueList.Select(o => o / sum);
            foreach (var item in percentList)
            {
                var radian = item * 2 * Math.PI;
                var builder = new CanvasPathBuilder(resourceCreator);
                builder.BeginFigure(centerPoint);
                builder.AddArc(centerPoint, radius, radius, startAngle, (float) radian);
                builder.EndFigure(CanvasFigureLoop.Closed);

                if (isShowData)
                {
                    var partCenter = new Point(
                            (radius - textFormat.Thickness.Top) * Math.Cos(startAngle + radian / 2) + centerPoint.X,
                            (radius - textFormat.Thickness.Top) * Math.Sin(startAngle + radian / 2) + centerPoint.Y)
                        .ToVector2();
                    var textLayout = ChartDrawHelper.CreateCanvasText(resourceCreator,
                        $"{string.Format("{0:F1}", item * 100)}%", textFormat);
                    var textRect = textLayout.DrawBounds;
                    var textPos = new Point(partCenter.X - textRect.Width / 2, partCenter.Y - textRect.Height / 2);
                    var part = new PiePartGeometry
                    {
                        ShapeGeometry = CanvasGeometry.CreatePath(builder),
                        TextGeometry = CanvasGeometry.CreateText(textLayout),
                        DataRect = new Rect(textPos, new Size(textRect.Width, textRect.Height))
                    };
                    geoList.Add(part);
                    builder.Dispose();
                    textLayout.Dispose();
                }
                else
                {
                    var part = new PiePartGeometry
                    {
                        ShapeGeometry = CanvasGeometry.CreatePath(builder)
                    };
                    geoList.Add(part);
                    builder.Dispose();
                }
                startAngle += (float) radian;
            }
            return geoList;
        }

        public override async Task<SoftwareBitmap> GetChartBitmapAsync()
        {
            if (Values == null || Values.GroupCount == 0 ||
                Style?.ChartType != ChartType.Pie)
            {
                return null;
            }
            var style = (PieChartStyle) Style;
            var colorSpace = Style.ColorSpace.Select(ColorConverter.ConvertHexToColor).ToList();
            var device = CanvasDevice.GetSharedDevice();
            var commandList = new CanvasCommandList(device);
            using (var session = commandList.CreateDrawingSession())
            {
                var geos = GetPieChart(device, Values.Data[0], style.DataLabelFormat, style.Radius,
                    Values.IsShowDataValues);
                var titleLayout = ChartDrawHelper.CreateCanvasText(device, Values.Title, style.TitleFormat);
                var titleRect = titleLayout.LayoutBounds;
                if (Values.IsShowTitle)
                    session.DrawTextLayout(titleLayout, new Vector2(),
                        ColorConverter.ConvertHexToColor(style.TitleFormat.Foreground));
                titleLayout.Dispose();
                var diameter = style.Radius * 2 + style.BorderWidth/2;
                var pieOffset =
                    new Point((titleRect.Width - diameter) / 2,
                        titleRect.Height + style.TitleFormat.Thickness.Bottom).ToVector2();
                using (var pieCommandList = new CanvasCommandList(session))
                {
                    using (var pieDrawSession = pieCommandList.CreateDrawingSession())
                    {
                        pieDrawSession.Transform = Matrix3x2.CreateTranslation(pieOffset);
                        for (int i = 0; i < geos.Count; i++)
                        {
                            pieDrawSession.FillGeometry(geos[i].ShapeGeometry,
                                colorSpace[i % Style.ColorSpace.Count]);
                        }
                        foreach (var t in geos)
                        {
                            pieDrawSession.DrawGeometry(t.ShapeGeometry,
                                ColorConverter.ConvertHexToColor(style.BorderColor), style.BorderWidth,
                                new CanvasStrokeStyle
                                {
                                    LineJoin = CanvasLineJoin.Round,
                                });
                            if (Values.IsShowDataValues)
                                pieDrawSession.FillGeometry(t.TextGeometry, (float) t.DataRect.X,
                                    (float) t.DataRect.Y,
                                    ColorConverter.ConvertHexToColor(style.DataLabelFormat.Foreground));
                            t.Dispose();
                        }
                    }
                    session.DrawImage(pieCommandList);
                }
                if (Values.IsShowLegend)
                {
                    var legendLabels = Values.GroupLabels.Take(Values.GroupCount).ToList();
                    var legend = ChartDrawHelper.CreateLegend(device, style.LegendStyle,
                        legendLabels, colorSpace, Values.LegendPosition);
                    var legendRect = legend.GetBounds(device);
                    switch (Values.LegendPosition)
                    {
                        case LegendPosition.Right:
                            var rightOffset =
                                new Point(
                                        pieOffset.X + diameter + style.LegendStyle.Thickness.Left - legendRect.X,
                                        pieOffset.Y + (diameter - legendRect.Height) - legendRect.Y)
                                    .ToVector2();
                            session.DrawImage(legend, rightOffset);
                            break;
                        case LegendPosition.Bottom:
                            var bottomOffset =
                                new Point(
                                        (titleRect.Width - legendRect.Width) / 2 - legendRect.X,
                                        titleRect.Height + style.TitleFormat.Thickness.Bottom +
                                        diameter + style.LegendStyle.Thickness.Top - legendRect.Y)
                                    .ToVector2();
                            session.DrawImage(legend, bottomOffset);
                            break;
                    }
                    legend.Dispose();
                }
            }
            var bitmap = await ChartDrawHelper.GetBitmapAsync(commandList);
            commandList.Dispose();
            return bitmap;
        }
    }
}