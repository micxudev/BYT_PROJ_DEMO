namespace _PRO.Models;

using BYTPRO.Data.Validation.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Order
{
    // ----------< Class Extent >----------
    [JsonIgnore] private static readonly List<Order> Extent = [];

    [JsonIgnore] public static IReadOnlyList<Order> All => Extent.AsReadOnly();


    // ----------< Attributes >----------
    private readonly DateTime _creationDate;
    private OrderStatus _status;


    // ----------< Properties with validation >----------
    public DateTime CreationDate
    {
        get => _creationDate;
        init
        {
            value.IsNotDefault(nameof(CreationDate));
            value.IsBefore(DateTime.Now, nameof(CreationDate), "Now");
            _creationDate = value;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public OrderStatus Status
    {
        get => _status;
        set
        {
            value.IsDefined(nameof(Status));
            _status = value;
        }
    }


    // ----------< Calculated Properties >----------
    [JsonIgnore] public decimal TotalPrice => _productQuantities.Sum(item => item.TotalPrice);


    // ----------< Constructor >----------
    public Order(
        DateTime creationDate,
        Dictionary<Product, int> cart)
    {
        CreationDate = creationDate;
        Status = OrderStatus.InProgress;

        // This builds a forward-only model (Order knows its Products).
        // Since at this moment we are not sure whether this Order object will be created
        // or its validation will fail in a child class constructor.
        // Thus., WE ARE NOT ABLE TO ASSOCIATE THIS ORDER WITH PRODUCTS AT THIS POINT.
        _productQuantities = MapCartToQuantities(cart);

        // ...
        // (child class constructor happens here)
        // ...

        // And finally, at this point, all the properties are validated.
        // And it is safe to add this object to associations and to the extent.
        // (p.s. Again, in the original project, this step IS DONE IN A CHILD CLASS CONSTRUCTOR,
        // since Order class there is abstract.)

        // 1. Associations
        AssociateWithProducts();

        // 2. Extents (parent, child or any other)
        Extent.Add(this);
    }


    // ----------< Associations >----------
    private readonly HashSet<ProductQuantityInOrder> _productQuantities;

    public HashSet<ProductQuantityInOrder> ProductQuantities => [.._productQuantities];


    // ----------< Association Methods >----------
    private HashSet<ProductQuantityInOrder> MapCartToQuantities(
        Dictionary<Product, int> cart)
    {
        cart.IsNotNull(nameof(cart));
        cart.Count.IsPositive(nameof(cart));
        return cart
            .Select(e => new ProductQuantityInOrder(e.Key, this, e.Value))
            .ToHashSet();
    }

    private void AssociateWithProducts()
    {
        foreach (var item in ProductQuantities)
            item.Product.AssociateWithOrder(item);
    }
}