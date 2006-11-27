using System;
using System.Drawing;

namespace Pybrary.Plot
{
    public delegate void SymbolDescriptionChangedHandler();

    public class SymbolDescription
    {
        public event SymbolDescriptionChangedHandler OnSymbolDescriptionChanged;

        private SymbolType type;
        private PenDescription foreground;
        private BrushDescription background;
        private float size;

        public SymbolDescription(SymbolType type, PenDescription f, BrushDescription b, float size)
        {
            this.type = type;
            this.foreground = f;
            this.background = b;
            this.size = size;

            this.foreground.OnPenDescriptionChanged += delegate() { raiseChanged(); };
            this.background.OnBrushDescriptionChanged += delegate() { raiseChanged(); };
        }

        public SymbolType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                raiseChanged();
            }
        }

        public PenDescription Foreground
        {
            get
            {
                return foreground;
            }
        }

        public BrushDescription Background
        {
            get
            {
                return background;
            }
        }

        public float Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                raiseChanged();
            }
        }

        public Symbol CreateSymbol()
        {
            return SymbolFactory.CreateSymbol(type, foreground, background, size);
        }

        private void raiseChanged()
        {
            SymbolDescriptionChangedHandler tmp = OnSymbolDescriptionChanged;
            if (tmp != null)
                tmp();
        }   
    }
}
