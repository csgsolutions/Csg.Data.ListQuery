using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Csg.ListQuery.Server;

[DataContract]
public class ListResponseMeta
{
    [DataMember]
    public virtual PageInfo? Next { get; set; }

    [DataMember]
    public virtual PageInfo? Prev { get; set; }

    [DataMember]
    public virtual int? CurrentCount { get; set; }

    [DataMember]
    public virtual int? TotalCount { get; set; }

    [DataMember]
    public virtual IEnumerable<string> Fields { get; set; }
}