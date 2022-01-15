using Andtech.Common;
using CommandLine;
using Ganss.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	internal class InstallOperation
	{
		private readonly Session session;

		public InstallOperation()
		{
			session = new Session();
		}

		public async Task OnParse(Options options)
		{
			Log.Verbosity = options.Verbose ? Verbosity.verbose : options.Verbosity;

			// Logging
			Log.WriteLine($"Dotfiles root is: '{session.DotfilesRoot}'", Verbosity.verbose);
			Log.WriteLine($"Platform is: '{session.Platform}'", Verbosity.verbose);

			// Read dotfile package
			var package = LoadPackage(options.Name);
			var sourceRootPath = Path.GetDirectoryName(package.Path);

			var preferredDestinationPlatform = options.Platform;
			var destinationPlatform = CoalescePlatform(preferredDestinationPlatform);
			var shell = Shell.GetCurrentShell(destinationPlatform);
			Log.WriteLine($"Preferred destination platform: '{preferredDestinationPlatform}'", Verbosity.verbose);

			// Prepare install location
			var installLocation = package.GetInstallLocation(destinationPlatform);
			if (string.IsNullOrEmpty(installLocation.Destination))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine($"Platform '{destinationPlatform}' is unsupported.");
				Console.ResetColor();
				Environment.Exit(-1);
			}
			var destinationRootsGlob = shell.ExpandEnvironmentVariables(installLocation.Destination);

			// Logging
			Log.WriteLine($"Installing dotfiles as '{destinationPlatform}'...", Verbosity.normal);
			Log.WriteLine($"Source root path: '{sourceRootPath}'", Verbosity.verbose);
			Log.WriteLine($"Destination root path: '{destinationRootsGlob}'", Verbosity.verbose);

			var executor = new Executor(options, shell);

			var sourcePaths = package.GetIncludedFiles();
			var destinationRoots = Glob.ExpandNames(destinationRootsGlob);

			foreach (var destinationRoot in destinationRoots)
			{
				foreach (var sourcePath in sourcePaths)
				{
					var relativeSourcePath = Path.GetRelativePath(sourceRootPath, sourcePath);
					try
					{
						var destinationPath = Path.Combine(destinationRoot, relativeSourcePath);

						executor.Execute(sourcePath, destinationPath);
						Log.WriteLine($"Installed '{relativeSourcePath}' to '{destinationPath}'", ConsoleColor.Green, Verbosity.normal);
					}
					catch (Exception ex)
					{
						Log.Error.WriteLine($"Failed to install '{relativeSourcePath}'", ConsoleColor.Red, Verbosity.normal);
						Log.Error.WriteLine(ex, ConsoleColor.Red, Verbosity.verbose);
					}
				}
			}
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

		Platform CoalescePlatform(Platform preferredDestinationPlatform)
		{
			if (preferredDestinationPlatform == Platform.auto)
			{
				if (session.Platform == Platform.wsl)
				{
					return Platform.linux;
				}
				else
				{
					return session.Platform;
				}
			}
			else
			{
				return preferredDestinationPlatform;
			}
		}

		[Verb("install", isDefault: true, HelpText = "Install dotfile package.")]
		public class Options
		{
			[Value(0, HelpText = "Name of the package.", Required = true)]
			public string Name { get; set; }
			[Option('s', "symbolic")]
			public bool CreateSymbolicLink { get; set; }
			[Option('n', "dry-run")]
			public bool DryRun { get; set; }
			[Option("verbosity", HelpText = "Verbosity of logging")]
			public Verbosity Verbosity { get; set; }
			[Option('v', "verbose")]
			public bool Verbose { get; set; }

			[Option("auto")]
			public bool IsAuto { get; set; }
			[Option("windows")]
			public bool IsWindows { get; set; }
			[Option("wsl")]
			public bool IsWSL { get; set; }
			[Option("macos")]
			public bool IsMacOS { get; set; }
			[Option("linux")]
			public bool IsLinux { get; set; }

			public Platform Platform
			{
				get
				{
					{
						if (IsAuto)
						{
							return Platform.auto;
						}

						if (IsWindows)
						{
							return Platform.windows;
						}

						if (IsWSL)
						{
							return Platform.wsl;
						}

						if (IsMacOS)
						{
							return Platform.macos;
						}

						if (IsLinux)
						{
							return Platform.linux;
						}

						return Platform.auto;
					}
				}
			}
		}
	}
}
