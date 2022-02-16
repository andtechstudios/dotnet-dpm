using Ganss.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech.DPM
{

	internal class PathGatherer
	{
		public class Result
		{
			public string SourcePathFull { get; set; }
			public string SourcePath { get; set; }
			public string DestinationPath { get; set; }
		}

		public IEnumerable<Result> Results { get; private set; }

		private readonly Session session;

		public PathGatherer(Session session, string packageName)
		{
			this.session = session;

			// Read dotfile package
			var package = LoadPackage(packageName);

			// Prepare install location
			var installLocation = package.GetInstallLocation(session.ClientPlatform);
			if (string.IsNullOrEmpty(installLocation.Destination))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine($"Platform '{session.ClientPlatform}' is unsupported.");
				Console.ResetColor();
				Environment.Exit(-1);
			}
			var destinationRootsGlob = session.ClientShell.ExpandEnvironmentVariables(installLocation.Destination);
			var destinationRoots = Glob.ExpandNames(destinationRootsGlob, dirOnly: true)
				.ToList();
			
			var results = new List<Result>();
			foreach (var destinationRoot in destinationRoots)
			{
				foreach (var include in package.include)
				{
					var sourcePaths = include.ExpandGlob(package.Root);
					foreach (var relativeSourcePath in sourcePaths)
					{
						var sourcePath = Path.Combine(package.Root, relativeSourcePath);
						var destinationPath = Path.Combine(destinationRoot, include.GetDestinationPath(relativeSourcePath));

						var result = new Result()
						{
							SourcePathFull = sourcePath,
							SourcePath = relativeSourcePath,
							DestinationPath = destinationPath,
						};

						results.Add(result);
					}
				}
			}

			Results = results;
		}

		Package LoadPackage(string name)
		{
			try
			{
				var packagePath = Path.Combine(session.DotfilesRoot, name);
				return Package.Read(packagePath);
			}
			catch (IOException)
			{
				throw new IOException($"Unable to load package '{name}'");
			}
		}
	}
}
