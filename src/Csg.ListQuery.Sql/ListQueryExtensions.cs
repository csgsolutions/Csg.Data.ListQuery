using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Csg.Data;
using Csg.Data.Sql;
using Csg.ListQuery.Sql.Internal;
using System.Threading;

namespace Csg.ListQuery.Sql;

public static class ListQueryExtensions
{
    public static IListQueryBuilder ValidateWith<TValidation>(this IListQueryBuilder listQuery, int? maxRecursionDepth = null) where TValidation : class, new()
    {
        return ValidateWith(listQuery, typeof(TValidation), maxRecursionDepth: maxRecursionDepth);
    }

    public static IListQueryBuilder ValidateWith(this IListQueryBuilder listQuery, Type validationType, int? maxRecursionDepth = null)
    {
        listQuery.Configuration.UseValidation = true;

        var properties = ListQuery.Internal.ReflectionHelper.GetFieldsFromType(validationType, maxRecursionDepth: maxRecursionDepth);

        foreach (var property in properties)
        {
            listQuery.Configuration.Validations.Add(property.Name, property);
        }

        return listQuery;
    }

    public static IListQueryBuilder ValidateWith(this IListQueryBuilder listQuery, IEnumerable<ListFieldMetadata> fields)
    {
        listQuery.Configuration.UseValidation = true;

        foreach (var field in fields)
        {
            listQuery.Configuration.Validations.Add(field.Name, field);
        }

        return listQuery;
    }

    public static IListQueryBuilder AddFilterHandler(this IListQueryBuilder listQuery, string name, ListQueryFilterHandler handler)
    {
        listQuery.Configuration.Handlers.Add(name, handler);

        return listQuery;
    }

    public static IListQueryBuilder AddFilterHandlers<THandlers>(this IListQueryBuilder listQuery) where THandlers : class, new()
    {
        return AddFilterHandlers(listQuery, typeof(THandlers));
    }

    public static IListQueryBuilder AddFilterHandlers(this IListQueryBuilder listQuery, Type handlersType)
    {
        var methods = handlersType.GetMethods(BindingFlags.Static | BindingFlags.Public);

        foreach (var method in methods)
        {
            var handler = (ListQueryFilterHandler)ListQueryFilterHandler.CreateDelegate(typeof(ListQueryFilterHandler), method);
            //TODO: Cache these maybe???
            //ListQueryFilterHandler handler = (where, filter, config) =>
            //{
            //    method.Invoke(null, new object[] { where, filter, config });
            //};

            listQuery.Configuration.Handlers.Add(method.Name, handler);
        }

        return listQuery;
    }

    public static IListQueryBuilder RemoveHandler(this IListQueryBuilder listQuery, string name)
    {
        listQuery.Configuration.Handlers.Remove(name);
        return listQuery;
    }

    public static IListQueryBuilder NoValidation(this IListQueryBuilder listQuery)
    {
        listQuery.Configuration.UseValidation = false;
        return listQuery;
    }

    public static IListQueryBuilder UseStreamingResult(this IListQueryBuilder listQuery)
    {
        listQuery.Configuration.UseStreamingResult = true;
        return listQuery;
    }

    public static IListQueryBuilder DefaultSort(this Csg.ListQuery.Sql.IListQueryBuilder builder, params string[] sortFields)
    {
        return DefaultSort(builder, (IEnumerable<string>)sortFields);
    }

    public static IListQueryBuilder DefaultSort(this Csg.ListQuery.Sql.IListQueryBuilder builder, IEnumerable<string> sortFields)
    {
        builder.AfterApply((config, query) =>
        {
            if (query.OrderBy.Count == 0)
            {
                foreach (var sortField in sortFields)
                {
                    query.OrderBy.Add(sortField);
                }
            }
        });
        return builder;
    }

    public static IListQueryBuilder MaxLimit(this Csg.ListQuery.Sql.IListQueryBuilder builder, int maxLimit, bool silent = true)
    {
        return builder.BeforeApply((config) =>
        {
            if (config.QueryDefinition.Limit > maxLimit && !silent)
            {
                throw new InvalidOperationException($"The value specified for limit {config.QueryDefinition.Limit} is greater than the maximum allowed value of {maxLimit}.");
            }

            config.QueryDefinition.Limit = Math.Min(maxLimit, config.QueryDefinition.Limit);
        });
    }

    public static IListQueryBuilder DefaultLimit(this Csg.ListQuery.Sql.IListQueryBuilder builder, int limit)
    {
        return builder.BeforeApply((config) =>
        {
            if (config.QueryDefinition.Limit <= 0)
            {
                config.QueryDefinition.Limit = limit;
            }
        });
    }

