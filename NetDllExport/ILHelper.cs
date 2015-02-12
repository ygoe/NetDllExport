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
