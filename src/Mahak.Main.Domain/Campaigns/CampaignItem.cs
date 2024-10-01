using System;
using System.Collections.Generic;
using Mahak.Main.Donations;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Mahak.Main.Campaigns;

public class CampaignItem : Entity<int>, IMayHaveCreator, IHasCreationTime, IHasModificationTime
{
    public int CampaignId { get; set; }
    public required string Title { get; set; }
    public Guid? ImageFileId { get; set; }
    public string? Description { get; set; }
    public decimal? TargetAmount { get; set; }
    public decimal RaisedAmount { get; set; }
    public int RaiseCount { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    public Campaign? Campaign { get; set; }
    public ICollection<CampaignItemAttribute> Attributes { get; set; } = new List<CampaignItemAttribute>();
    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}