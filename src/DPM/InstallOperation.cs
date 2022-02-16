using Andtech.Common;
using CommandLine;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	internal class InstallOperation
	{
		[Verb("install", isDefault: true, HelpText = "Install dotfile package.")]
		public class Options : BaseOptions { }

		private Session session;

		public async Task OnParse(Options options)
		{
			session = new Session(options);
			var gatherer = new PathGatherer(session, options.Name);
			var executor = new Executor(options, session.ClientShell);

			// Logging
			Log.WriteLine($"Installing dotfiles as '{session.ClientPlatform}'...", Verbosity.normal);
			Log.WriteLine($"Packaged content is:", Verbosity.diagnostic);
			Log.WriteLine(string.Join(Environment.NewLine, gatherer.Results.Select(x => x.SourcePath)), Verbosity.diagnostic);
			Log.WriteLine($"Destination paths are:", Verbosity.diagnostic);
			Log.WriteLine(string.Join(Environment.NewLine, gatherer.Results.Select(x => x.DestinationPath)), Verbosity.diagnostic);

			foreach (var result in gatherer.Results)
			{
				try
				{
					executor.Copy(result.SourcePathFull, result.DestinationPath, options.CreateSymbolicLink);
					Log.WriteLine($"Installed '{result.SourcePath}' to '{result.DestinationPath}'", ConsoleColor.Green, Verbosity.normal);
				}
				catch (Exception ex)
				{
					Log.Error.WriteLine($"Failed to install '{result.SourcePath}'", ConsoleColor.Red, Verbosity.normal);
					Log.Error.WriteLine(ex, ConsoleColor.Red, Verbosity.verbose);
				}
			}
		}
	}
}
