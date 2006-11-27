using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pybrary.Plot
{
    public class PlotControl : Control
    {
        private BrushDescription selectionBrush =
            new BrushDescription(Color.FromArgb(100, Color.FromKnownColor(KnownColor.Highlight)));

        private Plot plot = null;
        private EventHandler drawHandler;
        private IList<ToolStripMenuItem> scaleMenuItems = new List<ToolStripMenuItem>();

        public PlotControl()
        {
            drawHandler = delegate(object sender, EventArgs args) { Invalidate(); };
            this.ResizeRedraw = true;
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PlotControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PlotControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PlotControl_MouseUp);
            this.LostFocus += new System.EventHandler(this.PlotControl_LostFocus);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (plot == null)
            {
                using (SolidBrush white = new SolidBrush(Color.White))
                    pe.Graphics.FillRectangle(white, pe.ClipRectangle);
            }
            else
            {
                plot.PaintOn(pe.Graphics, this.ClientRectangle);
                if (selectionBr != null)
                {
                    GraphicsState _s = pe.Graphics.Save();
                    pe.Graphics.PageUnit = GraphicsUnit.Inch;
                    pe.Graphics.FillRectangle(selectionBr, selectionRect.Rect);
                    pe.Graphics.Restore(_s);
                }
                RepopulateScaleMenus();
            }
        }

        public Plot Plot
        {
            get
            {
                return plot;
            }
            set
            {
                if (plot != null)
                    plot.OnChanged -= drawHandler;
                plot = value;
                if (plot != null)
                    plot.OnChanged += drawHandler;
                Invalidate();
            }
        }

        

        public ToolStripMenuItem CreatePlotMenu()
        {
            ToolStripMenuItem plotMenu = new ToolStripMenuItem("Plot");

            ToolStripMenuItem manualScale = new ToolStripMenuItem("Set Scale...");
            scaleMenuItems.Add(manualScale);
            RepopulateScaleMenus();
            plotMenu.DropDownItems.Add(manualScale);

            ToolStripMenuItem zoom = new ToolStripMenuItem("Zoom");
            zoom.Click += delegate(object sender, EventArgs e) { enableZoom(); };
            zoom.Paint += delegate(object sender, PaintEventArgs e) { zoom.Enabled = (plot != null); };
            plotMenu.DropDownItems.Add(zoom);

            ToolStripMenuItem zoomReset = new ToolStripMenuItem("Reset Zoom");
            zoomReset.Click += delegate(object sender, EventArgs e) { resetZoom(); };
            zoomReset.Paint += new PaintEventHandler(ResetPaintHandler);
            plotMenu.DropDownItems.Add(zoomReset);

            plotMenu.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem clip = new ToolStripMenuItem("Copy to Clipboard");
            clip.Click += delegate(object sender, EventArgs e) { copyToClipboard(); };
            clip.Paint += delegate(object sender, PaintEventArgs e) { zoom.Enabled = (plot != null); };
            plotMenu.DropDownItems.Add(clip);

            return plotMenu;
        }

        private void RepopulateScaleMenus()
        {
            foreach (ToolStripMenuItem item in scaleMenuItems)
            {
                item.DropDownItems.Clear();
                if (plot == null)
                {
                    item.Enabled = false;
                    return;
                }
                else
                    item.Enabled = true;

                foreach (KeyValuePair<string, NumericYAxis> kv in plot.YAxes)
                {
                    NumericAxis axis = kv.Value;
                    if (axis.Visible)
                    {
                        ToolStripMenuItem newitem = new ToolStripMenuItem(axis.Title);
                        newitem.Click += delegate(object sender, EventArgs e) { ManuallyScale(axis); };
                        item.DropDownItems.Add(newitem);
                    }
                }
            }
        }

        private void ManuallyScale(NumericAxis axis)
        {
            ManualNumericAxisForm form = new ManualNumericAxisForm(axis);
            form.ShowDialog();
        }

        private void ResetPaintHandler(object sender, PaintEventArgs e)
        {
            ToolStripMenuItem zoomReset = sender as ToolStripMenuItem;
            bool enabled = false;
            if (plot != null)
            {
                enabled = enabled || plot.XAxis.ZoomedMinimum.HasValue || plot.XAxis.ZoomedMaximum.HasValue;
                foreach (KeyValuePair<string, NumericYAxis> kv in plot.YAxes)
                {
                    NumericYAxis y = kv.Value;
                    enabled = enabled || y.ZoomedMinimum.HasValue || y.ZoomedMaximum.HasValue;
                }
            }
            zoomReset.Enabled = enabled;
        }

        private void copyToClipboard()
        {
            if (plot == null)
                return;

            DataObject o = new DataObject();

            using (Bitmap bmp = new Bitmap(800, 600))
            {
                // Bitmap
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.PageUnit = GraphicsUnit.Pixel;
                    plot.PaintOn(g, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    g.Flush();

                    o.SetData(DataFormats.Bitmap, true, bmp);
                }

                o.SetData(DataFormats.Text, true, plot.Series.TextExport());

                Clipboard.SetDataObject(o, true);
            }
        }

        private bool mouseDown = false;
        private PointF mouseDownLocation;

        private bool zoomEnabled = false;
        private Cursor savedCursor = null;

        private bool zoomingPlotArea;
        private bool zoomingXAxis;
        private YAxis zoomingYAxis;

        private void enableZoom()
        {
            if (zoomEnabled)
                return;
            savedCursor = this.Cursor;
            //this.Cursor = Cursors.Cross;
            zoomEnabled = true;
        }

        private void disableZoom()
        {
            if (zoomEnabled)
            {
                this.Cursor = savedCursor;
                zoomingPlotArea = zoomingXAxis = false;
                zoomingYAxis = null;
                savedCursor = null;
                zoomEnabled = false;
            }
        }

        private void resetZoom()
        {
            using (plot.SuspendEvents())
            {
                plot.XAxis.ZoomedMinimum = null;
                plot.XAxis.ZoomedMaximum = null;
                foreach (KeyValuePair<string, NumericYAxis> kv in plot.YAxes)
                {
                    NumericYAxis y = kv.Value;
                    y.ZoomedMinimum = y.ZoomedMaximum = null;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.WParam.ToInt32() == Convert.ToInt32(Keys.Escape))
            {
                disableZoom();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void PlotControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.plot != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    mouseDown = true;
                    mouseDownLocation = screenToInches(e.Location);
                }
            }
        }

        private void PlotControl_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            finishMouseDragging();
        }

        private void PlotControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (plot == null)
                return;

            PointF cursor = screenToInches(e.Location);
            if (!mouseDragging && mouseDown && zoomEnabled)
            {
                // Dragging the mouse - ignore small drags
                if (Math.Abs(cursor.X - mouseDownLocation.X) < 0.05 &&
                    Math.Abs(cursor.Y - mouseDownLocation.Y) < 0.05)
                    return;
                beginMouseDragging(e.Location);
            }

            if (mouseDragging)
                duringMouseDragging(e.Location);

            if (!mouseDragging && zoomEnabled)
            {
                // otherwise, let's try to find where we are...
                if (plot.DataArea.Rect.Contains(cursor))
                    this.Cursor = Cursors.Cross;
                else if (plot.XAxis.DrawArea.HasValue && plot.XAxis.DrawArea.Value.Rect.Contains(cursor))
                    this.Cursor = Cursors.SizeWE;
                else if (plot.YAxes.FindAxisAt(cursor) != null)
                    this.Cursor = Cursors.SizeNS;
                else
                    this.Cursor = Cursors.Default;
            }
        }

        private void PlotControl_LostFocus(object sender, EventArgs e)
        {
            mouseDown = false;
            finishMouseDragging();
        }


        private bool mouseDragging = false;
        private AdvancedRect selectionRect;
        private Brush selectionBr;

        private void beginMouseDragging(Point location)
        {
            PointF cursor = screenToInches(location);

            Console.WriteLine("Begin dragging.");
            mouseDragging = true;
            selectionBr = selectionBrush.CreateBrush();

            // otherwise, let's try to find where we are...
            if (plot.DataArea.Rect.Contains(cursor))
                zoomingPlotArea = true;
            else if (plot.XAxis.DrawArea.HasValue && plot.XAxis.DrawArea.Value.Rect.Contains(cursor))
                zoomingXAxis = true;
            else
            {
                zoomingYAxis = plot.YAxes.FindAxisAt(cursor);
                if (zoomingYAxis == null)
                    finishMouseDragging();
            }
        }

        private void duringMouseDragging(Point cursor)
        {
            PointF realCursor = screenToInches(cursor);
            if (zoomingPlotArea)
            {
                selectionRect = new AdvancedRect(
                    new PointF(
                        Math.Max(plot.DataArea.TopLeft.X, Math.Min(realCursor.X, mouseDownLocation.X)),
                        Math.Max(plot.DataArea.TopLeft.Y, Math.Min(realCursor.Y, mouseDownLocation.Y))),
                    new PointF(
                        Math.Min(plot.DataArea.BottomRight.X, Math.Max(realCursor.X, mouseDownLocation.X)),
                        Math.Min(plot.DataArea.BottomRight.Y, Math.Max(realCursor.Y, mouseDownLocation.Y)))
                );
            }
            else if (zoomingXAxis)
            {
                selectionRect = plot.DataArea;
                selectionRect.TopLeft.X = Math.Max(selectionRect.TopLeft.X, Math.Min(realCursor.X, mouseDownLocation.X));
                selectionRect.BottomRight.X = Math.Min(selectionRect.BottomRight.X, Math.Max(realCursor.X, mouseDownLocation.X));
            }
            else if (zoomingYAxis != null)
            {
                selectionRect = plot.DataArea;
                selectionRect.TopLeft.Y = Math.Max(selectionRect.TopLeft.Y, Math.Min(realCursor.Y, mouseDownLocation.Y));
                selectionRect.BottomRight.Y = Math.Min(selectionRect.BottomRight.Y, Math.Max(realCursor.Y, mouseDownLocation.Y));
            }

            Invalidate();
        }

        private PointF screenToInches(Point x)
        {
            using (Graphics g = this.CreateGraphics())
            {
                GraphicsState _s = g.Save();
                g.PageUnit = GraphicsUnit.Inch;
                PointF[] arr = new PointF[] { new PointF(x.X, x.Y) };
                g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Device, arr);
                g.Restore(_s);
                return arr[0];
            }
        }

        private void finishMouseDragging()
        {
            if (!mouseDragging)
                return;

            using (plot.SuspendEvents())
            {
                AdvancedRect dataArea = plot.DataArea;

                if (zoomingPlotArea || zoomingXAxis)
                {
                    double newMin = plot.XAxis.CoordinateToData(selectionRect.TopLeft.X, dataArea);
                    double newMax = plot.XAxis.CoordinateToData(selectionRect.BottomRight.X, dataArea);
                    plot.XAxis.ZoomedMinimum = newMin;
                    plot.XAxis.ZoomedMaximum = newMax;

                    if (zoomingPlotArea)
                    {
                        foreach (KeyValuePair<string, NumericYAxis> kv in plot.YAxes)
                        {
                            NumericYAxis y = kv.Value;
                            newMax = y.CoordinateToData(selectionRect.TopLeft.Y, dataArea);
                            newMin = y.CoordinateToData(selectionRect.BottomRight.Y, dataArea);
                            y.ZoomedMinimum = newMin;
                            y.ZoomedMaximum = newMax;
                        }
                    }
                }
                else if (zoomingYAxis != null)
                {
                    double newMax = zoomingYAxis.CoordinateToData(selectionRect.TopLeft.Y, dataArea);
                    double newMin = zoomingYAxis.CoordinateToData(selectionRect.BottomRight.Y, dataArea);
                    zoomingYAxis.ZoomedMinimum = newMin;
                    zoomingYAxis.ZoomedMaximum = newMax;
                }

                selectionBr.Dispose();
                selectionBr = null;
                mouseDragging = false;

                disableZoom();
            }
        }
    }
}
