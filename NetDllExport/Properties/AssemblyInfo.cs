// Copyright (c) 2015, Yves Goergen, http://unclassified.de
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyProduct("NetDllExport")]
[assembly: AssemblyTitle("NetDllExport")]
[assembly: AssemblyDescription("Exports static methods in a managed DLL as library functions that can be called from an unmanaged Windows application.")]
[assembly: AssemblyCopyright("© 2010–2015 Yves Goergen")]
[assembly: AssemblyCompany("unclassified software development")]

// Assembly identity version. Must be a dotted-numeric version.
[assembly: AssemblyVersion("1.3")]

// Repeat for Win32 file version resource because the assembly version is expanded to 4 parts.
[assembly: AssemblyFileVersion("1.3")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]

// Version history:
//
// 1.3 (2015-02-08)
// * Updated code, .NET 4.0, VS 2010
// * Support for newer .NET SDKs
//
// 1.2.1 (2010-04-10)
//
// 1.1 (2010-04-06)
// * Added support for .NET 3.5 SDK path in the registry.
//
// 1.0 (2010-04-05)
// * Initial release.
