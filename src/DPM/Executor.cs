using Andtech.Common;
using System;
using System.IO;

namespace Andtech.DPM
{

	internal class Executor
	{
		private BaseOptions options;
		private Shell shell;

		public Executor(BaseOptions options, Shell shell)
		{
			this.options = options;
			this.shell = shell;
		}

		public void Copy(string sourceFilePath, string destinationFilePath, bool symbolicLink = false)
		{
			if (symbolicLink)
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
				DryRun.TryExecute(() => Directory.CreateDirectory(parent),
					$"Creating directory '{parent}'...",
					ConsoleColor.Yellow,
					Verbosity.silly);
			}
		}

		void DeletePathIfExists(string destination)
		{
			if (File.Exists(destination))
			{
				DryRun.TryExecute(() => File.Delete(destination),
					$"Deleting file '{destination}'...",
					ConsoleColor.Yellow,
					Verbosity.silly);
			}
			else if (Directory.Exists(destination))
			{
				DryRun.TryExecute(() => Directory.Delete(destination, true),
					$"Deleting directory '{destination}'...",
					ConsoleColor.Yellow,
					Verbosity.silly);
			}
		}

		void CreateSymbolicLink(string source, string destination)
		{
			source = Path.GetFullPath(source);
			destination = Path.GetFullPath(destination);

			DryRun.TryExecute(() => shell.CreateSymbolicLink(source, destination),
				$"Creating symlink for '{source}' at '{destination}'...",
				ConsoleColor.Yellow,
				Verbosity.silly);
		}

		void Copy(string source, string destination)
		{
			DryRun.TryExecute(() => shell.Copy(source, destination),
				$"Copying '{source}' to '{destination}'...",
				ConsoleColor.Yellow,
				Verbosity.silly);
		}
	}
}
