using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pybrary.Plot
{
    public abstract class NumericAxis : Axis
    {
        // axis scale order - zoomed, user, software, data, unscaled
        protected string title = null;
        protected FontDescription titleFont = new FontDescription("Arial", 12f, FontStyle.Bold);
        protected double? zoomedMinimum = null;
        protected double? zoomedMaximum = null;
        private double? userMaximum = null;
        private double? userMinimum = null;
        private double? softwareMaximum = null;
        private double? softwareMinimum = null;
        private double unscaledMaximum = 10;
        private double unscaledMinimum = 0;
        private double unscaledLogMinimum = 0.1;
        private bool visible = true;
        protected bool logAxis = false;
        private bool autoscaleIncludesZero = true;
        private bool integerAxis = false;

        protected NumericAxis()
        {
        }

        protected abstract bool AxisReverseOfCoordinateArea();
        protected abstract float AxisLength(AdvancedRect dataArea);
        protected abstract float AxisStart(AdvancedRect dataArea);
        protected abstract double? CalculateDataMinimum();
        protected abstract double? CalculateDataMaximum();
        protected abstract double? CalculateDataMinimumGtZero();

        public float DataToCoordinate(double v, AdvancedRect rect)
        {
            if (ScaleMinimum == ScaleMaximum)
                return rect.BottomRight.Y;
            double v2 = (double)v;
            if (!logAxis)
            {
                double r = (v2 - ScaleMinimum) / (ScaleMaximum - ScaleMinimum);
                if (AxisReverseOfCoordinateArea())
                    r = (1.0 - r);
                return (float)((AxisLength(rect) * r) + AxisStart(rect));
            }
            else
            {
                double r = (Math.Log10(v2) - Math.Log10(ScaleMinimum)) / (Math.Log10(ScaleMaximum) - Math.Log10(ScaleMinimum));
                if (AxisReverseOfCoordinateArea())
                    r = (1.0 - r);
                return (float)((AxisLength(rect) * r) + AxisStart(rect));
            }
        }

        public double CoordinateToData(float x, AdvancedRect rect)
        {
            if (!logAxis)
            {
                double r = ((x - AxisStart(rect)) / AxisLength(rect));
                if (AxisReverseOfCoordinateArea())
                    r = (1.0 - r);
                return (ScaleMaximum - ScaleMinimum) * r + ScaleMinimum;
            }
            else
            {
                double r = ((x - AxisStart(rect)) / AxisLength(rect));
                if (AxisReverseOfCoordinateArea())
                    r = (1.0 - r);
                return Math.Pow(10, Math.Log10(ScaleMinimum) + (r * (Math.Log10(ScaleMaximum) - Math.Log10(ScaleMinimum))));
            }
        }

        public String FormatLabel(double v)
        {
            return String.Format("{0}", v);
        }

        public double ScaleMaximum
        {
            get
            {
                if (zoomedMaximum.HasValue)
                    return zoomedMaximum.Value;
                if (userMaximum.HasValue)
                    return userMaximum.Value;
                if (softwareMaximum.HasValue)
                    return softwareMaximum.Value;
                double? max = CalculateDataMaximum();
                double? min = CalculateDataMinimum();
                if (max.HasValue && min.HasValue)
                {
                    if (logAxis)
                        return Math.Pow(10, Math.Ceiling(Math.Log10(max.Value)));
                    else
                    {
                        // add 5% of delta for extra white space
                        double realMax = CalculateRoundedMax(max.Value, min.Value);
                        if (realMax < 0 && autoscaleIncludesZero)
                            return 0;
                        else if (realMax > 0 && (min.Value <= 0 && max.Value <= 0) && autoscaleIncludesZero)
                            return 0;
                        return realMax;
                    }
                }
                return unscaledMaximum;
            }
        }

        public double ScaleMinimum
        {
            get
            {
                if (zoomedMinimum.HasValue)
                    return zoomedMinimum.Value;
                if (userMinimum.HasValue)
                    return userMinimum.Value;
                if (softwareMinimum.HasValue)
                    return softwareMinimum.Value;
                double? max = CalculateDataMaximum();
                double? min = CalculateDataMinimum();
                if (max.HasValue && min.HasValue)
                {
                    if (logAxis)
                    {
                        double? min_gt_zero = CalculateDataMinimumGtZero();
                        if (min_gt_zero.HasValue)
                            return Math.Pow(10, Math.Floor(Math.Log10(min_gt_zero.Value)));
                        return unscaledLogMinimum;
                    }
                    else
                    {
                        // add 5% of delta for extra white space
                        double realMin = CalculateRoundedMin(max.Value, min.Value);
                        if (realMin > 0 && autoscaleIncludesZero)
                            return 0;
                        else if (realMin < 0 && (min.Value >= 0 && max.Value >= 0) && autoscaleIncludesZero)
                            return 0;
                        return realMin;
                    }
                }
                return unscaledMinimum;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                raiseEvent();
            }
        }

        public double? SoftwareMinimum
        {
            get
            {
                return softwareMinimum;
            }
            set
            {
                softwareMinimum = (double?)value;
                raiseEvent();
            }
        }

        public double? SoftwareMaximum
        {
            get
            {
                return softwareMaximum;
            }
            set
            {
                softwareMaximum = (double?)value;
                raiseEvent();
            }
        }

        public double? UserMinimum
        {
            get
            {
                return userMinimum;
            }
            set
            {
                userMinimum = (double?)value;
                raiseEvent();
            }
        }

        public double? UserMaximum
        {
            get
            {
                return userMaximum;
            }
            set
            {
                userMaximum = (double?)value;
                raiseEvent();
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                raiseEvent();
            }
        }

        public bool LogAxis
        {
            get
            {
                return logAxis;
            }
            set
            {
                using (SuspendEvents())
                {
                    logAxis = value;
                    if (logAxis)
                    {
                        // If the user zoomed into the graph, or a min value was
                        // set by the software before, and it cannot be drawn on 
                        // a log axis - remove the restriction and let data scaling
                        // occur.
                        if (ZoomedMinimum <= 0)
                            ZoomedMinimum = null;
                        if (UserMinimum <= 0)
                            UserMinimum = null;
                        if (ZoomedMaximum <= 0)
                            ZoomedMaximum = null;
                        if (UserMaximum <= 0)
                            UserMaximum = null;
                    }
                    raiseEvent();
                }
            }
        }

        public double? ZoomedMaximum
        {
            get
            {
                return zoomedMaximum;
            }
            set
            {
                zoomedMaximum = value;
                raiseEvent();
            }
        }

        public double? ZoomedMinimum
        {
            get
            {
                return zoomedMinimum;
            }
            set
            {
                zoomedMinimum = value;
                raiseEvent();
            }
        }

        public bool IntegerAxis
        {
            get
            {
                return integerAxis;
            }
            set
            {
                integerAxis = value;
                raiseEvent();
            }
        }

        public IEnumerable<double> GenerateTickLocations(int maxIntervals)
        {
            int intervals;
            double delta = CalculateInterval(ScaleMinimum, ScaleMaximum, maxIntervals, out intervals);

            double start;
            if ((ScaleMinimum % delta) == 0)
                start = ScaleMinimum;
            else if (ScaleMinimum < 0)
                start = ScaleMinimum - (ScaleMinimum % delta);
            else
                start = ScaleMinimum + (delta - (ScaleMinimum % delta));

            for (int i = 0; i < intervals; i++)
            {
                double v = start + (delta * i);
                yield return v;
            }
        }

        public static double CalculateRoundedMax(double max, double min)
        {
            return max + ((max - min) * 0.05);
        }

        public static double CalculateRoundedMin(double max, double min)
        {
            return min - ((max - min) * 0.05);
        }

        public static bool ToleranceCheck(double a, double b, double tolerance)
        {
            return Math.Abs(a - b) < tolerance;
        }

        private double[] intervalSizes = new double[] {
            5e8, 2.5e8, 2e8, 1e8,
            5e7, 2.5e7, 2e7, 1e7, 5e6, 2.5e6, 2e6, 1e6,
            5e5, 2.5e5, 2e5, 1e5, 5e4, 2.5e4, 2e4, 1e4,
            5e3, 2.5e3, 2e3, 1e3, 5e2, 2.5e2, 2e2, 1e2,
            50, 25, 20, 10, 5, 2, 1, 0.5, 0.1, 0.05, 0.01
        };

        public double CalculateInterval(double min, double max, int maxIntervals, out int intervals)
        {
            double delta = (max - min);
            int i = 0;
            for (; i < intervalSizes.Length; i++)
            {
                double n = intervalSizes[i];
                if (IntegerAxis && n < 1)
                    break;
                double ni = Math.Floor(delta / n) + 1.0;
                if (ni > maxIntervals)
                    break;
            }

            if (i == 0)
                i = 1;

            // let's return i - 1
            intervals = (int)(Math.Floor(delta / intervalSizes[i - 1])) + 1;
            return intervalSizes[i - 1];

            /*
            // Worst case scenario - no intervals
            intervals = 2;
            return delta;
            */
        }
    }

    public class NumericYAxis : NumericAxis, YAxis
    {
        private string name = null;
        private SeriesCollection series;
        private bool rightSide;
        private PenDescription borderPen;

        public NumericYAxis(string name, SeriesCollection series, bool rightSide, PenDescription borderPen)
        {
            this.name = name;
            this.series = series;
            this.rightSide = rightSide;
            this.borderPen = borderPen;
        }

        protected override float AxisStart(AdvancedRect dataArea)
        {
            return dataArea.TopLeft.Y;
        }

        protected override float AxisLength(AdvancedRect dataArea)
        {
            return dataArea.Height;
        }

        protected override bool AxisReverseOfCoordinateArea()
        {
            return true;
        }

        protected override double? CalculateDataMinimum()
        {
            return series.GetMinYForAxis(this.Name);
        }

        protected override double? CalculateDataMaximum()
        {
            return series.GetMaxYForAxis(this.Name);
        }

        protected override double? CalculateDataMinimumGtZero()
        {
            return series.GetMinYGtZeroForAxis(this.Name);
        }

        public float CalculateWidth(Graphics g)
        {
            float width = 0;

            if (title != null)
            {
                // title
                using (Font f = titleFont.CreateFont())
                    width += g.MeasureString(title, f).Height;

                // Some padding between title and labels
                width += 0.02f;
            }

            // label widths
            if (logAxis)
            {
                // Longest labels will always be min or max.
                using (Font f = labelFont.CreateFont())
                {
                    double min = Math.Pow(10, Math.Ceiling(Math.Log10(ScaleMinimum)));
                    double max = Math.Pow(10, Math.Floor(Math.Log10(ScaleMaximum)));
                    SizeF sz1 = g.MeasureString(FormatLabel(min), f);
                    SizeF sz2 = g.MeasureString(FormatLabel(max), f);
                    width += Math.Max(sz1.Width, sz2.Width);
                }
            }
            else
            {
                int intervals;
                double delta = CalculateInterval(ScaleMinimum, ScaleMaximum, 10, out intervals);
                using (Font f = labelFont.CreateFont())
                {
                    SizeF sz1 = g.MeasureString(FormatLabel(delta), f);
                    SizeF sz2 = g.MeasureString(FormatLabel(delta * (intervals - 1)), f);
                    width += Math.Max(sz1.Width, sz2.Width);
                }
            }

            // tick marks
            width += tickLength;

            return width;
        }

        public void DrawY(Graphics g, AdvancedRect area, AdvancedRect plotArea)
        {
            drawArea = area;

            GraphicsState _s = g.Save();

            //using (Brush br = new SolidBrush(Color.Purple))
            //    g.FillRectangle(br, area.Rect);

            if (title != null)
            {
                using (Brush br = titleFont.CreateBrush())
                using (Font f = titleFont.CreateFont())
                {
                    SizeF titleSize = g.MeasureString(title, f);

                    GraphicsState s = g.Save();
                    if (rightSide)
                    {
                        g.TranslateTransform(area.BottomRight.X, area.Center.Y - (titleSize.Width / 2));
                        g.RotateTransform(90);
                    }
                    else
                    {
                        g.TranslateTransform(area.TopLeft.X, area.Center.Y + (titleSize.Width / 2));
                        g.RotateTransform(-90);
                    }
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(title, f, br, new PointF(0, 0));
                    g.Restore(s);
                }
            }

            if (!logAxis)
            {
                int maxIntervals = (int)Math.Ceiling(area.Height * 1.25);

                // calculate label and tick mark intervals
                using (Brush br = labelFont.CreateBrush())
                using (Font f = labelFont.CreateFont())
                using (Pen p = tickPen.CreatePen())
                {
                    foreach (double v in GenerateTickLocations(maxIntervals))
                    {
                        string txt = FormatLabel(v);
                        float yCoord = DataToCoordinate(v, area);

                        if (yCoord < area.TopLeft.Y || yCoord > area.BottomRight.Y)
                            continue;

                        SizeF sz = g.MeasureString(txt, f);
                        if (rightSide)
                        {
                            g.DrawLine(p, area.TopLeft.X, yCoord, area.TopLeft.X + tickLength, yCoord);
                            g.DrawString(txt, f, br, area.TopLeft.X + tickLength, yCoord - (sz.Height / 2));
                        }
                        else
                        {
                            g.DrawLine(p, area.BottomRight.X, yCoord, area.BottomRight.X - tickLength, yCoord);
                            g.DrawString(txt, f, br, area.BottomRight.X - sz.Width - tickLength, yCoord - (sz.Height / 2));
                        }
                    }
                }

                if (gridlinesEnabled)
                {
                    using (Pen p = gridlinePen.CreatePen())
                    {
                        foreach (double v in GenerateTickLocations(maxIntervals))
                        {
                            float yCoord = DataToCoordinate(v, area);
                            if (yCoord < area.TopLeft.Y || yCoord > area.BottomRight.Y)
                                continue;
                            g.DrawLine(p, plotArea.TopLeft.X, yCoord, plotArea.BottomRight.X, yCoord);
                        }
                    }
                }
            }
            else
            {
                // log axis drawing
                int start = (int)Math.Floor(Math.Log10(ScaleMinimum));
                int end = (int)Math.Ceiling(Math.Log10(ScaleMaximum));

                using (Brush br = labelFont.CreateBrush())
                using (Font f = labelFont.CreateFont())
                using (Pen p = tickPen.CreatePen())
                using (Pen p2 = gridlinePen.CreatePen())
                {
                    for (int i = start; i <= end; i++)
                    {
                        double v = Math.Pow(10, i);
                        float yCoord = DataToCoordinate(v, area);

                        // ticks and labels
                        string txt = FormatLabel(v);
                        SizeF sz = g.MeasureString(txt, f);
                        if (yCoord >= area.TopLeft.Y && yCoord <= area.BottomRight.Y)
                        {
                            if (rightSide)
                            {
                                g.DrawLine(p, area.TopLeft.X, yCoord, area.TopLeft.X + tickLength, yCoord);
                                g.DrawString(txt, f, br, area.TopLeft.X + tickLength, yCoord - (sz.Height / 2));
                            }
                            else
                            {
                                g.DrawLine(p, area.BottomRight.X, yCoord, area.BottomRight.X - tickLength, yCoord);
                                g.DrawString(txt, f, br, area.BottomRight.X - sz.Width - tickLength, yCoord - (sz.Height / 2));
                            }
                        }

                        if (gridlinesEnabled && i != end)
                        {
                            double v2 = Math.Pow(10, i + 1);
                            double delta = (v2 - v) / 9;
                            for (int j = 1; j < 10; j++)
                            {
                                double v3 = v + (delta * j);
                                yCoord = DataToCoordinate(v3, area);
                                if (yCoord > area.TopLeft.Y && yCoord < area.BottomRight.Y)
                                    g.DrawLine(p2, plotArea.TopLeft.X, yCoord, plotArea.BottomRight.X, yCoord);
                            }
                        }
                    }
                }
            }

            // Because there can be multiple Y axises that are not always next to the data area,
            // we draw our own border.  The plot might stroke over it again later, but that's
            // not such a big deal.
            using (Pen p = borderPen.CreatePen())
            {
                if (rightSide)
                    g.DrawLine(p, area.TopLeft, new PointF(area.TopLeft.X, area.BottomRight.Y));
                else
                    g.DrawLine(p, area.BottomRight, new PointF(area.BottomRight.X, area.TopLeft.Y));
            } 
            
            g.Restore(_s);
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
    }

    public class NumericXAxis : NumericAxis, XAxis
    {
        private Plot parent;

        public NumericXAxis(Plot parent)
        {
            this.parent = parent;
        }

        protected override float AxisStart(AdvancedRect dataArea)
        {
            return dataArea.TopLeft.X;
        }

        protected override float AxisLength(AdvancedRect dataArea)
        {
            return dataArea.Width;
        }

        protected override bool AxisReverseOfCoordinateArea()
        {
            return false;
        }

        protected override double? CalculateDataMinimum()
        {
            return parent.Series.MinX;
        }

        protected override double? CalculateDataMaximum()
        {
            return parent.Series.MaxX;
        }

        protected override double? CalculateDataMinimumGtZero()
        {
            return parent.Series.MinX_gt_Zero;
        }

        public float CalculateHeight(Graphics g, float maximumWidth)
        {
            float height = 0;

            if (title != null)
            {
                // title
                using (Font f = titleFont.CreateFont())
                    height += g.MeasureString(title, f).Height;

                // Some padding between title and labels
                height += 0.02f;
            }

            // label height
            using (Font f = labelFont.CreateFont())
            {
                double max = Math.Pow(10, Math.Floor(Math.Log10(ScaleMaximum)));
                height += g.MeasureString(FormatLabel(max), f).Height;
            }

            // tick marks
            height += tickLength;

            return height;
        }

        public void DrawX(Graphics g, AdvancedRect area, AdvancedRect plotArea)
        {
            drawArea = area;

            GraphicsState _s = g.Save();

            //using (Brush br = new SolidBrush(Color.Purple))
            //    g.FillRectangle(br, area.Rect);

            if (title != null)
            {
                using (Brush br = titleFont.CreateBrush())
                using (Font f = titleFont.CreateFont())
                {
                    SizeF titleSize = g.MeasureString(title, f);
                    g.DrawString(title, f, br, area.Center.X - (titleSize.Width / 2), area.BottomRight.Y - titleSize.Height);
                }
            }

            if (!logAxis)
            {
                int maxIntervals = (int)Math.Ceiling(area.Width * 1.25);

                // calculate label and tick mark intervals
                using (Brush br = labelFont.CreateBrush())
                using (Font f = labelFont.CreateFont())
                using (Pen p = tickPen.CreatePen())
                {
                    foreach (double v in GenerateTickLocations(maxIntervals))
                    {
                        string txt = FormatLabel(v);
                        float xCoord = DataToCoordinate(v, area);
                        SizeF sz = g.MeasureString(txt, f);

                        if (xCoord < area.TopLeft.X || xCoord > area.BottomRight.X)
                            continue;

                        g.DrawLine(p, xCoord, area.TopLeft.Y, xCoord, area.TopLeft.Y + tickLength);
                        g.DrawString(txt, f, br, xCoord - (sz.Width / 2), area.TopLeft.Y + tickLength);
                    }
                }

                if (gridlinesEnabled)
                {
                    using (Pen p = gridlinePen.CreatePen())
                    {
                        foreach (double v in GenerateTickLocations(maxIntervals))
                        {
                            float xCoord = DataToCoordinate(v, area);
                            if (xCoord < area.TopLeft.X || xCoord > area.BottomRight.X)
                                continue;
                            g.DrawLine(p, xCoord, plotArea.TopLeft.Y, xCoord, plotArea.BottomRight.Y);
                        }
                    }
                }
            }
            else
            {
                // log axis drawing
                int start = (int)Math.Floor(Math.Log10(ScaleMinimum));
                int end = (int)Math.Ceiling(Math.Log10(ScaleMaximum));

                using (Brush br = labelFont.CreateBrush())
                using (Font f = labelFont.CreateFont())
                using (Pen p = tickPen.CreatePen())
                using (Pen p2 = gridlinePen.CreatePen())
                {
                    for (int i = start; i <= end; i++)
                    {
                        double v = Math.Pow(10, i);
                        float xCoord = DataToCoordinate(v, area);

                        // ticks and labels
                        string txt = FormatLabel(v);
                        SizeF sz = g.MeasureString(txt, f);

                        if (xCoord >= area.TopLeft.X && xCoord <= area.BottomRight.X)
                        {
                            g.DrawLine(p, xCoord, area.TopLeft.Y, xCoord, area.TopLeft.Y + tickLength);
                            g.DrawString(txt, f, br, xCoord - (sz.Width / 2), area.TopLeft.Y + tickLength);
                        }

                        if (gridlinesEnabled && i != end)
                        {
                            double v2 = Math.Pow(10, i + 1);
                            double delta = (v2 - v) / 9;
                            for (int j = 1; j < 10; j++)
                            {
                                double v3 = v + (delta * j);
                                xCoord = DataToCoordinate(v3, area);
                                if (xCoord > area.TopLeft.X && xCoord < area.BottomRight.X)
                                    g.DrawLine(p2, xCoord, plotArea.TopLeft.Y, xCoord, plotArea.BottomRight.Y);
                            }
                        }
                    }
                }
            }

            g.Restore(_s);
        }

    }
}
