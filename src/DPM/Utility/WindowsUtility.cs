using Andtech.Common;
using CliWrap;
using System.Collections.Generic;
using System.IO;

namespace Andtech.DPM
{
	internal class WindowsUtility
	{

		public static async void Mklink(string targetPath, string linkName, bool isDirectory = false)
		{
			var linkType = isDirectory ? "/D" : string.Empty;
			var arguments = new List<string>
			{
				"/C",
				$"mklink {linkType} {linkName} {targetPath}"
			};

			var command = Cli.Wrap("cmd.exe")
				.WithArguments(arguments);

			Log.WriteLine(command, System.ConsoleColor.Green, Verbosity.verbose);

			command
				.ExecuteAsync().Task.Wait();
		}
	}
}
