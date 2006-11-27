using System;
using System.Collections.Generic;
using System.Text;

namespace Pybrary.Plot.Data
{
    public delegate void PointCollectionChangedHandler();
    public delegate double ObjectToDoubleConverter(object a);

    public class PointCollection
    {
        public event PointCollectionChangedHandler OnPointCollectionChanged;

        private double[] x = new double[0];
        private double?[] y = new double?[0];

        private double? minY_gt_zero;
        private double? minY;
        private double? maxY;
        private double? minX_gt_zero;
        private double? minX;
        private double? maxX;

        private bool setFromDateTime = false;
        private bool setFromTimeSpan = false;
        private bool setFromNumber = false;

        private void setFromObject(object[] x, double?[] y, ObjectToDoubleConverter converter)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("y array must have same length as x array", "y");

            this.x = new double[x.Length];
            this.y = new double?[x.Length];

            minY_gt_zero = minY = maxY = null;
            minX_gt_zero = minX = maxX = null;

            for (int i = 0; i < x.Length; i++)
            {
                double xv = converter(x[i]);
                double? yv = y[i];
                this.x[i] = xv;
                this.y[i] = yv;

                if (xv > 0 && (minX_gt_zero == null || xv < minX_gt_zero.Value))
                    minX_gt_zero = xv;
                if (minX == null || xv < minX.Value)
                    minX = xv;
                if (maxX == null || xv > maxX.Value)
                    maxX = xv;

                if (yv > 0 && (minY_gt_zero == null || yv < minY_gt_zero.Value))
                    minY_gt_zero = yv;
                if (minY == null || yv < minY.Value)
                    minY = yv;
                if (maxY == null || yv > maxY.Value)
                    maxY = yv;
            }

            raiseEvent();
        }

        public void Set(double[] x, double?[] y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            object[] xnew = new object[x.Length];
            x.CopyTo(xnew, 0);
            setFromObject(xnew, y, delegate(object a) { return (double)a; });
        }

        public void Set(DateTime[] x, double?[] y)
        {
            setFromNumber = setFromTimeSpan = false;
            setFromDateTime = true;
            object[] xnew = new object[x.Length];
            x.CopyTo(xnew, 0);
            setFromObject(xnew, y, delegate(object a) { return ((DateTime)a).ToOADate(); });
        }

        public void Set(TimeSpan[] x, double?[] y)
        {
            setFromNumber = setFromDateTime = false;
            setFromTimeSpan = true;
            object[] xnew = new object[x.Length];
            x.CopyTo(xnew, 0);
            setFromObject(xnew, y, delegate(object a) { return ((TimeSpan)a).Ticks / TimeSpan.TicksPerDay; });
        }

        public void Set(IList<double> x, IList<double> y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            double[] xnew = new double[x.Count];
            x.CopyTo(xnew, 0);
            double?[] ynew = new double?[y.Count];
            for (int i = 0; i < y.Count; i++)
                ynew[i] = y[i];
            this.Set(xnew, ynew);
        }

        public void Set(IList<DateTime> x, IList<double> y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            DateTime[] xnew = new DateTime[x.Count];
            x.CopyTo(xnew, 0);
            double?[] ynew = new double?[y.Count];
            for (int i = 0; i < y.Count; i++)
                ynew[i] = y[i];
            this.Set(xnew, ynew);
        }

        public void Set(IList<TimeSpan> x, IList<double> y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            TimeSpan[] xnew = new TimeSpan[x.Count];
            x.CopyTo(xnew, 0);
            double?[] ynew = new double?[y.Count];
            for (int i = 0; i < y.Count; i++)
                ynew[i] = y[i];
            this.Set(xnew, ynew);
        }

        public void Set(IList<double> x, IList<double?> y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            double[] xnew = new double[x.Count];
            x.CopyTo(xnew, 0);
            double?[] ynew = new double?[y.Count];
            y.CopyTo(ynew, 0);
            this.Set(xnew, ynew);
        }

        public void Set(IList<DateTime> x, IList<double?> y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            DateTime[] xnew = new DateTime[x.Count];
            x.CopyTo(xnew, 0);
            double?[] ynew = new double?[y.Count];
            y.CopyTo(ynew, 0);
            this.Set(xnew, ynew);
        }

        public void Set(IList<TimeSpan> x, IList<double?> y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            TimeSpan[] xnew = new TimeSpan[x.Count];
            x.CopyTo(xnew, 0);
            double?[] ynew = new double?[y.Count];
            y.CopyTo(ynew, 0);
            this.Set(xnew, ynew);
        }

        public void Clear()
        {
            minY_gt_zero = minY = maxY = null;
            minX_gt_zero = minX = maxX = null;
            this.x = new double[0];
            this.y = new double?[0];
            raiseEvent();
        }

        public double GetX(int idx)
        {
            return x[idx];
        }

        public double? GetY(int idx)
        {
            return y[idx];
        }

        public int Count
        {
            get
            {
                return x.Length;
            }
        }

        public double? MinY_gt_Zero
        {
            get
            {
                return minY_gt_zero;
            }
        }

        public double? MinY
        {
            get
            {
                return minY;
            }
        }

        public double? MaxY
        {
            get
            {
                return maxY;
            }
        }

        public double? MinX_gt_Zero
        {
            get
            {
                return minX_gt_zero;
            }
        }
         
        public double? MinX
        {
            get
            {
                return minX;
            }
        }

        public double? MaxX
        {
            get
            {
                return maxX;
            }
        }

        public bool IsSetFromDateTime
        {
            get
            {
                return setFromDateTime;
            }
        }

        public bool IsSetFromTimeSpan
        {
            get
            {
                return setFromTimeSpan;
            }
        }

        public bool IsSetFromNumber
        {
            get
            {
                return setFromNumber;
            }
        }

        private void raiseEvent()
        {
            PointCollectionChangedHandler tmp = OnPointCollectionChanged;
            if (tmp != null)
                tmp();
        }
    }
}
