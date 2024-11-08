using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Csg.ListQuery.Internal;

/// <summary>
/// Reflection helpers
/// </summary>
public static class ReflectionHelper
{
    private static System.Collections.Concurrent.ConcurrentDictionary<string, ICollection<ReflectedFieldMetadata>> s_typeCache 
        = new System.Collections.Concurrent.ConcurrentDictionary<string, ICollection<ReflectedFieldMetadata>>();

    public static int DefaultMaxRecursionDepth = 1;

    private static void PopulateFieldCollection(Type type, ICollection<ReflectedFieldMetadata> schema, int maxDepth, string prefix = null, bool? defaultSortable = null, bool? defaultFilterable = null, int depth = 1)
    {
        if (!defaultFilterable.HasValue && type.TryGetAttribute(out FilterableAttribute filterableAttr))
        {
            defaultFilterable = filterableAttr.IsFilterable;
        }

        if (!defaultSortable.HasValue && type.TryGetAttribute(out SortableAttribute sortableAttr))
        {
            defaultSortable = sortableAttr.IsSortable;
        }

        IEnumerable<PropertyInfo> properties;


        if (type.GetCustomAttributes<System.Runtime.Serialization.DataContractAttribute>().Any())
        {
            // If data contract attribute, then we only want things marked as DataMember, or explicity as filterable, sortable
            properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p =>
                    p.GetCustomAttribute<System.Runtime.Serialization.DataMemberAttribute>() != null
                    || p.GetCustomAttribute<SortableAttribute>() != null
                    || p.GetCustomAttribute<FilterableAttribute>() != null
                );
        }
        else
        {
            properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        }

        foreach (var property in properties)
        {
            var ci = new ReflectedFieldMetadata();

            ci.Name = prefix == null ? property.Name : string.Concat(prefix, ".", property.Name);
            ci.Description = property.TryGetAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>(out System.ComponentModel.DataAnnotations.DisplayAttribute displayAttr)
                ? displayAttr.GetDescription()
                : (string)null;

            // first see if the property has the DbType attribute, otherwise infer DbType from the property type
            if (property.TryGetAttribute(out DbTypeAttribute dbTypeAttr))
            {
                ci.DataType = dbTypeAttr.DbType;
                ci.DataTypeSize = dbTypeAttr.Size;
            }
            else
            {
                ci.DataType = TypeToDbType(property.PropertyType);
            }

            // if the property is decorated with StringLength or MaxLength, use that as the size.
            if (!ci.DataTypeSize.HasValue && property.TryGetAttribute(out System.ComponentModel.DataAnnotations.StringLengthAttribute stringLengthAttr))
            {
                ci.DataTypeSize = stringLengthAttr.MaximumLength;
            }
            else if (!ci.DataTypeSize.HasValue && property.TryGetAttribute(out System.ComponentModel.DataAnnotations.MaxLengthAttribute maxLengthAttr))
            {
                ci.DataTypeSize = maxLengthAttr.Length;
            }

            if (property.TryGetAttribute(out filterableAttr))
            {
                ci.IsFilterable = filterableAttr.IsFilterable;
                ci.Description = filterableAttr.Description ?? ci.Description;
            }
            else
            {
                ci.IsFilterable = defaultFilterable;
            }

            ci.IsSortable = property.TryGetAttribute(out sortableAttr) ? sortableAttr.IsSortable : defaultSortable;
            ci.PropertyInfo = property;

            schema.Add(ci);

            if (depth < maxDepth && IsNavigatableType(property.PropertyType))
            {
                PopulateFieldCollection(property.PropertyType, schema, maxDepth, prefix: ci.Name, defaultSortable: ci.IsSortable, defaultFilterable: ci.IsFilterable, depth: depth+1);
            }
        }          
    }

    private static bool IsNavigatableType(Type type)
    {
        return !type.IsValueType && !type.IsArray && type != typeof(string);
    }

    private static ICollection<ReflectedFieldMetadata> GetListPropertyInfoInternal(Type type, int? maxRecusionDepth = null)
    {
        var schema = new List<ReflectedFieldMetadata>();

        PopulateFieldCollection(type, schema, maxRecusionDepth ?? DefaultMaxRecursionDepth);

        return schema;
    }

    /// <summary>
    /// Gets a dictionary of field metadata created from the properties of the given type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fromCache"></param>
    /// <param name="maxRecusionDepth">The maximum number of levels to recurse down the object's child properties. The default value is 1 (no recursion).</param>
    /// <returns></returns>
    public static IEnumerable<ReflectedFieldMetadata> GetFieldsFromType(Type type, bool fromCache = true, int? maxRecursionDepth = null)
    {
        string cacheKey = $"{type.AssemblyQualifiedName}:{(fromCache ? 1 : 0)}:{maxRecursionDepth ?? DefaultMaxRecursionDepth}";
            
        if (!fromCache)
        {
            return GetListPropertyInfoInternal(type, maxRecursionDepth);
        }

        return s_typeCache.GetOrAdd(cacheKey, (key) =>
        {
            return GetListPropertyInfoInternal(type, maxRecursionDepth);
        });
    }

    /// <summary>
    /// Clears all cached metadata.
    /// </summary>
    public static void ClearCachedTypes()
    {
        s_typeCache.Clear();
    }

    /// <summary>
    /// Gets the <see cref="DbType"/> for a System type.
    /// </summary>
    /// <remarks>From https://github.com/csgsolutions/Csg.Data/blob/1a8b0c54047cc645a5b7b42119a64f4c70c31ba5/Csg.Data/DbConvert.cs#L145</remarks>
    /// <param name="type"></param>
    /// <returns></returns>
    public static System.Data.DbType TypeToDbType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type == typeof(byte)) return DbType.Byte;
        if (type == typeof(short)) return DbType.Int16;
        if (type == typeof(int)) return DbType.Int32;
        if (type == typeof(long)) return DbType.Int64;
        if (type == typeof(bool)) return DbType.Boolean;
        if (type == typeof(Guid)) return DbType.Guid;
        if (type == typeof(DateTime)) return DbType.DateTime2;
        if (type == typeof(DateTimeOffset)) return DbType.DateTimeOffset;
        if (type == typeof(TimeSpan)) return DbType.Time;
        if (type == typeof(string)) return DbType.String;
        if (type == typeof(char)) return DbType.StringFixedLength;
        if (type == typeof(byte[])) return DbType.Binary;
        if (type == typeof(decimal)) return DbType.Decimal;
        if (type == typeof(float)) return DbType.Single;
        if (type == typeof(double)) return DbType.Double;

        return DbType.Object;
    }
}