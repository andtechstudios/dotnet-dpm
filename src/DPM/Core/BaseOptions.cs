using Andtech.Common;
using CommandLine;

namespace Andtech.DPM
{

	internal class BaseOptions
	{
		[Value(0, HelpText = "Name of the package.", Required = true)]
		public string Name { get; set; }
		[Option('n', "dry-run")]
		public bool DryRun { get; set; }
		[Option("verbosity", HelpText = "Verbosity of logging")]
		public Verbosity Verbosity { get; set; }
		[Option('v', "verbose")]
		public bool Verbose { get; set; }
		[Option('s', "symbolic")]
		public bool CreateSymbolicLink { get; set; }

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
