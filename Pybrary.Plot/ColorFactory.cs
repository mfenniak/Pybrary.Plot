using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pybrary.Plot
{
    public static class ColorFactory
    {
        public static IEnumerable<Color> StandardColors
        {
            get
            {
                yield return Color.Red;
                yield return Color.Green;
                yield return Color.Blue;
                yield return Color.Purple;
                yield return Color.Cyan;
                yield return Color.LightGreen;
                yield return Color.LightCyan;
                yield return Color.Orange;
                yield return Color.Gray;
                yield return Color.Black;
                yield return Color.DarkRed;
                yield return Color.DarkGreen;
                // Never run out - just repeat ourselves.
                foreach (Color c in ColorFactory.StandardColors)
                    yield return c;
            }
        }
    }
}
