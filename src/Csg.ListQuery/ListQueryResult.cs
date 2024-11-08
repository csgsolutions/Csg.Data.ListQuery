using System.Collections.Generic;

namespace Csg.ListQuery;

/// <summary>
/// Represents the result of a list query against a data source.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListQueryResult<T>
{
    /// <summary>
    /// Initializes a new instance with the given option.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataCount"></param>
    /// <param name="totalCount"></param>
    /// <param name="isBuffered"></param>
    /// <param name="limit"></param>
    /// <param name="nextOffset"></param>
    /// <param name="prevOffset"></param>
    public ListQueryResult(IEnumerable<T> data,
        int? dataCount = null,
        int? totalCount = null,
        bool isBuffered = true,
        int? limit = null,
        int? nextOffset = null,
        int? prevOffset = null
    )
    {
        this.Data = data;
        this.DataCount = dataCount;
        this.IsBuffered = isBuffered;
        this.TotalCount = totalCount;
        this.Limit = limit;
        this.NextOffset = nextOffset;
        this.PreviousOffset = prevOffset;
    }

    /// <summary>
    /// Gets an enumerator for the data returned from the query.
    /// </summary>
    public virtual IEnumerable<T> Data { get; private set; }

    /// <summary>
    /// Gets the total rows available at the source, regardless of any Limit or Offset applied to the query.
    /// </summary>
    public int? TotalCount { get; set; }

    /// <summary>
    /// Gets a value that indicates if the data in <see cref="Data"/> is buffered or streamed. If
    /// </summary>
    public bool IsBuffered { get; private set; }

    /// <summary>
    /// Gets the length of the data in <see cref="Data"/> if the result is buffered.
    /// </summary>
    public int? DataCount { get; private set; }

    /// <summary>
    /// Gets the limit value (if any) that was applied during query execution.
    /// </summary>
    public int? Limit { get; private set; }

    /// <summary>
    /// Gets a value that indicates what the offset value should be to request the next page.
    /// </summary>
    public int? NextOffset { get; private set; }

    /// <summary>
    /// Gets a value that indicates what the offset value should be to request the previous page.
    /// </summary>
    public int? PreviousOffset { get; private set; }
}