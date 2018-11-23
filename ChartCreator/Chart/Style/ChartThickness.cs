using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public class ChartThickness
    {
        public ChartThickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        [JsonProperty("left")]
        public float Left { get; set; }
        [JsonProperty("top")]
        public float Top { get; set; }
        [JsonProperty("right")]
        public float Right { get; set; }
        [JsonProperty("bottom")]
        public float Bottom { get; set; }
    }
}
