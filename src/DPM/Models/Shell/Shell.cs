using System;

namespace Andtech.DPM
{

	internal abstract class Shell
	{
		private readonly VariableExpander variableExpander;

		public Shell()
		{
			variableExpander = CreateVariableExpander();
		}

		public string ExpandEnvironmentVariables(string input) => variableExpander.ExpandEnvironmentVariables(input);

		public abstract void Copy(string sourcePath, string destPath);

		public abstract void CreateSymbolicLink(string targetPath, string linkPath);

		protected abstract VariableExpander CreateVariableExpander();

		public static Shell GetCurrentShell(Platform platform)
		{
			switch (platform)
			{
				case Platform.windows:
					return new WindowsShell();
				case Platform.wsl:
					return new WSLShell();
				default:
					return new UnixShell();
			}

			throw new PlatformNotSupportedException();
		}
	}
}
