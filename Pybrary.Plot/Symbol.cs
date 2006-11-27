using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace Pybrary.Plot
{
    public enum SymbolType
    {
        Square, Circle, XCross, Cross, TriangleUp, Diamond,
        TriangleDown, TriangleLeft, TriangleRight
    };

    public static class SymbolFactory
    {
        public static IEnumerable<SymbolType> StandardSymbols
        {
            get
            {
                yield return SymbolType.Square;
                yield return SymbolType.Circle;
                yield return SymbolType.XCross;
                yield return SymbolType.Cross;
                yield return SymbolType.TriangleUp;
                yield return SymbolType.Diamond;
                yield return SymbolType.TriangleDown;
                yield return SymbolType.TriangleLeft;
                yield return SymbolType.TriangleRight;
                // Never run out - just repeat ourselves.
                foreach (SymbolType s in SymbolFactory.StandardSymbols)
                    yield return s;
            }
        }

        public static Symbol CreateSymbol(SymbolType type, PenDescription f, BrushDescription b, float size)
        {
            switch (type)
            {
                case SymbolType.Square:
                    return new SquareSymbol(f, b, size);
                case SymbolType.Circle:
                    return new CircleSymbol(f, b, size);
                case SymbolType.XCross:
                    return new XCrossSymbol(f, size);
                case SymbolType.Cross:
                    return new CrossSymbol(f, size);
                case SymbolType.Diamond:
                    return new DiamondSymbol(f, b, size);
                case SymbolType.TriangleUp:
                    return new TriangleUpSymbol(f, b, size);
                case SymbolType.TriangleDown:
                    return new TriangleDownSymbol(f, b, size);
                case SymbolType.TriangleLeft:
                    return new TriangleLeftSymbol(f, b, size);
                case SymbolType.TriangleRight:
                    return new TriangleRightSymbol(f, b, size);
            }
            return null;
        }
    }

    public abstract class Symbol : IDisposable
    {
        protected float size;

        public Symbol(float size)
        {
            this.size = size;
        }

        public abstract void DrawCenteredAt(Graphics g, PointF point);

        public virtual void Dispose()
        {
        }
    }

    abstract class PenSymbol : Symbol
    {
        protected Pen p;
        public PenSymbol(PenDescription f, float size)
            : base(size)
        {
            p = f.CreatePen();
        }
        public override void Dispose()
        {
            p.Dispose();
            p = null;
        }
    }

    abstract class PenBrushSymbol : PenSymbol
    {
        protected Brush br;
        public PenBrushSymbol(PenDescription f, BrushDescription b, float size)
            : base(f, size)
        {
            this.br = b.CreateBrush();
        }
        public override void Dispose()
        {
            br.Dispose();
            br = null;
            base.Dispose();
        }
    }

    class CircleSymbol : PenBrushSymbol
    {
        public CircleSymbol(PenDescription f, BrushDescription b, float size) : base(f, b, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            g.FillEllipse(br, point.X - (size / 2), point.Y - (size / 2), size, size);
            g.DrawEllipse(p, point.X - (size / 2), point.Y - (size / 2), size, size);
        }
    }

    class SquareSymbol : PenBrushSymbol
    {
        public SquareSymbol(PenDescription f, BrushDescription b, float size) : base(f, b, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            g.FillRectangle(br, point.X - (size / 2), point.Y - (size / 2), size, size);
            g.DrawRectangle(p, point.X - (size / 2), point.Y - (size / 2), size, size);
        }
    }

    class XCrossSymbol : PenSymbol
    {
        public XCrossSymbol(PenDescription f, float size) : base(f, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            float half = size / 2;
            g.DrawLine(p, point.X - half, point.Y - half, point.X + half, point.Y + half);
            g.DrawLine(p, point.X - half, point.Y + half, point.X + half, point.Y - half);
        }
    }

    class CrossSymbol : PenSymbol
    {
        public CrossSymbol(PenDescription f, float size) : base(f, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            float half = size / 2;
            g.DrawLine(p, point.X - half, point.Y, point.X + half, point.Y);
            g.DrawLine(p, point.X, point.Y + half, point.X, point.Y - half);
        }
    }

    class DiamondSymbol : PenBrushSymbol
    {
        public DiamondSymbol(PenDescription f, BrushDescription b, float size) : base(f, b, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            float half = size / 2;
            PointF[] poly = new PointF[] {
                new PointF(point.X - half, point.Y),
                new PointF(point.X, point.Y - half),
                new PointF(point.X + half, point.Y),
                new PointF(point.X, point.Y + half)
            };
            g.FillPolygon(br, poly);
            g.DrawPolygon(p, poly);
        }
    }

    class TriangleDownSymbol : PenBrushSymbol
    {
        public TriangleDownSymbol(PenDescription f, BrushDescription b, float size) : base(f, b, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            float half = size / 2;
            PointF[] poly = new PointF[] {
                new PointF(point.X, point.Y + half),
                new PointF(point.X - half, point.Y - half),
                new PointF(point.X + half, point.Y - half),
            };
            g.FillPolygon(br, poly);
            g.DrawPolygon(p, poly);
        }
    }

    class TriangleLeftSymbol : PenBrushSymbol
    {
        public TriangleLeftSymbol(PenDescription f, BrushDescription b, float size) : base(f, b, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            float half = size / 2;
            PointF[] poly = new PointF[] {
                new PointF(point.X - half, point.Y),
                new PointF(point.X + half, point.Y - half),
                new PointF(point.X + half, point.Y + half),
            };
            g.FillPolygon(br, poly);
            g.DrawPolygon(p, poly);
        }
    }

    class TriangleRightSymbol : PenBrushSymbol
    {
        public TriangleRightSymbol(PenDescription f, BrushDescription b, float size) : base(f, b, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            float half = size / 2;
            PointF[] poly = new PointF[] {
                new PointF(point.X + half, point.Y),
                new PointF(point.X - half, point.Y + half),
                new PointF(point.X - half, point.Y - half),
            };
            g.FillPolygon(br, poly);
            g.DrawPolygon(p, poly);
        }
    }

    class TriangleUpSymbol : PenBrushSymbol
    {
        public TriangleUpSymbol(PenDescription f, BrushDescription b, float size) : base(f, b, size) { }
        public override void DrawCenteredAt(Graphics g, PointF point)
        {
            float half = size / 2;
            PointF[] poly = new PointF[] {
                new PointF(point.X, point.Y - half),
                new PointF(point.X + half, point.Y + half),
                new PointF(point.X - half, point.Y + half),
            };
            g.FillPolygon(br, poly);
            g.DrawPolygon(p, poly);
        }
    }
}
