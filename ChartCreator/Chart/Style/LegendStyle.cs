using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public class LegendStyle
    {
        [JsonProperty("colorBlockWidth")]
        public float ColorBlockWidth { get; set; }
        [JsonProperty("colorBlockHeight")]
        public float ColorBlockHeight { get; set; }
        [JsonProperty("labelFormat")]
        public ChartTextFormat LabelFormat { get; set; }
        [JsonProperty("thickness")]
        public ChartThickness Thickness { get; set; }
    }
}
