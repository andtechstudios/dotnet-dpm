using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Andtech.DPM.Tests
{

	public class VariableTests
	{

		[SetUp]
		public void SetUp()
		{
			Environment.SetEnvironmentVariable("HOME", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
		}

		[Test]
		public void DetectVariable()
		{
			var pattern = VariableExpander.GetVariablePattern("~");
			var output = Regex.Replace("~/foo/bar", pattern, "$HOME/");

			Assert.AreEqual("$HOME/foo/bar", output);
		}

		[Test]
		public void TestHomeDirectorySubstitution()
		{
			var variableExpander = new WindowsVariableExpander();
			var path = "~/foo/settings.json";
			var outputPath = variableExpander.ExpandEnvironmentVariables(path);

			var expectedPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/foo/settings.json";
			Assert.AreEqual(Path.GetFullPath(expectedPath), Path.GetFullPath(outputPath));
		}

		[Test]
		public void TestHomeDirectorySubstitutionUnix()
		{
			var variableExpander = new UnixVariableExpander();
			var path = "~/foo/settings.json";
			var outputPath = variableExpander.ExpandEnvironmentVariables(path);

			var expectedPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/foo/settings.json";
			Assert.AreEqual(Path.GetFullPath(expectedPath), Path.GetFullPath(outputPath));
		}
	}
}