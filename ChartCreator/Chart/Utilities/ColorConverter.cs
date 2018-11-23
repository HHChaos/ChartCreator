using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ChartCreator.Chart.Utilities
{
    public static class ColorConverter
    {
        public static Color ConvertHexToColor(string hex)
        {
            hex = hex.Remove(0, 1);
            byte a = hex.Length == 8 ? Byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) : (byte)255;
            byte r = Byte.Parse(hex.Substring(hex.Length - 6, 2), NumberStyles.HexNumber);
            byte g = Byte.Parse(hex.Substring(hex.Length - 4, 2), NumberStyles.HexNumber);
            byte b = Byte.Parse(hex.Substring(hex.Length - 2), NumberStyles.HexNumber);
            return Color.FromArgb(a, r, g, b);
        }

        public static string ConvertColorToHex(Color color)
        {
            return $"#{color.A}{color.R}{color.G}{color.B}";
        }
    }
}
