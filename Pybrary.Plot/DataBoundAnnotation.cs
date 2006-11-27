using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pybrary.Plot
{
    public enum AnnotationCrosshair
    {
        Left,
        Top,
        Right,
        Bottom
    }

    /// <summary>
    /// An annotation that is bound to a data location.  It's location is
    /// specified by an X and a Y coordinate, and the name of the Y axis
    /// that the Y coordinate refers to.
    /// </summary>
    public class DataBoundAnnotation : Annotation
    {
        private float crosshairCircleRadius = 0.05f;
        private float crosshairHandleSize = 0.05f;
        private PenDescription crosshairPen = new PenDescription(Color.Black, 1f / 96);

        private string yAxisName = "Default";

        private double xCoordinate;
        private double yCoordinate;

        private AnnotationCrosshair crosshair = AnnotationCrosshair.Left;

        public DataBoundAnnotation(Plot plot) 
            : base(plot)
        {
        }

        public override void Draw(Graphics g, AdvancedRect dataArea)
        {
            PointF pt = new PointF(
                Plot.XAxis.DataToCoordinate(X, dataArea),
                Plot.YAxes[YAxisName].DataToCoordinate(Y, dataArea));

            using (Pen p = crosshairPen.CreatePen())
            {
                g.DrawLine(p, pt.X - crosshairCircleRadius, pt.Y, pt.X + crosshairCircleRadius, pt.Y);
                g.DrawLine(p, pt.X, pt.Y - crosshairCircleRadius, pt.X, pt.Y + crosshairCircleRadius);
                g.DrawEllipse(p, pt.X - crosshairCircleRadius, pt.Y - crosshairCircleRadius, crosshairCircleRadius * 2, crosshairCircleRadius * 2);

                SizeF size = Size(g);
                switch (crosshair)
                {
                    case AnnotationCrosshair.Left:
                        g.DrawLine(p, pt.X + crosshairCircleRadius, pt.Y, pt.X + crosshairCircleRadius + crosshairHandleSize, pt.Y);
                        pt.X += crosshairCircleRadius;
                        pt.X += crosshairHandleSize;
                        pt.Y -= size.Height / 2;
                        break;
                    case AnnotationCrosshair.Right:
                        g.DrawLine(p, pt.X - crosshairCircleRadius, pt.Y, pt.X - crosshairCircleRadius - crosshairHandleSize, pt.Y);
                        pt.X -= crosshairCircleRadius;
                        pt.X -= crosshairHandleSize;
                        pt.X -= size.Width;
                        pt.Y -= size.Height / 2;
                        break;
                    case AnnotationCrosshair.Bottom:
                        g.DrawLine(p, pt.X, pt.Y - crosshairCircleRadius, pt.X, pt.Y - crosshairCircleRadius - crosshairHandleSize);
                        pt.X -= size.Width / 2;
                        pt.Y -= size.Height;
                        pt.Y -= crosshairCircleRadius;
                        pt.Y -= crosshairHandleSize;
                        break;
                    case AnnotationCrosshair.Top:
                        g.DrawLine(p, pt.X, pt.Y + crosshairCircleRadius, pt.X, pt.Y + crosshairCircleRadius + crosshairHandleSize);
                        pt.X -= size.Width / 2;
                        pt.Y += crosshairCircleRadius;
                        pt.Y += crosshairHandleSize;
                        break;
                }
            }
            DrawTextBox(g, pt);
        }

        public AnnotationCrosshair Crosshair
        {
            get
            {
                return crosshair;
            }
            set
            {
                crosshair = value;
            }
        }

        public virtual string YAxisName
        {
            get
            {
                return yAxisName;
            }
            set
            {
                yAxisName = value;
                raiseEvent();
            }
        }

        public virtual double X
        {
            get
            {
                return xCoordinate;
            }
            set
            {
                xCoordinate = value;
                raiseEvent();
            }
        }

        public virtual DateTime XAsDateTime
        {
            get
            {
                return DateTime.FromOADate(xCoordinate);
            }
            set
            {
                xCoordinate = value.ToOADate();
                raiseEvent();
            }
        }

        public virtual double Y
        {
            get
            {
                return yCoordinate;
            }
            set
            {
                yCoordinate = value;
                raiseEvent();
            }
        }
    }
}
