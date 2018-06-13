using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;
using ChartCreator.Chart.Style;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;

namespace ChartCreator.Chart.Utilities
{
    public static class ChartDrawHelper
    {
        public static async Task<SoftwareBitmap> GetBitmapAsync(CanvasCommandList canvasCommandList)
        {
            var chartRect = canvasCommandList.GetBounds(canvasCommandList.Device);
            var offScreen = new CanvasRenderTarget(canvasCommandList.Device, (float) chartRect.Width,
                (float) chartRect.Height, 96);
            var chartOffset = new Point(-chartRect.X, -chartRect.Y).ToVector2();
            using (var session = offScreen.CreateDrawingSession())
            {
                session.Transform = Matrix3x2.CreateTranslation(chartOffset);
                session.DrawImage(canvasCommandList);
            }
            var bitmap = await SoftwareBitmap.CreateCopyFromSurfaceAsync(offScreen, BitmapAlphaMode.Premultiplied);
            offScreen.Dispose();
            return bitmap;
        }

        public static AxisGeometry GetAxis(ICanvasResourceCreator resourceCreator, AxisStyle style, float dataWidth,
            float maxValue)
        {
            var totalHeight = style.DataAreaHeight + style.TopSpace + style.ArrowHeight;
            var totalWidth = dataWidth + style.RightSpace + style.StartOffset + style.ArrowHeight;
            var zeroPoint = new Vector2();

            var topPoint = new Vector2(zeroPoint.X, zeroPoint.Y - totalHeight);
            var rightPoint = new Vector2(zeroPoint.X + totalWidth, zeroPoint.Y);

            //xy轴
            var builder = new CanvasPathBuilder(resourceCreator);
            builder.BeginFigure(new Vector2(topPoint.X, topPoint.Y + style.ArrowHeight));
            builder.AddLine(zeroPoint);
            builder.AddLine(new Vector2(rightPoint.X - style.ArrowHeight, rightPoint.Y));
            builder.EndFigure(CanvasFigureLoop.Open);

            //y轴箭头
            var arrowBuilder = new CanvasPathBuilder(resourceCreator);
            arrowBuilder.BeginFigure(new Vector2(topPoint.X - style.ArrowWidth / 2, topPoint.Y + style.ArrowHeight));
            arrowBuilder.AddLine(topPoint);
            arrowBuilder.AddLine(new Vector2(topPoint.X + style.ArrowWidth / 2, topPoint.Y + style.ArrowHeight));
            arrowBuilder.EndFigure(CanvasFigureLoop.Open);
            //x轴箭头
            arrowBuilder.BeginFigure(new Vector2(rightPoint.X - style.ArrowHeight,
                rightPoint.Y - style.ArrowWidth / 2));
            arrowBuilder.AddLine(rightPoint);
            arrowBuilder.AddLine(new Vector2(rightPoint.X - style.ArrowHeight, rightPoint.Y + style.ArrowWidth / 2));
            arrowBuilder.EndFigure(CanvasFigureLoop.Open);

            var dataLeftTopPoint = new Point(style.StartOffset, -style.DataAreaHeight);
            var dataRightBottomPoint = new Point(dataWidth + style.StartOffset, -style.LineWidth/2);

            var valueGap = maxValue / 5;
            var valueHeightGap = style.DataAreaHeight / 5;
            var markerPoints = new List<Vector2>();
            var markerStrs = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                var markerPoint = new Vector2(0, -i * valueHeightGap);
                markerPoints.Add(markerPoint);
                if (i == 5)
                {
                    markerStrs.Add($"{maxValue}");
                }
                else
                {
                    var value = ValueFormatHelper.Round(i * valueGap, 3);
                    markerStrs.Add($"{value}");
                }
                builder.BeginFigure(markerPoint);
                builder.AddLine(new Vector2(markerPoint.X + style.ValueMarkerLineLength + style.LineWidth,
                    markerPoint.Y));
                builder.EndFigure(CanvasFigureLoop.Open);
            }
            var axis = new AxisGeometry
            {
                AxisLine = CanvasGeometry.CreatePath(builder),
                AxisArrow = CanvasGeometry.CreatePath(arrowBuilder),
                AxisValueMarker = CreateValueMarker(resourceCreator, style.ValueMarkerFormat, markerPoints, markerStrs,
                    MarkerPosition.Left),
                DataRect = new Rect(dataLeftTopPoint, dataRightBottomPoint)
            };

            return axis;
        }

