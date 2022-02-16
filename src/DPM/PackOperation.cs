using Andtech.Common;
using CommandLine;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	internal class PackOperation
	{
		[Verb("pack", HelpText = "Save the files as dotfile package.")]
		public class Options : BaseOptions { }

		private Session session;

		public async Task OnParse(Options options)
		{
			session = new Session(options);
			var gatherer = new PathGatherer(session, options.Name);
			var executor = new Executor(options, session.HostShell);

			// Logging
			Log.WriteLine($"Packing dotfiles as '{session.HostPlatform}'...", Verbosity.normal);
			Log.WriteLine($"Paths to be packaged:", Verbosity.diagnostic);
			Log.WriteLine(string.Join(Environment.NewLine, gatherer.Results.Select(x => x.DestinationPath)), Verbosity.diagnostic);
			Log.WriteLine($"Package content will be:", Verbosity.diagnostic);
			Log.WriteLine(string.Join(Environment.NewLine, gatherer.Results.Select(x => x.SourcePath)), Verbosity.diagnostic);

			foreach (var result in gatherer.Results)
			{
				try
				{
					executor.Copy(result.DestinationPath, result.SourcePathFull);
					var packagedPath = Path.GetRelativePath(session.DotfilesRoot, result.SourcePathFull);
					Log.WriteLine($"Packed '{result.DestinationPath}' as '{packagedPath}'", ConsoleColor.Green, Verbosity.normal);
				}
				catch (Exception ex)
				{
					Log.Error.WriteLine($"Failed to pack '{result.DestinationPath}'", ConsoleColor.Red, Verbosity.normal);
					Log.Error.WriteLine(ex, ConsoleColor.Red, Verbosity.verbose);
				}
			}
		}
	}
}
