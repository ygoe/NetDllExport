using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Unclassified;

namespace NetDllExport
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			CommandLineParser clp = new CommandLineParser();
			clp.AddKnownOption("h", "help");
			clp.AddKnownOption("v", "verbose");
			clp.AddKnownOption("o", "out", true);
			clp.AddKnownOption("c", "class", true);
			clp.AddKnownOption("d", "debug");

			bool debug = clp.IsOptionSet("d");
			bool verbose = clp.IsOptionSet("v");
			string inFile = clp.GetArgument(0);
			string outFile = clp.GetOptionValue("o");
			string clsName = clp.GetOptionValue("c");
			if (string.IsNullOrEmpty(clsName))
				clsName = "DllExport";

			if (clp.IsOptionSet("h") || string.IsNullOrEmpty(inFile))
			{
				string productVersion = "";

				object[] customAttributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					productVersion = ((AssemblyFileVersionAttribute) customAttributes[0]).Version;
				}

				Console.WriteLine("NetDllExport");
				Console.WriteLine("Exports static methods in a managed DLL as library functions that can be");
				Console.WriteLine("called from an unmanaged Windows application.");
				Console.WriteLine("Version " + productVersion);
				Console.WriteLine("Copyright by Yves Goergen, http://unclassified.software/apps/netdllexport");
				Console.WriteLine("");
				Console.WriteLine("No input file specified.");
				Console.WriteLine("");
				Console.WriteLine("Usage:");
				Console.WriteLine("  NetDllExport [options] InputFile");
				Console.WriteLine("");
				Console.WriteLine("Options:");
				Console.WriteLine("  -c, --class ClassName");
				Console.WriteLine("      Specifies the managed class whose static methods are exported.");
				Console.WriteLine("      Default: DllExport");
				Console.WriteLine("  -d, --debug");
				Console.WriteLine("      Rebuilds the assembly in debug mode and creates a PDB file.");
				Console.WriteLine("      Does not delete intermediate .il and .res files.");
				Console.WriteLine("  -h, --help");
				Console.WriteLine("      Show this help page.");
				Console.WriteLine("  -o, --out OutputFile");
				Console.WriteLine("      Specifies the output DLL file name.");
				Console.WriteLine("      Default: InputFile");
				Console.WriteLine("  -v, --verbose");
				Console.WriteLine("      Be verbose.");
				return 1;
			}

			if (string.IsNullOrEmpty(outFile))
			{
				outFile = inFile;
			}

			if (!Path.IsPathRooted(inFile))
			{
				inFile = Path.Combine(Environment.CurrentDirectory, inFile);
			}
			if (!Path.IsPathRooted(outFile))
			{
				outFile = Path.Combine(Environment.CurrentDirectory, outFile);
			}

			string ilFile = Path.Combine(
				Path.GetDirectoryName(inFile),
				Path.GetFileNameWithoutExtension(inFile) + ".il");
			string resFile = Path.Combine(
				Path.GetDirectoryName(inFile),
				Path.GetFileNameWithoutExtension(inFile) + ".res");

			if (verbose)
			{
				Console.WriteLine("Input file: " + inFile);
				Console.WriteLine("Output file: " + outFile);
				Console.WriteLine("IL file: " + ilFile);
				Console.WriteLine("RES file: " + resFile);
				Console.WriteLine("Exported class: " + clsName);
			}

			// Disassemble to IL and read that file
			if (verbose)
			{
				Console.WriteLine("Disassembling input file...");
			}
			ILHelper.DisassembleFile(inFile, ilFile);
			string ilCode = File.ReadAllText(ilFile);

			// TODO: Add more return values (void|string|...)
			string methodRegex =
				@"^\s*.method\s+public\s+(hidebysig\s+)?static\s+(void|bool|int32|int64|uint32|uint64|string|class\s+[\w.]+)\s+(marshal\s*\([^)]+\)\s+)?" +
				@"(\w+)\s*\([^)]*\)" +
				@"(\s+cil)?(\s+managed)?" +
				@"\s*\{";

			// Make class name case-insensitive
			string clsNameRegex = "";
			foreach (char c in clsName)
			{
				clsNameRegex += "[" + char.ToLower(c) + char.ToUpper(c) + "]";
			}

			// Extract the class to export
			int funcCount = 0;
			Match m = Regex.Match(ilCode, @"^(.*\n)(\.class\s([^ \t\r\n.]+\s+)*\w+\." + clsNameRegex + @"\s+.*?\{.*?)(\n.class\s.*)?$", RegexOptions.Singleline);
			if (m.Success)
			{
				string clsCode = m.Groups[2].Value;

				//string testMethodRegex =
				//    @"^\s*.method\s+public\s+(hidebysig\s+)?static\s+(void|int32|string|class\s+[\w.]+)\s+(marshal\s*\([^)]+\)\s+)?" +
				//    @"(EventMsg)\s*\([^)]*\)" +
				//    @"(\s+cil)?(\s+managed)?" +
				//    @"\s*\{";
				//if (Regex.IsMatch(clsCode, testMethodRegex, RegexOptions.Multiline | RegexOptions.Singleline))
				//{
				//}

				// Modify each .method in clsCode
				clsCode = Regex.Replace(
					clsCode,
					methodRegex,
					delegate(Match m2)
					{
						funcCount++;
						if (verbose)
						{
							Console.WriteLine("Exported function " + funcCount + ": " + m2.Groups[4].Value);
						}
						return m2.Value + Environment.NewLine +
							".vtentry " + funcCount + " : 1" + Environment.NewLine +
							".export [" + funcCount + "] as " + m2.Groups[4].Value + Environment.NewLine;
					},
					RegexOptions.Multiline | RegexOptions.Singleline);

				ilCode = m.Groups[1].Value + clsCode + m.Groups[4].Value;
			}
			else
			{
				Console.Error.WriteLine("Exported class \"" + clsName + "\" not found.");
				return 1;
			}

			if (funcCount == 0)
			{
				Console.Error.WriteLine("No functions to export.");
				return 1;
			}

			// Insert vtable code
			string vtCode = "";
			for (int i = 1; i <= funcCount; i++)
			{
				vtCode += ".vtfixup [1] int32 fromunmanaged at VT_" + i.ToString("00") + Environment.NewLine +
					".data VT_" + i.ToString("00") + " = int32(0)" + Environment.NewLine;
			}

			// Change to x86 - should better be done by the source assembly already.
			// corflags bit meanings: http://stackoverflow.com/a/13767541/143684
			ilCode = Regex.Replace(
				ilCode,
				"^.corflags 0x0000000[123] .*$",
				".corflags 0x00000002" + Environment.NewLine + vtCode,
				RegexOptions.Multiline);

			// Write new IL file and assemble it
			if (verbose)
			{
				Console.WriteLine("Assembling IL file...");
			}
			File.WriteAllText(ilFile, ilCode);
			ILHelper.AssembleFile(ilFile, outFile, debug);

			// Clean up
			if (!debug)
			{
				File.Delete(ilFile);
				File.Delete(resFile);
			}

			// Finished
			if (verbose)
			{
				Console.WriteLine("DLL functions successfully exported.");
			}
			return 0;
		}
	}
}
