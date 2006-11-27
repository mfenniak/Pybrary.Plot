using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Pybrary.Plot
{
    /// <summary>
    /// Defines margins for an object, in inches.
    /// </summary>
    class Margins
    {
        private float left;
        private float right;
        private float top;
        private float bottom;

        public Margins(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public void Apply(ref AdvancedRect rect)
        {
            rect.TopLeft.X += this.left;
            rect.TopLeft.Y += this.top;
            rect.BottomRight.X -= this.right;
            rect.BottomRight.Y -= this.bottom;
        }
    }
}
