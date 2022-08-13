using Andtech.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Andtech.DPM
{

	internal class Session
	{
		public Platform HostPlatform { get; private set; }
		public Platform PreferredPlatform { get; private set; }
		public Platform ClientPlatform { get; private set; }
		public Shell HostShell { get; private set; }
		public Shell ClientShell { get; private set; }
		public readonly string HomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		public string DotfilesRoot
		{
			get
			{
				var path = Environment.GetEnvironmentVariable("XDG_DOTFILES_DIR");
				if (!string.IsNullOrEmpty(path))
				{
					return path;
				}

				path = Path.Combine(HomeDirectory, ".dotfiles");
				if (Directory.Exists(path))
				{
					return path;
				}

				path = Path.Combine(HomeDirectory, "dotfiles");
				if (Directory.Exists(path))
				{
					return path;
				}

				return null;
			}
		}

		public Session(BaseOptions options)
		{
			Log.Verbosity = options.Verbose ? Verbosity.verbose : options.Verbosity;
			DryRun.IsDryRun |= options.DryRun;

			PreferredPlatform = options.Platform;
			HostPlatform = GetCurrentPlatform();
			ClientPlatform = CoalescePlatform(options.Platform);

			ClientShell = Shell.GetShell(ClientPlatform);
			HostShell = Shell.GetShell(HostPlatform);

			// Logging
			Log.WriteLine($"Dotfiles root is: '{DotfilesRoot}'", Verbosity.diagnostic);
			Log.WriteLine($"Host platform is: '{HostPlatform}'", Verbosity.diagnostic);
			Log.WriteLine($"Preferred client platform: '{PreferredPlatform}'", Verbosity.diagnostic);
		}

		public static Platform GetCurrentPlatform()
		{
			if (WSLUtility.IsWSL())
			{
				return Platform.wsl;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return Platform.windows;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return Platform.macos;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				return Platform.linux;
			}

			return Platform.unknown;
		}

		Platform CoalescePlatform(Platform preferredDestinationPlatform)
		{
			if (preferredDestinationPlatform == Platform.auto)
			{
				if (HostPlatform == Platform.wsl)
				{
					return Platform.linux;
				}
				else
				{
					return HostPlatform;
				}
			}
			else
			{
				return preferredDestinationPlatform;
			}
		}
	}
}
