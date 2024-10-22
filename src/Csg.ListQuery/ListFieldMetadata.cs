namespace Csg.ListQuery;

/// <summary>
/// Provides metadata about a field.
/// </summary>
public class ListFieldMetadata
{
    /// <summary>
    /// Gets or sets the column name as defined in the provider (database).
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the provider specific type name (e.g. varchar)
    /// </summary>
    public System.Data.DbType DataType { get; set; }

    /// <summary>
    /// Gets or sets the data type size of the column, if applicable.
    /// </summary>
    public int? DataTypeSize { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this field is filterable or not.
    /// </summary>
    public bool? IsFilterable { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this field is sortable or not.
    /// </summary>
    public bool? IsSortable { get; set; }

    /// <summary>
    /// Gets or sets a value describing this property
    /// </summary>
    public string Description { get; set; }
}

/// <summary>
/// Represents property metadata obtained by reflection.
/// </summary>
public class ReflectedFieldMetadata : ListFieldMetadata
{
    /// <summary>
    /// Gets or sets the property info inforation for the underlying field.
    /// </summary>
    public System.Reflection.PropertyInfo PropertyInfo { get; set; }
}