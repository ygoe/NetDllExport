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
