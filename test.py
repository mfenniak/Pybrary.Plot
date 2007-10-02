import clr
clr.AddReferenceToFile("Pybrary.Plot.dll")
clr.AddReference("System.Drawing")
clr.AddReference("System.Windows.Forms")

import System
from Pybrary import Plot
from System import Drawing
from GasProps import GasProps
from System.Drawing import Point, Size
from System.Windows.Forms import Application, Form, Label, DockStyle, MenuStrip

class MainForm(Form):
    def __init__(self):
        self.Text = self.Name = "Gas Property Calculator"
        self.Size = Size(800, 600)

        self.SuspendLayout()

        pc = Plot.PlotControl()
        pc.Location = Point(0, 22)
        pc.Width = 640
        pc.Height = 480
        pc.Dock = DockStyle.Fill
        pc.Plot = self.CreatePlot()
        self.Controls.Add(pc)

        menu = MenuStrip()
        menu.Items.Add(pc.CreatePlotMenu())
        self.Controls.Add(menu)
        self.MainMenuStrip = menu

        self.ResumeLayout()

    def CreatePlot(self):
        plot = Plot.Plot()
        plot.UseNumericXAxis()
        plot.DisplayLegend = True
        
        plot.XAxis.Title = "Pressure (psia)"
        plot.YAxes["Default"].Title = "z-factor"
        plot.YAxes["Default"].AutoscaleIncludesZero = False
        plot.YAxes.AddRight("Viscosity")
        plot.YAxes["Viscosity"].Title = "Viscosity (cp)"
        plot.YAxes["Viscosity"].GridlinesEnabled = False
        #plot.YAxes["Viscosity"].AutoscaleIncludesZero = False
        
        prop = GasProps()
        prop.GG = 0.54
        prop.N2 = prop.CO2 = prop.H2S = 0
        p_data, z_data, mu_data = [], [], []
        for p in range(20, 10000):
            p_data.append(p)
            z_data.append(prop.z(p, 560))
            mu_data.append(prop.mug(p, 560))
        
        series = Plot.ScatterSeries()
        series.Data.Set(System.Array[float](p_data), System.Array[System.Nullable[float]](z_data))
        series.YAxisName = "Default"
        plot.Series[u"z @ 100\u00b0F"] = series
        
        series = Plot.ScatterSeries()
        series.Data.Set(System.Array[float](p_data), System.Array[System.Nullable[float]](mu_data))
        series.YAxisName = "Viscosity"
        series.Line.Color = Drawing.Color.Red
        plot.Series[u"\u03bcg @ 560\u00b0F"] = series

        return plot

form = MainForm()
Application.Run(form)


#import System
#from Pybrary import Plot
#from System import Drawing
#from GasProps import GasProps
#
#
#width_inches = 10
#height_inches = 8
#x_res = 96
#y_res = 96
#bmp = Drawing.Bitmap(width_inches*x_res, height_inches*y_res)
#bmp.SetResolution(x_res, y_res)
#
#g = Drawing.Graphics.FromImage(bmp)
#rect = Drawing.RectangleF(0, 0, width_inches*x_res, height_inches*y_res)
#plot.PaintOn(g, rect)
#g.Dispose()
#
#bmp.Save("test.png")

