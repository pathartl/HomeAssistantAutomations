using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Models
{
    public class Color
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }

        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Lightness { get; set; }

        public string Hex { get; set; }

        public Color() { }

        public Color(string hex) {
            Hex = hex;
        }

        public Color(int r, int g, int b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public Color(double h, double s, double l)
        {
            Hue = h;
            Saturation = s;
            Lightness = l;
        }

        public Color HexToRGB()
        {
            if (Hex.StartsWith("#"))
                Hex = Hex.TrimStart('#');

            Red = int.Parse(Hex.Substring(0, 2), NumberStyles.HexNumber);
            Green = int.Parse(Hex.Substring(2, 2), NumberStyles.HexNumber);
            Blue = int.Parse(Hex.Substring(4, 2), NumberStyles.HexNumber);

            return this;
        }

        public Color RGBToHex()
        {
            Hex = ((1 << 24) + (Red << 16) + (Blue << 8) + Green).ToString();

            return this;
        }

        public Color RGBToHSL()
        {
            var r = (double)Red / 255;
            var g = (double)Green / 255;
            var b = (double)Blue / 255;

            var rgb = new double[] { r, g, b };

            var max = rgb.Max();
            var min = rgb.Min();

            var h = (max + min) / 2;
            var s = (max + min) / 2;
            var l = (max + min) / 2;

            if (max == min)
            {
                h = 0;
                s = 0;
                // achromatic
            }
            else
            {
                var d = max - min;

                s = (l > .5 ? d / (2 - max - min) : d / (max + min));

                if (max == r)
                    h = (g - b) / d + (g < b ? 6 : 0);

                else if (max == g)
                    h = (b - r) / d + 2;

                else if (max == b)
                    h = (r - g) / d + 4;

                h /= 6;
            }

            Hue = h;
            Saturation = s;
            Lightness = l;

            return this;
        }

        public Color HSLToRGB()
        {
            var h = Hue;
            var s = Saturation;
            var l = Lightness;

            if (s == 0)
            {
                l = Math.Round(l * 255);

                Hue = l;
                Saturation = l;
                Lightness = l;

                return this;
            }
            else
            {
                var q = (l < 0.5 ? l * (1 + s) : l + s - l * s);
                var p = 2 * l - q;

                Red = (int)Math.Round(HueToRGB(p, q, h + 1 / 3) * 255);
                Green = (int)Math.Round(HueToRGB(p, q, h));
                Blue = (int)Math.Round(HueToRGB(p, q, h - 1 / 3));

                return this;
            }
        }

        private double HueToRGB(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6) return p + (q - p) * 6 * t;
            if (t < 1 / 2) return q;
            if (t < 2 / 3) return p + (q - p) * (2/3 - t) * 6;

            return p;
        }

        public static Color InterpolateRGB(Color from, Color to, double factor = 0.5)
        {
            var interpolatedColor = new Color();

            interpolatedColor.Red = (int)Math.Round(from.Red + factor * (to.Red - from.Red));
            interpolatedColor.Blue = (int)Math.Round(from.Blue + factor * (to.Blue - from.Blue));
            interpolatedColor.Green = (int)Math.Round(from.Green + factor * (to.Green - from.Green));

            return interpolatedColor;
        }

        public static Color InterpolateHSL(Color from, Color to, double factor = 0.5)
        {
            var interpolatedColor = new Color();

            from = from.RGBToHSL();
            to = to.RGBToHSL();

            interpolatedColor.Hue = from.Hue;
            interpolatedColor.Saturation = from.Saturation;
            interpolatedColor.Lightness = from.Lightness;

            interpolatedColor.Hue += factor * (to.Hue - from.Hue);
            interpolatedColor.Saturation += factor * (to.Saturation - from.Saturation);
            interpolatedColor.Lightness += factor * (to.Lightness - from.Lightness);

            return interpolatedColor.HSLToRGB();
        }

        public static IEnumerable<Color> InterpolateColors(Color from, Color to, int steps)
        {
            var colors = new List<Color>();

            var factorStep = 1 / (double)(steps - 1);

            for (int i = 0; i < steps; i++)
            {
                colors.Add(InterpolateRGB(from, to, factorStep * i));
            }

            return colors;
        }
    }
}
