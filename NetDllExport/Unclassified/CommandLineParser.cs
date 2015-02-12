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
using System.Collections.Generic;

namespace Unclassified
{
	public class CommandLineParser
	{
		private List<Option> knownOptions = new List<Option>();
		private List<Option> parsedOptions;
		private List<string> parsedArguments;

		public CommandLineParser()
		{
		}

		public List<Option> KnownOptions
		{
			get { return knownOptions; }
		}

		public void Parse()
		{
			parsedOptions = new List<Option>();
			parsedArguments = new List<string>();
			string[] parts = EasyConvert.SplitQuoted(Environment.CommandLine);
			for (int i = 1; i < parts.Length; i++)
			{
				string part = parts[i];
				if (part == "") continue;
				if (part == "--")
				{
					// Eat up all remaining arguments
					for (i++; i < parts.Length; i++)
					{
						parsedArguments.Add(parts[i]);
					}
					return;
				}
				if (part.StartsWith("--"))
				{
					Option opt = GetOptionForLongName(part.Substring(2));
					if (opt != null)
					{
						if (opt.HasParameter && i + 1 < parts.Length)
						{
							opt.Value = parts[++i];
						}
						parsedOptions.Add(opt);
					}
					else
					{
						parsedArguments.Add(part);
					}
				}
				else if (part.StartsWith("-") && part.Length > 1)
				{
					for (int pos = 1; pos < part.Length; pos++)
					{
						Option opt = GetOptionForShortName(part[pos].ToString());
						if (opt != null)
						{
							if (opt.HasParameter && i + 1 < parts.Length)
							{
								opt.Value = parts[++i];
							}
							parsedOptions.Add(opt);
						}
					}
				}
				else
				{
					parsedArguments.Add(part);
				}
			}
		}

		public bool IsOptionSet(string name)
		{
			if (parsedArguments == null) Parse();

			foreach (Option o in parsedOptions)
			{
				if (name.Length == 1 && o.ShortName == name ||
					name.Length > 1 && o.LongName == name)
				{
					return true;
				}
			}
			return false;
		}

		public string GetOptionValue(string name)
		{
			if (parsedArguments == null) Parse();

			foreach (Option o in parsedOptions)
			{
				if (name.Length == 1 && o.ShortName == name ||
					name.Length > 1 && o.LongName == name)
				{
					return o.Value;
				}
			}
			return "";
		}

		public string GetArgument(int index)
		{
			if (parsedArguments == null) Parse();

			if (index >= 0 && index < parsedArguments.Count)
			{
				return parsedArguments[index];
			}
			return "";
		}

		public Option GetOptionForShortName(string shortName)
		{
			foreach (Option o in knownOptions)
			{
				if (o.ShortName == shortName) return o;
			}
			return null;
		}

		public Option GetOptionForLongName(string longName)
		{
			foreach (Option o in knownOptions)
			{
				if (o.LongName == longName) return o;
			}
			return null;
		}

		public void AddKnownOption(string shortName)
		{
			knownOptions.Add(new Option(shortName));
		}

		public void AddKnownOption(string shortName, bool hasParameter)
		{
			knownOptions.Add(new Option(shortName, hasParameter));
		}

		public void AddKnownOption(string shortName, string longName)
		{
			knownOptions.Add(new Option(shortName, longName));
		}

		public void AddKnownOption(string shortName, string longName, bool hasParameter)
		{
			knownOptions.Add(new Option(shortName, longName, hasParameter));
		}

		public class Option
		{
			public string ShortName = "";
			public string LongName = "";
			public bool HasParameter = false;
			public string Value = "";

			public Option(string shortName)
				: this(shortName, "", false)
			{ }

			public Option(string shortName, bool hasParameter)
				: this(shortName, "", hasParameter)
			{ }

			public Option(string shortName, string longName)
				: this(shortName, longName, false)
			{ }

			public Option(string shortName, string longName, bool hasParameter)
			{
				this.ShortName = shortName;
				this.LongName = longName;
				this.HasParameter = hasParameter;
			}
		}
	}
}
