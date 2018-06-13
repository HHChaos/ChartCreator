using Newtonsoft.Json;

namespace ChartCreator.Chart.Style
{
    public class AxisStyle
    {
        [JsonProperty("valueMarkerFormat")]
        public ChartTextFormat ValueMarkerFormat { get; set; }

        [JsonProperty("valueMarkerLineLength")]
        public float ValueMarkerLineLength { get; set; }

        [JsonProperty("lineWidth")]
        public float LineWidth { get; set; }

        [JsonProperty("lineColor")]
        public string LineColor { get; set; }

        [JsonProperty("dataAreaHeight")]
        public float DataAreaHeight { get; set; }
        /// <summary>
        /// 纵轴最大值到上箭头距离
        /// </summary>
        [JsonProperty("topSpace")]
        public float TopSpace { get; set; }
        /// <summary>
        /// 横轴最后一个值横坐标到右箭头距离
        /// </summary>
        [JsonProperty("rightSpace")]
        public float RightSpace { get; set; }
        /// <summary>
        /// 原点到数据区的距离
        /// </summary>
        [JsonProperty("startOffset")]
        public float StartOffset { get; set; }
        [JsonProperty("arrowWidth")]
        public float ArrowWidth { get; set; }
        [JsonProperty("arrowHeight")]
        public float ArrowHeight { get; set; }
    }
}
