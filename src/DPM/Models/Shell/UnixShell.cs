using Andtech.Common;

namespace Andtech.DPM
{
	internal class UnixShell : Shell
	{

		protected override VariableExpander CreateVariableExpander() => new UnixVariableExpander();

		public override void Copy(string sourcePath, string destPath)
		{
			ShellUtility.Copy(sourcePath, destPath, false);
		}

		public override void CreateSymbolicLink(string targetPath, string linkPath)
		{
			UnixUtility.Ln(targetPath, linkPath);
		}
	}
}
