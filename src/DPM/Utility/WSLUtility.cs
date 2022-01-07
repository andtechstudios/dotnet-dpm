using CliWrap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	public enum WSLPathOption
	{
		/// <summary>
		/// Same as '-u'
		/// </summary>
		TranslateFromAWindowsPathToAWSLPath,
		/// <summary>
		/// Same as '-w'
		/// </summary>
		TranslateFromAWSLPathToAWindowsPath,
		/// <summary>
		/// Same as '-m'
		/// </summary>
		TranslateFromAWSLPathToAWindowsPathWithForwardSlashInsteadOfBackSlash,
		/// <summary>
		/// Same as '-a'
		/// </summary>
		ForceResultToAbsolutePathMode,
	}

	internal static class WSLUtility
	{

		public static async Task<string> WSLVar(string path)
		{
			try
			{
				var stdOutBuffer = new StringBuilder();
				await Cli.Wrap("wslvar")
					.WithArguments(path)
					.WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
					.ExecuteAsync(cancellationToken: default);

				return stdOutBuffer.ToString().Trim();
			}
			catch { }

			return path;
		}

		public static async Task<string> WSLPath(string path, WSLPathOption option = WSLPathOption.TranslateFromAWindowsPathToAWSLPath)
		{
			string flag;
			switch (option)
			{
				case WSLPathOption.TranslateFromAWSLPathToAWindowsPath:
					flag = "-w";
					break;
				case WSLPathOption.TranslateFromAWSLPathToAWindowsPathWithForwardSlashInsteadOfBackSlash:
					flag = "-m";
					break;
				case WSLPathOption.ForceResultToAbsolutePathMode:
					flag = "-a";
					break;
				default:
					flag = "-u";
					break;
			}

			var stdOutBuffer = new StringBuilder();
			var arguments = new List<string>()
			{
				flag,
				path,
			};

			try
			{
				await Cli.Wrap("wslpath")
					.WithArguments(arguments)
					.WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
					.ExecuteAsync(cancellationToken: default);

				return stdOutBuffer.ToString().Trim();
			}
			catch { }

			return path;
		}

		public static bool IsWSL()
		{
			string value;
			value = Environment.GetEnvironmentVariable("IS_WSL");
			if (!string.IsNullOrEmpty(value))
			{
				return true;
			}

			value = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME");
			if (!string.IsNullOrEmpty(value))
			{
				return true;
			}

			return false;
		}
	}
}
