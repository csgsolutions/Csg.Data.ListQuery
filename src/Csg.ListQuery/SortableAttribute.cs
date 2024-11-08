using System;

namespace Csg.ListQuery;

/// <summary>
/// If applied to a property, indicates that the property is allowed to be specified as a <see cref="ListQueryDefinition.Order"/> sort option. If applied to a class, indicates that all properties are sortable unless decorated with Sortable(false)
/// </summary>
[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class SortableAttribute : System.Attribute
{
    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="sortable"></param>
    public SortableAttribute(bool sortable = true)
    {
        this.IsSortable = sortable;
    }

    /// <summary>
    /// Gets a value that indicates if the property can be sorted.
    /// </summary>
    public bool IsSortable { get; private set; }
}