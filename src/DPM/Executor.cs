using System;
using System.IO;

namespace Andtech.DPM
{

	internal class Executor
	{
		private readonly Session session;
		private readonly Options options;
		private readonly BaseFileTransferrer fileTransferrer;

		public Executor(Session session, Options options)
		{
			this.options = options;
			this.session = session;
			fileTransferrer = options.IsDestinationWindowsPath ? new WindowsFileTransferrer() : new FileTransferrer();
		}

		public void Execute(UniversalPath sourceFilePath, UniversalPath destinationFilePath)
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

		void CreateDirectoryIfNoExists(UniversalPath path)
		{
			string directory;
			if (session.IsInWindowsShell)
			{
				directory = Path.GetDirectoryName(path.AsWindows());
			}
			else
			{
				directory = Path.GetDirectoryName(path.AsUnix());
			}

			if (!Directory.Exists(directory))
			{
				Print($"Creating directory '{directory}'...");
				if (!options.DryRun)
				{
					Directory.CreateDirectory(directory);
				}
			}
		}

		void DeletePathIfExists(UniversalPath destination)
		{
			string path;
			if (session.IsInWindowsShell)
			{
				path = destination.AsWindows();
			}
			else
			{
				path = destination.AsUnix();
			}

			if (File.Exists(path))
			{
				Print($"Deleting file '{path}'...");
				if (!options.DryRun)
				{
					File.Delete(path);
				}
			}
			else if (Directory.Exists(path))
			{
				Print($"Creating directory '{path}'...");
				if (!options.DryRun)
				{
					Directory.Delete(path);
				}
			}
		}

		void CreateSymbolicLink(UniversalPath source, UniversalPath destination)
		{
			Print($"Creating symlink for '{source}' at '{destination}'...");
			if (!options.DryRun)
			{
				fileTransferrer.CreateSymlink(source, destination);
			}
		}

		void Copy(UniversalPath source, UniversalPath destination)
		{
			Print($"Copying '{source}' to '{destination}'...");
			if (!options.DryRun)
			{
				fileTransferrer.Copy(source, destination);
			}
		}

		void Print(object message)
		{
			string prefix = string.Empty;
			if (options.DryRun)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				prefix = $"[DRY RUN] ";
			}
			Console.WriteLine($"{prefix}{message}");
			Console.ResetColor();
		}
	}
}
