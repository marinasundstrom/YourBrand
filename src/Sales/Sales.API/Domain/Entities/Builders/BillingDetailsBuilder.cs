using YourBrand.Domain;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities.Builders;

public class BillingDetailsBuilder
{
    private readonly string _firstName;
    private readonly string _lastName;
    private readonly string _phoneNumber;
    private readonly string _email;
    private Address _address = new Address();
    private string? _ssn;

    private BillingDetailsBuilder(string firstName, string lastName, string phoneNumber, string email)
    {
        if (string.IsNullOrEmpty(firstName))
        {
            throw new ArgumentException($"'{nameof(firstName)}' cannot be null or empty.", nameof(firstName));
        }

        if (string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentException($"'{nameof(lastName)}' cannot be null or empty.", nameof(lastName));
        }

        if (string.IsNullOrEmpty(phoneNumber))
        {
            throw new ArgumentException($"'{nameof(phoneNumber)}' cannot be null or empty.", nameof(phoneNumber));
        }

        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
        }

        _firstName = firstName;
        _lastName = lastName;
        _phoneNumber = phoneNumber;
        _email = email;
    }

    public static BillingDetailsBuilder NewBillingDetails(string firstName, string lastName, string phoneNumber, string email)
        => new BillingDetailsBuilder(firstName, lastName, phoneNumber, email);

    public BillingDetailsBuilder WithSSN(string ssn)
    {
        _ssn = ssn ?? throw new ArgumentNullException(nameof(ssn));
        return this;
    }

    public BillingDetailsBuilder WithAddress(Address address)
    {
        _address = address ?? throw new ArgumentNullException(nameof(address));
        return this;
    }

    public BillingDetails Build()
    {
        return new BillingDetails
        {
            FirstName = _firstName,
            LastName = _lastName,
            SSN = _ssn,
            PhoneNumber = _phoneNumber,
            Email = _email,
            Address = _address
        };
    }
}