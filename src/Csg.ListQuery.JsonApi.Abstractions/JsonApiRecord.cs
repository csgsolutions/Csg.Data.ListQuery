using System.Text.Json.Serialization;

namespace Csg.ListQuery.JsonApi.Abstractions;

public class JsonApiRecord<TAttributes>
{
    public static JsonApiRecord<T> Create<T>(T model) where T : IJsonApiModel
    {
        return new JsonApiRecord<T>(model.Type, model.ID, model);
    }

    public JsonApiRecord()
    {

    }

    public JsonApiRecord(string type, string id, TAttributes attributes) : this()
    {
        this.Type = type;
        this.ID = id;
        this.Attributes = attributes;
    }
               
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public string ID { get; set; }

    [JsonPropertyName("attributes")]
    public TAttributes Attributes { get; set; }
}