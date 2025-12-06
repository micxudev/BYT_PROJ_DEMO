using _PRO.Models;

namespace _PRO.Json;

public class AppState
{
    public List<Product> Products { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}
