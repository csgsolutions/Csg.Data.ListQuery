using System.Collections.Generic;

namespace Csg.ListQuery;

/// <summary>
/// Defines the selections, filters, and sort order of a query for a list of data.
/// </summary>
public class ListQueryDefinition
{
    /// <summary>
    /// Gets or sets the fields that will be returned.
    /// </summary>
    public virtual ICollection<string> Fields
    {
        get => _fields = _fields ?? new List<string>();
        set => _fields = value;
    }
    private ICollection<string> _fields;

    /// <summary>
    /// Gets or sets the filters that will be applied.
    /// </summary>
    public virtual ICollection<ListFilter> Filters
    {
        get => _filters = _filters ?? new List<ListFilter>();
        set => _filters = value;
    }
    private ICollection<ListFilter> _filters;

    /// <summary>
    /// Gets or sets the sort columns that will be applied.
    /// </summary>
    public virtual ICollection<SortField> Order
    {
        get => _order = _order ?? new List<SortField>();
        set => _order = value;
    }
    private ICollection<SortField> _order;

    /// <summary>
    /// Gets or sets the zero-based index of the first record in the result set that will be returned.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of records that will be returned in the result set.
    /// </summary>
    public int Limit { get; set; }
}