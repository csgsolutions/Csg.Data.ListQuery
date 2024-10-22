using Csg.ListQuery.Server;
using System.Collections.Generic;

namespace Csg.ListQuery.Client;

public abstract class ListRequestBuilder<T>
{
    public ListRequestBuilder()
    {
        Request = new ListRequest()
        {
            Filters = new List<ListQuery.ListFilter>(),
            Fields = new List<string>(),
            Order = new List<ListQuery.SortField>()
        };
    }

    public Csg.ListQuery.Server.ListRequest Request { get; set; }

    public abstract System.Threading.Tasks.Task<Csg.ListQuery.Server.IListResponse<T>> GetResponseAsync();
}