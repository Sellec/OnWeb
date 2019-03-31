//using System.Collections.ObjectModel;
//using System.Web.Mvc;

//#pragma warning disable CS1591
//namespace External.ActionParameterAlias
//{
//	public class ParameterAliasValueProviderFactory : ValueProviderFactory
//	{
//		private readonly ActionParameterAlias[] _aliases;

//		public ParameterAliasValueProviderFactory(params ActionParameterAlias[] aliases)
//		{
//			_aliases = aliases;
//		}

//		public override IValueProvider GetValueProvider(ControllerContext controllerContext)
//		{
//			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
//			//IL_0005: Expected O, but got Unknown
//			ValueProviderCollection val = new ValueProviderCollection();
//			ActionParameterAlias[] aliases = _aliases;
//			foreach (ActionParameterAlias actionParameterAlias in aliases)
//			{
//				((Collection<IValueProvider>)val).Add(new ParameterAliasValueProvider(controllerContext, actionParameterAlias));
//			}
//			return val;
//		}
//	}
//}
