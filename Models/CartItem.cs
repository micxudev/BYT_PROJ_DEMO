using Newtonsoft.Json;

namespace _PRO.Models;

public record CartItem(
    [JsonProperty(nameof(Product))] Product Product,
    [JsonProperty(nameof(Quantity))] int Quantity
);