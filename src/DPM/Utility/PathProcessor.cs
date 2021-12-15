namespace Andtech.DPM
{

	internal abstract class BasePathProcessor
	{
		protected readonly Options options;

		public BasePathProcessor(Options options)
		{
			this.options = options;
		}

		public abstract UniversalPath ProcessSource(UniversalPath path);

		public abstract UniversalPath ProcessDestination(UniversalPath path);
	}

	internal class PathProcessor : BasePathProcessor
	{

		public PathProcessor(Options options) : base(options) { }

		public override UniversalPath ProcessSource(UniversalPath path)
		{
			return path;
		}

		public override UniversalPath ProcessDestination(UniversalPath path)
		{
			return path;
		}
	}

	internal class WSLPathProcessor : BasePathProcessor
	{

		public WSLPathProcessor(Options options) : base(options) { }

		public override UniversalPath ProcessSource(UniversalPath path)
		{
			if (options.CreateSymbolicLink)
			{
				if (!path.IsWindowsPath)
				{
					var newPath = WSLUtility.WSLPath(path, WSLPathOption.TranslateFromAWSLPathToAWindowsPath).Result;
					return UniversalPath.New(newPath, true);
				}
			}

			return path;
		}

		public override UniversalPath ProcessDestination(UniversalPath path)
		{
			if (!options.CreateSymbolicLink)
			{
				if (path.IsWindowsPath)
				{
					var newPath = WSLUtility.WSLPath(path, WSLPathOption.TranslateFromAWindowsPathToAWSLPath).Result;
					return UniversalPath.New(newPath, false);
				}
			}

			return path;
		}
	}
}
