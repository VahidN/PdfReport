using System;
using System.Collections.Generic;
using PdfRpt.Core.Contracts;

namespace PdfRpt.Core.Helper
{
    /// <summary>
    /// A helper class for dumping nested property values
    /// </summary>
    public class DumpNestedProperties
    {
        private readonly IList<CellData> _result = new List<CellData>();
        private int _index;

        /// <summary>
        /// Dumps Nested Property Values
        /// </summary>
        /// <param name="data">an instance object</param>
        /// <param name="parent">parent object's name</param>
        /// <param name="dumpLevel">how many levels should be searched</param>
        /// <returns>Nested Property Values List</returns>
        public IList<CellData> DumpPropertyValues(object data, string parent = "", int dumpLevel = 2)
        {
            if (data == null) return null;

            var propertyGetters = FastReflection.Instance.GetGetterDelegates(data.GetType());
            foreach (var propertyGetter in propertyGetters)
            {
                var dataValue = propertyGetter.GetterFunc(data);
                if (dataValue == null)
                {
                    var nullDisplayText = propertyGetter.MemberInfo.GetNullDisplayTextAttribute();
                    _result.Add(new CellData
                                {
                                    PropertyName = parent + propertyGetter.Name,
                                    PropertyValue = nullDisplayText,
                                    PropertyIndex = _index++,
                                    PropertyType = propertyGetter.PropertyType
                                });
                }
                else if (propertyGetter.PropertyType.IsEnum)
                {
                    var enumValue = ((Enum)dataValue).GetEnumStringValue();
                    _result.Add(new CellData
                                {
                                    PropertyName = parent + propertyGetter.Name,
                                    PropertyValue = enumValue,
                                    PropertyIndex = _index++,
                                    PropertyType = propertyGetter.PropertyType
                                });
                }
                else if (isNestedProperty(propertyGetter.PropertyType))
                {
                    _result.Add(new CellData
                                {
                                    PropertyName = parent + propertyGetter.Name,
                                    PropertyValue = dataValue,
                                    PropertyIndex = _index++,
                                    PropertyType = propertyGetter.PropertyType
                                });

                    if (parent.Split('.').Length > dumpLevel)
                    {
                        continue;
                    }
                    DumpPropertyValues(dataValue, parent + propertyGetter.Name + ".", dumpLevel);
                }
                else
                {
                    _result.Add(new CellData
                                {
                                    PropertyName = parent + propertyGetter.Name,
                                    PropertyValue = dataValue,
                                    PropertyIndex = _index++,
                                    PropertyType = propertyGetter.PropertyType
                                });
                }
            }
            return _result;
        }

        private static bool isNestedProperty(Type type)
        {
            if (type.Assembly.FullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase)) return false;
            return
                   (type.IsClass || type.IsInterface) &&
                   !type.IsValueType &&
                   !string.IsNullOrEmpty(type.Namespace) &&
                   !type.Namespace.StartsWith("System.", StringComparison.OrdinalIgnoreCase);
        }
    }
}