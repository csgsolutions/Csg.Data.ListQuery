using System.Runtime.Serialization;

namespace Csg.ListQuery.Tests.Mock;

[DataContract]
public class DataContractTestModel
{
    [DataMember]
    public int DataMember1 { get; set; }

    public string NonDataMember1 { get; set; }

    [Filterable]
    public string NonDataMember2 { get; set; }
        
    [Sortable]
    public string NonDataMember3 { get; set; }
}