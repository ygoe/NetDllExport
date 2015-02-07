using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;

namespace NetDllExport
{
	public static class ILHelper
	{
		private static string GetIlasmPath()
		{
			return Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "ilasm.exe");
		}

		private static string GetIldasmPath()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\.NETFramework\v2.0");
			if (key != null)
			{
				return Path.Combine((string) key.GetValue("InstallationFolder") + "bin", "ildasm.exe");
			}
			else
			{
				key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows");
				if (key != null)
				{
					return Path.Combine((string) key.GetValue("CurrentInstallFolder") + "bin", "ildasm.exe");
				}
				else
				{
					throw new Exception("No .NET SDK path found in the Windows registry.");
				}
			}
		}

		public static void DisassembleFile(string inFile, string ilFile)
		{
			if (!File.Exists(inFile))
			{
				throw new FileNotFoundException("Input file \"" + inFile + "\" not found.");
			}

			File.Delete(ilFile);
			
			Process proc = new Process();
			proc.StartInfo.FileName = "\"" + GetIldasmPath() + "\"";
			proc.StartInfo.Arguments = "\"" + inFile + "\" /out=\"" + ilFile + "\" /nobar";
			proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			proc.Start();
			proc.WaitForExit();

			if (!File.Exists(ilFile))
			{
				throw new Exception("File \"" + inFile + "\" could not be disassembled.");
			}
		}

		public static string AssembleFile(string ilFile, string outFile, bool debug)
		{
			if (!File.Exists(ilFile))
			{
				throw new FileNotFoundException("IL file \"" + ilFile + "\" not found.");
			}

			File.Delete(outFile);

			Process proc = new Process();
			proc.StartInfo.FileName = "\"" + GetIlasmPath() + "\"";
			proc.StartInfo.Arguments = "\"" + ilFile + "\" /out=\"" + outFile + "\" /dll" + (debug ? " /debug" : "");
			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.UseShellExecute = false;
			proc.Start();
			string stdout = proc.StandardOutput.ReadToEnd();
			proc.WaitForExit();

			if (!File.Exists(outFile))
			{
				throw new Exception("File \"" + ilFile + "\" could not be assembled.");
			}

			return stdout;
		}
	}
}
