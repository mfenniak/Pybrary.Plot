using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Pybrary.Plot
{
    public delegate void AxisChangedHandler();

    public abstract class Axis : EventObject
    {
        protected FontDescription labelFont = new FontDescription("Arial", 12f, FontStyle.Regular);
        protected float tickLength = 0.1f; // in inches
        protected PenDescription tickPen = new PenDescription(Color.Black, 1f / 96);
        protected bool gridlinesEnabled = true;
        protected PenDescription gridlinePen = new PenDescription(Color.FromArgb(192, 192, 192), 1f / 96);

        protected AdvancedRect? drawArea;

        public bool GridlinesEnabled
        {
            get
            {
                return gridlinesEnabled;
            }
            set
            {
                gridlinesEnabled = value;
                raiseEvent();
            }
        }

        public AdvancedRect? DrawArea
        {
            get
            {
                return drawArea;
            }
        }
    }
}
