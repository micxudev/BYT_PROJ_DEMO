using _PRO.Json;
using _PRO.Models;

// ----------< Try Load From File >----------
var customersFile = new FileInfo("./db/customers.json");
var loaded = JsonUtils.Load(customersFile, new List<Customer>());
Console.WriteLine($"Loaded {loaded.Count} customers from file {customersFile.FullName}");

// -----< Add Some Data If Nothing Loaded >-----
if (loaded.Count == 0)
{
    try
    {
        new Customer(
            1,
            "John",
            "Doe",
            "+123456789",
            "john.doe@gmail.com",
            "123456",
            DateTime.Now
        );

        new Customer(
            2,
            "Jane",
            "Doe",
            "+123456789",
            "jane.doe@gmail.com",
            "123456",
            DateTime.Now.Subtract(TimeSpan.FromDays(365 * 3))
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return;
    }
}


// ----------< Output >----------
Console.WriteLine("\nCustomer.All:");
foreach (var customer in Customer.All)
    Console.WriteLine(customer);


// ----------< Save To File >----------
JsonUtils.SaveAsync(Customer.All, customersFile);

await JsonUtils.ShutdownSaveExecutorAsync();