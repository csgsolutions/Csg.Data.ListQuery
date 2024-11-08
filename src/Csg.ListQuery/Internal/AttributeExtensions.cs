using System;
using System.Reflection;

namespace Csg.ListQuery.Internal;

/// <summary>
/// Extension methods for attributes
/// </summary>
public static class AttributeExtensions
{
    /// <summary>
    /// Gets a given attribute instance applied to the given property if it exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property"></param>
    /// <param name="attr"></param>
    /// <returns>True if the attribute exists on the given property. False otherwise.</returns>
    public static bool TryGetAttribute<T>(this PropertyInfo property, out T attr) where T : Attribute
    {
        attr = property.GetCustomAttribute<T>();

        if (attr == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a given attribute instance applied to the given type if it exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="attr"></param>
    /// <returns>True if the attribute exists on the given type, false otherwise.</returns>
    public static bool TryGetAttribute<T>(this Type type, out T attr) where T : Attribute
    {
        attr = type.GetCustomAttribute<T>();

        if (attr == null)
        {
            return false;
        }

        return true;
    }
}