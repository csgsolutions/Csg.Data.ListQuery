using Csg.ListQuery.Server;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Csg.ListQuery.JsonApi.Abstractions;

public class JsonApiListResponse<TAttributes> : ListResponse<JsonApiRecord<TAttributes>>
{
    [JsonPropertyName("data")]
    public override IEnumerable<JsonApiRecord<TAttributes>> Data { get => base.Data; set => base.Data = value; }
        
    [JsonPropertyName("links")]
    public override ListResponseLinks Links { get => base.Links; set => base.Links = value; }

    [JsonPropertyName("meta")]
    public override ListResponseMeta Meta { get => base.Meta; set => base.Meta = value; }
}