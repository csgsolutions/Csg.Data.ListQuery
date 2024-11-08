namespace Csg.ListQuery.Sql;

/// <summary>
/// Represents an object used to build a list query.
/// </summary>
public interface IListQueryBuilder
{
    /// <summary>
    /// Gets the current configuration.
    /// </summary>
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    ListQueryBuilderConfiguration Configuration { get; }
}