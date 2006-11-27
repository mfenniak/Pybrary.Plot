using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pybrary.Plot
{
    public delegate void BrushDescriptionChangedHandler();

    public enum BrushStyle
    {
        SolidBrushStyle,
        HatchBrushStyle
    }

    public class BrushDescription
    {
        public event BrushDescriptionChangedHandler OnBrushDescriptionChanged;

        private BrushStyle style;
        private Color color;
        private HatchStyle hatchStyle;
        private Color backgroundColor;

        public BrushDescription()
        {
            this.color = Color.Black;
            this.style = BrushStyle.SolidBrushStyle;
        }

        public BrushDescription(Color color)
        {
            this.color = color;
            this.style = BrushStyle.SolidBrushStyle;
        }

        public BrushStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                raiseChanged();
            }
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

        public HatchStyle HatchStyle
        {
            get
            {
                return hatchStyle;
            }
            set
            {
                hatchStyle = value;
                raiseChanged();
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                raiseChanged();
            }
        }

        public Brush CreateBrush()
        {
            switch (style)
            {
                case BrushStyle.SolidBrushStyle:
                    return new SolidBrush(Color);

                case BrushStyle.HatchBrushStyle:
                    return new HatchBrush(HatchStyle, Color, BackgroundColor);
            }

            return null;
        }

        private void raiseChanged()
        {
            BrushDescriptionChangedHandler tmp = OnBrushDescriptionChanged;
            if (tmp != null)
                tmp();
        }
    }
}
