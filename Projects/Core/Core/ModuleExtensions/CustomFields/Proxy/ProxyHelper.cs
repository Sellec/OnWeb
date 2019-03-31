using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Reflection.Emit;

using System.ComponentModel.DataAnnotations;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Proxy
{
    using Data;
    using Scheme;
    using Field;

    class ProxyHelper
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<string, Type> _proxyTypes = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();

        private static void CreatePassThroughConstructors(TypeBuilder builder, Type baseType)
        {
            foreach (var constructor in baseType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    //throw new InvalidOperationException("Variadic constructors are not supported");
                    continue;
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                    {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                    {
                        parameterBuilder.SetCustomAttribute(attribute);
                    }
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData()))
                {
                    ctor.SetCustomAttribute(attribute);
                }

                var emitter = ctor.GetILGenerator();
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i)
                {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);
            }
        }

        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute =>
            {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }

        public static TParentType CreateTypeObjectFromParent<TParentType, TField>(IScheme<TField> scheme) where TField : IField
        {
            try
            {
                var schemeKey = string.Join(";", scheme.Keys.OrderBy(x => x));
                var instanceType = _proxyTypes.GetOrAdd(schemeKey, (key) => CreateTypeFromParent<TParentType, TField>(scheme));
                if (instanceType != null)
                {
                    var obj = Activator.CreateInstance(instanceType, new object[] { scheme });

                    return (TParentType)obj;
                }
            }
            catch (Exception ex) { Debug.WriteLine("CreateTypeObjectFromParent<{0}>: {1}", typeof(TParentType).Name, ex.Message); }

            return default(TParentType);
        }

        public static Type CreateTypeFromParent<TParentType, TField>(IScheme<TField> scheme) where TField : IField
        {
            try
            {
                var parentType = typeof(TParentType);
                var instanceType = parentType;

                var aName = new AssemblyName("DynamicAssemblyExample_" + Guid.NewGuid().ToString());
                var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
                var mb = ab.DefineDynamicModule(aName.Name);
                var tb = mb.DefineType("CustomFieldsProxy_" + Guid.NewGuid().ToString(), TypeAttributes.Public, parentType);

                var ProxyGetValue_ = parentType.GetMethod(nameof(IProxyGetSet.ProxyGetValue), BindingFlags.Instance | BindingFlags.NonPublic);
                var ProxySetValue_ = parentType.GetMethod(nameof(IProxyGetSet.ProxySetValue), BindingFlags.Instance | BindingFlags.NonPublic);

                if (ProxyGetValue_ != null && ProxySetValue_ != null)
                {
                    //Создаем пронумерованные свойства
                    foreach (var field in scheme)
                    {
                        var name = $"fieldValue_{field.Value.IdField}";

                        var returnType = typeof(object);
                        returnType = field.Value.GetValueType();
                        if (field.Value.IsMultipleValues) returnType = typeof(IEnumerable<>).MakeGenericType(returnType);
                        if (!field.Value.IsValueRequired && returnType.IsValueType)
                            returnType = typeof(Nullable<>).MakeGenericType(returnType);

                        var ProxyGetValue = ProxyGetValue_.MakeGenericMethod(returnType);
                        var ProxySetValue = ProxySetValue_.MakeGenericMethod(returnType);

                        var property = tb.DefineProperty(name, PropertyAttributes.None, returnType, null);

                        /*
                         * Атрибуты
                         * */
                        if (field.Value.IsValueRequired)
                        {
                            var requiredAttribute = typeof(MetadataAndValues.RequiredAttributeCustom).GetConstructor(Type.EmptyTypes);
                            property.SetCustomAttribute(new CustomAttributeBuilder(requiredAttribute, new object[] { }));
                        }

                        switch (field.Value.IdValueType)
                        {
                            case FieldValueType.Email:
                                property.SetCustomAttribute(new CustomAttributeBuilder(typeof(EmailAddressAttribute).GetConstructor(Type.EmptyTypes), new object[] { }));
                                break;

                            case FieldValueType.Phone:
                                property.SetCustomAttribute(new CustomAttributeBuilder(typeof(PhoneAttribute).GetConstructor(Type.EmptyTypes), new object[] { }));
                                break;

                            case FieldValueType.URL:
                                property.SetCustomAttribute(new CustomAttributeBuilder(typeof(UrlAttribute).GetConstructor(Type.EmptyTypes), new object[] { }));
                                break;

                        }

                        var displayAttribute = typeof(DisplayAttribute).GetConstructor(Type.EmptyTypes);
                        var displayAttributePropName = typeof(DisplayAttribute).GetProperty(nameof(DisplayAttribute.Name));
                        property.SetCustomAttribute(new CustomAttributeBuilder(displayAttribute, new object[] { }, new PropertyInfo[] { displayAttributePropName }, new object[] { field.Value.GetDisplayName() }));

                        var additionalAttributes = field.Value.FieldType.GetCustomAttributeBuildersForModel(field.Value);
                        if (additionalAttributes != null)
                            foreach (var attribute in additionalAttributes)
                                if (attribute != null)
                                    property.SetCustomAttribute(attribute);

                        /*
                         * Методы GET-SET
                         * */
                        var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

                        MethodBuilder getter = tb.DefineMethod("get_" + name, getSetAttr, returnType, Type.EmptyTypes);

                        var getIL = getter.GetILGenerator();
                        getIL.Emit(OpCodes.Ldarg_0);
                        getIL.Emit(OpCodes.Ldc_I4, field.Value.IdField);
                        getIL.Emit(OpCodes.Call, ProxyGetValue);
                        getIL.Emit(OpCodes.Ret);

                        var setter = tb.DefineMethod("set_" + name, getSetAttr, null, new Type[] { returnType });

                        var setIL = setter.GetILGenerator();
                        setIL.Emit(OpCodes.Ldarg_0);
                        setIL.Emit(OpCodes.Ldc_I4, field.Value.IdField);
                        setIL.Emit(OpCodes.Ldarg_1);
                        setIL.Emit(OpCodes.Call, ProxySetValue);
                        setIL.Emit(OpCodes.Ret);

                        property.SetGetMethod(getter);
                        property.SetSetMethod(setter);
                    }

                    //Создаем именованные свойства
                    foreach (var field in scheme)
                    {
                        if (!string.IsNullOrEmpty(field.Value.alias))
                        {
                            var name = field.Value.alias;

                            var returnType = typeof(object);
                            returnType = field.Value.GetValueType();
                            if (field.Value.IsMultipleValues) returnType = typeof(IEnumerable<>).MakeGenericType(returnType);

                            var ProxyGetValue = ProxyGetValue_.MakeGenericMethod(returnType);
                            var ProxySetValue = ProxySetValue_.MakeGenericMethod(returnType);

                            var property = tb.DefineProperty(name, PropertyAttributes.None, returnType, null);

                            /*
                             * Атрибуты
                             * */
                            if (field.Value.IsValueRequired)
                            {
                                var requiredAttribute = typeof(MetadataAndValues.RequiredAttributeCustom).GetConstructor(Type.EmptyTypes);
                                property.SetCustomAttribute(new CustomAttributeBuilder(requiredAttribute, new object[] { }));
                            }

                            var displayAttribute = typeof(DisplayAttribute).GetConstructor(Type.EmptyTypes);
                            var displayAttributePropName = typeof(DisplayAttribute).GetProperty(nameof(DisplayAttribute.Name));
                            property.SetCustomAttribute(new CustomAttributeBuilder(displayAttribute, new object[] { }, new PropertyInfo[] { displayAttributePropName }, new object[] { field.Value.GetDisplayName() }));

                            //var additionalAttributes = field.Value.type.GetCustomAttributeBuildersForModel(field.Value);
                            //if (additionalAttributes != null)
                            //    foreach (var attribute in additionalAttributes)
                            //        if (attribute != null)
                            //            property.SetCustomAttribute(attribute);

                            /*
                             * Методы GET-SET
                             * */
                            var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

                            MethodBuilder getter = tb.DefineMethod("get_" + name, getSetAttr, returnType, Type.EmptyTypes);

                            var getIL = getter.GetILGenerator();
                            getIL.Emit(OpCodes.Ldarg_0);
                            getIL.Emit(OpCodes.Ldc_I4, field.Value.IdField);
                            getIL.Emit(OpCodes.Call, ProxyGetValue);
                            getIL.Emit(OpCodes.Ret);

                            var setter = tb.DefineMethod("set_" + name, getSetAttr, null, new Type[] { returnType });

                            var setIL = setter.GetILGenerator();
                            setIL.Emit(OpCodes.Ldarg_0);
                            setIL.Emit(OpCodes.Ldc_I4, field.Value.IdField);
                            setIL.Emit(OpCodes.Ldarg_1);
                            setIL.Emit(OpCodes.Call, ProxySetValue);
                            setIL.Emit(OpCodes.Ret);

                            property.SetGetMethod(getter);
                            property.SetSetMethod(setter);
                        }
                    }

                    CreatePassThroughConstructors(tb, parentType);

                    var typeddd = tb.CreateTypeInfo();
                    instanceType = typeddd;

                }
                return instanceType;
            }
            catch (Exception ex) { Debug.WriteLine("CreateTypeFromParent<{0}>: {1}", typeof(TParentType).Name, ex.Message); }

            return null;
        }

        public static void ClearCache()
        {
            _proxyTypes.Clear();
        }
    }
}
