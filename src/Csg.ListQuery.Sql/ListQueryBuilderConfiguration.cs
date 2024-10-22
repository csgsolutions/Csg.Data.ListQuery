using System.Collections.Generic;
using Csg.Data;
using Csg.ListQuery.Sql.Internal;

namespace Csg.ListQuery.Sql;

/// <summary>
/// Configuration properties for the list builder fluent API.
/// </summary>
public class ListQueryBuilderConfiguration
{
    /// <summary>
    /// Gets or sets the query builder used to generate the SQL command that will be executed.
    /// </summary>
    public virtual IDbQueryBuilder QueryBuilder { get; set; }

    public virtual IListQueryDataAdapter DataAdapter { get; set; }

    /// <summary>
    /// Gets or sets a dictionary of functions used to handle applying filters to the query builder. If a handler is specified in this dictionary, any default handler will not be used for a given property name.
    /// </summary>
    public virtual IDictionary<string, ListQueryFilterHandler> Handlers { get; set; }

    /// <summary>
    /// Gets or sets a dictionary of validation data by property name.
    /// </summary>
    public virtual IDictionary<string, ListFieldMetadata> Validations { get; set; }

    /// <summary>
    /// Gets or sets the query definition.
    /// </summary>
    public virtual ListQueryDefinition QueryDefinition { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the <see cref="QueryDefinition"/> fields, filters, and sorting will be validated against the data in <see cref="Validations"/>.
    /// </summary>
    public virtual bool UseValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets a value that indicates if data source query result will be streamed or buffered (default). When streaming results, some features, such as prev/next page links, and returning the total number of rows, are not supported.
    /// </summary>
    public virtual bool UseStreamingResult { get; set; } = false;

    /// <summary>
    /// Gets or sets a value that indicates if an additional row beyond <see cref="ListQueryDefinition.Limit"/> will be requested from the data source in order to determine if additional rows can be fetched.
    /// </summary>
    /// <remarks>If <see cref="UseStreamingResult"/> is true, this value is ignored and defaults to false.</remarks>
    public virtual bool UseLimitOracle { get; set; } = true;

    /// <summary>
    /// An event that fires before <see cref="ListQueryExtensions.Apply(IListQueryBuilder)"/> 
    /// </summary>
    public event ApplyEventHandler BeforeApply;

    /// <summary>
    /// An event that fires after <see cref="ListQueryExtensions.Apply(IListQueryBuilder)"/> 
    /// </summary>
    public event ApplyEventHandler AfterApply;

    /// <summary>
    /// Invokes callbacks registered for the <see cref="BeforeApply"/> event
    /// </summary>
    internal protected void OnBeforeApply()
    {
        if (this.BeforeApply != null)
        {
            this.BeforeApply(this, new ApplyEventArgs(this));
        }
    }

    /// <summary>
    /// Invokes callbacks registered for the <see cref="AfterApply"/> event
    /// </summary>
    internal protected void OnAfterApply(IDbQueryBuilder queryBuilder)
    {
        if (this.AfterApply != null)
        {
            this.AfterApply(this, new ApplyEventArgs(this, queryBuilder));
        }
    }
}