@echo off
set PATH=C:\Programme\Microsoft Visual Studio 8\SDK\v2.0\Bin;%PATH%
ildasm.exe DvbTestPlugin.dll /out=DvbTestPlugin.il /utf8
