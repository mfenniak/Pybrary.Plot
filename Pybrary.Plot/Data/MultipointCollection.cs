using System;
using System.Collections.Generic;
using System.Text;

namespace Pybrary.Plot.Data
{
    public class MultipointCollection : EventObject
    {
        private double[] x = new double[0];
        private double?[][] y = new double?[0][];

        private double? minX_gt_zero;
        private double? minX;
        private double? maxX;

        private bool setFromDateTime = false;
        private bool setFromTimeSpan = false;
        private bool setFromNumber = false;

        private void setFromObject(object[] x, IList<double?>[] y, ObjectToDoubleConverter converter)
        {
            foreach (List<double?> yv in y)
                if (x.Length != yv.Count)
                    throw new ArgumentException("y array must have same length as x array", "y");

            this.x = new double[x.Length];
            this.y = new double?[x.Length][];
            for (int i = 0; i < x.Length; i++)
                this.y[i] = new double?[y.Length];

            minX_gt_zero = minX = maxX = null;

            for (int i = 0; i < x.Length; i++)
            {
                double xv = converter(x[i]);
                this.x[i] = xv;

                if (xv > 0 && (minX_gt_zero == null || xv < minX_gt_zero.Value))
                    minX_gt_zero = xv;
                if (minX == null || xv < minX.Value)
                    minX = xv;
                if (maxX == null || xv > maxX.Value)
                    maxX = xv;

                for (int j = 0; j < y.Length; j++)
                    this.y[i][j] = y[j][i];
            }

            raiseEvent();
        }

        public void Set(double[] x, IList<double?>[] y)
        {
            setFromDateTime = setFromTimeSpan = false;
            setFromNumber = true;
            object[] xnew = new object[x.Length];
            x.CopyTo(xnew, 0);
            setFromObject(xnew, y, delegate(object a) { return (double)a; });
        }

        public void Set(DateTime[] x, IList<double?>[] y)
        {
            setFromNumber = setFromTimeSpan = false;
            setFromDateTime = true;
            object[] xnew = new object[x.Length];
            x.CopyTo(xnew, 0);
            setFromObject(xnew, y, delegate(object a) { return ((DateTime)a).ToOADate(); });
        }

        public void Set(TimeSpan[] x, IList<double?>[] y)
        {
            setFromNumber = setFromDateTime = false;
            setFromTimeSpan = true;
            object[] xnew = new object[x.Length];
            x.CopyTo(xnew, 0);
            setFromObject(xnew, y, delegate(object a) { return ((TimeSpan)a).Ticks / TimeSpan.TicksPerDay; });
        }

        public void Clear()
        {
            minX_gt_zero = minX = maxX = null;
            this.x = new double[0];
            this.y = new double?[0][];
            raiseEvent();
        }

        public double GetX(int idx)
        {
            return x[idx];
        }

        public double?[] GetY(int idx)
        {
            return y[idx];
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

        public int Count
        {
            get
            {
                return x.Length;
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
    }
}
