using System;
using System.Collections.Generic;
using Mahak.Main.Donations;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Mahak.Main.Campaigns;

public class Campaign : AggregateRoot<int>, IMayHaveCreator, IHasCreationTime, IHasModificationTime
{
    public CampaignType Type { get; set; }
    public required string Title { get; set; }
    public Guid? CoverImageFileId { get; set; }
    public Guid? ThumbnailImageFileId { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public decimal? TargetAmount { get; set; }
    public decimal RaisedAmount { get; set; }
    public int RaiseCount { get; set; }
    public bool IsVisible { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    public ICollection<CampaignItem> CampaignItems { get; set; } = new List<CampaignItem>();
    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}