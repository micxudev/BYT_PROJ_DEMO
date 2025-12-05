namespace _PRO.Models;

using Newtonsoft.Json;
using BYTPRO.Data.Validation.Validators;

public record ProductQuantityInOrder
{
    // ----------< Properties >----------
    public Product Product { get; }
    public Order Order { get; }
    public int Quantity { get; }


    // ----------< Calculated Properties >----------
    // TODO:
    // One thing to check here is
    // what happens if the price of the product changes.
    // Will we somehow preserve the old price for already completed orders?
    // Or it will override the old price?
    // (For now we have price final in product, so no need to care much)
    [JsonIgnore] public decimal TotalPrice => Quantity * Product.Price;


    // ----------< Constructor with validation >----------
    public ProductQuantityInOrder(
        Product product,
        Order order,
        int quantity)
    {
        product.IsNotNull(nameof(Product));
        order.IsNotNull(nameof(Order));
        quantity.IsPositive(nameof(Quantity));

        Product = product;
        Order = order;
        Quantity = quantity;
    }
}