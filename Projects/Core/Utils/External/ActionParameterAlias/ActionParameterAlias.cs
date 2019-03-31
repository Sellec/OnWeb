#pragma warning disable CS1591
namespace OnWeb.Core.CoreUtils.External.ActionParameterAlias
{
	public class ActionParameterAlias
	{
		public string ActionParameter
		{
			get;
			set;
		}

		public string Alias
		{
			get;
			set;
		}

		public ActionParameterAlias()
		{
		}

		public ActionParameterAlias(string parameterName, string alias)
		{
			ActionParameter = parameterName;
			Alias = alias;
		}
	}
}
