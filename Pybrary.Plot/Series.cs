using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pybrary.Plot
{
    public delegate void SeriesEventHandler();

    public abstract class Series
    {
        private string yAxisName = "Default";

        public event SeriesEventHandler OnSeriesChanged;

        public Series()
        {
        }

        public string YAxisName
        {
            get
            {
                return yAxisName;
            }
            set
            {
                yAxisName = value;
                raiseSeriesChanged();
            }
        }

        protected void raiseSeriesChanged()
        {
            SeriesEventHandler tmp = OnSeriesChanged;
            if (tmp != null)
                tmp();
        }

        public abstract double GetXValueByIndex(int index);
        public abstract double? GetYValueByIndex(int index);

        public abstract double? MinY { get; }
        public abstract double? MinY_gt_Zero { get; }
        public abstract double? MaxY { get; }
        public abstract double? MinX { get; }
        public abstract double? MinX_gt_Zero { get; }
        public abstract double? MaxX { get; }

        public abstract void Draw(Graphics g, AxisCollection yAxisCollection, XAxis xAxis, AdvancedRect area);
        public abstract int TextExport(List<string> rows, string name);

        public abstract IEnumerable<LegendEntry> LegendEntries { get; }
    }
}
