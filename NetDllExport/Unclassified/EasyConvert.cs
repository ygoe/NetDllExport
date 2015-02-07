using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Unclassified
{
	public class EasyConvert
	{
		public static string[] SplitQuoted(string str)
		{
			string[] rawChunks = str.Split(' ');
			List<string> chunks = new List<string>();
			bool inStr = false;
			foreach (string chunk in rawChunks)
			{
				if (!inStr && chunk.StartsWith("\"") && chunk.EndsWith("\""))
				{
					chunks.Add(chunk.Substring(1, chunk.Length - 2));
				}
				else if (!inStr && chunk.StartsWith("\""))
				{
					inStr = true;
					chunks.Add(chunk.Substring(1));
				}
				else if (inStr && chunk.EndsWith("\""))
				{
					inStr = false;
					chunks[chunks.Count - 1] += " " + chunk.Substring(0, chunk.Length - 1);
				}
				else if (!inStr)
				{
					chunks.Add(chunk);
				}
				else if (inStr)
				{
					chunks[chunks.Count - 1] += " " + chunk;
				}
			}
			return chunks.ToArray();
		}

		public static Dictionary<string, int> CountDuplicates(string[] parts)
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			foreach (string part in parts)
			{
				if (dict.ContainsKey(part))
					dict[part]++;
				else
					dict.Add(part, 1);
			}
			return dict;
		}

		public static string Repeat(string s, int count, string separator)
		{
			StringBuilder sb = new StringBuilder();
			while (count-- > 0)
			{
				if (sb.Length > 0) sb.Append(separator);
				sb.Append(s);
			}
			return sb.ToString();
		}

		public static string JoinQuoted(string[] parts)
		{
			string all = "";
			foreach (string part in parts)
			{
				all += (all.Length > 0 ? " " : "") +
					(part.Contains(" ") ? "\"" : "") +
					part +
					(part.Contains(" ") ? "\"" : "");
			}
			return all;
		}

		public static string LimitLength(string s, int length)
		{
			if (s.Length <= length) return s;
			return s.Substring(0, length);
		}

		public static int ToInt(string s)
		{
			try
			{
				s = s.Trim();
				if (s == "") return 0;
				return int.Parse(s);
			}
			catch (FormatException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Wandelt eine Zeit im DateTime-Format in einen UNIX-TimeStamp um.
		/// </summary>
		/// <param name="TimeStamp">Der umzuwandelnde Timestamp</param>
		/// <returns></returns>
		/// <seealso cref="http://beta.unclassified.de/code/dotnet/unixdatetime/"/>
		public static int DateTime2TimeStamp(DateTime Time)
		{
			DateTime x = new DateTime(1970, 1, 1, 0, 0, 0);
			return (int) ((Time.ToUniversalTime().Ticks - x.Ticks) / 10000000);
		}

		/// <summary>
		/// Wandelt einen UNIX-TimeStamp in das DateTime-Format um.
		/// </summary>
		/// <param name="TimeStamp">Der umzuwandelnde Timestamp</param>
		/// <returns></returns>
		public static DateTime TimeStamp2DateTime(int TimeStamp)
		{
			DateTime x = new DateTime(1970, 1, 1, 0, 0, 0);
			return x.AddSeconds(TimeStamp).ToLocalTime();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="seconds"></param>
		/// <param name="level">0: seconds, 1: minutes, 2: hours, 3: days, 4: weeks</param>
		/// <returns></returns>
		public static string SecondsToString(int seconds, int level)
		{
			string s = "";
			if (level >= 4)
			{
				s += (s != "" ? " " : "") + seconds / (86400 * 7) + "wo";
				seconds %= 86400 * 7;
			}
			if (level >= 3)
			{
				s += (s != "" ? " " : "") + seconds / 86400 + "d";
				seconds %= 86400;
			}
			if (level >= 2)
			{
				s += (s != "" ? " " : "") + seconds / 3600 + "h";
				seconds %= 3600;
			}
			if (level >= 1)
			{
				int min = seconds / 60;
				s += (s != "" ? " " : "") + min.ToString("00") + ":";
				seconds %= 60;
			}
			if (level >= 0)
			{
				s += seconds.ToString("00");
			}
			return s;
		}

		public static string FormatBytes(long bytes)
		{
			string[] names = new string[] { "B", "kB", "MB", "GB", "TB" };

			double d = bytes;
			int level = 0;
			while (d >= 900)
			{
				d /= 1024.0;
				level++;
			}

			if (d >= 100)
				return d.ToString("0") + " " + names[level];
			if (d >= 10)
				return d.ToString("0.0") + " " + names[level];
			return d.ToString("0.00") + " " + names[level];
		}

		public static string UCWords(string s)
		{
			StringBuilder sb = new StringBuilder();

			bool space = true;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == ' ' || s[i] == '\t' || s[i] == '\r' || s[i] == '\n' || s[i] == '.' || s[i] == ',' || s[i] == '-')
				{
					space = true;
					sb.Append(s[i]);
				}
				else if (space)
				{
					space = false;
					sb.Append(char.ToUpper(s[i], CultureInfo.CurrentCulture));
				}
				else
				{
					sb.Append(s[i]);
				}
			}
			return sb.ToString();
		}

		public static string UCLines(string s)
		{
			StringBuilder sb = new StringBuilder();

			bool space = true;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '\r' || s[i] == '\n')
				{
					space = true;
					sb.Append(s[i]);
				}
				else if (space)
				{
					space = false;
					sb.Append(char.ToUpper(s[i], CultureInfo.CurrentCulture));
				}
				else
				{
					sb.Append(s[i]);
				}
			}
			return sb.ToString();
		}

		public static string TrimEndLines(string s)
		{
			string[] lines = s.Replace("\r", "").Split('\n');
			for (int i = 0; i < lines.Length; i++)
			{
				lines[i] = lines[i].TrimEnd();
			}
			return string.Join(Environment.NewLine, lines);
		}

		public static int HexToDec(char c)
		{
			if (c >= '0' && c <= '9') return c - '0';
			c = Char.ToUpper(c);
			if (c >= 'A' && c <= 'F') return 10 + c - 'A';
			throw new FormatException("Not a valid hexadecimal character: " + c);
		}

		public static string DecodeBase64(string base64)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
		}

		public static string EncodeBase64(string text)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
		}

		public static string DecodeUrl(string s)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '%')
				{
					// TODO: This is not Unicode-capable (would need to support UTF-8 or something)
					try
					{
						sb.Append((char) (HexToDec(s[i + 1]) * 16 + HexToDec(s[i + 2])));
						i += 2;
					}
					catch (FormatException)
					{
						// Not a valid hex value, keep it undecoded
						sb.Append(s[i]);
					}
				}
				else
				{
					sb.Append(s[i]);
				}
			}
			return sb.ToString();
		}

		public static string EncodeUrl(string s)
		{
			return EncodeUrl(s, "\t\r\n #&+/=?");
		}

		public static string EncodeUrl(string s, string specials)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '%' || specials.Contains(s[i].ToString()))
				{
					sb.Append("%");
					sb.Append(((int) s[i]).ToString("X2"));
				}
				else
				{
					sb.Append(s[i]);
				}
			}
			return sb.ToString();
		}

		public static string EncodeWebUrl(string s, Encoding encoding)
		{
			const string specials = "\"%&+/:<=>?\\_|~";
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] < 0x20 || s[i] > 0x7E || specials.Contains(s[i].ToString()))
				{
					byte[] bytes = encoding.GetBytes(s.Substring(i, 1));
					foreach (byte b in bytes)
					{
						sb.Append("%" + b.ToString("X2"));
					}
				}
				else
				{
					sb.Append(s[i]);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Safe replacement for String.Substring that gracefully returns an empty string instead of throwing an ArgumentOutOfRangeException.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="startIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string SafeSubstring(string input, int startIndex, int length)
		{
			if (input.Length < startIndex) return "";
			if (input.Length < startIndex + length) return input.Substring(startIndex);
			return input.Substring(startIndex, length);
		}

		public static string ToHtml(string str)
		{
			return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&#39;");
		}

		/// <summary>
		/// Ensures that an HTML-encoded string is kept at the same character length as the input string.
		/// Adds &amp;nbsp; entities at the beginning and the end if there is a ' ' character that would be trimmed when displayed.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string KeepHtmlLength(string str)
		{
			str = str.Replace("  ", "&nbsp; ").Replace("  ", "&nbsp; ");
			if (str.EndsWith(" "))
				str = str.Substring(0, str.Length - 1) + "&nbsp;";
			if (str.StartsWith(" "))
				str = "&nbsp;" + str.Substring(1);
			return str;
		}

		/// <summary>
		/// Converts a string for safe use in file names. Invalid characters are removed or replaced.
		/// </summary>
		/// <param name="filename">Input string</param>
		/// <param name="replacement">Replacement string for each invalid character, may be empty</param>
		/// <param name="mergeMultipleReplacements">Merge multiple subsequent occurances of the replacement string into one</param>
		/// <returns>Safe filename. Warning: This may result in an empty string!</returns>
		public static string ToSafeFilename(string filename, string replacement, bool mergeMultipleReplacements)
		{
			if (replacement == null)
			{
				replacement = string.Empty;
			}
			filename = filename.
				Replace("\"", replacement).
				Replace("*", replacement).
				Replace("/", replacement).
				Replace(":", replacement).
				Replace("<", replacement).
				Replace(">", replacement).
				Replace("?", replacement).
				Replace("|", replacement).
				Replace("\\", replacement);
			if (mergeMultipleReplacements && replacement.Length > 0)
			{
				filename = filename.Replace(replacement + replacement, replacement).Replace(replacement + replacement, replacement);
			}
			return filename;
		}
	}
}
