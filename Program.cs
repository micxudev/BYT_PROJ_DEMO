using _PRO.Json;
using _PRO.Models;

// ----------< Try Load From Files >----------
var productsFile = new FileInfo("./db/products.json");
var loadedProducts = JsonUtils.Load(productsFile, new List<Product>());
Console.WriteLine($"Loaded {loadedProducts.Count} products from file {productsFile.FullName}");

var ordersFile = new FileInfo("./db/orders.json");
var loadedOrders = JsonUtils.Load(ordersFile, new List<Order>());
Console.WriteLine($"Loaded {loadedOrders.Count} orders from file {ordersFile.FullName}");

// -----< Add Some Data If Nothing Loaded >-----
if (loadedProducts.Count == 0 && loadedOrders.Count == 0)
{
    try
    {
        // -----< Products >-----
        var p1 = new Product(
            "Product 1",
            100m
        );
        var p2 = new Product(
            "Product 2",
            200m
        );
        var p3 = new Product(
            "Product 3",
            300m
        );

        // -----< Orders >-----
        var o1 = new Order(
            DateTime.Now,
            new Dictionary<Product, int>
            {
                { p1, 2 },
                { p2, 3 }
            }
        );
        var o2 = new Order(
            DateTime.Now,
            new Dictionary<Product, int>
            {
                { p2, 4 },
                { p3, 6 }
            }
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return;
    }
}


// ----------< Output >----------
Console.WriteLine("Product.All:");
foreach (var product in Product.All)
    Console.WriteLine(product);

Console.WriteLine("\n----------------------------------------\n");

Console.WriteLine("Order.All:");
foreach (var order in Order.All)
    Console.WriteLine(order);


// ----------< Save To Files >----------
JsonUtils.SaveAsync(Product.All, productsFile);
JsonUtils.SaveAsync(Order.All, ordersFile);

await JsonUtils.ShutdownSaveExecutorAsync();