using CliWrap;
using System.Collections.Generic;
using System.IO;

namespace Andtech.DPM
{
	internal class WindowsUtility
	{

		public static void Mklink(string targetPath, string linkName)
		{
			var linkType = Directory.Exists(targetPath) ? "/D" : string.Empty;
			var arguments = new List<string>
			{
				"/C",
				$"mklink {linkType} {linkName} {targetPath}"
			};

			Cli.Wrap("cmd.exe")
				.WithArguments(arguments)
				.ExecuteAsync().Task.Wait();
		}
	}
}
