using CliWrap;
using System.Collections.Generic;

namespace Andtech.DPM
{
	internal class WindowsUtility
	{

		public static void Mklink(string targetPath, string linkName)
		{
			var arguments = new List<string>
			{
				"/C",
				$"mklink {linkName} {targetPath}"
			};

			Cli.Wrap("cmd.exe")
				.WithArguments(arguments)
				.ExecuteAsync().Task.Wait();
		}
	}
}
