using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public class PieChartStyle : ChartStyle
    {
        public PieChartStyle()
        {
            ChartType = ChartType.Pie;
        }
        [JsonProperty("radius")]
        public float Radius { get; set; }
        [JsonProperty("borderWidth")]
        public float BorderWidth { get; set; }
        [JsonProperty("borderColor")]
        public string BorderColor { get; set; }
    }
}
