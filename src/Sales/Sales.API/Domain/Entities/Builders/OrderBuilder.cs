namespace YourBrand.Sales.Domain.Entities.Builders;

using System;

using YourBrand.Domain;
using YourBrand.Sales.Domain.ValueObjects;

public class OrderBuilder
{
    // Required fields
    private readonly OrganizationId _organizationId;
    private readonly bool _vatIncluded;

    // Optional fields with default values
    private int? _typeId = 1;
    private string _currency = "SEK";
    private int _initialStatus = (int)OrderStatusEnum.Draft;
    private Customer? _customer = null;
    private BillingDetails? _billingDetails = null;
    private ShippingDetails? _shippingDetails = null;
    private Subscription? _subscription = null;
    private OrderSchedule? _schedule = null;

    // Private constructor for required fields
    private OrderBuilder(OrganizationId organizationId, int typeId, string currency, bool vatIncluded)
    {
        _organizationId = organizationId;
        _typeId = typeId;
        _currency = currency;
        _vatIncluded = vatIncluded;
    }

    // Static method to create a new builder instance
    public static OrderBuilder NewOrder(OrganizationId organizationId, int typeId, string currency, bool vatIncluded)
        => new OrderBuilder(organizationId, typeId, currency, vatIncluded);

    public OrderBuilder WithTypeId(int typeId)
    {
        _typeId = typeId;
        return this;
    }

    public OrderBuilder WithCurrency(string currency)
    {
        _currency = currency ?? throw new ArgumentNullException(nameof(currency));
        return this;
    }


    public OrderBuilder WithInitialStatus(int initialStatus)
    {
        _initialStatus = initialStatus;
        return this;
    }

    public OrderBuilder WithCustomer(Customer customer)
    {
        _customer = customer ?? throw new ArgumentNullException(nameof(customer));
        return this;
    }

    public OrderBuilder WithBillingDetails(BillingDetails billingDetails)
    {
        _billingDetails = billingDetails ?? throw new ArgumentNullException(nameof(billingDetails));
        return this;
    }

    public OrderBuilder WithShippingDetails(ShippingDetails shippingDetails)
    {
        _shippingDetails = shippingDetails ?? throw new ArgumentNullException(nameof(shippingDetails));
        return this;
    }

    public OrderBuilder WithSubscription(Subscription subscription)
    {
        _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));
        return this;
    }

    public OrderBuilder WithSchedule(Action<OrderSchedule> config)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        var schedule = new OrderSchedule();
        config(schedule);
        _schedule = schedule;
        return this;
    }

    // Build method to create the Order instance
    public Order Build()
    {
        return new Order(
            _organizationId,
            _typeId,
            _vatIncluded,
            _currency,
            _initialStatus,
            _customer,
            _billingDetails,
            _shippingDetails,
            _subscription,
            _schedule
        );
    }
}