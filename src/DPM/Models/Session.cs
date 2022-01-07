using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Andtech.DPM
{

	internal class Session
	{
		public Platform Platform { get; private set; }
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

		public Session()
		{
			Platform = GetCurrentPlatform();
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
	}
}
