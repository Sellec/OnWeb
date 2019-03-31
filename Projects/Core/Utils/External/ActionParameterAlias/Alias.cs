using System;

#pragma warning disable CS1591
namespace OnWeb.Core.CoreUtils.External.ActionParameterAlias
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class Alias : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public Alias(string aliasName)
		{
			Name = aliasName;
		}
	}
}
