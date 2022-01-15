using CommandLine;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	class Program
	{

		static async Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<InstallOperation.Options, object>(args);
			await result.WithParsedAsync<InstallOperation.Options>(new InstallOperation().OnParse);
		}
	}
}
