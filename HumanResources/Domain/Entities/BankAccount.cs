namespace YourBrand.HumanResources.Domain.Entities;

public class BankAccount
{
    private BankAccount()
    {

    }

    public BankAccount(string bic, string clearingNo)
    {
        Id = Guid.NewGuid().ToString();
        Bic = bic;
        ClearingNo = clearingNo;
    }

    public string Id { get; private set; }

    public Person? Person { get; set; }

    public string? PersonId { get; set; }

    public string BIC { get; set; }

    public string ClearingNo { get; set; }

    public string Bic { get; }
}