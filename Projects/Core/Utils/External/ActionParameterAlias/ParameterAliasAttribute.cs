using System.Collections.ObjectModel;
using System.Web.Mvc;

using System;

namespace External.ActionParameterAlias
{
    /// <summary>
    /// Позволяет задать альтернативное имя аргумента метода, которое будет восприниматься парсером из адресной строки или из данных формы.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterAliasAttribute : Attribute
    {
        private readonly string _parameterName;

        private readonly string _aliasName;

        /// <summary>
        /// </summary>
        public ParameterAliasAttribute(string parameterName, string aliasName)
        {
            _parameterName = parameterName;
            _aliasName = aliasName;
        }

        //	public override void OnAuthorization(AuthorizationContext filterContext)
        //	{
        //		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
        //		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //		//IL_000b: Expected O, but got Unknown
        //		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
        //		//IL_0011: Expected O, but got Unknown
        //		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
        //		IValueProvider val = filterContext.Controller.ValueProvider;
        //		ValueProviderCollection val2 = new ValueProviderCollection();
        //		((Collection<IValueProvider>)val2).Add(val);
        //		((Collection<IValueProvider>)val2).Add(new ParameterAliasValueProvider(val, new ActionParameterAlias(_parameterName, _aliasName)));
        //		filterContext.Controller.ValueProvider = val2;
        //	}
    }
}
