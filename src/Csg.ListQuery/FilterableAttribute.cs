using System;

namespace Csg.ListQuery;

/// <summary>
/// Indicates that a property can be specified as part of the <see cref="ListQueryDefinition.Filters"/>, or if applied to a class, indicates that any property on the class can be filtered.
/// </summary>
[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class FilterableAttribute : System.Attribute
{
    /// <summary>
    /// Initializes a ne instance
    /// </summary>
    /// <param name="filterable"></param>
    public FilterableAttribute(bool filterable = true)
    {
        this.IsFilterable = filterable;
    }

    /// <summary>
    /// Gets a value that indicates if the property or class can be filtered.
    /// </summary>
    public bool IsFilterable { get; private set; }

    /// <summary>
    /// Gets or sets a value that will be used to descibe the property when used as a filter.
    /// </summary>
    public string Description { get; set; }
}