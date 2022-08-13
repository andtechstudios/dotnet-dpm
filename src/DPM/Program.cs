using Andtech.Common;
using CommandLine;
using System.Threading.Tasks;

namespace Andtech.DPM
{

	class Program
	{

		static async Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<InstallOperation.Options, PackOperation.Options>(args);
			await result.WithParsedAsync<InstallOperation.Options>(new InstallOperation().OnParse);
			await result.WithParsedAsync<PackOperation.Options>(new PackOperation().OnParse);
		}
	}
}
