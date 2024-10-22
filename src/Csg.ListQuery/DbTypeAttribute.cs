using System;

namespace Csg.ListQuery;

/// <summary>
/// Indicates the underlying data type for a given property. Used for applying filters in a data source.
/// </summary>
[System.AttributeUsage(AttributeTargets.Property)]
public class DbTypeAttribute : System.Attribute
{
    private readonly System.Data.DbType _type;
    private readonly int? _size;

    /// <summary>
    /// Initializes an instance with the given options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="size"></param>
    public DbTypeAttribute(System.Data.DbType type, int size = 0)
    {
        _type = type;
        _size = size > 0 ? (int?)size : null;
    }

    /// <summary>
    /// Gets the data source type.
    /// </summary>
    public System.Data.DbType DbType
    {
        get
        {
            return _type;
        }
    }

    /// <summary>
    /// Gets the data type size.
    /// </summary>
    public int? Size
    {
        get
        {
            return _size;
        }
    }
}