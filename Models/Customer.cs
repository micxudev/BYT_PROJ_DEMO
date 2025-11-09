using BYTPRO.Data.Validation.Validators;

namespace _PRO.Models;

public class Customer : Person
{
    // ----------< Class Extent >----------
    private static readonly List<Customer> Extent = [];
    public new static IReadOnlyList<Customer> All => Extent.AsReadOnly();


    // ----------< Constants / Business Rules >----------
    public static readonly decimal LoyaltyDiscountPercentage = 0.03m;

    // ----------< Attributes >----------
    private readonly DateTime _registrationDate;

    // ----------< Properties with validation >----------
    public DateTime RegistrationDate
    {
        get => _registrationDate;
        init
        {
            value.IsNotDefault(nameof(RegistrationDate));
            _registrationDate = value;
        }
    }

    public bool IsLoyal => RegistrationDate.AddYears(2) <= DateTime.Today /*&& SuccessfulOrders() > 12*/;

    // ----------< Constructor >----------
    public Customer(
        int id,
        string name,
        string surname,
        string phone,
        string email,
        string password,
        DateTime registrationDate)
        : base(id, name, surname, phone, email, password)
    {
        RegistrationDate = registrationDate;

        RegisterPerson();
        Extent.Add(this);
    }

    // ----------< Methods >----------
    public override string ToString() => $"{base.ToString()}, {RegistrationDate}, Loyal: {IsLoyal}";
}