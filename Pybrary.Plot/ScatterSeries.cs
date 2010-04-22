using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Pybrary.Plot.Data;

namespace Pybrary.Plot
{
    public enum SymbolStyle
    {
        NoSymbols,

        /// <summary>
        /// Draw a symbol at every point.
        /// </summary>
        IdentifyPoints,

        /// <summary>
        /// Draw a few symbols on the line, at any location, spaced out.
        /// The purpose of the symbols is to identify multiple similar
        /// looking lines, not to identify the location of the points.
        /// </summary>
        IdentifyLine,
    };

    public class ScatterSeries : Series
    {
        private PenDescription line = new PenDescription(Color.Blue, 2f / 96);
        private PointCollection data = new PointCollection();
        private bool stepLine = false;
        private SymbolStyle symbolStyle = SymbolStyle.NoSymbols;
        private SymbolDescription symbol = new SymbolDescription(
            SymbolType.Circle,
            new PenDescription(Color.Red, 1f / 96),
            new BrushDescription(Color.Black),
            1f / 16);
        private bool appearsOnLegend = true;
        private FontDescription legendFont = new FontDescription("Arial", 12f, FontStyle.Regular);

        public ScatterSeries()
        {
            data.OnPointCollectionChanged += delegate() { raiseSeriesChanged(); };
            line.OnPenDescriptionChanged += delegate() { raiseSeriesChanged(); };
            symbol.OnSymbolDescriptionChanged += delegate() { raiseSeriesChanged(); };
        }

        public override double GetXValueByIndex(int index)
        {
            return data.GetX(index);
        }

        public override double? GetYValueByIndex(int index)
        {
            return data.GetY(index);
        }

        public override void Draw(Graphics g, AxisCollection yAxisCollection, XAxis xAxis, AdvancedRect area)
        {
            if (data.Count == 0)
                return;

            GraphicsState _s = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            NumericAxis yAxis = yAxisCollection[YAxisName];

            using (Pen p = line.CreatePen())
            using (Symbol s = (SymbolStyle != SymbolStyle.NoSymbols) ? symbol.CreateSymbol() : null)
            {
                // find continuous groups of non-null Y points
                // if our yAxis is LogRate, then we need to find continuous
                // groups of non-null, >0 Y points.  Yay.
                int start = 0, end = 0, i = 0;
                while (i < data.Count)
                {
                    // chomp all nulls
                    // then draw all non-nulls
                    while (i < data.Count && (data.GetY(i) == null || (yAxis.LogAxis && data.GetY(i) <= 0)))
                        i++;
                    // found a non-null at i.
                    start = i;
                    while (i < data.Count && data.GetY(i) != null && (!yAxis.LogAxis || data.GetY(i) > 0))
                        i++;
                    // found a null at i;
                    end = i;
                    if (start != end)
                        drawLines(g, p, s, xAxis, yAxis, area, start, end);
                }
            }

            g.Restore(_s);
        }

        private void drawLines(Graphics g, Pen p, Symbol s, XAxis xAxis, NumericAxis yAxis, AdvancedRect area, int start, int end)
        {
            PointF[] pt;
            int count = end - start;
            if (stepLine)
            {
                if (end == data.Count && count == 1)
                    // drawing a single point at the end going nowhere,
                    // not possible.  fixme: make a symbol or something to
                    // indicate a single point?
                    return;
                pt = new PointF[count * 2 - (end == data.Count ? 1 : 0)];
                for (int i = 0; i < count; i++)
                {
                    pt[i * 2].X = xAxis.DataToCoordinate(data.GetX(start + i), area);
                    pt[i * 2].Y = yAxis.DataToCoordinate(data.GetY(start + i).Value, area);
                    if ((start + i + 1) < data.Count)
                    {
                        pt[(i * 2) + 1].X = xAxis.DataToCoordinate(data.GetX(start + i + 1), area);
                        pt[(i * 2) + 1].Y = pt[i * 2].Y;
                    }
                }
            }
            else
            {
                if (count == 1)
                    // can't really draw a line here.
                    // fixme: make a symbol or something to indicate a single
                    // point?
                    return;
                pt = new PointF[count];
                for (int i = 0; i < count; i++)
                {
                    pt[i].X = xAxis.DataToCoordinate(data.GetX(start + i), area);
                    pt[i].Y = yAxis.DataToCoordinate(data.GetY(start + i).Value, area);
                }
            }
            g.DrawLines(p, pt);

            switch (SymbolStyle)
            {
                case SymbolStyle.IdentifyPoints:
                    foreach (PointF x in pt)
                        s.DrawCenteredAt(g, x);
                    break;

                case SymbolStyle.IdentifyLine:
                    PointF lastPt = pt[0];
                    for (int i = 1; i < pt.Length; i++)
                    {
                        PointF curPt = pt[i];
                        double dist = Math.Sqrt(Math.Pow(curPt.X - lastPt.X, 2) + Math.Pow(curPt.Y - lastPt.Y, 2));
                        if (dist > 0.5)
                        {
                            s.DrawCenteredAt(g, curPt);
                            lastPt = curPt;
                        }
                    }
                    break;
            }
        }

