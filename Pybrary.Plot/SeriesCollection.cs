using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pybrary.Plot
{
    public delegate void SeriesCollectionEventHandler();

    public class SeriesCollection : IEnumerable<KeyValuePair<string, Series>>
    {
        private IDictionary<string, Series> seriesByName = new Dictionary<string, Series>();

        private SeriesEventHandler seriesChangedHandler;
        public event SeriesCollectionEventHandler OnSeriesCollectionChanged;

        private double? minX_gt_Zero = null;
        private double? minX = null;
        private double? maxX = null;

        public SeriesCollection()
        {
            seriesChangedHandler = new SeriesEventHandler(onSeriesChanged);
        }

        private void onSeriesChanged()
        {
            minX_gt_Zero = minX = maxX = null;
            raiseChanged();
        }

        public Series this[string name]
        {
            get
            {
                return seriesByName[name];
            }
            set
            {
                if (seriesByName.ContainsKey(name))
                    value.OnSeriesChanged -= seriesChangedHandler;
                seriesByName[name] = value;
                value.OnSeriesChanged += seriesChangedHandler;
                raiseChanged();
            }
        }

        public void Clear()
        {
            foreach (KeyValuePair<string, Series> kv in this)
            {
                Series s = kv.Value;
                s.OnSeriesChanged -= seriesChangedHandler;
            }
            seriesByName.Clear();
            raiseChanged();
        }

        private void raiseChanged()
        {
            SeriesCollectionEventHandler tmp = OnSeriesCollectionChanged;
            if (tmp != null)
                tmp();
        }

        public double? GetMinYForAxis(string yAxisName)
        {
            // fixme - cache value
            double? minY = null;

            foreach (KeyValuePair<string, Series> s in seriesByName)
            {
                if (s.Value.YAxisName.Equals(yAxisName))
                {
                    double? seriesMinY = s.Value.MinY;
                    if (minY == null || (seriesMinY != null && seriesMinY.Value < minY.Value))
                        minY = seriesMinY;
                }
            }

            return minY;
        }

        public double? GetMaxYForAxis(string yAxisName)
        {
            // fixme - cache value
            double? maxY = null;

            foreach (KeyValuePair<string, Series> s in seriesByName)
            {
                if (s.Value.YAxisName.Equals(yAxisName))
                {
                    double? seriesMaxY = s.Value.MaxY;
                    if (maxY == null || (seriesMaxY != null && seriesMaxY.Value > maxY.Value))
                        maxY = seriesMaxY;
                }
            }

            return maxY;
        }

        public double? GetMinYGtZeroForAxis(string yAxisName)
        {
            // fixme - cache value
            double? retval = null;

            foreach (KeyValuePair<string, Series> s in seriesByName)
            {
                if (s.Value.YAxisName.Equals(yAxisName))
                {
                    double? seriesMinY = s.Value.MinY_gt_Zero;
                    if (retval == null || (seriesMinY != null && seriesMinY.Value < retval.Value))
                        retval = seriesMinY;
                }
            }

            return retval;
        }

        private void calculateMinMaxX()
        {
            if (minX != null)
                return;

            foreach (KeyValuePair<string, Series> s in seriesByName)
            {
                double? seriesMinX_gt_Zero = s.Value.MinX_gt_Zero;
                if (minX_gt_Zero == null || (seriesMinX_gt_Zero != null && seriesMinX_gt_Zero < minX_gt_Zero.Value))
                    minX_gt_Zero = seriesMinX_gt_Zero;
                double? seriesMinX = s.Value.MinX;
                if (minX == null || (seriesMinX != null && seriesMinX < minX.Value))
                    minX = seriesMinX;
                double? seriesMaxX = s.Value.MaxX;
                if (maxX == null || (seriesMaxX != null && seriesMaxX > maxX.Value))
                    maxX = seriesMaxX;
            }
        }

        public double? MinX_gt_Zero
        {
            get
            {
                calculateMinMaxX();
                return minX_gt_Zero;
            }
        }

        public double? MinX
        {
            get
            {
                calculateMinMaxX();
                return minX;
            }
        }

        public double? MaxX
        {
            get
            {
                calculateMinMaxX();
                return maxX;
            }
        }

        public void Draw(Graphics g, AxisCollection yAxisCollection, XAxis xAxis, AdvancedRect area)
        {
            GraphicsState _s = g.Save();
            g.SetClip(area.Rect);
            foreach (KeyValuePair<string, Series> s in seriesByName)
                s.Value.Draw(g, yAxisCollection, xAxis, area);
            g.Restore(_s);
        }

        public IEnumerator<KeyValuePair<string, Series>> GetEnumerator()
        {
            return seriesByName.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return seriesByName.GetEnumerator();
        }

        public string TextExport()
        {
            // text
            List<List<string>> series = new List<List<string>>();
            List<int> numColumns = new List<int>();
            foreach (KeyValuePair<string, Series> kv in this)
            {
                List<string> rows = new List<string>();
                series.Add(rows);
                numColumns.Add(kv.Value.TextExport(rows, kv.Key));

            }
            int max = 0;
            foreach (List<string> lst in series)
                max = Math.Max(max, lst.Count);
            string finalValue = "";
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < series.Count; j++)
                {
                    if (i >= series[j].Count)
                    {
                        for (int k = 0; k < numColumns[j]; k++)
                            finalValue += "\t";
                    }
                    else
                        finalValue += series[j][i];
                }
                finalValue += "\r\n";
            }
            return finalValue;
        }
    }
}