    public static IListQueryBuilder BeforeApply(this IListQueryBuilder builder, Action<ListQueryBuilderConfiguration> action)
    {
        builder.Configuration.BeforeApply += (sender, e) =>
        {
            action(e.Configuration);
        };

        return builder;
    }

    public static IListQueryBuilder AfterApply(this IListQueryBuilder builder, Action<ListQueryBuilderConfiguration, IDbQueryBuilder> action)
    {
        builder.Configuration.AfterApply += (sender, e) =>
        {
            action(e.Configuration, e.QueryBuilder);
        };

        return builder;
    }

    public static void ApplyFilters(IListQueryBuilder listQuery, IDbQueryBuilder queryBuilder)
    {
        if (listQuery.Configuration.QueryDefinition.Filters != null)
        {
            var where = new DbQueryWhereClause(queryBuilder.Root, Csg.Data.Sql.SqlLogic.And);

            //tODO: IsFilterable is not being enforced. Shoult it be?

            foreach (var filter in listQuery.Configuration.QueryDefinition.Filters)
            {
                var hasConfig = listQuery.Configuration.Validations.TryGetValue(filter.Name, out ListFieldMetadata validationField);

                if (listQuery.Configuration.Handlers.TryGetValue(filter.Name, out ListQueryFilterHandler handler))
                {
                    handler(where, filter, validationField);
                }
                else if (hasConfig || !listQuery.Configuration.UseValidation)
                {
                    where.AddFilter(filter.Name, filter.Operator ?? ListFilterOperator.Equal, filter.Value, validationField?.DataType ?? System.Data.DbType.String, validationField?.DataTypeSize);
                }
                else if (listQuery.Configuration.UseValidation)
                {
                    throw new Exception($"No handler is defined for the filter '{filter.Name}'.");
                }
            }

            if (where.Filters.Count > 0)
            {
                where.ApplyToQuery(queryBuilder);
            }
        }
    }

    public static void ApplySelections(IListQueryBuilder listQuery, IDbQueryBuilder queryBuilder)
    {
        if (listQuery.Configuration.QueryDefinition.Fields != null)
        {
            foreach (var column in listQuery.Configuration.QueryDefinition.Fields)
            {
                if (listQuery.Configuration.Validations.TryGetValue(column, out ListFieldMetadata config))
                {
                    queryBuilder.SelectColumns.Add(new Csg.Data.Sql.SqlColumn(queryBuilder.Root, config.Name));
                }
                else if (listQuery.Configuration.UseValidation)
                {
                    throw new Exception($"The selection field '{column}' does not exist.");
                }
                else
                {
                    queryBuilder.SelectColumns.Add(new Csg.Data.Sql.SqlColumn(queryBuilder.Root, column));
                }
            }
        }
    }

