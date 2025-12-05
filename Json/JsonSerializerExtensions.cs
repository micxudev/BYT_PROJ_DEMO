using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace _PRO.Json;

public static class JsonSerializerExtensions
{
    public static readonly JsonSerializerSettings Options = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        // ContractResolver = new ParentFirstContractResolver() // not needed for demo
    };

    static JsonSerializerExtensions()
    {
        Options.Converters.Add(new StringEnumConverter());
    }

    public static string ToJson(this IEnumerable collection)
    {
        return JsonConvert.SerializeObject(collection, Options);
    }
}