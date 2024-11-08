using Csg.Data;

namespace Csg.ListQuery.Sql.Internal;

/// <summary>
/// Represents an event handler for the <see cref="ListQueryExtensions.Apply(IListQueryBuilder)"/> apply process of a list query builder.
/// </summary>
/// <param name="sender"></param>
/// <param name="args"></param>
public delegate void ApplyEventHandler(object sender, ApplyEventArgs args);

/// <summary>
/// Event data for the <see cref="ApplyEventHandler"/>.
/// </summary>
public class ApplyEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="config"></param>
    public ApplyEventArgs(ListQueryBuilderConfiguration config)
    {
        this.Configuration = config;
        this.QueryBuilder = config.QueryBuilder;
    }

    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="config"></param>
    /// <param name="queryBuilder"></param>
    public ApplyEventArgs(ListQueryBuilderConfiguration config, IDbQueryBuilder queryBuilder)
    {
        this.Configuration = config;
        this.QueryBuilder = queryBuilder;
    }

    /// <summary>
    /// Gets or sets the list builder configuration
    /// </summary>
    public ListQueryBuilderConfiguration Configuration { get; set; }

    /// <summary>
    /// Gets or sets the querybuilder associated with this event.
    /// </summary>
    public IDbQueryBuilder QueryBuilder { get; set; }
}