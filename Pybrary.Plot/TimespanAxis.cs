using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pybrary.Plot
{
    public class TimespanAxis : Axis, XAxis
    {
        // priority of scales - zoomed, user, data, unscaled

        private TimeSpan? zoomedMinimum = null;
        private TimeSpan? zoomedMaximum = null;
        private TimeSpan? userMinimum = null;
        private TimeSpan? userMaximum = null;
        private TimeSpan unscaledMinimum = new TimeSpan(0);
        private TimeSpan unscaledMaximum = new TimeSpan(365, 0, 0, 0);
        private float minorTickLength = 0.025f; // in inches
        private PenDescription minorTickPen = new PenDescription(Color.Black, 1f / 96);
        private FontDescription titleFont = new FontDescription("Arial", 12f, FontStyle.Bold);

        private Plot parent;
        private bool dailyTicks = false;
        private bool weeklyLabels = false;
        private bool monthlyLabels = false;
        private bool quarterlyLabels = false;
        private bool yearlyLabels = false;

        private TimeSpan week = new TimeSpan(7, 0, 0, 0);
        private TimeSpan month = new TimeSpan((int)(730.5 * 60) * TimeSpan.TicksPerMinute); // 730.5 hrs per month
        private TimeSpan year = new TimeSpan((int)(365.25 * 24) * TimeSpan.TicksPerHour); // 365.25 days per year

        public TimespanAxis(Plot parent)
        {
            this.parent = parent;
        }

        public float CalculateHeight(Graphics g, float maximumWidth)
        {
            float height = 0;

            dailyTicks = weeklyLabels = monthlyLabels = quarterlyLabels = yearlyLabels = false;

            using (Font f = labelFont.CreateFont())
            {
                weeklyLabels = true;
                TimeSpan duration = ScaleMaximum - ScaleMinimum;

                SizeF labelSize = g.MeasureString(String.Format("{0}", ScaleMaximum.Ticks / week.Ticks), f);
                int numLabels = (int)(duration.Ticks / week.Ticks);

                float estimatedWidth = (labelSize.Width + 0.3f) * numLabels;
                if (estimatedWidth < maximumWidth)
                {
                    // monthly horizontal labels
                    height += labelSize.Height;

                    // do we also have room for daily ticks?
                    int numDays = duration.Days;
                    dailyTicks = (0.05f * numDays) < maximumWidth;
                }
                else
                {
                    // our total width is going to be too wide.  fallback #1 -
                    // months
                    weeklyLabels = false;
                    monthlyLabels = true;
                    duration = ScaleMaximum - ScaleMinimum;
                    labelSize = g.MeasureString(String.Format("{0}", ScaleMaximum.Ticks / month.Ticks), f);
                    numLabels = (int)(duration.Ticks / month.Ticks);
                    estimatedWidth = (labelSize.Width + 0.3f) * numLabels;

                    if (estimatedWidth < maximumWidth)
                    {
                        // That'll do
                        height += labelSize.Height;
                    }
                    else
                    {
                        // fallback #2 - 3 month periods
                        monthlyLabels = false;
                        quarterlyLabels = true;
                        duration = ScaleMaximum - ScaleMinimum;
                        labelSize = g.MeasureString(String.Format("{0}", ScaleMaximum.Ticks / (month.Ticks * 3)), f);
                        numLabels = (int)(duration.Ticks / (month.Ticks * 3));
                        estimatedWidth = (labelSize.Width + 0.3f) * numLabels;
                        if (estimatedWidth < maximumWidth)
                        {
                            // that'll do
                            height += labelSize.Height;
                        }
                        else
                        {
                            // fallback #3 - years
                            quarterlyLabels = false;
                            yearlyLabels = true;
                            duration = ScaleMaximum - ScaleMinimum;
                            labelSize = g.MeasureString(String.Format("{0}", ScaleMaximum.Ticks / year.Ticks), f);
                            numLabels = (int)(duration.Ticks / year.Ticks);
                            height += labelSize.Height;
                        }
                    }
                }
            }

            // Add in an axis title, "Time (years)"
            using (Font f = titleFont.CreateFont())
            {
                SizeF labelSize = g.MeasureString("Time (years)", f);
                height += labelSize.Height;
                // Some padding between title and labels
                height += 0.02f;
            }

            // tick marks
            height += tickLength;

            return height;
        }

        public void DrawX(Graphics g, AdvancedRect area, AdvancedRect plotArea)
        {
            drawArea = area;

            //using (Brush br = new SolidBrush(Color.Green))
            //    g.FillRectangle(br, area.Rect);
            GraphicsState _s = g.Save();

            using (Font f = titleFont.CreateFont())
            using (Brush br = titleFont.CreateBrush())
            {
                string txt = "Time (";
                if (weeklyLabels)
                    txt += "weeks)";
                else if (monthlyLabels || quarterlyLabels)
                    txt += "months)";
                else if (yearlyLabels)
                    txt += "years)";
                SizeF sz = g.MeasureString(txt, f);
                g.DrawString(txt, f, br, area.Center.X - (sz.Width / 2), area.BottomRight.Y - sz.Height);
            }

            using (Brush br = labelFont.CreateBrush())
            using (Font f = labelFont.CreateFont())
            using (Pen p = tickPen.CreatePen())
            using (Pen pgrid = gridlinePen.CreatePen())
            using (Pen pminor = minorTickPen.CreatePen())
            {
                float tick = tickLength;
                float dayTick = minorTickLength;

                TimeSpan dur;
                if (weeklyLabels)
                    dur = week;
                else if (monthlyLabels)
                    dur = month;
                else if (quarterlyLabels)
                    dur = new TimeSpan(month.Ticks * 3);
                else
                    dur = year;

                int cnt = (int)(ScaleMinimum.Ticks / dur.Ticks);
                while (true)
                {
                    TimeSpan loc = new TimeSpan(dur.Ticks * cnt);
                    float x1 = DataToCoordinate(loc, area);
                    cnt++;

                    if (x1 < area.TopLeft.X)
                        continue;
                    if (x1 > area.BottomRight.X)
                        break;

                    g.DrawLine(p, x1, area.TopLeft.Y, x1, area.TopLeft.Y + tick);
                    if (gridlinesEnabled && x1 != area.TopLeft.X && x1 != area.BottomRight.X)
                        g.DrawLine(pgrid, x1, plotArea.TopLeft.Y, x1, plotArea.BottomRight.Y);

                    if (dailyTicks)
                    {
                        for (int d = 0; d < 7; d++)
                        {
                            float xd = DataToCoordinate(loc + new TimeSpan(d, 0, 0, 0), area);
                            if (xd > area.TopLeft.X && xd < area.BottomRight.X)
                                g.DrawLine(pminor, xd, area.TopLeft.Y, xd, area.TopLeft.Y + dayTick);
                        }
                    }

                    int labelNumber = cnt - 1;
                    if (quarterlyLabels)
                        labelNumber *= 3;
                    string txt = String.Format("{0}", labelNumber);
                    SizeF sz = g.MeasureString(txt, f);
                    g.DrawString(txt, f, br, x1 - (sz.Width / 2), area.TopLeft.Y + tick);
                }
            }

            g.Restore(_s);
        }

        public float DataToCoordinate(TimeSpan v2, AdvancedRect rect)
        {
            double r = (double)(v2.Ticks - ScaleMinimum.Ticks) / (double)(ScaleMaximum.Ticks - ScaleMinimum.Ticks);
            return (float)((rect.Width * r) + rect.TopLeft.X);
        }

        public float DataToCoordinate(double v, AdvancedRect rect)
        {
            TimeSpan v2 = asTimeSpan(v);
            return DataToCoordinate(v2, rect);
        }

        public double CoordinateToData(float x, AdvancedRect rect)
        {
            double r = ((x - rect.TopLeft.X) / rect.Width);
            return asDouble(new TimeSpan((long)((ScaleMaximum.Ticks - ScaleMinimum.Ticks) * r) + ScaleMinimum.Ticks));
        }

        private double asDouble(TimeSpan v)
        {
            return (double)v.Ticks / (double)TimeSpan.TicksPerDay;
        }

        private TimeSpan asTimeSpan(double d)
        {
            return new TimeSpan((long)(d * TimeSpan.TicksPerDay));
        }

        private double? asNullableDouble(TimeSpan? v)
        {
            if (v == null)
                return null;
            return asDouble(v.Value);
        }

        private TimeSpan? asNullableTimeSpan(double? v)
        {
            if (v == null)
                return null;
            return asTimeSpan(v.Value);
        }

        public TimeSpan ScaleMaximum
        {
            get
            {
                if (zoomedMaximum.HasValue)
                    return zoomedMaximum.Value;
                if (userMaximum.HasValue)
                    return userMaximum.Value;
                if (parent.Series.MaxX != null)
                {
                    TimeSpan dur;
                    if (weeklyLabels)
                        dur = week;
                    else if (monthlyLabels)
                        dur = month;
                    else if (quarterlyLabels)
                        dur = new TimeSpan(month.Ticks * 3);
                    else
                        dur = year;

                    TimeSpan? maxX = asTimeSpan(parent.Series.MaxX.Value);

                    // rounds up to nearest full duration
                    return new TimeSpan(maxX.Value.Ticks + (dur.Ticks - (maxX.Value.Ticks % dur.Ticks)));
                }
                return unscaledMaximum;
            }
        }

        public TimeSpan ScaleMinimum
        {
            get
            {
                if (zoomedMinimum.HasValue)
                    return zoomedMinimum.Value;
                if (userMinimum.HasValue)
                    return userMinimum.Value;
                if (parent.Series.MinX != null)
                    return new TimeSpan(0);
                return unscaledMinimum;
            }
        }

        public TimeSpan? UserMaximum2
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

        public TimeSpan? UserMinimum2
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
                userMaximum = asNullableTimeSpan(value);
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
                userMinimum = asNullableTimeSpan(value);
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
                zoomedMaximum = asNullableTimeSpan(value);
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
                zoomedMinimum = asNullableTimeSpan(value);
                raiseEvent();
            }
        }
    }
}
