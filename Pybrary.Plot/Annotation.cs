using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pybrary.Plot
{
    public abstract class Annotation : EventObject
    {
        private BrushDescription background = new BrushDescription(Color.FromArgb(192, Color.White));
        private FontDescription textFont = new FontDescription("Arial", 12f, FontStyle.Regular);
        private PenDescription border = new PenDescription(Color.Black, 1f / 96);
        private string text;
        private Plot plot;

        public Annotation(Plot plot)
        {
            this.plot = plot;
            background.OnBrushDescriptionChanged += delegate() { raiseEvent(); };
            textFont.OnBrushDescriptionChanged += delegate() { raiseEvent(); };
            border.OnPenDescriptionChanged += delegate() { raiseEvent(); };
        }

        protected Plot Plot
        {
            get
            {
                return plot;
            }
        }

        public abstract void Draw(Graphics g, AdvancedRect dataArea);

        protected SizeF Size(Graphics g)
        {
            using (Font f = TextFont.CreateFont(g))
                return g.MeasureString(text, f);
        }

        protected void DrawTextBox(Graphics g, PointF pt)
        {
            SizeF size = Size(g);

            using (Pen p = Border.CreatePen())
            using (Brush br = Background.CreateBrush())
            {
                RectangleF rect = new RectangleF(pt, size);
                g.FillRectangle(br, rect);
                g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
            }

            using (Font f = TextFont.CreateFont(g))
            using (Brush br = TextFont.CreateBrush())
            {
                g.DrawString(Text, f, br, pt);
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                raiseEvent();
            }
        }

        public BrushDescription Background
        {
            get
            {
                return background;
            }
        }

        public FontDescription TextFont
        {
            get
            {
                return textFont;
            }
        }

        public PenDescription Border
        {
            get
            {
                return border;
            }
        }
    }
}
