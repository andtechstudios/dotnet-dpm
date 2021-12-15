using System.IO;

namespace Andtech.DPM
{

	public enum PathFormat
	{
		Unix,
		Windows,
	}

	internal struct UniversalPath
	{
		public bool IsWindowsPath { get; set; }
		public string Value
		{
			get
			{
				string path = this.path;
				if (IsWindowsPath)
				{
					path = path.Replace("/", "\\");
				}
				else
				{
					path = path.Replace('\\', '/');
				}

				return path;
			}
		}
		private readonly string path;

		private UniversalPath(string path)
		{
			IsWindowsPath = true;
			this.path = path;
		}

		public static implicit operator string(UniversalPath universalPath)
		{
			return universalPath.Value;
		}

		public static UniversalPath New(string path, bool isWindowsPath)
		{
			return new UniversalPath(path)
			{
				IsWindowsPath = isWindowsPath,
			};
		}

		public UniversalPath AsWindows()
		{
			if (IsWindowsPath)
			{
				return this;
			}

			var path = WSLUtility.WSLPath(Value, WSLPathOption.TranslateFromAWSLPathToAWindowsPath).Result;
			return new UniversalPath(path)
			{
				IsWindowsPath = true
			};
		}

		public UniversalPath AsUnix()
		{
			if (!IsWindowsPath)
			{
				return this;
			}

			var path = WSLUtility.WSLPath(Value, WSLPathOption.TranslateFromAWindowsPathToAWSLPath).Result;
			return new UniversalPath(path)
			{
				IsWindowsPath = false
			};
		}

		public string GetRelativePath(string relativeTo) => Path.GetRelativePath(relativeTo, Value);

		public static UniversalPath Combine(UniversalPath pathA, string pathB) => New(Path.Combine(pathA, pathB), pathA.IsWindowsPath);

		public override string ToString() => Value;
	}
}