        //public override void DrawLegendEntry(Graphics g, AdvancedRect rect)
        //{
        //    using (Pen p = line.CreatePen())
        //        g.DrawLine(p,
        //            new PointF(rect.TopLeft.X, rect.Center.Y),
        //            new PointF(rect.BottomRight.X, rect.Center.Y)
        //        );
        //    if (SymbolStyle != SymbolStyle.NoSymbols)
        //    {
        //        using (Symbol sym = Symbol.CreateSymbol())
        //            sym.DrawCenteredAt(g, rect.Center);
        //    }
        //}

        public override IEnumerable<LegendEntry> LegendEntries
        {
            get
            {
                if (appearsOnLegend)
                    yield return new ScatterLegendEntry(this);
            }
        }

        private class ScatterLegendEntry : LegendEntry
        {
            private ScatterSeries series;
            public ScatterLegendEntry(ScatterSeries series)
            {
                this.series = series;
            }

            public SizeF CalculateSize(Graphics g, string seriesName)
            {
                SizeF sz;
                using (Font f = series.LegendFont.CreateFont(g))
                    sz = g.MeasureString(seriesName, f);
                sz.Width += 3f / 96; // space between swatch and name
                sz.Width += 0.2f; // swatch width
                sz.Height = Math.Max(0.2f, sz.Height); // swatch height
                sz.Width += 3f / 96; // space on right side
                return sz;
            }

            public void Draw(Graphics g, AdvancedRect area, string seriesName)
            {
                using (Pen p = series.Line.CreatePen())
                    g.DrawLine(p,
                        new PointF(area.TopLeft.X, area.Center.Y),
                        new PointF(area.TopLeft.X + 0.2f, area.Center.Y)
                    );
                if (series.SymbolStyle != SymbolStyle.NoSymbols)
                {
                    using (Symbol sym = series.Symbol.CreateSymbol())
                        sym.DrawCenteredAt(g, new PointF(area.TopLeft.X + 0.1f, area.Center.Y));
                }

                using (Font f = series.LegendFont.CreateFont(g))
                using (Brush br = series.LegendFont.CreateBrush())
                {
                    SizeF sz = g.MeasureString(seriesName, f);
                    g.DrawString(seriesName, f, br, area.TopLeft.X + 0.2f + (3f / 96), area.Center.Y - (sz.Height / 2));
                }
            }
        }

        public override int TextExport(List<string> rows, string name)
        {
            rows.Add(String.Format("{0} X\t{0} Y\t", name, name));
            for (int i = 0; i < Data.Count; i++)
            {
                string x;
                if (Data.IsSetFromDateTime)
                    x = String.Format("{0:g}", DateTime.FromOADate(Data.GetX(i)));
                else
                    x = String.Format("{0}", Data.GetX(i));
                rows.Add(String.Format("{0}\t{1}\t", x, Data.GetY(i)));
            }
            return 2;
        }

        public PenDescription Line
        {
            get
            {
                return line;
            }
        }

        public PointCollection Data
        {
            get
            {
                return data;
            }
        }

        public SymbolDescription Symbol
        {
            get
            {
                return symbol;
            }
        }

        public bool StepLine
        {
            get
            {
                return stepLine;
            }
            set
            {
                stepLine = value;
                raiseSeriesChanged();
            }
        }

        public SymbolStyle SymbolStyle
        {
            get
            {
                return symbolStyle;
            }
            set
            {
                symbolStyle = value;
                raiseSeriesChanged();
            }
        }

        public bool AppearsOnLegend
        {
            get
            {
                return appearsOnLegend;
            }
            set
            {
                appearsOnLegend = value;
                raiseSeriesChanged();
            }
        }

        public FontDescription LegendFont
        {
            get
            {
                return legendFont;
            }
        }

        public override double? MinY
        {
            get
            {
                return data.MinY;
            }
        }

        public override double? MaxY
        {
            get
            {
                return data.MaxY;
            }
        }

        public override double? MinY_gt_Zero
        {
            get
            {
                return data.MinY_gt_Zero;
            }
        }

        public override double? MinX
        {
            get
            {
                return data.MinX;
            }
        }

        public override double? MaxX
        {
            get
            {
                return data.MaxX;
            }
        }

        public override double? MinX_gt_Zero
        {
            get
            {
                return data.MinX_gt_Zero;
            }
        }
    }
}
