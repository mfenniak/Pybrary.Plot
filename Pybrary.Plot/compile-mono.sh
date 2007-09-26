#!/bin/sh

gmcs \
    -target:library \
    -out:Pybrary.Plot.dll \
    -reference:System.Drawing.dll,System.Data.dll,System.Windows.Forms.dll \
    *.cs Data/*.cs
