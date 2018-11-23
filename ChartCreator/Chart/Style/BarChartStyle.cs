using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public class BarChartStyle: ChartStyle
    {
        public BarChartStyle()
        {
            ChartType = ChartType.Bar;
        }
        [JsonProperty("axisStyle")]
        public AxisStyle AxisStyle { get; set; }
        [JsonProperty("groupLabelFormat")]
        public ChartTextFormat GroupLabelFormat { get; set; }
        [JsonProperty("barBorderWidth")]
        public float BarBorderWidth { get; set; }
        [JsonProperty("barBorderColor")]
        public string BarBorderColor { get; set; }
        [JsonProperty("barMaxWidth")]
        public float BarMaxWidth { get; set; }
        [JsonProperty("barMinWidth")]
        public float BarMinWidth { get; set; }
        [JsonProperty("barMaxNumber")]
        public int BarMaxNumber { get; set; }
        /// <summary>
        /// 组间距
        /// </summary>
        [JsonProperty("groupSpace")]
        public float GroupSpace { get; set; }
        /// <summary>
        /// 柱间距
        /// </summary>
        [JsonProperty("barSpace")]
        public float BarSpace { get; set; }
    }
}
