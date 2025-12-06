using _PRO.Validation;
using _PRO.Validation.Validators;

namespace _PRO.Models;

using Newtonsoft.Json;

public class Product
{
    // ----------< Class Extent >----------
    [JsonIgnore] private static readonly List<Product> Extent = [];

    [JsonIgnore] public static IReadOnlyList<Product> All => Extent.AsReadOnly();


    // ----------< Attributes >----------
    private readonly string _name = null!;
    private readonly decimal _price;


    // ----------< Properties with validation >----------
    public string Name
    {
        get => _name;
        init
        {
            value.IsNotNullOrEmpty(nameof(Name));
            value.IsBelowMaxLength(100);
            _name = value;
        }
    }

    public decimal Price
    {
        get => _price;
        init
        {
            value.IsPositive(nameof(Price));
            _price = value;
        }
    }


    // ----------< Constructor >----------
    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;

        Extent.Add(this);
    }


    // ----------< Associations >----------

    // -----< with attribute >-----
    private readonly HashSet<ProductQuantityInOrder> _usedInOrders = [];

    [JsonIgnore] public HashSet<ProductQuantityInOrder> AssociatedOrders => [.._usedInOrders];

    // TODO: We could accept only Order object, not ProductQuantityInOrder if we need.
    public void AssociateWithOrder(ProductQuantityInOrder orderItem)
    {
        orderItem.IsNotNull(nameof(orderItem));
        if (orderItem.Product != this)
            throw new ValidationException($"{nameof(orderItem.Product)} must reference this Product instance.");
        _usedInOrders.Add(orderItem);
    }
}