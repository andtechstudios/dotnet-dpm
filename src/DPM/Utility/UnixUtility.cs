using CliWrap;
using System.Collections.Generic;

namespace Andtech.DPM
{
	internal class UnixUtility
	{

		public static void Ln(string targetPath, string name)
		{
			var arguments = new List<string>
			{
				"-s",
				targetPath,
				name
			};

			Cli.Wrap("ln")
				.WithArguments(arguments)
				.ExecuteAsync().Task.Wait();
		}
	}
}
