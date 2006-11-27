using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Pybrary.Plot
{
    public interface XAxis
    {
        event EventHandler OnChanged;

        float CalculateHeight(Graphics g, float maximumWidth);
        void DrawX(Graphics g, AdvancedRect area, AdvancedRect plotArea);
        float DataToCoordinate(double v, AdvancedRect rect);
        double CoordinateToData(float x, AdvancedRect rect);

        double? UserMinimum { get; set; }
        double? UserMaximum { get; set; }
        double? ZoomedMinimum { get; set; }
        double? ZoomedMaximum { get; set; }

        AdvancedRect? DrawArea { get; }
    }
}
