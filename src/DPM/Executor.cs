using Andtech.Common;
using System;
using System.IO;

namespace Andtech.DPM
{

	internal class Executor
	{
		private InstallOperation.Options options;
		private Shell shell;

		public Executor(InstallOperation.Options options, Shell shell)
		{
			this.options = options;
			this.shell = shell;
		}

		public void Execute(string sourceFilePath, string destinationFilePath)
		{
			if (options.CreateSymbolicLink)
			{
				DeletePathIfExists(destinationFilePath);
				CreateDirectoryIfNoExists(destinationFilePath);
				CreateSymbolicLink(sourceFilePath, destinationFilePath);
			}
			else
			{
				DeletePathIfExists(destinationFilePath);
				CreateDirectoryIfNoExists(destinationFilePath);
				Copy(sourceFilePath, destinationFilePath);
			}
		}

		void CreateDirectoryIfNoExists(string path)
		{
			string parent = Path.GetDirectoryName(path);

			if (!Directory.Exists(parent))
			{
				Log.WriteLine($"Creating directory '{parent}'...", ConsoleColor.Yellow, Verbosity.silly);
				if (!options.DryRun)
				{
					Directory.CreateDirectory(parent);
				}
			}
		}

		void DeletePathIfExists(string destination)
		{
			if (File.Exists(destination))
			{
				Log.WriteLine($"Deleting file '{destination}'...", ConsoleColor.Yellow, Verbosity.silly);
				if (!options.DryRun)
				{
					File.Delete(destination);
				}
			}
			else if (Directory.Exists(destination))
			{
				Log.WriteLine($"Deleting directory '{destination}'...", ConsoleColor.Yellow, Verbosity.silly);
				if (!options.DryRun)
				{
					Directory.Delete(destination, true);
				}
			}
		}

		void CreateSymbolicLink(string source, string destination)
		{
			source = Path.GetFullPath(source);
			destination = Path.GetFullPath(destination);
			Log.WriteLine($"Creating symlink for '{source}' at '{destination}'...", ConsoleColor.Yellow, Verbosity.silly);
			if (!options.DryRun)
			{
				shell.CreateSymbolicLink(source, destination);
			}
		}

		void Copy(string source, string destination)
		{
			Log.WriteLine($"Copying '{source}' to '{destination}'...", ConsoleColor.Yellow, Verbosity.silly);
			if (!options.DryRun)
			{
				shell.Copy(source, destination);
			}
		}
	}
}
