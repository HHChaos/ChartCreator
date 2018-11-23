using ChartCreator.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ChartCreator.Sample.Controls
{
    public class ChartDataGrid : ContentControl
    {
        private bool _inited;
        public ChartDataGrid()
        {
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
            var rootGrid = new Grid();
            this.Content = rootGrid;
            UpdateLayout(rootGrid, 7, 5);
            this.Loaded += ChartDataGrid_Loaded;
            this.Unloaded += ChartDataGrid_Unloaded;
        }
        private bool _isChangingTextWithCode;
        private void ChartDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateValuesText();
        }

        private void ChartDataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes();
            this.Loaded -= ChartDataGrid_Loaded;
            this.Unloaded -= ChartDataGrid_Unloaded;
        }

        private readonly List<TextBox> _serieLabelsTb = new List<TextBox>();
        private readonly List<TextBox> _groupLabelsTb = new List<TextBox>();
        private readonly List<NumberTextBox[]> _valuesTb = new List<NumberTextBox[]>();


        public ChartValues Values
        {
            get { return (ChartValues)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(ChartValues), typeof(ChartDataGrid), new PropertyMetadata(null, PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartDataGrid grid)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                    grid.UpdateValuesText();
            }
        }

        private void UpdateValuesText()
        {
            if (Values == null || !_inited)
                return;
            _isChangingTextWithCode = true;
            for (int i = 0; i < Values.LegendItemLabels.Count; i++)
            {
                _serieLabelsTb[i].Text = Values.LegendItemLabels[i];
            }
            for (int i = 0; i < Values.GroupLabels.Count; i++)
            {
                _groupLabelsTb[i].Text = Values.GroupLabels[i];
            }
            for (int i = 0; i < Values.Data.Count; i++)
            {
                var values = Values.Data[i];
                for (int j = 0; j < values.Length; j++)
                {
                    _valuesTb[i][j].Value = values[j];
                }
            }
            _isChangingTextWithCode = false;
        }

        private ChartValues CreateNewValues()
        {
            var value = new ChartValues
            {
                Title = string.Empty,
                GroupLabels = new List<string>(),
                LegendItemLabels = new List<string>(),
                Data = new List<float[]>(),
                LegendPosition = LegendPosition.Bottom,
                IsShowTitle = true,
                IsShowGroupLabels = true,
                IsShowLegend = true,
                IsShowDataValues = true
            };
            return value;
        }

        private void TrimValues()
        {
            if (Values == null)
            {
                return;
            }
            for (int i = Values.LegendItemCount - 1; i > 0; i--)
            {
                var values = Values.Data[i];
                var isValid = false;
                foreach (var value in values)
                {
                    if (value != 0)
                        isValid = true;
                }
                if (!isValid)
                {
                    Values.Data.Remove(values);
                    Values.LegendItemLabels.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            for (int i = Values.GroupCount - 1; i > 0; i--)
            {
                var values = Values.Data;
                var isValid = false;
                foreach (var value in values)
                {
                    if (value[i] != 0)
                        isValid = true;
                }
                if (!isValid)
                {
                    for (int j = 0; j < values.Count; j++)
                    {
                        values[j] = values[j].Take(values[j].Length - 1).ToArray();
                    }
                    Values.GroupLabels.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            var tmpVal = Values;
            Values = null;
            Values = tmpVal;
        }

        private void UpdateValues()
        {
            if (_isChangingTextWithCode)
            {
                return;
            }
            if (Values == null)
            {
                Values = CreateNewValues();
            }
            Values.Data.Clear();
            foreach (var item in _valuesTb)
            {
                var values = item.Select(o => o.Value ?? 0);
                Values.Data.Add(values.ToArray());
            }
            Values.GroupLabels.Clear();
            foreach (var item in _groupLabelsTb)
            {
                Values.GroupLabels.Add(item.Text);
            }
            Values.LegendItemLabels.Clear();
            foreach (var item in _serieLabelsTb)
            {
                Values.LegendItemLabels.Add(item.Text);
            }
            TrimValues();
        }

        private void UpdateLayout(Grid container, int row, int col)
        {
            ResetGrid(container, row, col);
            ClearTextBoxes();
            var firstBlock = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Color.FromArgb(0xff, 0xe4, 0xe4, 0xf0))
            };
            Grid.SetRow(firstBlock, 0);
            Grid.SetColumn(firstBlock, 0);
            container.Children.Add(firstBlock);

            for (int i = 1; i < col; i++)
            {
                var textBox = GetTextBox();
                Grid.SetRow(textBox, 0);
                Grid.SetColumn(textBox, i);
                container.Children.Add(textBox);
                _serieLabelsTb.Add(textBox);
                textBox.TextChanged += TextChanged;
            }
            for (int i = 1; i < row; i++)
            {
                var textBox = GetTextBox();
                Grid.SetRow(textBox, i);
                Grid.SetColumn(textBox, 0);
                container.Children.Add(textBox);
                _groupLabelsTb.Add(textBox);
                textBox.TextChanged += TextChanged;
            }
            for (int i = 1; i < col; i++)
            {
                var tbs = new NumberTextBox[row - 1];
                for (int j = 1; j < row; j++)
                {
                    var textBox = GetNumberTextBox();
                    Grid.SetColumn(textBox, i);
                    Grid.SetRow(textBox, j);
                    container.Children.Add(textBox);
                    tbs[j - 1] = textBox;
                    textBox.ValueChanged += DataValueChanged;
                }
                _valuesTb.Add(tbs);
            }
            _inited = true;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateValues();
        }
        private void DataValueChanged(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void ClearTextBoxes()
        {
            foreach (var item in _valuesTb)
            {
                foreach (var valueTextBox in item)
                {
                    valueTextBox.ValueChanged -= DataValueChanged;
                }
            }
            foreach (var item in _serieLabelsTb)
            {
                item.TextChanged -= TextChanged;
            }
            foreach (var item in _groupLabelsTb)
            {
                item.TextChanged -= TextChanged;
            }
            _serieLabelsTb.Clear();
            _groupLabelsTb.Clear();
            _valuesTb.Clear();
            _inited = false;
        }

        private TextBox GetTextBox()
        {
            return new TextBox()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.White),
                Padding = new Thickness(8, 10, 0, 10),
                Background = new SolidColorBrush(Color.FromArgb(0xff, 0xe4, 0xe4, 0xf0))
            };
        }

        private NumberTextBox GetNumberTextBox()
        {
            return new NumberTextBox()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.White),
                Padding = new Thickness(8, 10, 0, 10),
                Background = new SolidColorBrush(Color.FromArgb(0xff, 0xf7, 0xf5, 0xfc))
            };
        }

        private void ResetGrid(Grid container, int row, int col)
        {
            container.Children.Clear();
            var rowDef = container.RowDefinitions;
            var colDef = container.ColumnDefinitions;
            rowDef.Clear();
            colDef.Clear();
            for (int i = 0; i < row; i++)
            {
                rowDef.Add(new RowDefinition());
            }
            for (int i = 0; i < col; i++)
            {
                colDef.Add(new ColumnDefinition());
            }
        }
    }
}
