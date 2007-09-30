using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Pybrary.Plot
{
    public class DateTimeAxis : Axis, XAxis
    {
        // priority of scales - zoomed, user, data, unscaled

        private DateTime? zoomedMinimum = null;
        private DateTime? zoomedMaximum = null;
        private DateTime? userMinimum = null;
        private DateTime? userMaximum = null;
        private DateTime unscaledMinimum = new DateTime(2000, 1, 1);
        private DateTime unscaledMaximum = new DateTime(2000, 12, 31);
        private float minorTickLength = 0.025f; // in inches
        private PenDescription minorTickPen = new PenDescription(Color.Black, 1f / 96);
        private FontDescription smallLabelFont = new FontDescription("Arial", 8f, FontStyle.Regular);

        private Plot parent;

        private enum AxisType {
            MonthsHorizontalWithDailyLabels,
            MonthsHorizontalWithDailyTicks,
            MonthsHorizontal,
            MonthsVertical,
            Quarters
        };
        private AxisType axisType = AxisType.MonthsHorizontalWithDailyTicks;

        private GregorianCalendar cal = new GregorianCalendar();

        public DateTimeAxis(Plot parent)
        {
            this.parent = parent;
        }

        public float CalculateHeight(Graphics g, float maximumWidth)
        {
            float height = 0;

            axisType = AxisType.MonthsHorizontalWithDailyLabels;

            using (Font f = labelFont.CreateFont())
            using (Font f2 = smallLabelFont.CreateFont())
            {
                SizeF labelSize = g.MeasureString("Mar '05", f);
                int numLabels = calculateNumLabels();

                float estimatedWidth = (labelSize.Width + 0.3f) * numLabels;
                if (estimatedWidth < maximumWidth)
                {
                    // monthly horizontal labels
                    height += labelSize.Height;

                    SizeF dayLabelSize = g.MeasureString("30", f2);

                    // do we also have room for daily ticks?
                    int numDays = (ScaleMaximum - ScaleMinimum).Days;
                    if (numDays * dayLabelSize.Width < maximumWidth)
                    {
                        axisType = AxisType.MonthsHorizontalWithDailyLabels;
                        height += 1f / 16;
                    }
                    else if ((0.05f * numDays) < maximumWidth)
                        axisType = AxisType.MonthsHorizontalWithDailyTicks;
                    else
                        axisType = AxisType.MonthsHorizontal;
                }
                else
                {
                    // our total width is going to be too wide.  fallback #1 -
                    // vertically drawn labels

                    estimatedWidth = (labelSize.Height + 0.3f) * numLabels;
                    if (estimatedWidth < maximumWidth)
                    {
                        // vertical labels will work fine.
                        axisType = AxisType.MonthsVertical;
                        height += labelSize.Width;
                    }
                    else
                    {
                        // still too wide - switch to "Q1 '04", "Q2 '04", ect.
                        axisType = AxisType.Quarters;
                        labelSize = g.MeasureString("Q1 '05", f);
                        height += labelSize.Width;

                        // future work - switch to just the year, then the decade
                        // if ever necessary
                    }
                }
            }

            // tick marks
            height += tickLength;

            return height;
        }

        private int calculateNumLabels()
        {
            DateTime v = ScaleMinimum;
            int i;
            for (i = 0; ; i++)
            {
                if (v > ScaleMaximum)
                    break;
                v = cal.AddMonths(v, (axisType == AxisType.Quarters) ? 3 : 1);
            }
            return i + 1;
        }

        public void DrawX(Graphics g, AdvancedRect area, AdvancedRect plotArea)
        {
            drawArea = area;

            //using (Brush br = new SolidBrush(Color.Green))
            //    g.FillRectangle(br, area.Rect);
            GraphicsState _s = g.Save();

            using (Brush br = labelFont.CreateBrush())
            using (Font f = labelFont.CreateFont())
            using (Font f2 = smallLabelFont.CreateFont())
            using (Pen p = tickPen.CreatePen())
            using (Pen pgrid = gridlinePen.CreatePen())
            using (Pen pminor = minorTickPen.CreatePen())
            {
                float tick = tickLength;
                float dayTick = minorTickLength;
                float dayLabelSpacing = 0;
                if (axisType == AxisType.MonthsHorizontalWithDailyLabels)
                    dayLabelSpacing = 1f / 16;
                DateTime v;
                if (axisType == AxisType.Quarters)
                    // set v to beginning of quarter which ScaleMinimum is in
                    v = new DateTime(ScaleMinimum.Year, (((ScaleMinimum.Month - 1) / 3) * 3) + 1, 1);
                else // monthly labels
                    v = new DateTime(ScaleMinimum.Year, ScaleMinimum.Month, 1);
                for (int i = 0; i < calculateNumLabels(); i++)
                {
                    float x1 = DataToCoordinate(v, area);
                    x1 = Math.Max(x1, area.TopLeft.X);

                    if (x1 > area.BottomRight.X)
                        break;

                    DateTime v2 = cal.AddMonths(v, (axisType == AxisType.Quarters) ? 3 : 1);
                    float x2 = DataToCoordinate(v2, area);
                    x2 = Math.Min(x2, area.BottomRight.X);

                    g.DrawLine(p, x1, area.TopLeft.Y, x1, area.TopLeft.Y + tick);
                    if (gridlinesEnabled && x1 > area.TopLeft.X && x1 < area.BottomRight.X)
                        g.DrawLine(pgrid, x1, plotArea.TopLeft.Y, x1, plotArea.BottomRight.Y);

                    if (axisType == AxisType.MonthsHorizontalWithDailyTicks || axisType == AxisType.MonthsHorizontalWithDailyLabels)
                    {
                        // Draw daily ticks
                        int days = cal.GetDaysInMonth(v.Year, v.Month);
                        for (int d = 1; d <= days; d++)
                        {
                            float xd = DataToCoordinate(new DateTime(v.Year, v.Month, d), area);
                            if (xd > area.TopLeft.X && xd < area.BottomRight.X)
                                g.DrawLine(pminor, xd, area.TopLeft.Y, xd, area.TopLeft.Y + dayTick);
                        }
                    }
                    else if (axisType == AxisType.Quarters)
                    {
                        for (int d = 1; d < 3; d++)
                        {
                            float xd = DataToCoordinate(new DateTime(v.Year, v.Month + d, 1), area);
                            if (xd > area.TopLeft.X && xd < area.BottomRight.X)
                                g.DrawLine(pminor, xd, area.TopLeft.Y, xd, area.TopLeft.Y + dayTick);
                        }
                    }

                    if (axisType == AxisType.MonthsHorizontalWithDailyLabels)
                    {
                        // Draw day labels
                        int days = cal.GetDaysInMonth(v.Year, v.Month);
                        for (int d = 1; d <= days; d++)
                        {
                            float xd_left = DataToCoordinate(new DateTime(v.Year, v.Month, d), area);
                            float xd_right = DataToCoordinate(new DateTime(v.Year, v.Month, d) + new TimeSpan(1, 0, 0, 0), area);
                            if (xd_left < area.TopLeft.X || xd_right > area.BottomRight.X)
                                continue;
                            string dtxt = String.Format("{0}", d);
                            SizeF dsz = g.MeasureString(dtxt, f2);
                            g.DrawString(dtxt, f2, br, ((xd_left + xd_right) / 2) - (dsz.Width / 2), area.TopLeft.Y);
                        }
                    }

                    StringFormat form = new StringFormat();
                    if (axisType == AxisType.MonthsVertical || axisType == AxisType.Quarters)
                        form.FormatFlags = StringFormatFlags.DirectionVertical;

                    string txt;
                    if (axisType == AxisType.Quarters)
                        txt = String.Format("Q{0} {1:\\'yy}", (v.Month / 3) + 1, v);
                    else
                        txt = String.Format("{0:MMM \\'yy}", v);

                    SizeF sz = g.MeasureString(txt, f, 100, form);
                    g.DrawString(txt, f, br, ((x1 + x2) / 2) - (sz.Width / 2), area.TopLeft.Y + tick + dayLabelSpacing, form);

                    v = v2;
                }

                // one final tick to signify end of last visible month
                float xL = DataToCoordinate(v, area);
                if (xL < area.BottomRight.X)
                {
                    g.DrawLine(p, xL, area.TopLeft.Y, xL, area.TopLeft.Y + tick);
                    if (gridlinesEnabled)
                        g.DrawLine(pgrid, xL, plotArea.TopLeft.Y, xL, plotArea.BottomRight.Y);
                }
            }

            g.Restore(_s);
        }

        public float DataToCoordinate(DateTime v, AdvancedRect rect)
        {
            return DataToCoordinate(asDouble(v), rect);
        }

        public float DataToCoordinate(double v, AdvancedRect rect)
        {
            double r = (v - asDouble(ScaleMinimum)) / (asDouble(ScaleMaximum) - asDouble(ScaleMinimum));
            return (float)((rect.Width * r) + rect.TopLeft.X);
        }

        public double CoordinateToData(float x, AdvancedRect rect)
        {
            double r = ((x - rect.TopLeft.X) / rect.Width);
            return (asDouble(ScaleMaximum) - asDouble(ScaleMinimum)) * r + asDouble(ScaleMinimum);
        }

        private double asDouble(DateTime v)
        {
            return v.ToOADate();
        }

        private DateTime asDateTime(double d)
        {
            return DateTime.FromOADate(d);
        }

        private double? asNullableDouble(DateTime? v)
        {
            if (v == null)
                return null;
            return asDouble(v.Value);
        }

        private DateTime? asNullableDateTime(double? v)
        {
            if (v == null)
                return null;
            return asDateTime(v.Value);
        }

        public DateTime ScaleMaximum
        {
            get
            {
                if (zoomedMaximum.HasValue)
                    return zoomedMaximum.Value;
                if (userMaximum.HasValue)
                    return userMaximum.Value;
                if (parent.Series.MaxX != null)
                {
                    DateTime maxX = asDateTime(parent.Series.MaxX.Value);
                    // scale to the end of the month of maxX;
                    return new DateTime(maxX.Year, maxX.Month, cal.GetDaysInMonth(maxX.Year, maxX.Month) + 1);
                }
                return unscaledMaximum;
            }
        }

        public DateTime ScaleMinimum
        {
            get
            {
                if (zoomedMinimum.HasValue)
                    return zoomedMinimum.Value;
                if (userMinimum.HasValue)
                    return userMinimum.Value;
                if (parent.Series.MinX != null)
                {
                    DateTime minX = asDateTime(parent.Series.MinX.Value);
                    // scale to the end of the month of maxX;
                    return new DateTime(minX.Year, minX.Month, 1);
                }
                return unscaledMinimum;
            }
        }

        public DateTime? UserMaximum2
        {
            get
            {
                return userMaximum;
            }
            set
            {
                userMaximum = value;
                raiseEvent();
            }
        }

        public DateTime? UserMinimum2
        {
            get
            {
                return userMinimum;
            }
            set
            {
                userMinimum = value;
                raiseEvent();
            }
        }

        public double? UserMaximum
        {
            get
            {
                return asNullableDouble(userMaximum);
            }
            set
            {
                userMaximum = asNullableDateTime(value);
                raiseEvent();
            }
        }

        public double? UserMinimum
        {
            get
            {
                return asNullableDouble(userMinimum);
            }
            set
            {
                userMinimum = asNullableDateTime(value);
                raiseEvent();
            }
        }

        public double? ZoomedMaximum
        {
            get
            {
                return asNullableDouble(zoomedMaximum);
            }
            set
            {
                zoomedMaximum = asNullableDateTime(value);
                raiseEvent();
            }
        }

        public double? ZoomedMinimum
        {
            get
            {
                return asNullableDouble(zoomedMinimum);
            }
            set
            {
                zoomedMinimum = asNullableDateTime(value);
                raiseEvent();
            }
        }
    }
}
