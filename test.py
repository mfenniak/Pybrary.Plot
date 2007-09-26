import clr
clr.AddReferenceToFile("Pybrary.Plot.dll")
clr.AddReference("System.Drawing")

import System
from Pybrary import Plot
from System import Drawing

plot = Plot.Plot()
plot.UseNumericXAxis()

plot.XAxis.Title = "X Axis"
plot.YAxes["Default"].Title = "Y Axis"

series = Plot.ScatterSeries()
xdata, ydata = [], []
for i in range(100):
    xdata.append(float(i))
    ydata.append(float(i**2))
series.Data.Set(System.Array[float](xdata), System.Array[System.Nullable[float]](ydata))
plot.Series["Test"] = series

bmp = Drawing.Bitmap(640, 480)
g = Drawing.Graphics.FromImage(bmp)
rect = Drawing.RectangleF(0, 0, 640, 480)
plot.PaintOn(g, rect)
g.Dispose()
bmp.Save("test.png")


