using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Campaigns;

public class CampaignItemAttributeDto : EntityDto<int>
{
    public string Title { get; set; }
    public string Value { get; set; }
    public string? ValueType { get; set; }
    public string? ValueTypeTitle { get; set; }
    public string? Description { get; set; }
}