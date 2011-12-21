using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Pybrary.Plot
{
    public delegate void AxisCollectionChangedHandler();

    public class AxisCollection : IEnumerable<KeyValuePair<string, NumericYAxis>>
    {
        public event AxisCollectionChangedHandler OnAxisCollectionChanged;

        private Dictionary<string, NumericYAxis> axesByName = new Dictionary<string, NumericYAxis>();
        private List<NumericYAxis> leftAxes = new List<NumericYAxis>();
        private List<NumericYAxis> rightAxes = new List<NumericYAxis>();
        private float axisSpacing = 0.1f;   // spacing between multiple axis on one side, in inches

        private Plot parent;
        private float[] axisWidthLeft;
        private float[] axisWidthRight;

        public AxisCollection(Plot parent)
        {
            this.parent = parent;
        }

        public NumericAxis this[string name]
        {
            get
            {
                return axesByName[name];
            }
        }

        public NumericYAxis AddLeft(string name)
        {
            NumericYAxis axis = new NumericYAxis(name, parent.Series, false, parent.BorderPen);
            axis.OnChanged += delegate(object sender, EventArgs args) { raiseChanged(); };
            leftAxes.Add(axis);
            axesByName[axis.Name] = axis;
            return axis;
        }

        public NumericYAxis AddRight(string name)
        {
            NumericYAxis axis = new NumericYAxis(name, parent.Series, true, parent.BorderPen);
            axis.OnChanged += delegate(object sender, EventArgs args) { raiseChanged(); };
            rightAxes.Add(axis);
            axesByName[axis.Name] = axis;
            return axis;
        }

        public float CalculateWidthLeft(Graphics g)
        {
            return calculateWidth(g, leftAxes, ref axisWidthLeft);
        }

        public float CalculateWidthRight(Graphics g)
        {
            return calculateWidth(g, rightAxes, ref axisWidthRight);
        }

        private float calculateWidth(Graphics g, List<NumericYAxis> axes, ref float[] axisWidth)
        {
            axisWidth = new float[axes.Count];
            float total = 0;
            for (int i = 0; i < axes.Count; i++)
            {
                float f;
                if (axes[i].Visible)
                    f = axes[i].CalculateWidth(g);
                else
                    f = 0;
                axisWidth[i] = f;
                total += f;
            }
            // Add spacing between multiple axes
            if (axes.Count != 0)
                total += axisSpacing * (axes.Count - 1);
            return total;
        }

        public void Draw(Graphics g, AdvancedRect dataArea)
        {
            AdvancedRect area = dataArea.Clone();
            for (int i = 0; i < leftAxes.Count; i++)
            {
                if (!leftAxes[i].Visible)
                    continue;
                area.BottomRight.X = area.TopLeft.X;
                area.TopLeft.X -= axisWidthLeft[i];
                leftAxes[i].DrawY(g, area, dataArea);
                area.TopLeft.X -= axisSpacing;
            }
            area = dataArea.Clone();
            for (int i = 0; i < rightAxes.Count; i++)
            {
                if (!rightAxes[i].Visible)
                    continue;
                area.TopLeft.X = area.BottomRight.X;
                area.BottomRight.X += axisWidthRight[i];
                rightAxes[i].DrawY(g, area, dataArea);
                area.BottomRight.X += axisSpacing;
            }
        }

        public YAxis FindAxisAt(PointF pt)
        {
            for (int i = 0; i < leftAxes.Count; i++)
                if (leftAxes[i].DrawArea.HasValue && leftAxes[i].DrawArea.Value.Rect.Contains(pt))
                    return leftAxes[i];
            for (int i = 0; i < rightAxes.Count; i++)
                if (rightAxes[i].DrawArea.HasValue && rightAxes[i].DrawArea.Value.Rect.Contains(pt))
                    return rightAxes[i];
            return null;
        }

        public IEnumerator<KeyValuePair<string, NumericYAxis>> GetEnumerator()
        {
            return axesByName.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return axesByName.GetEnumerator();
        }

        private void raiseChanged()
        {
            AxisCollectionChangedHandler tmp = OnAxisCollectionChanged;
            if (tmp != null)
                tmp();
        }

        public ReadOnlyCollection<NumericYAxis> LeftAxes
        {
            get { return leftAxes.AsReadOnly(); }
        }

        public ReadOnlyCollection<NumericYAxis> RightAxes
        {
            get { return rightAxes.AsReadOnly(); }
        }
    }
}
