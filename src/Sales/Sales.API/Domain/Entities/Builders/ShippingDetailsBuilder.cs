using YourBrand.Domain;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities.Builders;


/*
var address = AddressBuilder.NewAddress("123 Main St", "Springfield", "12345", "USA")
    .WithStateOrProvince("IL")
    .WithAddressLine2("Apt 4B")
    .Build();

var billingDetails = BillingDetailsBuilder.NewBillingDetails("John", "Doe", "555-1234", "john.doe@example.com")
    .WithSSN("123-45-6789")
    .WithAddress(address)
    .Build();

var shippingDetails = ShippingDetailsBuilder.NewShippingDetails("Jane", "Doe")
    .WithCareOf("C/O John's Friend")
    .WithAddress(address)
    .Build();

var order = OrderBuilder.NewOrder(new OrganizationId("YourOrganization"), true)
    .WithTypeId(2)
    .WithCurrency("USD")
    .WithInitialStatus(OrderStatusEnum.Confirmed)
    .WithCustomer(new Customer { Id = "CustomerId", CustomerNo = 12345, Name = "CustomerName" })
    .WithBillingDetails(BillingDetailsBuilder.NewBillingDetails("John", "Doe", "555-1234", "john.doe@example.com")
        .WithAddress(AddressBuilder.NewAddress("123 Main St", "Springfield", "12345", "USA").Build())
        .Build())
    .WithShippingDetails(ShippingDetailsBuilder.NewShippingDetails("Jane", "Doe")
        .WithAddress(AddressBuilder.NewAddress("456 Elm St", "Shelbyville", "54321", "USA").Build())
        .Build())
    .WithSubscription(new Subscription())
    .WithSchedule(schedule => 
    {
        schedule.SetPlannedStartDate(DateTimeOffset.Now.AddDays(1));
        schedule.SetPlannedEndDate(DateTimeOffset.Now.AddDays(5));
    })
    .Build();
*/

public class ShippingDetailsBuilder
    {
        private readonly string _firstName;
        private readonly string _lastName;
        private Address _address = new Address();

        private ShippingDetailsBuilder(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException($"'{nameof(firstName)}' cannot be null or empty.", nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException($"'{nameof(lastName)}' cannot be null or empty.", nameof(lastName));
            }

            _firstName = firstName;
            _lastName = lastName;
        }

        public static ShippingDetailsBuilder NewShippingDetails(string firstName, string lastName)
            => new ShippingDetailsBuilder(firstName, lastName);

        public ShippingDetailsBuilder WithAddress(Address address)
        {
            _address = address ?? throw new ArgumentNullException(nameof(address));
            return this;
        }

        public ShippingDetails Build()
        {
            if(_address is null) 
            {
                throw new ArgumentNullException(nameof(_address));
            }

            return new ShippingDetails
            {
                FirstName = _firstName,
                LastName = _lastName,
                Address = _address
            };
        }
    }

public class AddressBuilder
{
    private readonly string _street;
    private readonly string _city;
    private readonly string _postalCode;
    private readonly string _country;

    private string? _stateOrProvince;
    private string? _addressLine2;

    private AddressBuilder(string street, string city, string postalCode, string country)
    {
        _street = street;
        _city = city;
        _postalCode = postalCode;
        _country = country;
    }

    public static AddressBuilder NewAddress(string street, string city, string postalCode, string country)
        => new AddressBuilder(street, city, postalCode, country);

    public AddressBuilder WithStateOrProvince(string stateOrProvince)
    {
        _stateOrProvince = stateOrProvince;
        return this;
    }

    public AddressBuilder WithAddressLine2(string addressLine2)
    {
        _addressLine2 = addressLine2;
        return this;
    }

    public Address Build()
    {
        return new Address
        {
            Street = _street,
            City = _city,
            PostalCode = _postalCode,
            Country = _country,
            StateOrProvince = _stateOrProvince,
            AddressLine2 = _addressLine2
        };
    }
}