using System;
using System.Collections.Generic;
using System.Linq;
using Csg.ListQuery.Server;

namespace Csg.ListQuery.AspNetCore;

/// <summary>
/// Provides extension methods for <see cref="ListQueryResult"/>
/// </summary>
public static class ListQueryResultExtensions
{
    /// <summary>
    /// Creates a list response from the given query result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queryResult"></param>
    /// <param name="request"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static ListResponse<T> ToListResponse<T>(
        this ListQueryResult<T> queryResult,
        IListRequest request,
        IDictionary<string, ListItemPropertyInfo> properties) where T : new()
    {
        return ToListResponse<T, T>(queryResult, request, properties, s => s);
    }

    /// <summary>
    /// Creates a list response from the given query result using the given selector to map produce the <see cref="ListResponse{T}.Data"/> type.
    /// </summary>
    /// <typeparam name="TQueryType">The data type that will be returned in the <see cref="ListQueryResult{T}"/></typeparam>
    /// <typeparam name="TResponseType">The data type that will be used in the <see cref="ListResponse{T}"/></typeparam>
    /// <param name="queryResult"></param>
    /// <param name="request"></param>
    /// <param name="properties">The properties that will be provided in <see cref="ListResponse{T}.Meta"/> properties.</param>
    /// <param name="selector">A function that maps <typeparamref name="TQueryType"/> to <typeparamref name="TResponseType"/></param>
    /// <returns></returns>
    public static ListResponse<TResponseType> ToListResponse<TQueryType, TResponseType>(
        this ListQueryResult<TQueryType> queryResult,
        IListRequest request,
        IDictionary<string, ListItemPropertyInfo> properties,
        Func<TQueryType, TResponseType> selector)
    {
        properties = properties ?? throw new ArgumentNullException(nameof(properties));

        IEnumerable<TResponseType> data = queryResult.Data.Select(selector);
        int? dataCount = queryResult.IsBuffered ? data.Count() : (int?)null;

        var response = new ListResponse<TResponseType>()
        {
            Meta = new ListResponseMeta()
            {
                TotalCount = queryResult.TotalCount
            }
        };

        if (request.Fields != null)
        {
            response.Meta.Fields = properties.Select(s => s.Value.JsonName).Intersect(request.Fields, StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            response.Meta.Fields = properties.Select(s => s.Value.JsonName);
        }

        if (dataCount.HasValue)
        {
            response.Meta.CurrentCount = dataCount;
        }
            
        response.Meta.Next = queryResult.NextOffset.HasValue ? new PageInfo(queryResult.NextOffset.Value) : (PageInfo?)null;
        response.Meta.Prev = queryResult.PreviousOffset.HasValue ? new PageInfo(queryResult.PreviousOffset.Value) : (PageInfo?)null;
        response.Data = data;

        return response;
    }

    /// <summary>
    /// Creates a list response with links for the given query result using the given selector to map produce the <see cref="ListResponse{TDomain}.Data"/> type.
    /// </summary>
    /// <typeparam name="TQueryType">The data type that will be returned in the <see cref="ListQueryResult{T}"/></typeparam>
    /// <typeparam name="TResponseType">The data type that will be used in the <see cref="ListResponse{T}"/></typeparam>
    /// <param name="queryResult"></param>
    /// <param name="request"></param>
    /// <param name="properties"></param>
    /// <param name="selector"></param>
    /// <param name="currentUri">The URI of the current request.</param>
    /// <returns></returns>
    public static ListResponse<TResponseType> ToListResponse<TQueryType, TResponseType>(
        this ListQueryResult<TQueryType> queryResult,
        IListRequest request,
        IDictionary<string, ListItemPropertyInfo> properties,
        Func<TQueryType, TResponseType> selector,
        System.Uri currentUri
    )
    {
        var response = ToListResponse<TQueryType, TResponseType>(queryResult, request, properties, selector);

        response.Links = new ListResponseLinks()
        {
            Self = currentUri.AbsoluteUri
        };

        if (response.Meta?.Next?.Offset >= 0)
        {
            response.Links.Next = CreateUri(request, currentUri, response.Meta.Next.Value.Offset).ToString();
        }

        if (response.Meta?.Prev?.Offset >= 0)
        {
            response.Links.Prev = CreateUri(request, currentUri, response.Meta.Prev.Value.Offset).ToString();
        }

        return response;
    }

    private static Uri CreateUri(IListRequest request, Uri currentUri, int offset)
    {
        var keyPairs = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(currentUri.Query)
            .Where(x => x.Key.ToLower() != "offset")
            .Append(new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>(nameof(request.Offset).ToLower(), offset.ToString()));
            
        var queryString = Microsoft.AspNetCore.Http.QueryString.Create(keyPairs).ToString()
            .Replace("%5B", "[")
            .Replace("%5D", "]")
            .Replace("%3A", ":");

        return new Uri(string.Concat(currentUri.GetUriWithoutQueryString(), queryString));
    }
}