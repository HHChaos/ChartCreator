using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public sealed class ChartTextFormat
    {
        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }
        [JsonProperty("fontSize")]
        public float FontSize { get; set; }
        [JsonProperty("foreground")]
        public string Foreground { get; set; }
        //bold相当于字重为700,nomal相当于字重为400
        [JsonProperty("isBold")]
        public bool IsBold { get; set; }
        [JsonProperty("thickness")]
        public ChartThickness Thickness { get; set; }

        public ChartTextFormat Clone()
        {
            return new ChartTextFormat
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                Foreground = Foreground,
                IsBold = IsBold,
                Thickness = Thickness
            };
        }
    }
}
