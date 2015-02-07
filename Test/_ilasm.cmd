@echo off
set PATH=C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727;%PATH%
ilasm.exe DvbTestPlugin.il /output=DvbTestPlugin-exp.dll /dll
