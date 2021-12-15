using System;
using System.IO;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	internal class Runner
	{
		private readonly Options options;

		public Runner(Options options)
		{
			this.options = options;
		}

		public async Task RunAsync()
		{
			var session = new Session();
			var isDestinationWindowsPath = options.IsDestinationWindowsPath;
			var destinationPlatform = options.Platform;

			Manifest manifest = null;
			try
			{
				manifest = Manifest.Read(options.ManifestPath);
			}
			catch (IOException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine($"Dotfile manifest not found at path '{options.ManifestPath}'");
				Console.ResetColor();
				Environment.Exit(-1);
			}

			// Prepare source target
			var sourceRootPath = UniversalPath.New(Path.GetDirectoryName(manifest.Path), session.IsInWindowsShell);

			// Prepare destination target
			var variableExpander = GetVariableExpander(destinationPlatform);
			var destinationRootRaw = manifest.GetDestinationRoot(options.Platform);
			destinationRootRaw = variableExpander.ExpandEnvironmentVariables(destinationRootRaw);
			var destinationRootPath = UniversalPath.New(destinationRootRaw, isDestinationWindowsPath);

			// Preprocess paths
			BasePathProcessor sourcePathProcessor = isDestinationWindowsPath ? new WSLPathProcessor(options) : new PathProcessor(options);
			BasePathProcessor destinationPathProcessor = isDestinationWindowsPath ? new WSLPathProcessor(options) : new PathProcessor(options);
			sourceRootPath = sourcePathProcessor.ProcessSource(sourceRootPath);
			destinationRootPath = destinationPathProcessor.ProcessDestination(destinationRootPath);

			var executor = new Executor(session, options);
			foreach (var file in manifest.include)
			{
				var sourceFilePath = UniversalPath.Combine(sourceRootPath, file.file);
				var destinationFilePath = UniversalPath.Combine(destinationRootPath, file.GetDestinationPath());

				try
				{
					executor.Execute(sourceFilePath, destinationFilePath);
				}
				catch
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Error.WriteLine($"Failed to load '{sourceFilePath.GetRelativePath(Environment.CurrentDirectory)}'");
					Console.ResetColor();
				}
			}

			VariableExpander GetVariableExpander(Platform platform)
			{
				switch (platform)
				{
					case Platform.windows:
						return new WindowsVariableExpander();
					case Platform.wsl:
						return new WSLVariableExpander();
					default:
						return new UnixVariableExpander();
				}
			}
		}
	}
}
