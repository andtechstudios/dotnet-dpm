using System;
using System.Text.RegularExpressions;

namespace Andtech.DPM
{

	public abstract class VariableExpander
	{
		public static readonly string Delimiters = @"[/\\]";
		public static readonly Regex HomeRegex = new Regex(GetVariablePattern("~"));
		public static readonly Regex BashRegex = new Regex(@"\$[\w]+");

		public abstract string ExpandEnvironmentVariables(string expression);

		protected string ExpandVariable(Match match)
		{
			var variable = match.Value;
			variable = variable.Trim('$');
			var value = Environment.GetEnvironmentVariable(variable);
			if (value is null)
			{
				return match.Value;
			}

			return value;
		}

		public static string GetVariablePattern(string token) => $@"(?<pre>({Delimiters})|^)(?<value>{token})(?<post>({Delimiters})|$)";

		protected static string ReplacePattern(string input, string pattern, Func<string, string> evaluator)
			=> Regex.Replace(input, GetVariablePattern(pattern), x => Replace(x, evaluator));

		protected static string ReplacePattern(string input, Regex regex, Func<string, string> evaluator)
			=> regex.Replace(input, x => Replace(x, evaluator));

		private static string Replace(Match match, Func<string, string> evaluator)
		{
			var pre = match.Groups["pre"].Value;
			var inner = match.Groups["value"].Value;
			var post = match.Groups["post"].Value;
			var value = evaluator(inner);

			return $"{pre}{value}{post}";
		}
	}

	public class UnixVariableExpander : VariableExpander
	{

		public override string ExpandEnvironmentVariables(string expression)
		{
			expression = ReplacePattern(expression, HomeRegex, x => "$HOME");
			expression = BashRegex.Replace(expression, ExpandVariable);

			return expression;
		}
	}

	public class WindowsVariableExpander : VariableExpander
	{

		public override string ExpandEnvironmentVariables(string expression)
		{
			expression = ReplacePattern(expression, HomeRegex, x => "%userprofile%");
			expression = Environment.ExpandEnvironmentVariables(expression);

			return expression;
		}
	}

	public class WSLVariableExpander : VariableExpander
	{
		public override string ExpandEnvironmentVariables(string expression)
		{
			expression = ReplacePattern(expression, HomeRegex, x => "%userprofile%");
			expression = ReplacePattern(expression, @"\%[\w]+\%", ExpandWSLVariable);
			expression = Environment.ExpandEnvironmentVariables(expression);

			return expression;
		}

		private string ExpandWSLVariable(string match)
		{
			var variable = match;
			variable = variable.Trim('%');
			variable = WSLUtility.WSLVar(variable).Result;

			return variable;
		}
	}
}
