using System;
using System.Collections.Generic;
using Csg.Data;

namespace Csg.ListQuery.Sql;

/// <summary>
/// Represents a method that handles application of a <see cref="ListFilter"/> to a given <see cref="IDbQueryWhereClause"/>.
/// </summary>
/// <param name="where"></param>
/// <param name="value"></param>
/// <param name="config"></param>
public delegate void ListQueryFilterHandler(Csg.Data.IDbQueryWhereClause where, ListFilter value, ListFieldMetadata config);

public class ListQueryBuilder : IListQueryBuilder
{
    /// <summary>
    /// Creates a <see cref="ListQueryBuilder"/> from the given query builder and query definition.
    /// </summary>
    /// <param name="queryBuilder"></param>
    /// <param name="queryDefinition"></param>
    /// <returns></returns>
    public static IListQueryBuilder Create(IDbQueryBuilder queryBuilder, ListQueryDefinition queryDefinition)
    {
        return new ListQueryBuilder()
        {
            Configuration = new ListQueryBuilderConfiguration()
            {
                QueryBuilder = queryBuilder,
                DataAdapter = new DapperListDataAdapter(queryBuilder.Connection, queryBuilder.Transaction),
                QueryDefinition = queryDefinition,
                Validations = new Dictionary<string, ListFieldMetadata>(StringComparer.OrdinalIgnoreCase),
                Handlers = new Dictionary<string, ListQueryFilterHandler>(StringComparer.OrdinalIgnoreCase)
            }
        };
    }

    /// <summary>
    /// Gets or sets the list builder configuration.
    /// </summary>
    protected virtual ListQueryBuilderConfiguration Configuration { get; set; }

    ListQueryBuilderConfiguration IListQueryBuilder.Configuration { get => this.Configuration; }
}