using Andtech.Common;
using System.IO;

namespace Andtech.DPM
{
	internal class WindowsShell : Shell
	{

		protected override VariableExpander CreateVariableExpander() => new WindowsVariableExpander();

		public override void Copy(string sourcePath, string destPath)
		{
			ShellUtility.Copy(sourcePath, destPath, false);
		}

		public override void CreateSymbolicLink(string targetPath, string linkPath)
		{
			var isDirectory = Directory.Exists(targetPath);
			WindowsUtility.Mklink(targetPath, linkPath, isDirectory);
		}
	}
}
