using CommandLine;
using System;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	public enum Platform
	{
		linux,
		macos,
		windows,
		wsl,
	}

	class Options
	{
		[Value(0, HelpText = "Location of the manifest", Required = false, Default = ".")]
		public string ManifestPath { get; set; }
		[Value(1, HelpText = "Platform to install as. Can be: windows, wsl, linux, macos", Required = true)]
		public Platform Platform { get; set; }
		[Option('s', "symbolic")]
		public bool CreateSymbolicLink { get; set; }
		[Option('n', "dry-run")]
		public bool DryRun { get; set; }

		public bool IsDestinationWindowsPath => Platform == Platform.windows || Platform == Platform.wsl;
	}

	class Program
	{

		static async Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<Options>(args);
			await result.WithParsedAsync(OnParse);
		}

		public static async Task OnParse(Options options)
		{
			try
			{
				var runner = new Runner(options);
				await runner.RunAsync();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				Environment.Exit(-1);
			}
		}
	}
}
