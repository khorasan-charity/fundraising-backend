using System;
using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Donations;

public class DonationDto : EntityDto<long>
{
    public int CampaignId { get; set; }
    public int? CampaignItemId { get; set; }
    public decimal Amount { get; set; }
    public string? Name { get; set; }
    public string? Message { get; set; }
    public string? Description { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
}