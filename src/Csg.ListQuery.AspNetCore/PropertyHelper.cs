using Csg.ListQuery.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Csg.ListQuery.AspNetCore;

/// <summary>
/// Helper methods for property reflection
/// </summary>
public static class PropertyHelper
{
    /// <summary>
    /// Gets a list of the properties for a give type and optionally matching the given predicate.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Dictionary<string, ListItemPropertyInfo> GetProperties(Type type, Func<ListItemPropertyInfo, bool> predicate = null, int? maxRecursionDepth = null)
    {
        var listConfigs = Csg.ListQuery.Internal.ReflectionHelper.GetFieldsFromType(type, fromCache: true, maxRecursionDepth: maxRecursionDepth);
           
        return listConfigs
            .Select(prop =>
            {
                var propInfo = new ListItemPropertyInfo();
                var jsonPropertyAttribute = prop.PropertyInfo.GetCustomAttributes(typeof(System.Text.Json.JsonProperty), false).FirstOrDefault();

                propInfo.Property = prop.PropertyInfo;
                propInfo.PropertyName = prop.Name;
                if (jsonPropertyAttribute != null)
                    propInfo.JsonName = ((System.Text.Json.JsonProperty)jsonPropertyAttribute).Name;
                if (jsonPropertyAttribute == null)
                    propInfo.JsonName = prop.Name;
                propInfo.IsFilterable = prop.IsFilterable == true;
                propInfo.IsSortable = prop.IsSortable == true;
                propInfo.Description = prop.Description;

                return propInfo;
            })
            .Where(x => predicate == null || predicate(x))
            .ToDictionary(k => k.PropertyName, StringComparer.OrdinalIgnoreCase);
    }
}