using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SimplePaletteQuantizer.Quantizers
{
    /// <summary>
    /// The simple (slightly tweaked) palette quantizer implementation.
    /// </summary>
    public class PaletteQuantizer : IColorQuantizer
    {
        private readonly List<Color> palette;
        private readonly Dictionary<Color, Byte> cache;
        private readonly Dictionary<Int32, Int32> colorMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteQuantizer"/> class.
        /// </summary>
        public PaletteQuantizer()
        {
            palette = new List<Color>();
            cache = new Dictionary<Color, Byte>();
            colorMap = new Dictionary<Int32, Int32>();
        }

        /// <summary>
        /// Adds the color to quantizer, only unique colors are added.
        /// </summary>
        /// <param name="color">The color to be added.</param>
        public void AddColor(Color color)
        {
            Int32 argb = color.ToArgb();

            if (colorMap.ContainsKey(argb))
            {
                colorMap[argb]++;
            }
            else
            {
                colorMap.Add(argb, 1);
            }
        }

        /// <summary>
        /// Gets the palette with a specified count of the colors.
        /// </summary>
        /// <param name="colorCount">The color count.</param>
        /// <returns></returns>
        public List<Color> GetPalette(Int32 colorCount)
        {
            palette.Clear();

            // lucky seed :)
            Random random = new Random(13);

            // shuffles the colormap
            IEnumerable<Color> colors = colorMap.
                OrderBy(entry => random.NextDouble()).
                Select(entry => Color.FromArgb(entry.Key));

            // if there're less colors in the image then allowed, simply pass them all
            if (colorMap.Count > colorCount)
            {
                // solves the color quantization
                colors = SolveRootLevel(colorCount, colors);

                // if there're still too much colors, just snap them from the top))
                if (colors.Count() > colorCount)
                {
                    colors.OrderBy(color => colorMap[color.ToArgb()]);
                    colors = colors.Take(colorCount);
                }
            }

            // clears the hit cache
            cache.Clear();

            // adds the selected colors to a final palette
            palette.AddRange(colors);

            // returns our new palette
            return palette;
        }

        /// <summary>
        /// Gets the index of the palette for specific color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public Byte GetPaletteIndex(Color color)
        {
            Byte result;

            // checks whether color was already requested, in that case returns an index from a cache
            if (cache.ContainsKey(color))
            {
                result = cache[color];
            }
            else
            {
                // otherwise finds the nearest color
                result = (Byte) GetNearestColor(color, palette);
                cache[color] = result;
            }

            // returns a palette index
            return result; 
        }

        /// <summary>
        /// Gets the color count.
        /// </summary>
        /// <returns></returns>
        public Int32 GetColorCount()
        {
            return colorMap.Count;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            // clears all the information
            cache.Clear();
            colorMap.Clear();
        }

        #region | Helper methods and classes |

        private static IEnumerable<Color> SolveRootLevel(int colorCount, IEnumerable<Color> colors)
        {
            // initializes the comparers based on hue, saturation and brightness (HSB color model)
            ColorHueComparer hueComparer = new ColorHueComparer();
            ColorSaturationComparer saturationComparer = new ColorSaturationComparer();
            ColorBrightnessComparer brightnessComparer = new ColorBrightnessComparer();

            // selects three palettes: 1) hue is unique, 2) saturation is unique, 3) brightness is unique
            IEnumerable<Color> hueColors = colors.Distinct(hueComparer);
            IEnumerable<Color> saturationColors = colors.Distinct(saturationComparer);
            IEnumerable<Color> brightnessColors = colors.Distinct(brightnessComparer);

            // retrieves number of colors, ie. how many we've eliminated
            Int32 hueColorCount = hueColors.Count();
            Int32 saturationColorCount = saturationColors.Count();
            Int32 brightnessColorCount = brightnessColors.Count();

            // selects the palette (from those 3) which has the most colors, because an image has some details in that category
            if (hueColorCount > saturationColorCount && hueColorCount > brightnessColorCount)
            {
                colors = Solve2ndLevel(hueColors, saturationComparer, brightnessComparer, colorCount);
            }
            else if (saturationColorCount < hueColorCount && saturationColorCount < brightnessColorCount)
            {
                colors = Solve2ndLevel(saturationColors, hueComparer, brightnessComparer, colorCount);
            }
            else
            {
                colors = Solve2ndLevel(brightnessColors, hueComparer, saturationComparer, colorCount);
            }

            return colors;
        }

        private static IEnumerable<Color> Solve2ndLevel(IEnumerable<Color> defaultColors, IEqualityComparer<Color> firstComparer, IEqualityComparer<Color> secondComparer, Int32 colorCountLimit)
        {
            IEnumerable<Color> result = defaultColors;

            if (result.Count() > colorCountLimit)
            {
                IEnumerable<Color> firstColors = result.Distinct(firstComparer);
                IEnumerable<Color> secondColors = result.Distinct(secondComparer);

                if (firstColors.Count() > secondColors.Count())
                {
                    result = Solve3rdLevel(firstColors, secondComparer, colorCountLimit);
                }
                else
                {
                    result = Solve3rdLevel(secondColors, firstComparer, colorCountLimit);
                }
            }

            return result;
        }

        private static IEnumerable<Color> Solve3rdLevel(IEnumerable<Color> defaultColors, IEqualityComparer<Color> secondComparer, Int32 colorCountLimit)
        {
            IEnumerable<Color> result = defaultColors;

            if (result.Count() > colorCountLimit)
            {
                IEnumerable<Color> colors = result.Distinct(secondComparer);

                if (colors.Count() >= colorCountLimit)
                {
                    result = colors;
                }
            }

            return result;
        }

        private static Int32 GetNearestColor(Color color, IList<Color> palette)
        {
            Int32 bestIndex = 0;
            Int32 bestFactor = Int32.MaxValue;

            for (Int32 index = 0; index < palette.Count; index++)
            {
                Color targetColor = palette[index];

                Int32 deltaA = color.A - targetColor.A;
                Int32 deltaR = color.R - targetColor.R;
                Int32 deltaG = color.G - targetColor.G;
                Int32 deltaB = color.B - targetColor.B;

                Int32 factorA = deltaA * deltaA;
                Int32 factorR = deltaR * deltaR;
                Int32 factorG = deltaG * deltaG;
                Int32 factorB = deltaB * deltaB;

                Int32 factor = factorA + factorR + factorG + factorB;

                if (factor == 0) return index;

                if (factor < bestFactor)
                {
                    bestFactor = factor;
                    bestIndex = index;
                }
            }

            return bestIndex;
        }

        private class ColorHueComparer : IEqualityComparer<Color>
        {
            public Boolean Equals(Color x, Color y)
            {
                return x.GetHue() == y.GetHue();
            }

            public Int32 GetHashCode(Color color)
            {
                return color.GetHue().GetHashCode();
            }
        }

        private class ColorSaturationComparer : IEqualityComparer<Color>
        {
            public Boolean Equals(Color x, Color y)
            {
                return x.GetSaturation() == y.GetSaturation();
            }

            public Int32 GetHashCode(Color color)
            {
                return color.GetSaturation().GetHashCode();
            }
        }

        private class ColorBrightnessComparer : IEqualityComparer<Color>
        {
            public Boolean Equals(Color x, Color y)
            {
                return x.GetBrightness() == y.GetBrightness();
            }

            public Int32 GetHashCode(Color color)
            {
                return color.GetBrightness().GetHashCode();
            }
        }

        #endregion
    }
}


