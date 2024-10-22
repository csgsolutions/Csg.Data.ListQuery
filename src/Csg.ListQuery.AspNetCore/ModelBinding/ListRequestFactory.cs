using Csg.ListQuery.Server;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Csg.ListQuery.AspNetCore.ModelBinding;

public class ListRequestFactory
{
    private static readonly System.Text.RegularExpressions.Regex s_filterNameRegex = new System.Text.RegularExpressions.Regex(@"where\[(.*)\]");
    private const string c_where = "where";
    private const string c_fields = "fields";
        
    // sort is standardish, order is acceptable
    private const string c_order = "order";
    private const string c_sort = "sort";

    private const string c_start = "offset";
    private const string c_limit = "limit";
    private const string op_eq = "eq";
    private const string op_gt = "gt";
    private const string op_ge = "ge";
    private const string op_lt = "lt";
    private const string op_le = "le";
    private const string op_ne = "ne";
    private const string op_like = "like";
    private const string op_isnull = "isnull";
    private const string op_in = "in";
    private const string op_nin = "nin";
    private const char c_colon = ':';

    /// <summary>
    /// Creates a request from the given querystring and using <typeparamref name="T"/> as the validation type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queryString"></param>
    /// <returns></returns>
    public virtual T CreateRequest<T>(string queryString) where T : IListRequest
    {
        return (T)CreateRequest(typeof(T), Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queryString));
    }

    /// <summary>
    /// Creates a request from the given querystring and using <paramref name="modelType"/> as the validation type.
    /// </summary>
    /// <param name="modelType"></param>
    /// <param name="queryString"></param>
    /// <returns></returns>
    public virtual IListRequest CreateRequest(System.Type modelType, string queryString)
    {
        return CreateRequest(modelType, Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queryString));
    }

    /// <summary>
    /// Creates a request from the querystring of the given <paramref name="request"/> and using <paramref name="modelType"/> as the validation type.
    /// </summary>
    /// <param name="modelType"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public virtual IListRequest CreateRequest(System.Type modelType, Microsoft.AspNetCore.Http.HttpRequest request)
    {
        return CreateRequest(modelType, request.Query.ToDictionary(s => s.Key, t => t.Value));
    }

    //private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Dictionary<string, System.Reflection.PropertyInfo>> _cache = new System.Collections.Concurrent.ConcurrentDictionary<Type, Dictionary<string, System.Reflection.PropertyInfo>>();

    /// <summary>
    /// Creates a request from the given values and using <paramref name="modelType"/> as the validation type.
    /// </summary>
    /// <param name="modelType"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IListRequest CreateRequest(System.Type modelType, IDictionary<string, StringValues> values)
    {
        if (!typeof(IListRequest).IsAssignableFrom(modelType))
        {
            throw new InvalidCastException($"{nameof(modelType)} must be a type that implments IListRequest.");
        }

        var listRequest = (IListRequest)Activator.CreateInstance(modelType);
        var filterList = new List<Csg.ListQuery.ListFilter>();
        //var validationProperties = listRequest.GetValidationType()
        //    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
        //    .ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);

        listRequest.Filters = filterList;

        foreach (var pair in values)
        {
            if (pair.Key.StartsWith(c_where, StringComparison.OrdinalIgnoreCase))
            {
                filterList.AddRange(ParseFilters(pair.Key, pair.Value).Select(f =>
                {
                    //if (!validationProperties.TryGetValue(f.Name, out System.Reflection.PropertyInfo propInfo))
                    //{
                    //    throw new FormatException($"The filter parameter '{f.Name}' is not recognized.");
                    //}

                    //f.Name = propInfo.Name;

                    //TODO: Remove this selection
                    return f;
                }));
            }
            else if (pair.Key.Equals(c_fields, StringComparison.OrdinalIgnoreCase))
            {
                listRequest.Fields = pair.Value.SelectMany(s => s.Split(',')).Select(fieldName =>
                {
                    //if (!validationProperties.TryGetValue(fieldName, out System.Reflection.PropertyInfo propInfo))
                    //{
                    //    throw new FormatException($"The selection field '{fieldName}' is not recognized.");
                    //}

                    return fieldName.Trim(); //propInfo.Name;
                }).ToList();
            }
            // support order or sort
            else if (pair.Key.Equals(c_order, StringComparison.OrdinalIgnoreCase) || pair.Key.Equals(c_sort, StringComparison.OrdinalIgnoreCase))
            {
                listRequest.Order = pair.Value.Select(sortField =>
                {
                    //if (!validationProperties.TryGetValue(sortField.TrimStart('-'), out System.Reflection.PropertyInfo propInfo))
                    //{
                    //    throw new FormatException($"The sort field '{sortField}' is not recognized.");
                    //}

                    return new Csg.ListQuery.SortField()
                    {
                        Name = sortField.TrimStart('-'), //propInfo.Name,
                        SortDescending = sortField.StartsWith("-")
                    };
                }).ToList();
            }
            else if (pair.Key.Equals(c_start, StringComparison.OrdinalIgnoreCase))
            {
                listRequest.Offset = int.Parse(pair.Value.First());
            }
            else if (pair.Key.Equals(c_limit, StringComparison.OrdinalIgnoreCase))
            {
                listRequest.Limit = int.Parse(pair.Value.First());
            }
            else
            {
                throw new FormatException($"The parameter '{pair.Key}' is not recognized.");
            }
        }

        return listRequest;
    }

    /// <summary>
    /// Parses the given key and string values and adds each generated filter to the given collection.
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="key"></param>
    /// <param name="values"></param>
    public virtual IEnumerable<Csg.ListQuery.ListFilter> ParseFilters(string key, StringValues values)
    {
        var nameMatches = s_filterNameRegex.Match(key);
        var name = nameMatches.Groups[1].Value;

        foreach (var value in values)
        {
            var dto = new Csg.ListQuery.ListFilter()
            {
                Name = name,
                Operator = Csg.ListQuery.ListFilterOperator.Equal
            };

            var valueAndOperator = SplitValue(value);

            dto.Operator = valueAndOperator.Operator;
            dto.Value = valueAndOperator.Value;
            //TODO: handle IN & NOT IN???

            yield return dto;
        }
    }

    /// <summary>
    /// Converts the given operator prefix into the appropriate <see cref="Csg.ListQuery.ListFilterOperator"/> operator.
    /// </summary>
    /// <param name="oper"></param>
    /// <returns></returns>
    protected virtual Csg.ListQuery.ListFilterOperator PrefixToOperator(ReadOnlySpan<char> oper)
    {
        if (oper.Equals(op_eq.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.Equal;
        if (oper.Equals(op_gt.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.GreaterThan;
        if (oper.Equals(op_ge.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.GreaterThanOrEqual;
        if (oper.Equals(op_lt.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.LessThan;
        if (oper.Equals(op_le.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.LessThanOrEqual;
        if (oper.Equals(op_ne.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.NotEqual;
        if (oper.Equals(op_like.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.Like;
        if (oper.Equals(op_isnull.AsSpan(), StringComparison.OrdinalIgnoreCase)) return Csg.ListQuery.ListFilterOperator.IsNull;
        //TODO: handle IN & NOT IN operators???

        throw new NotSupportedException($"The given operator prefix '{oper.ToString()}' is not supported.");
    }

    protected virtual (string Value, Csg.ListQuery.ListFilterOperator @Operator) SplitValue(string s)
    {
        var span = s.AsSpan();
        var indexOfFirstColon = span.IndexOf(c_colon);

        if (indexOfFirstColon > 0)
        {
            return (
                span.Slice(indexOfFirstColon + 1).ToString(),
                PrefixToOperator(span.Slice(0, indexOfFirstColon))
            );
        }
        else
        {
            return (
                s,
                Csg.ListQuery.ListFilterOperator.Equal
            );
        }
    }
}