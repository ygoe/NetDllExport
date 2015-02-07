using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

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
			string fileName;

			// Try all Windows SDKs that may contain the tool, newest first
			fileName = FindIldasm(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1A", "InstallationFolder");
			if (fileName != null) return fileName;
			fileName = FindIldasm(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.0A", "InstallationFolder");
			if (fileName != null) return fileName;

			throw new Exception("ILdasm tool could not be found.");
		}

		private static string FindIldasm(string regKey, string regValue)
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey(regKey);
			if (key != null)
			{
				string path = (string) key.GetValue(regValue);
				string fileName = Path.Combine(path, "bin", "ildasm.exe");
				if (File.Exists(fileName))
				{
					return fileName;
				}
			}
			return null;
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
