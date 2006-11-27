using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Pybrary.Plot
{
    public interface YAxis
    {
        float CalculateWidth(Graphics g);
        void DrawY(Graphics g, AdvancedRect area, AdvancedRect plotArea);

        float DataToCoordinate(double v, AdvancedRect rect);
        double CoordinateToData(float x, AdvancedRect rect);

        double? ZoomedMinimum { get; set; }
        double? ZoomedMaximum { get; set; }

        AdvancedRect? DrawArea { get; }
    }
}
