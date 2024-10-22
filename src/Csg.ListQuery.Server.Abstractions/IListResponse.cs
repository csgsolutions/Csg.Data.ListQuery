using System.Collections.Generic;

namespace Csg.ListQuery.Server;

/// <summary>
/// Represents the response from an API call requested via <see cref="IListRequest"/>
/// </summary>
public interface IListResponse<T>
{
    /// <summary>
    /// Gets the records returned.
    /// </summary>
    IEnumerable<T> Data { get; }

    /// <summary>
    /// Gets the links for this response
    /// </summary>
    ListResponseLinks Links { get; }

    /// <summary>
    /// Gets the metadata set for this response.
    /// </summary>
    ListResponseMeta Meta { get; }
}