    public static void ApplySort(IListQueryBuilder listQuery, IDbQueryBuilder queryBuilder)
    {
        if (listQuery.Configuration.QueryDefinition.Order != null)
        {
            foreach (var column in listQuery.Configuration.QueryDefinition.Order)
            {
                if (listQuery.Configuration.Validations.TryGetValue(column.Name, out ListFieldMetadata config) && config.IsSortable == true)
                {
                    queryBuilder.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn()
                    {
                        ColumnName = config.Name,
                        SortDirection = column.SortDescending ? Csg.Data.Sql.DbSortDirection.Descending : Csg.Data.Sql.DbSortDirection.Ascending
                    });
                }
                else if (listQuery.Configuration.UseValidation)
                {
                    throw new Exception($"The sort field '{column.Name}' does not exist.");
                }
                else
                {
                    queryBuilder.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn()
                    {
                        ColumnName = column.Name,
                        SortDirection = column.SortDescending ? Csg.Data.Sql.DbSortDirection.Descending : Csg.Data.Sql.DbSortDirection.Ascending
                    });
                }
            }
        }
    }

    public static void ApplyLimit(IListQueryBuilder listQuery, IDbQueryBuilder queryBuilder)
    {
        if (listQuery.Configuration.QueryDefinition.Limit > 0)
        {
            queryBuilder.PagingOptions = new Csg.Data.Sql.SqlPagingOptions()
            {
                Limit = listQuery.Configuration.UseLimitOracle && !listQuery.Configuration.UseStreamingResult ? listQuery.Configuration.QueryDefinition.Limit + 1 : listQuery.Configuration.QueryDefinition.Limit,
                Offset = listQuery.Configuration.QueryDefinition.Offset
            };
        }
    }

    /// <summary>
    /// Applies the given list query configuration and returns a <see cref="IDbQueryBuilder"/>.
    /// </summary>
    /// <param name="listQuery"></param>
    /// <returns></returns>
    public static IDbQueryBuilder Apply(this IListQueryBuilder listQuery)
    {
        listQuery.Configuration.OnBeforeApply();

        var query = listQuery.Configuration.QueryBuilder.Fork();

        ApplySelections(listQuery, query);
        ApplyFilters(listQuery, query);
        ApplySort(listQuery, query);
        ApplyLimit(listQuery, query);

        listQuery.Configuration.OnAfterApply(query);

        return query;
    }

    public static IDbQueryBuilder GetCountQuery(IListQueryBuilder query)
    {
            
        var fullQuery = query.Apply().Fork();
        fullQuery.PagingOptions = null;
        fullQuery.OrderBy.Clear();
        fullQuery.Prefix = null;
        fullQuery.Suffix = null;
            
        var sqlString = fullQuery.ToString();

        fullQuery = new DbQueryBuilder(sqlString, fullQuery.Connection);
        return fullQuery.SelectOnly(new SqlRawColumn("COUNT(1)"));
    }

    public static SqlStatementBatch Render(this Csg.ListQuery.Sql.IListQueryBuilder builder, bool getTotalWhenLimiting = true)
    {
        var appiedQuery = builder.Apply();
        if (getTotalWhenLimiting && appiedQuery.PagingOptions?.Limit > 0 && appiedQuery.PagingOptions?.Offset == 0)
        {
            var prefix = appiedQuery.Prefix;
            var Suffix = appiedQuery.Suffix;
                
            var countQuery = GetCountQuery(builder);
                  
            var buildertest = new DbQueryBuilder[] { (DbQueryBuilder)countQuery, (DbQueryBuilder)appiedQuery }
                .RenderBatch();

            return new SqlStatementBatch(2, (prefix!=null ? prefix + ";" :"")+ buildertest.CommandText + (Suffix != null ? Suffix + ";" : "") , buildertest.Parameters);
        }
        else
        {
            var stmt = appiedQuery.Render();
            return new SqlStatementBatch(1, stmt.CommandText, stmt.Parameters);
        }
    }

    public async static System.Threading.Tasks.Task<ListQueryResult<T>> GetResultAsync<T>(this Csg.ListQuery.Sql.IListQueryBuilder builder, bool getTotalWhenLimiting = true, CancellationToken cancellationToken = default)
    {
        var stmt = builder.Render(getTotalWhenLimiting);
        int? totalCount = null;
        IEnumerable<T> data = null;
        bool isBuffered = !builder.Configuration.UseStreamingResult;
        bool limitOracle = builder.Configuration.QueryDefinition.Limit > 0 && builder.Configuration.UseLimitOracle && !builder.Configuration.UseStreamingResult;
        int? dataCount = null;
        int? nextOffset = null;
        int? prevOffset = null;

        if (stmt.Count == 1)
        {
            data = await builder.Configuration.DataAdapter.GetResultsAsync<T>(stmt, builder.Configuration.UseStreamingResult, builder.Configuration.QueryBuilder.CommandTimeout, cancellationToken).ConfigureAwait(false);
        }
        else if (stmt.Count == 2)
        {
            var batchResult = await builder.Configuration.DataAdapter.GetTotalCountAndResultsAsync<T>(stmt, builder.Configuration.UseStreamingResult, builder.Configuration.QueryBuilder.CommandTimeout, cancellationToken).ConfigureAwait(false);
            data = batchResult.Items;
            totalCount = batchResult.TotalCount;
        }
        else
        {
            throw new NotSupportedException("A statement with more than 2 queries is not supported.");
        }

        //TODO: Can we still use .Take() here with a limit oracle when streaming?
        // if the data is streamed, we can't provide a total count, and we can't use the next page oracle
        if (limitOracle)
        {
            // if we used a limit oracle, then strip the last row off
            int actualCount = data.Count();
            data = data.Take(builder.Configuration.QueryDefinition.Limit);
            dataCount = data.Count();

            // if more records were fetched than limit, then there is at least one more page of data to fetch.
            if (actualCount > dataCount)
            {
                nextOffset = builder.Configuration.QueryDefinition.Offset + builder.Configuration.QueryDefinition.Limit;
            }
        }
        else if (builder.Configuration.QueryDefinition.Limit > 0)
        {
            nextOffset = builder.Configuration.QueryDefinition.Offset + builder.Configuration.QueryDefinition.Limit;
        }

        if (builder.Configuration.QueryDefinition.Offset > 0)
        {
            prevOffset = Math.Max(builder.Configuration.QueryDefinition.Offset - builder.Configuration.QueryDefinition.Limit, 0);
        }

        return new ListQueryResult<T>(data, dataCount, totalCount, isBuffered, builder.Configuration.QueryDefinition.Limit, nextOffset, prevOffset);
    }
}