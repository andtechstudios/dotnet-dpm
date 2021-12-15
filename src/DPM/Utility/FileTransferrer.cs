using CliWrap;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	internal abstract class BaseFileTransferrer
	{

		public abstract void Copy(UniversalPath from, UniversalPath to);

		public abstract void CreateSymlink(UniversalPath from, UniversalPath to);
	}

	internal class FileTransferrer : BaseFileTransferrer
	{

		public override void Copy(UniversalPath from, UniversalPath to)
		{
			File.Copy(from, to, true);
		}

		public override void CreateSymlink(UniversalPath from, UniversalPath to)
		{
			LnAsync(from, to).Wait();
		}

		private async Task LnAsync(string from, string to)
		{
			var arguments = new List<string>
			{
				"-s",
				from,
				to
			};

			await Cli.Wrap("ln")
				.WithArguments(arguments)
				.ExecuteAsync();
		}
	}

	internal class WindowsFileTransferrer : BaseFileTransferrer
	{

		public override void Copy(UniversalPath from, UniversalPath to)
		{
			File.Copy(from, to, false);
		}

		public override void CreateSymlink(UniversalPath from, UniversalPath to)
		{
			MklinkAsync(from.AsWindows(), to.AsWindows()).Wait();
		}

		private async Task MklinkAsync(string from, string to)
		{
			var arguments = new List<string>
			{
				"/C",
				$"mklink {to} {from}"
			};

			await Cli.Wrap("cmd.exe")
				.WithArguments(arguments)
				.ExecuteAsync();
		}
	}
}
