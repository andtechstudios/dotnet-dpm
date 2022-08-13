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

			foreach (var include in package.include)
			{
				var sourcePaths = include.ExpandGlob(package.Root);
				foreach (var sourcePath in sourcePaths)
				{
					// Evaluate destinations
					var destinationPathGlob = session.ClientShell.ExpandEnvironmentVariables(include.destination);
					
					if (Path.IsPathRooted(destinationPathGlob))
					{
						foreach (var destinationPath in Glob.ExpandNames(destinationPathGlob))
						{
							Add(destinationPath);
						}
					}
					else
					{
						foreach (var destinationRoot in destinationRoots)
						{
							var destinationPath = Path.Combine(destinationRoot, include.GetDestinationPath(sourcePath));

							Add(destinationPath);
						}
					}

					// Local functions
					void Add(string destination)
					{
						var result = new Result()
						{
							SourcePathFull = Path.Combine(package.Root, sourcePath),
							SourcePath = sourcePath,
							DestinationPath = destination,
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
