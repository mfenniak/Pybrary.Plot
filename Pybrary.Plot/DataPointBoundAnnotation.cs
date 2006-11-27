using System;
using System.Collections.Generic;
using System.Text;

namespace Pybrary.Plot
{
    /// <summary>
    /// An annotation where the location is defined by the name of a data
    /// series, and an index into the data series's data array.
    /// </summary>
    public class DataPointBoundAnnotation : DataBoundAnnotation
    {
        string seriesName;
        int pointIndex;

        public DataPointBoundAnnotation(Plot plot)
            : base(plot)
        {
        }

        protected Series Series
        {
            get
            {
                return Plot.Series[SeriesName];
            }
        }

        public string SeriesName
        {
            get
            {
                return seriesName;
            }
            set
            {
                seriesName = value;
            }
        }

        public int PointIndex
        {
            get
            {
                return pointIndex;
            }
            set
            {
                pointIndex = value;
            }
        }

        public override double X
        {
            get
            {
                return Series.GetXValueByIndex(pointIndex);
            }
            set
            {
                throw new NotSupportedException("use SeriesName/PointIndex to set location");
            }
        }

        public override DateTime XAsDateTime
        {
            get
            {
                throw new NotSupportedException("XAsDateTime invalid for DataPointBoundAnnotation");
            }
            set
            {
                throw new NotSupportedException("use SeriesName/PointIndex to set location");
            }
        }

        public override double Y
        {
            get
            {
                return Series.GetYValueByIndex(pointIndex).Value;
            }
            set
            {
                throw new NotSupportedException("use SeriesName/PointIndex to set location");
            }
        }

        public override string YAxisName
        {
            get
            {
                return Series.YAxisName;
            }
            set
            {
                throw new NotSupportedException("use SeriesName/PointIndex to set location");
            }
        }
    }
}
