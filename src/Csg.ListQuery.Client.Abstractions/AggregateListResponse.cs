using System.Collections.Generic;

namespace Csg.ListQuery.Client;

/// <summary>
/// Represents the result of multiple calls to a ListQuery API endpoint.
/// </summary>
/// <typeparam name="TData"></typeparam>
public class AggregateListResponse<TData>
{
    /// <summary>
    /// Initializes a new instance with the given parameters.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataCount"></param>
    /// <param name="requestCount"></param>
    public AggregateListResponse(IEnumerable<TData> data, int dataCount, int requestCount)
    {
        this.Data = data;
        this.DataCount = dataCount;
        this.RequestCount = requestCount;
    }

    /// <summary>
    /// Gets the aggregate set of data returned by all requests in the order they were sent.
    /// </summary>
    public virtual IEnumerable<TData> Data { get; private set; }

    /// <summary>
    /// Gets the total number of requests sent to accumulate <see cref="Data"/>.
    /// </summary>
    public virtual int RequestCount { get; private set; }

    /// <summary>
    /// Gets the total number of records in <see cref="Data"/>.
    /// </summary>
    public virtual int DataCount { get; private set; }

    /// <summary>
    /// Gets the list of fields returned in the metadata of the first request.
    /// </summary>
    public virtual IEnumerable<string> Fields { get; set; }
}