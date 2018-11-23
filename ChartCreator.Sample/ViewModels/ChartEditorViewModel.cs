using HHChaosToolkit.UWP.Picker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartCreator;
using HHChaosToolkit.UWP.Mvvm;
using ChartCreator.Chart;
using ChartCreator.Sample.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ChartCreator.Chart.Style;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;

namespace ChartCreator.Sample.ViewModels
{
    public class ChartEditorViewModel : ObjectPickerBase<Chart.Chart>
    {
        public override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Reset(e.Parameter as Chart.Chart);
            base.OnNavigatedTo(e);
        }
        private bool _inited;
        public async Task Reset(Chart.Chart chart)
        {
            if (!_inited)
            {
                await Init();
            }
            if (chart != null)
            {
                Values = chart.Values;
                Title = Values.Title;
                SelectedChartType = ChartTypeList.Find(o => o.Value == chart.ChartType);
            }
            else
            {
                Values = new ChartValues
                {
                    Title = "Sample Chart",
                    GroupLabels = new List<string>
                    {
                        "group 1",
                        "group 2",
                        "group 3",
                        "group 4",
                        "group 5",
                        "group 6",
                    },
                    LegendItemLabels = new List<string>
                    {
                        "legend 1",
                        "legend 2",
                        "legend 3",
                        "legend 4"
                    },
                    Data = new List<float[]>
                    {
                        new float[] {12, 34, 45, 65, 43, 78},
                        new float[] {22, 44, 88, 23, 9, 23},
                        new float[] {66, 75, 32, 55, 11, 80},
                        new float[] {98, 81, 39, 18, 16, 33},
                    },
                    LegendPosition = LegendPosition.Bottom,
                    IsShowTitle = true,
                    IsShowGroupLabels = true,
                    IsShowLegend = true,
                    IsShowDataValues = true
                };
                Title = Values.Title;
                if (LegendPositionList.Count > 0)
                    SelectedLegendPosition = LegendPositionList[0];
                if (ChartTypeList.Count > 0)
                    SelectedChartType = ChartTypeList[0];
                _inited = true;
            }
        }

        public async Task Init()
        {
            LegendPositionList.Add(new KeyValuePair<string, LegendPosition>("Bottom", LegendPosition.Bottom));
            LegendPositionList.Add(new KeyValuePair<string, LegendPosition>("Right", LegendPosition.Right));
            ChartTypeList.Add(new KeyValuePair<string, ChartType>("BarChart", ChartType.Bar));
            ChartTypeList.Add(new KeyValuePair<string, ChartType>("LineChart", ChartType.Line));
            ChartTypeList.Add(new KeyValuePair<string, ChartType>("PieChart", ChartType.Pie));
            _defaultStylePack = new ChartStylePack
            {
                BarChartStyle = await GetStyleFromFile<BarChartStyle>("ms-appx:///ChartStyle/barChart.json"),
                LineChartStyle = await GetStyleFromFile<LineChartStyle>("ms-appx:///ChartStyle/lineChart.json"),
                PieChartStyle = await GetStyleFromFile<PieChartStyle>("ms-appx:///ChartStyle/pieChart.json"),
            };
        }
        private ChartStylePack _defaultStylePack;
        public async Task<T> GetStyleFromFile<T>(string fileUri)
            where T : ChartStyle
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileUri));
            var text = await FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<T>(text);
        }

        public List<KeyValuePair<string, ChartType>> ChartTypeList { get; } =
            new List<KeyValuePair<string, ChartType>>();

        private KeyValuePair<string, ChartType> _selectedChartType;

        public KeyValuePair<string, ChartType> SelectedChartType
        {
            get => _selectedChartType;
            set
            {
                Set(ref _selectedChartType, value);
            }
        }

        public List<KeyValuePair<string, LegendPosition>> LegendPositionList { get; } =
            new List<KeyValuePair<string, LegendPosition>>();

        private KeyValuePair<string, LegendPosition> _selectedLegendPosition;

        public KeyValuePair<string, LegendPosition> SelectedLegendPosition
        {
            get => _selectedLegendPosition;
            set
            {
                Set(ref _selectedLegendPosition, value);
            }
        }
        private bool _isShowTitle = true;
        private bool _isShowGroupLabels = true;
        private bool _isShowLegend = true;
        private bool _isShowDataValues = true;
        public bool IsShowTitle
        {
            get { return _isShowTitle; }
            set
            {
                Set(ref _isShowTitle, value);
            }
        }
        public bool IsShowGroupLabels
        {
            get { return _isShowGroupLabels; }
            set
            {
                Set(ref _isShowGroupLabels, value);
            }
        }
        public bool IsShowLegend
        {
            get { return _isShowLegend; }
            set
            {
                Set(ref _isShowLegend, value);
            }
        }
        public bool IsShowDataValues
        {
            get { return _isShowDataValues; }
            set
            {
                Set(ref _isShowDataValues, value);
            }
        }
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                Set(ref _title, value);
            }
        }
        private ChartValues _values;

        public ChartValues Values
        {
            get { return _values; }
            set
            {
                Set(ref _values, value);
            }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!_inited || _values == null)
                        return;
                    _values.Title = _title;
                    _values.LegendPosition = _selectedLegendPosition.Value;
                    _values.IsShowTitle = IsShowTitle;
                    _values.IsShowLegend = IsShowLegend;
                    _values.IsShowGroupLabels = IsShowGroupLabels;
                    _values.IsShowDataValues = IsShowDataValues;
                    Chart.Chart chart = null;
                    var type = _selectedChartType.Value;
                    switch (type)
                    {
                        case ChartType.Bar:
                            chart = new BarChart
                            {
                                Values = _values,
                                Style = _defaultStylePack.GetStyle(type)
                            };
                            break;
                        case ChartType.Pie:
                            chart = new PieChart
                            {
                                Values = _values,
                                Style = _defaultStylePack.GetStyle(type)
                            };
                            break;
                        case ChartType.Line:
                            chart = new LineChart
                            {
                                Values = _values,
                                Style = _defaultStylePack.GetStyle(type)
                            };
                            break;
                    }
                    SetResult(chart);
                });
            }
        }
    }
}
