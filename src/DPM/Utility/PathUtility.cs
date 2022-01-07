using System.Text.RegularExpressions;

namespace Andtech.DPM
{

	internal static class PathUtility
	{

		public static bool IsDirectory(string path) => Regex.IsMatch(path, @"[/\\]$");
	}
}
