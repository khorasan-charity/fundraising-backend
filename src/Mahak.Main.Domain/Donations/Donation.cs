using System;
using Mahak.Main.Campaigns;
using Mahak.Main.Payments;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Mahak.Main.Donations;

public class Donation : AggregateRoot<long>, IMayHaveCreator, IHasCreationTime
{
    public int CampaignId { get; set; }
    public CampaignType Type { get; set; }
    public int? CampaignItemId { get; set; }
    public decimal Amount { get; set; }
    public long? PaymentId { get; set; }
    public string? Name { get; set; }
    public string? Message { get; set; }
    public string? Description { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    public Campaign? Campaign { get; set; }
    public CampaignItem? CampaignItem { get; set; }
    public Payment? Payment { get; set; }
}