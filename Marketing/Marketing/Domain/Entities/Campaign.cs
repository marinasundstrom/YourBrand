using YourBrand.Marketing.Domain.Common;
using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Domain.Entities;

public class Campaign
{
    readonly HashSet<Address> _addresses = new HashSet<Address>();

    protected Campaign() { }

    public Campaign(string name, string organizationNo, string vatNo)
    {
        Name = name;
        CampaignNo = organizationNo;
        VatNo = vatNo;

        //AddDomainEvent(new ProspectCreated(Id));
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; }

    public string CampaignNo { get; set; }

    public string VatNo { get; set; }

    public string Email { get; set; }

    public string? PhoneHome { get; set; }

    public string PhoneMobile { get; set; }
}
