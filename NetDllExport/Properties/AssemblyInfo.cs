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
