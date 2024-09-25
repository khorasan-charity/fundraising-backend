using System;
using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Campaigns;

public class CampaignDto : EntityDto<int>
{
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
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
}