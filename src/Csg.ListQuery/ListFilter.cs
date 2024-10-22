namespace Csg.ListQuery;

/// <summary>
/// Represents a filter to be applied to a query.
/// </summary>
public class ListFilter
{
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the operator.
    /// </summary>
    public virtual ListFilterOperator? Operator { get; set; }

    /// <summary>
    /// Gets or sets the value(s) to be applied as part of the filter. This can be a single scalar value, or an IEnumerable.
    /// </summary>
    public virtual object Value { get; set; }
}