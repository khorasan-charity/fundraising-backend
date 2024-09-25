using System;
using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Campaigns;

public class CampaignItemDto : EntityDto<int>
{
    public int CampaignId { get; set; }
    public required string Title { get; set; }
    public Guid? ImageFileId { get; set; }
    public string? Description { get; set; }
    public decimal? TargetAmount { get; set; }
    public decimal RaisedAmount { get; set; }
    public int RaiseCount { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
}