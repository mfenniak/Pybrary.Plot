using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Pybrary.Plot
{
    public delegate void PenDescriptionChangedHandler();

    public class PenDescription
    {
        public event PenDescriptionChangedHandler OnPenDescriptionChanged;

        private Color color;
        private float width;

        public PenDescription(Color color, float width)
        {
            this.color = color;
            this.width = width;
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                raiseChanged();
            }
        }

        public Pen CreatePen()
        {
            return new Pen(color, width);
        }

        private void raiseChanged()
        {
            PenDescriptionChangedHandler tmp = OnPenDescriptionChanged;
            if (tmp != null)
                tmp();
        }
    }
}
