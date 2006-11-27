using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pybrary.Plot
{
    public class Plot : EventObject
    {
        private BrushDescription background = new BrushDescription(Color.White);
        private FontDescription centerHeaderFont = new FontDescription("Arial", 16f, FontStyle.Bold);
        private string centerHeader = null;
        private FontDescription rightHeaderFont = new FontDescription("Arial", 12f, FontStyle.Bold);
        private string rightHeader = null;
        private FontDescription leftHeaderFont = new FontDescription("Arial", 12f, FontStyle.Bold);
        private string leftHeader = null;
        private PenDescription borderPen = new PenDescription(Color.Black, 1f / 96);
        private SeriesCollection series = new SeriesCollection();
        private bool displayLegend = false;
        private PenDescription legendBorder = new PenDescription(Color.Black, 1f / 96);
        private AnnotationCollection annotations = new AnnotationCollection();

        private Margins outerMargins = new Margins(0.1f, 0.1f, 0.1f, 0.1f);

        private AxisCollection yAxes;
        private XAxis xAxis;

        private AdvancedRect dataArea;

        public Plot()
        {
            UseDateTimeXAxis();

            yAxes = new AxisCollection(this);
            yAxes.AddLeft("Default");

            series.OnSeriesCollectionChanged += delegate() { raiseEvent(); };
            yAxes.OnAxisCollectionChanged += delegate() { raiseEvent(); };
            background.OnBrushDescriptionChanged += delegate() { raiseEvent(); };
            annotations.OnChanged += delegate(object sender, EventArgs args) { raiseEvent(); };
        }

        public void UseDateTimeXAxis()
        {
            xAxis = new DateTimeAxis(this);
            xAxis.OnChanged += delegate(object sender, EventArgs args) { raiseEvent(); };
            raiseEvent();
        }

        public void UseTimespanXAxis()
        {
            xAxis = new TimespanAxis(this);
            xAxis.OnChanged += delegate(object sender, EventArgs args) { raiseEvent(); };
            raiseEvent();
        }

        public void UseNumericXAxis()
        {
            xAxis = new NumericXAxis(this);
            xAxis.OnChanged += delegate(object sender, EventArgs args) { raiseEvent(); };
            raiseEvent();
        }

        public void PaintOn(Graphics g, RectangleF _area)
        {
            GraphicsState _s = g.Save();

            // _area is assumed to be provided in the same coordinate system as
            // g.PageUnit provides.  The plotting library works assuming that
            // PageUnit is Inch, so we must transform the Rectangle into the
            // appropriate rect.
            PointF[] trans = new PointF[] {
                new PointF(_area.Left, _area.Top),
                new PointF(_area.Left + _area.Width, _area.Top + _area.Height)
            };
            g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.Page, trans);
            g.PageUnit = GraphicsUnit.Inch;
            g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Device, trans);
            AdvancedRect area = new AdvancedRect(trans[0], trans[1]);

            using (Brush bg = background.CreateBrush())
                g.FillRectangle(bg, area.Rect);

            outerMargins.Apply(ref area);

            //using (Brush tmp = new SolidBrush(Color.Red))
            //    g.FillRectangle(tmp, area.Rect);

            SizeF headerSize = new SizeF(0, 0);
            if (centerHeader != null)
            {
                using (Brush br = centerHeaderFont.CreateBrush())
                using (Font f = centerHeaderFont.CreateFont())
                {
                    headerSize = g.MeasureString(centerHeader, f);
                    g.DrawString(centerHeader, f, br, area.Center.X - (headerSize.Width / 2), area.TopLeft.Y);
                }
            }
            if (rightHeader != null || leftHeader != null)
            {
                using (Brush left_br = leftHeaderFont.CreateBrush())
                using (Font left_f = leftHeaderFont.CreateFont())
                using (Brush right_br = rightHeaderFont.CreateBrush())
                using (Font right_f = rightHeaderFont.CreateFont())
                {
                    SizeF leftHeaderSize = (leftHeader != null) ? g.MeasureString(leftHeader, left_f) : new SizeF(0, 0);
                    SizeF rightHeaderSize = (rightHeader != null) ? g.MeasureString(rightHeader, right_f) : new SizeF(0, 0);

                    // determine y location = is this going to be aligned to the bottom of the center
                    // header, or underneath it?  underneath is necessary if the two will hit each other.
                    float left_xLoc = area.TopLeft.X;
                    float right_xLoc = area.BottomRight.X - rightHeaderSize.Width;
                    float left_yLoc;
                    float right_yLoc;
                    if ((right_xLoc - 0.2f) < (area.Center.X + (headerSize.Width / 2)) ||
                        (left_xLoc + leftHeaderSize.Width + 0.2f) > (area.Center.X - (headerSize.Width / 2)))
                    {
                        // looks like we're too close - have to move down a bit.
                        left_yLoc = right_yLoc = area.TopLeft.Y + headerSize.Height;
                        headerSize.Height += Math.Max(rightHeaderSize.Height, leftHeaderSize.Height);
                    }
                    else
                    {
                        // We're good to place it next to the header.
                        // But, check our height - are the side headers bigger than the center header?
                        if (rightHeaderSize.Height > headerSize.Height || leftHeaderSize.Height > headerSize.Height)
                        {
                            // the side headers are bigger than the center header.  Make enough room.
                            headerSize.Height = Math.Max(leftHeaderSize.Height, rightHeaderSize.Height);
                        }
                        right_yLoc = area.TopLeft.Y + (headerSize.Height - rightHeaderSize.Height);
                        left_yLoc = area.TopLeft.Y + (headerSize.Height - leftHeaderSize.Height);
                    }
                    if (rightHeader != null)
                        g.DrawString(rightHeader, right_f, right_br, right_xLoc, right_yLoc);
                    if (leftHeader != null)
                        g.DrawString(leftHeader, left_f, left_br, left_xLoc, left_yLoc);
                }
            }
            if (headerSize != null)
            {
                area.TopLeft.Y += headerSize.Height;
                area.TopLeft.Y += 0.1f;
            }

            if (displayLegend)
            {
                area.BottomRight.Y -= DrawLegend(g, area);
            }

            float yAxisWidthLeft = yAxes.CalculateWidthLeft(g);
            float yAxisWidthRight = yAxes.CalculateWidthRight(g);
            float xAxisHeight = xAxis.CalculateHeight(g, area.Width - yAxisWidthLeft - yAxisWidthRight);

            AdvancedRect xAxisArea = area.Clone();
            xAxisArea.TopLeft.X += yAxisWidthLeft;
            xAxisArea.BottomRight.X -= yAxisWidthRight;
            xAxisArea.TopLeft.Y = xAxisArea.BottomRight.Y - xAxisHeight;

            // calculate internal data area
            area.TopLeft.X += yAxisWidthLeft;
            area.BottomRight.X -= yAxisWidthRight;
            area.BottomRight.Y -= xAxisHeight;

            yAxes.Draw(g, area);
            xAxis.DrawX(g, xAxisArea, area);

            series.Draw(g, yAxes, xAxis, area);

            annotations.Draw(g, area);

            using (Pen p = borderPen.CreatePen())
            {
                g.DrawLine(p, area.TopLeft.X, area.TopLeft.Y, area.TopLeft.X, area.BottomRight.Y);
                g.DrawLine(p, area.TopLeft.X, area.TopLeft.Y, area.BottomRight.X, area.TopLeft.Y);
                g.DrawLine(p, area.BottomRight.X, area.BottomRight.Y, area.BottomRight.X, area.TopLeft.Y);
                g.DrawLine(p, area.BottomRight.X, area.BottomRight.Y, area.TopLeft.X, area.BottomRight.Y);
            }

            dataArea = area;

            g.Restore(_s);
        }

        private float DrawLegend(Graphics g, AdvancedRect area)
        {
            // function returns the height necessary for the legend.

            GraphicsState _s = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float maxWidth = 0, maxHeight = 0;
            List<LegendEntry> entries = new List<LegendEntry>();
            List<string> seriesNames = new List<string>();
            foreach (KeyValuePair<string, Series> kv in series)
            {
                foreach (LegendEntry entry in kv.Value.LegendEntries)
                {
                    entries.Add(entry);
                    seriesNames.Add(kv.Key);
                    SizeF sz = entry.CalculateSize(g, kv.Key);
                    maxWidth = Math.Max(maxWidth, sz.Width);
                    maxHeight = Math.Max(maxHeight, sz.Height);
                }
            }

            //float totalWidth = maxWidth * entries.Count;
            int entriesPerRow = (int)Math.Floor(area.Width / maxWidth);
            if (entriesPerRow == 0)
                entriesPerRow = 1;
            else if (entriesPerRow > entries.Count)
                entriesPerRow = entries.Count;
            int numRows = (int)Math.Ceiling((float)entries.Count / entriesPerRow);
            //int entriesPerRow = (int)Math.Ceiling((float)entries.Count / numRows);

            float leftX = area.Center.X - ((float)entriesPerRow / 2) * maxWidth;

            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < entriesPerRow; c++)
                {
                    int idx = (r * entriesPerRow) + c;
                    if (idx >= entries.Count)
                        continue;

                    LegendEntry entry = entries[idx];
                    entry.Draw(g, new AdvancedRect(
                        new PointF(leftX + (c * maxWidth), area.BottomRight.Y - (maxHeight * (numRows - r))),
                        new PointF(leftX + ((c + 1) * maxWidth), area.BottomRight.Y - (maxHeight * (numRows - r - 1)))),
                        seriesNames[idx]
                    );
                }
            }

            g.Restore(_s); // turn off antialiasing

            /*
            using (Pen p = legendBorder.CreatePen())
            {
                g.DrawRectangle(p, 
                    area.Center.X - (width / 2), area.BottomRight.Y - height,
                    width, height);
            }
            */

            return maxHeight * numRows;
        }

        public SeriesCollection Series
        {
            get
            {
                return series;
            }
        }

        public AxisCollection YAxes
        {
            get
            {
                return yAxes;
            }
        }

        public XAxis XAxis
        {
            get
            {
                return xAxis;
            }
        }

        public TimespanAxis TimeSpanXAxis
        {
            get
            {
                return xAxis as TimespanAxis;
            }
        }

        public DateTimeAxis DateTimeXAxis
        {
            get
            {
                return xAxis as DateTimeAxis;
            }
        }

        public NumericXAxis NumericXAxis
        {
            get
            {
                return xAxis as NumericXAxis;
            }
        }

        public string HeaderCenter
        {
            get
            {
                return centerHeader;
            }
            set
            {
                centerHeader = value;
                raiseEvent();
            }
        }

        public string HeaderRight
        {
            get
            {
                return rightHeader;
            }
            set
            {
                rightHeader = value;
                raiseEvent();
            }
        }

        public string HeaderLeft
        {
            get
            {
                return leftHeader;
            }
            set
            {
                leftHeader = value;
                raiseEvent();
            }
        }

        public AdvancedRect DataArea
        {
            get
            {
                return dataArea.Clone();
            }
        }

        public PenDescription BorderPen
        {
            get
            {
                return borderPen;
            }
        }

        public bool DisplayLegend
        {
            get
            {
                return displayLegend;
            }
            set
            {
                displayLegend = value;
                raiseEvent();
            }
        }

        public AnnotationCollection Annotations
        {
            get
            {
                return annotations;
            }
        }
    }
}
