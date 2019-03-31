//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Web.Mvc;

//#pragma warning disable CS1591
//namespace External.ActionParameterAlias
//{
//	public class AliasModelBinder : DefaultModelBinder
//	{
//		private readonly ValueProviderCollection _valueProviderCollection = new ValueProviderCollection();

//		protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
//		{
//			return this.CreateModel(controllerContext, bindingContext, modelType);
//		}

//		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
//		{
//			this.BindProperty(controllerContext, bindingContext, propertyDescriptor);
//		}

//		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
//		{
//			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
//			//IL_0005: Expected O, but got Unknown
//			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
//			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
//			//IL_005e: Expected O, but got Unknown
//			ValueProviderCollection val = new ValueProviderCollection();
//			foreach (string key in bindingContext.ModelMetadata.AdditionalValues.Keys)
//			{
//				((Collection<IValueProvider>)val).Add(new ParameterAliasValueProvider(controllerContext, new ActionParameterAlias(bindingContext.ModelName, key)));
//			}
//			((Collection<IValueProvider>)val).Add(bindingContext.ValueProvider);
//			bindingContext.ValueProvider = val;
//			return this.BindModel(controllerContext, bindingContext);
//		}

//		private void AddAliasesForProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
//		{
//			IEnumerable<Alias> enumerable = propertyDescriptor.Attributes.OfType<Alias>();
//			if (enumerable.Any())
//			{
//				foreach (Alias item in enumerable)
//				{
//					((Collection<IValueProvider>)_valueProviderCollection).Add(new ParameterAliasValueProvider(controllerContext, new ActionParameterAlias(propertyDescriptor.Name, item.Name)));
//				}
//			}
//		}

//	}
//}
