using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Pybrary.Plot
{
    public struct AdvancedRect
    {
        public PointF TopLeft;
        public PointF BottomRight;

        public AdvancedRect(PointF topLeft, PointF bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        public AdvancedRect(RectangleF orig)
        {
            TopLeft = new PointF(orig.X, orig.Y);
            BottomRight = new PointF(orig.Width + orig.X, orig.Height + orig.Y);
        }

        public AdvancedRect(AdvancedRect orig)
        {
            TopLeft = new PointF(orig.TopLeft.X, orig.TopLeft.Y);
            BottomRight = new PointF(orig.BottomRight.X, orig.BottomRight.Y);
        }

        public AdvancedRect Clone()
        {
            return new AdvancedRect(this);
        }

        public float Width
        {
            get
            {
                return BottomRight.X - TopLeft.X;
            }
        }

        public float Height
        {
            get
            {
                return BottomRight.Y - TopLeft.Y;
            }
        }

        public Rectangle Rect2
        {
            get
            {
                return new Rectangle((int)TopLeft.X, (int)TopLeft.Y, (int)Width, (int)Height);
            }
        }

        public RectangleF Rect
        {
            get
            {
                return new RectangleF(TopLeft, this.Size);
            }
        }

        public SizeF Size
        {
            get
            {
                return new SizeF(this.Width, this.Height);
            }
        }

        public PointF Center
        {
            get
            {
                return new PointF(
                    (TopLeft.X + BottomRight.X) / 2,
                    (TopLeft.Y + BottomRight.Y) / 2
                    );
            }
        }

    }
}
