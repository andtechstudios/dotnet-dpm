using System.Runtime.InteropServices;

namespace Andtech.DPM
{

	internal class Session
	{
		public bool IsInWindowsShell => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	}
}
