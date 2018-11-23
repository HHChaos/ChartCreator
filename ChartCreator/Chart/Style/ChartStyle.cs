using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public abstract class ChartStyle
    {
        [JsonProperty("chartType")]
        public ChartType ChartType { get;internal set; }
        [JsonProperty("titleFormat")]
        public ChartTextFormat TitleFormat { get; set; }
        [JsonProperty("dataLabelFormat")]
        public ChartTextFormat DataLabelFormat { get; set; }
        [JsonProperty("legendStyle")]
        public LegendStyle LegendStyle { get; set; }
        [JsonProperty("colorSpace")]
        public List<string> ColorSpace { get; set; }
    }
}
