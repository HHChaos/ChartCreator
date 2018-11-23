using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace ChartCreator.Sample.Controls
{
    public class NumberTextBox : TextBox
    {
        public event EventHandler ValueChanged;

        public NumberTextBox()
        {
            this.Loaded += NumberTextBox_Loaded;
        }

        private void NumberTextBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.TextChanged += NumberTextBox_TextChanged;
            this.KeyDown += NumberTextBox_KeyDown;
            this.GotFocus += NumberTextBox_GotFocus;
            this.LostFocus += NumberTextBox_LostFocus;
            this.Unloaded += NumberTextBox_Unloaded;
        }

        private void NumberTextBox_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.SelectAll();
        }

        private void NumberTextBox_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.UpdateValueText();
        }

        private void NumberTextBox_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.TextChanged -= NumberTextBox_TextChanged;
            this.KeyDown -= NumberTextBox_KeyDown;
            this.GotFocus -= NumberTextBox_GotFocus;
            this.LostFocus -= NumberTextBox_LostFocus;
            this.Loaded -= NumberTextBox_Loaded;
            this.Unloaded -= NumberTextBox_Unloaded;
        }

        private void NumberTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.UpdateValueText();
                this.SelectAll();
                e.Handled = true;
            }
        }

        private bool _isChangingTextWithCode;
        private bool _isChangingValueWithCode;
        private float? _value = null;

        public float? Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                UpdateValueText();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateValueFromText();
        }

        private bool UpdateValueFromText()
        {
            if (_isChangingTextWithCode)
            {
                return false;
            }
            if (float.TryParse(this.Text, NumberStyles.Any, CultureInfo.CurrentUICulture, out float val))
            {
                if (val >= 0)
                {
                    _isChangingValueWithCode = true;
                    Value = val;
                    _isChangingValueWithCode = false;
                    return true;
                }
            }
            else if (string.IsNullOrEmpty(this.Text))
            {
                Value = null;
                return true;
            }
            return false;
        }

        private void UpdateValueText()
        {
            if (_isChangingValueWithCode)
            {
                return;
            }
            _isChangingTextWithCode = true;
            this.Text = this.Value?.ToString() ?? string.Empty;
            _isChangingTextWithCode = false;
        }
    }
}
