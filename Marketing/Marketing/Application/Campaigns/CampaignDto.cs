using System.ComponentModel.DataAnnotations;

using YourBrand.Marketing.Application;
using YourBrand.Marketing.Application.Common.Models;

namespace YourBrand.Marketing.Application.Campaigns;

public record CampaignDto
(
    string Id,
    string Name
);
