using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public class LineChartStyle : ChartStyle
    {
        public LineChartStyle()
        {
            ChartType = ChartType.Line;
        }
        [JsonProperty("axisStyle")]
        public AxisStyle AxisStyle { get; set; }
        [JsonProperty("groupLabelFormat")]
        public ChartTextFormat GroupLabelFormat { get; set; }
        [JsonProperty("dataAreaWidth")]
        public float DataAreaWidth { get; set; }
        [JsonProperty("lineWidth")]
        public float LineWidth { get; set; }
        [JsonProperty("dotSizeWidth")]
        public float DotSizeWidth { get; set; }
        [JsonProperty("dotSizeHeight")]
        public float DotSizeHeight { get; set; }
        [JsonProperty("dotShape")]
        public DotShape DotShape { get; set; }
    }
}
