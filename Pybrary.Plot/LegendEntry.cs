using System;
using System.Drawing;

namespace Pybrary.Plot
{
    public interface LegendEntry
    {
        SizeF CalculateSize(Graphics g, string seriesName);
        void Draw(Graphics g, AdvancedRect area, string seriesName);
    }
}
