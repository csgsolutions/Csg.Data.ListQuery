using System.Runtime.Serialization;

namespace Csg.ListQuery.Server;

[DataContract]
public class ListResponseLinks
{
    [DataMember]
    public virtual string Next { get; set; }
    [DataMember]
    public virtual string Self { get; set; }
    [DataMember]
    public virtual string Prev { get; set; }
}