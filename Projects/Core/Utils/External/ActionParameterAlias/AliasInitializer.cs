//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Reflection;
//using System.Web.Mvc;

//#pragma warning disable CS1591
//namespace External.ActionParameterAlias
//{
//	public class AliasInitializer
//	{
//		public void OnAuthorization(AuthorizationContext filterContext)
//		{
//			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
//			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
//			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
//			//IL_0024: Expected O, but got Unknown
//			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
//			//IL_002a: Expected O, but got Unknown
//			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
//			List<ActionParameterAlias> list = new List<ActionParameterAlias>();
//			ParameterDescriptor[] parameters = filterContext.ActionDescriptor.GetParameters();
//			AddAliases(list, parameters);
//			IValueProvider val = filterContext.Controller.ValueProvider;
//			ValueProviderCollection val2 = new ValueProviderCollection();
//			((Collection<IValueProvider>)val2).Add(val);
//			ValueProviderCollection val3 = val2;
//			foreach (ActionParameterAlias item in list)
//			{
//				((Collection<IValueProvider>)val3).Add(new ParameterAliasValueProvider(val, item));
//			}
//            filterContext.Controller.ValueProvider = val3;
//		}

//		private static void AddAliases(List<ActionParameterAlias> parameterAliases, IEnumerable<ParameterDescriptor> parameterDescriptors)
//		{
//			foreach (ParameterDescriptor parameterDescriptor in parameterDescriptors)
//			{
//				AddAlias(parameterAliases, (ICustomAttributeProvider)parameterDescriptor, parameterDescriptor.ParameterType, parameterDescriptor.ParameterName);
//			}
//		}

//		private static void AddAliases(List<ActionParameterAlias> parameterAliases, IEnumerable<PropertyInfo> incomingPropertyInfos)
//		{
//			foreach (PropertyInfo incomingPropertyInfo in incomingPropertyInfos)
//			{
//				AddAlias(parameterAliases, incomingPropertyInfo, incomingPropertyInfo.PropertyType, incomingPropertyInfo.Name);
//			}
//		}

//		private static void AddAlias(List<ActionParameterAlias> parameterAliases, ICustomAttributeProvider propertyInfo, Type propertyType, string propertyName)
//		{
//			Alias[] array = (Alias[])propertyInfo.GetCustomAttributes(typeof(Alias), false);
//			if (array.Any())
//			{
//				Alias[] array2 = array;
//				foreach (Alias alias in array2)
//				{
//					parameterAliases.Add(new ActionParameterAlias(propertyName, alias.Name));
//				}
//			}
//			PropertyInfo[] properties = propertyType.GetProperties();
//			AddAliases(parameterAliases, properties);
//		}
//	}
//}
