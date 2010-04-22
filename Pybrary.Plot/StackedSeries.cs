using System;
using System.Collections.Generic;
using System.Text;
using Pybrary.Plot.Data;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pybrary.Plot
{
    public class StackedSeries : Series
    {
        private MultipointCollection data = new MultipointCollection();
        private IList<string> datasetNames = new List<string>();
        private IList<BrushDescription> brushes = new List<BrushDescription>();
        private IList<PenDescription> pens = new List<PenDescription>();
        private FontDescription legendFont = new FontDescription("Arial", 12f, FontStyle.Regular);

        private double? minY_gt_Zero;
        private double? maxY;

        public StackedSeries()
        {
            data.OnChanged += delegate(object sender, EventArgs a)
            {
                minY_gt_Zero = null;
                maxY = null;
                raiseSeriesChanged();
            };
        }

        public override double GetXValueByIndex(int index)
        {
            return data.GetX(index);
        }

        public override double? GetYValueByIndex(int index)
        {
            double?[] yValues = data.GetY(index);
            double sum = 0;
            bool hasValue = false;
            foreach (double? y in yValues)
            {
                if (y.HasValue)
                {
                    hasValue = true;
                    sum += y.Value;
                }
            }
            if (hasValue)
                return sum;
            else
                return null;
        }

        public override void Draw(Graphics g, AxisCollection yAxisCollection, XAxis xAxis, AdvancedRect area)
        {
            if (data.Count == 0)
                return;

            GraphicsState _s = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            NumericAxis yAxis = yAxisCollection[YAxisName];

            double[] baseY = new double[data.Count];
            if (yAxis.LogAxis)
            {
                for (int i = 0; i < data.Count; i++)
                    baseY[i] = yAxis.ScaleMinimum;
            }
            else
            {
                for (int i = 0; i < data.Count; i++)
                    baseY[i] = 0;
            }

            for (int ySet = 0; ySet < data.GetY(0).Length; ySet++)
            {
                PointF[] polygon = new PointF[data.Count * 2];
                PointF[] line = new PointF[data.Count];
                int j = 0;
                for (int i = (data.Count - 1); i >= 0; i--, j++)
                {
                    polygon[j] = new PointF(
                        xAxis.DataToCoordinate(data.GetX(i), area),
                        yAxis.DataToCoordinate(baseY[i], area)
                    );
                }
                for (int i = 0; i < data.Count; i++, j++)
                {
                    double? y = data.GetY(i)[ySet];
                    if (y == null)
                        y = baseY[i];
                    else
                        y = baseY[i] + y.Value;
                    polygon[j] = new PointF(
                        xAxis.DataToCoordinate(data.GetX(i), area),
                        yAxis.DataToCoordinate(y.Value, area)
                    );
                    line[i] = polygon[j];
                    baseY[i] = y.Value;
                }

                if (brushes.Count != 0)
                {
                    int idx = ySet % brushes.Count;
                    using (Brush br = brushes[idx].CreateBrush())
                        g.FillPolygon(br, polygon);
                }
                if (pens.Count != 0 && line.Length >= 2)
                {
                    int idx = ySet % pens.Count;
                    using (Pen p = pens[idx].CreatePen())
                        g.DrawLines(p, line);
                }
            }

            g.Restore(_s);
        }

        public override IEnumerable<LegendEntry> LegendEntries
        {
            get
            {
                if (data.Count == 0)
                    yield break;
                for (int ySet = 0; ySet < data.GetY(0).Length; ySet++)
                    yield return new StackedLegendEntry(this, ySet);
            }
        }

        public override int TextExport(List<string> rows, string name)
        {
            if (data.Count == 0)
                return 0;

            string title = String.Format("{0} X\t", name);
            for (int c = 0; c < data.GetY(0).Length; c++)
            {
                string datasetname;
                if (c < DatasetNames.Count)
                    datasetname = DatasetNames[c];
                else
                    datasetname = String.Format("Dataset {0}", c);
                title += String.Format("{0}\t", datasetname);
            }
            rows.Add(title);

            for (int r = 0; r < Data.Count; r++)
            {
                string row;
                if (Data.IsSetFromDateTime)
                    row = String.Format("{0:g}", DateTime.FromOADate(Data.GetX(r)));
                else
                    row = String.Format("{0}", Data.GetX(r));
                row += "\t";
                for (int c = 0; c < data.GetY(0).Length; c++)
                {
                    row += String.Format("{0}\t", data.GetY(r)[c]);
                }
                rows.Add(row);
            }
            return data.GetY(0).Length + 1;
        }

        private void calculateMinMax()
        {
            minY_gt_Zero = null;
            maxY = null;
            for (int i = 0; i < Data.Count; i++)
            {
                double?[] yValues = Data.GetY(i);
                if (yValues[0] != null && yValues[0] > 0 && (minY_gt_Zero == null || yValues[0] < minY_gt_Zero))
                    minY_gt_Zero = yValues[0];
                double? sum = null;
                for (int j = 0; j < yValues.Length; j++)
                {
                    double? y = yValues[j];
                    if (y.HasValue)
                    {
                        if (sum == null)
                            sum = y;
                        else
                            sum += y;
                    }
                }
                if (sum != null && (maxY == null || sum > maxY))
                    maxY = sum;
            }
        }

        public MultipointCollection Data
        {
            get
            {
                return data;
            }
        }

        public override double? MinY
        {
            get
            {
                return 0;
            }
        }

        public override double? MinY_gt_Zero
        {
            get
            {
                if (minY_gt_Zero == null)
                    calculateMinMax();
                return minY_gt_Zero;
            }
        }

        public override double? MaxY
        {
            get
            {
                if (maxY == null)
                    calculateMinMax();
                return maxY;
            }
        }

        public override double? MinX
        {
            get
            {
                return Data.MinX;
            }
        }

        public override double? MinX_gt_Zero
        {
            get
            {
                return Data.MinX_gt_Zero;
            }
        }

        public override double? MaxX
        {
            get
            {
                return Data.MaxX;
            }
        }

        public IList<BrushDescription> Brushes
        {
            get
            {
                return brushes;
            }
        }

        public IList<PenDescription> Pens
        {
            get
            {
                return pens;
            }
        }

        public IList<string> DatasetNames
        {
            get
            {
                return datasetNames;
            }
        }

        public FontDescription LegendFont
        {
            get
            {
                return legendFont;
            }
        }

        private class StackedLegendEntry : LegendEntry
        {
            private StackedSeries series;
            private int ySet;
            public StackedLegendEntry(StackedSeries series, int ySet)
            {
                this.series = series;
                this.ySet = ySet;
            }

            public string Name
            {
                get
                {
                    string name;
                    if (ySet < series.DatasetNames.Count)
                        name = series.DatasetNames[ySet];
                    else
                        name = String.Format("Dataset {0}", ySet);
                    return name;
                }
            }

            public SizeF CalculateSize(Graphics g, string seriesName)
            {
                SizeF sz;
                using (Font f = series.LegendFont.CreateFont(g))
                    sz = g.MeasureString(Name, f);
                sz.Width += 3f / 96; // space between swatch and name
                sz.Width += 0.2f; // swatch width
                sz.Height = Math.Max(0.2f, sz.Height); // swatch height
                sz.Width += 3f / 96; // space on right side
                return sz;
            }

            public void Draw(Graphics g, AdvancedRect area, string seriesName)
            {
                int idx = ySet % series.Brushes.Count;
                using (Brush br = series.Brushes[idx].CreateBrush())
                    g.FillRectangle(br, new RectangleF(area.TopLeft.X, area.TopLeft.Y, 0.2f, 0.2f));

                using (Font f = series.LegendFont.CreateFont(g))
                using (Brush br = series.LegendFont.CreateBrush())
                {
                    SizeF sz = g.MeasureString(Name, f);
                    g.DrawString(Name, f, br, area.TopLeft.X + 0.2f + (3f / 96), area.Center.Y - (sz.Height / 2));
                }
            }
        }
    }
}
