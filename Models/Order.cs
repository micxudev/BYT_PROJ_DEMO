using _PRO.Validation;
using _PRO.Validation.Validators;

namespace _PRO.Models;

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
    private readonly List<(Product Product, int Quantity)> _cart = null!;


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

    // TODO: expose getter as read-only (wrap with DeserializableReadOnlyList)
    public List<(Product Product, int Quantity)> Cart
    {
        get => _cart;
        init
        {
            // 1. Check for nullability and count
            value.IsNotNull(nameof(Cart));
            value.Count.IsPositive(nameof(Cart));

            // 2. Check for cart-items nullability and positive quantity
            foreach (var cartItem in value)
            {
                cartItem.IsNotNull(nameof(cartItem));
                cartItem.Product.IsNotNull(nameof(cartItem.Product));
                cartItem.Quantity.IsNotNull(nameof(cartItem.Quantity));
                cartItem.Quantity.IsPositive(nameof(cartItem.Quantity));
            }

            // 3. Check for uniqueness of products in the cart
            var duplicates = value
                .GroupBy(cartItem => cartItem.Product)
                .Where(group => group.Count() > 1)
                .ToList();

            if (duplicates.Count > 0)
                throw new ValidationException("Duplicate products in Cart.");

            _cart = value;
        }
    }


    // ----------< Calculated Properties >----------
    [JsonIgnore] public decimal TotalPrice => _associatedProducts.Sum(item => item.TotalPrice);


    // ----------< Constructor >----------
    public Order(
        DateTime creationDate,
        OrderStatus status,
        List<(Product Product, int Quantity)> cart)
    {
        CreationDate = creationDate;
        Status = status;
        Cart = cart;

        // ...
        // (child class constructor happens here)
        // ...

        // And finally, at this point, all the properties are validated.
        // And it is safe to add this object to associations and to the extent.
        // (p.s. Again, in the original project, this step IS DONE IN A CHILD CLASS CONSTRUCTOR,
        // since Order class there is abstract.)

        // 1. Associations
        Associate();

        // 2. Extents (parent, child or any other)
        Extent.Add(this);
    }


    // ----------< Associations >----------
    private readonly HashSet<ProductQuantityInOrder> _associatedProducts = [];

    [JsonIgnore] public HashSet<ProductQuantityInOrder> AssociatedProducts => [.._associatedProducts];


    // ----------< Association Methods >----------
    // TODO: this still is NOT a "proper reverse connection creation"
    //  AssociateWithProduct, AssociateWithOrder are not atomic - meaning
    //  calling one does not guarantee the other to be called as well.
    private void Associate()
    {
        foreach (var cartItem in Cart)
        {
            var association = new ProductQuantityInOrder(cartItem.Product, this, cartItem.Quantity);
            association.Order.AssociateWithProduct(association);
            association.Product.AssociateWithOrder(association);
        }
    }

    public void AssociateWithProduct(ProductQuantityInOrder orderItem)
    {
        orderItem.IsNotNull(nameof(orderItem));
        if (orderItem.Order != this)
            throw new ValidationException($"{nameof(orderItem.Order)} must reference this Order instance.");
        _associatedProducts.Add(orderItem);
    }
}