using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartCreator.Chart.Style;
using Newtonsoft.Json;

namespace ChartCreator.Chart
{
    public class ChartValues
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// 组标签集合
        /// </summary>
        [JsonProperty("groupLabels")]
        public List<string> GroupLabels { get; set; }

        /// <summary>
        /// 系列标签集合
        /// </summary>
        [JsonProperty("legendItemLabels")]
        public List<string> LegendItemLabels { get; set; }

        /// <summary>
        /// 系列数据列表(每系列为一组)
        /// </summary>
        [JsonProperty("data")]
        public List<float[]> Data { get; set; }

        [JsonProperty("legendPosition")]
        public LegendPosition LegendPosition { get; set; }
        [JsonProperty("isShowTitle")]
        public bool IsShowTitle { get; set; }
        [JsonProperty("isShowGroupLabels")]
        public bool IsShowGroupLabels { get; set; }
        [JsonProperty("isShowLegend")]
        public bool IsShowLegend { get; set; }
        [JsonProperty("isShowDataValues")]
        public bool IsShowDataValues { get; set; }
        [JsonIgnore]
        public int LegendItemCount => Data?.Count ?? 0;
        [JsonIgnore]
        public int GroupCount
        {
            get
            {
                if (Data?.Count > 0)
                {
                    return Data[0]?.Length ?? 0;
                }
                return 0;
            }
        }
    }
}
