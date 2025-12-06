using _PRO.Json;
using _PRO.Models;

// ----------< Try Load From File >----------
var storageFile = new FileInfo("./db/storage.json");
JsonUtils.Load(storageFile, new AppState());

var productExtentTotalCount = Product.All.Count;
var orderExtentTotalCount = Order.All.Count;

Console.WriteLine($"Now in extent {productExtentTotalCount} products.");
Console.WriteLine($"Now in extent {orderExtentTotalCount} orders.");

// -----< Add Some Data If Nothing Loaded >-----
if (productExtentTotalCount == 0 && orderExtentTotalCount == 0)
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
            OrderStatus.InProgress,
            [(p1, 2), (p2, 3)]
        );

        var o2 = new Order(
            DateTime.Now,
            OrderStatus.InProgress,
            [(p2, 4), (p3, 6)]
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
    Console.WriteLine(product.Name);

Console.WriteLine("\n----------------------------------------\n");

Console.WriteLine("Order.All:");
foreach (var order in Order.All)
    Console.WriteLine(order);


// ----------< Save To File >----------
var appState = new AppState
{
    Products = Product.All.ToList(),
    Orders = Order.All.ToList()
};
JsonUtils.SaveAsync(appState, storageFile);

await JsonUtils.ShutdownSaveExecutorAsync();