using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Attribute = Mahak.Main.Attributes.Attribute;

namespace Mahak.Main.Campaigns;

public class CampaignItemAttribute : Entity<int>, IMayHaveCreator, IHasCreationTime, IHasModificationTime
{
    public int CampaignItemId { get; set; }
    public int AttributeId { get; set; }
    public string Value { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    public CampaignItem? CampaignItem { get; set; }
    public Attribute? Attribute { get; set; }
}