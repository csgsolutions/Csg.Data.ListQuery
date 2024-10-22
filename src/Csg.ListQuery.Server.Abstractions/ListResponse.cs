using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Csg.ListQuery.Server;

[DataContract]
public class ListResponse<T> : IListResponse<T>
{
    public ListResponse()
    {
    }

    public ListResponse(IListRequest request, IEnumerable<T> data)
    {
        this.Data = data;
        this.Meta = this.Meta ?? new ListResponseMeta();
        this.Meta.Fields = request.Fields;
    }

    [DataMember]
    public virtual IEnumerable<T> Data { get; set; }

    [DataMember]
    public virtual ListResponseMeta Meta { get; set; }

    [DataMember]
    public virtual ListResponseLinks Links { get; set; }
}