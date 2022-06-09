// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using YourBrand.IdentityService.Domain.ValueObjects;

namespace YourBrand.IdentityService.Domain.Entities;

public class Contract 
{
    private Contract()
    {
    }

    public Contract(DateTime date, string title, string text, CurrencyAmount salary)
    {
        Id = Guid.NewGuid().ToString();
        Date = date;
        Title = title;
        Text = text;
        Salary = salary;
    }

    public string Id { get; private set; }

    public DateTime Date { get; private set; }
    
    public Organization Organization { get; private set; }

    public Person Person { get; private set; } = null!;

    public string PersonId { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public string Text { get; private set; } = null!;

    //public PaymentTerms Terms { get; private set; } = null!;

    public CurrencyAmount Salary { get; set; }
}

public class PaymentTerms
{
    public string Id { get; private set; }

    public DateTime? ValidThru { get; private set; }

    //ublic PaymentTermsPayment Payment { get; private set; } = null!;
}

public record PaymentTermsPayment
{
    public CurrencyAmount Sum { get; set; }

    public Frequency Frequency { get; set; }

    public int Day { get; set; }
}

public enum Frequency
{
    Weekly,
    Monthly,
    Yearly
}