using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PdfRpt.Core.Helper
{
    /// <summary>
    /// Getter method's info.
    /// </summary>
    public class GetterInfo
    {
        /// <summary>
        /// Property/Field's name.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Property/Field's Getter method.
        /// </summary>
        public Func<object, object> GetterFunc { set; get; }

        /// <summary>
        /// Property/Field's Type.
        /// </summary>
        public Type PropertyType { set; get; }

        /// <summary>
        /// Obtains information about the attributes of a member and provides access.
        /// </summary>
        public MemberInfo MemberInfo { set; get; }
    }

    /// <summary>
    /// Fast property access, using Reflection.Emit.
    /// </summary>
    public class FastReflection
    {
        /// <summary>
        /// Singleton instance of FastReflection.
        /// </summary>
        public readonly static FastReflection Instance = new FastReflection();
        private FastReflection()
        {
        }

        private readonly Dictionary<Type, List<GetterInfo>> _gettersCache = new Dictionary<Type, List<GetterInfo>>();

        static Func<object, object> createGetterFieldDelegate(Type type, FieldInfo fieldInfo)
        {
            var dynamicGet = new DynamicMethod("_", typeof(object), new[] { typeof(object) }, type);

            var il = dynamicGet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (Func<object, object>)dynamicGet.CreateDelegate(typeof(Func<object, object>));
        }

        static Func<object, object> createGetterPropertyDelegate(Type type, PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            var getter = new DynamicMethod("_", typeof(object), new[] { typeof(object) }, type);

            var il = getter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.EmitCall(OpCodes.Call, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.EmitCall(OpCodes.Callvirt, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (Func<object, object>)getter.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Fast property access, using Reflection.Emit.
        /// </summary>
        public IList<GetterInfo> GetGetterDelegates(Type type)
        {
            List<GetterInfo> cachedGetters;
            if (_gettersCache.TryGetValue(type, out cachedGetters))
                return cachedGetters;

            var gettersList = new List<GetterInfo>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var getterDelegate = createGetterPropertyDelegate(type, property);
                if (getterDelegate == null)
                    continue;

                var info = new GetterInfo
                {
                    Name = property.Name,
                    GetterFunc = getterDelegate,
                    PropertyType = property.PropertyType,
                    MemberInfo = property
                };
                gettersList.Add(info);
            }

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var getterDelegate = createGetterFieldDelegate(type, field);
                if (getterDelegate == null)
                    continue;

                var info = new GetterInfo
                {
                    Name = field.Name,
                    GetterFunc = getterDelegate,
                    PropertyType = field.FieldType,
                    MemberInfo = field
                };
                gettersList.Add(info);
            }

            _gettersCache.Add(type, gettersList);
            return gettersList;
        }
    }
}