        public static CanvasTextLayout CreateCanvasText(ICanvasResourceCreator resourceCreator, string text,
            ChartTextFormat textFormat, float requestedWidth = 0, float requestedHeight = 0)
        {
            var textLayout = new CanvasTextLayout(resourceCreator, text,
                new CanvasTextFormat
                {
                    FontWeight=new Windows.UI.Text.FontWeight
                    {
                        Weight=textFormat.IsBold?(ushort)700: (ushort)400
                    },
                    FontFamily = textFormat.FontFamily,
                    FontSize = textFormat.FontSize,
                    WordWrapping = CanvasWordWrapping.NoWrap,
                    TrimmingSign = CanvasTrimmingSign.Ellipsis
                }, requestedWidth, requestedHeight);
            return textLayout;
        }

        public static ICanvasImage CreateLegend(ICanvasResourceCreator resourceCreator, LegendStyle style,
            List<string> serieLabels, List<Color> colorSpace,
            LegendPosition legendPosition)
        {
            if (serieLabels?.Count > 0 && colorSpace?.Count >= serieLabels.Count)
            {
                var commandList = new CanvasCommandList(resourceCreator);
                using (var drawSession = commandList.CreateDrawingSession())
                {
                    var cancasTextArray =
                        serieLabels.Select(o => CreateCanvasText(resourceCreator, o, style.LabelFormat)).ToArray();
                    var textRectHeight = cancasTextArray.Select(o => o.DrawBounds.Height).Max();
                    var groupHeight = (float) Math.Max(textRectHeight, style.ColorBlockHeight);
                    var colorBlockYOffset = (float) (textRectHeight - style.ColorBlockHeight) / 2;
                    var startXOffset = 0f;
                    var startYOffset = colorBlockYOffset;
                    for (int i = 0; i < serieLabels.Count; i++)
                    {
                        drawSession.FillRectangle(startXOffset, startYOffset, style.ColorBlockWidth,
                            style.ColorBlockHeight, colorSpace[i]);
                        drawSession.DrawTextLayout(cancasTextArray[i],
                            startXOffset + style.ColorBlockWidth + style.LabelFormat.Thickness.Left,
                            startYOffset - colorBlockYOffset - (float) cancasTextArray[i].DrawBounds.Y,
                            ColorConverter.ConvertHexToColor(style.LabelFormat.Foreground));
                        if (legendPosition == LegendPosition.Bottom)
                        {
                            startXOffset += style.ColorBlockWidth;
                            startXOffset += style.LabelFormat.Thickness.Left;
                            startXOffset += (float) cancasTextArray[i].LayoutBounds.Width;
                            startXOffset += style.LabelFormat.Thickness.Right;
                        }
                        else if (legendPosition == LegendPosition.Right)
                        {
                            startYOffset += groupHeight;
                            startYOffset += style.LabelFormat.Thickness.Bottom;
                        }
                        cancasTextArray[i].Dispose();
                    }
                }
                return commandList;
            }
            return null;
        }

        public static ICanvasImage CreateValueMarker(ICanvasResourceCreator resourceCreator,
            ChartTextFormat markerFormat, List<Vector2> positions, List<string> labels, MarkerPosition markerPosition)
        {
            if (positions?.Count > 0 && positions.Count == labels?.Count)
            {
                var commandList = new CanvasCommandList(resourceCreator);
                using (var drawSession = commandList.CreateDrawingSession())
                {
                    var cancasTextArray =
                        labels.Select(o => CreateCanvasText(resourceCreator, o, markerFormat)).ToArray();

                    for (int i = 0; i < positions.Count; i++)
                    {
                        var textRect = cancasTextArray[i].DrawBounds;
                        double x = positions[i].X, y = positions[i].Y;
                        switch (markerPosition)
                        {
                            case MarkerPosition.Top:
                                x -= textRect.Width / 2 + textRect.X;
                                y -= textRect.Height + textRect.Y + markerFormat.Thickness.Bottom;
                                break;
                            case MarkerPosition.Right:
                                x += markerFormat.Thickness.Left - textRect.X;
                                y -= textRect.Height / 2 + textRect.Y;
                                break;
                            case MarkerPosition.Bottom:
                                x -= textRect.Width / 2 + textRect.X;
                                y += -textRect.Y + markerFormat.Thickness.Top;
                                break;
                            case MarkerPosition.Left:
                                x -= textRect.Width + markerFormat.Thickness.Right - textRect.X;
                                y -= textRect.Height / 2 + textRect.Y;
                                break;
                        }
                        drawSession.DrawTextLayout(cancasTextArray[i], (float) x, (float) y,
                            ColorConverter.ConvertHexToColor(markerFormat.Foreground));
                        cancasTextArray[i].Dispose();
                    }
                }
                return commandList;
            }
            return null;
        }
    }
}
