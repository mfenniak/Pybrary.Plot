using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Pybrary.Plot
{
    public class FontDescription : BrushDescription
    {
        private string familyName = "Arial";
        private FontStyle style = FontStyle.Regular;
        private float size = 12;

        public FontDescription(string familyName, float size, FontStyle style)
            : base(Color.Black)
        {
            this.familyName = familyName;
            this.size = size;
            this.style = style;
        }

        public Font CreateFont()
        {
            return new Font(familyName, size, style);
        }
    }
}
