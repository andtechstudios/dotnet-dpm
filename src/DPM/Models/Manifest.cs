using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Andtech.DPM
{

	public class ManifestFile
	{
		public string file { get; set; }
		public string destination { get; set; }

		public string GetDestinationPath()
		{
			string fileName;
			if (!string.IsNullOrEmpty(destination))
			{
				fileName = destination;
				if (PathUtility.IsDirectory(destination))
				{
					fileName = Path.Combine(fileName, file);
				}
			}
			else
			{
				fileName = file;
			}

			return fileName;
		}
	}

	public class ManifestTarget
	{
		public string destination { get; set; }
	}

	internal class Manifest
	{
		public List<ManifestFile> include { get; set; }
		public List<string> exclude { get; set; }
		public string destination;
		public string load_script;
		public string save_script;
		public ManifestTarget windows;
		public ManifestTarget macos;
		public ManifestTarget linux;
		public string Path { get; set; }

		public ManifestTarget GetTarget(Platform platform)
		{
			switch (platform)
			{
				case Platform.windows:
				case Platform.wsl:
					return windows;
				case Platform.macos:
					return macos;
				case Platform.linux:
					return linux;
			}

			return null;
		}

		public string GetDestinationRoot(Platform platform)
		{
			string path = string.Empty;
			switch (platform)
			{
				case Platform.windows:
				case Platform.wsl:
					path = windows?.destination;
					break;
				case Platform.macos:
					path = macos?.destination;
					break;
				case Platform.linux:
					path = linux?.destination;
					break;
			}

			return string.IsNullOrEmpty(path) ? destination : path;
		}

		public static Manifest Read(string path)
		{
			path = System.IO.Path.Combine(path, "manifest.yaml");
			var deserializer = new DeserializerBuilder()
				.Build();

			var manifest = deserializer.Deserialize<Manifest>(new StreamReader(path));
			manifest.Path = System.IO.Path.GetFullPath(path);

			return manifest;
		}
	}
}
