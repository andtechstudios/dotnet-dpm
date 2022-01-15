using Andtech.Common;
using System.IO;

namespace Andtech.DPM
{
	internal class WSLShell : Shell
	{

		protected override VariableExpander CreateVariableExpander() => new WSLVariableExpander();

		public override void Copy(string sourcePath, string destPath)
		{
			ShellUtility.Copy(sourcePath, destPath, false);
		}

		public override void CreateSymbolicLink(string targetPath, string linkPath)
		{
			var isDirectory = Directory.Exists(targetPath);
			// Assume link directory already exists
			var linkDirectory = Path.GetDirectoryName(linkPath);
			var linkFileName = Path.GetFileName(linkPath);
			targetPath = WSLUtility.WSLPath(targetPath, WSLPathOption.TranslateFromAWSLPathToAWindowsPath).Result;
			linkDirectory = WSLUtility.WSLPath(linkDirectory, WSLPathOption.TranslateFromAWSLPathToAWindowsPath).Result;
			linkPath = Path.Combine(linkDirectory, linkFileName)
				.Replace("/", "\\");
			WindowsUtility.Mklink(targetPath, linkPath, isDirectory);
		}
	}
}
