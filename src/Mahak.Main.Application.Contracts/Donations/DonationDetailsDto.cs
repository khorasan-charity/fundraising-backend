using System;
using System.Security.Principal;
using Mahak.Main.Payments;
using Volo.Abp.Application.Dtos;

namespace Mahak.Main.Donations;

public class DonationDetailsDto : EntityDto<long>
{
    public Guid Hash { get; set; }
    public int CampaignId { get; set; }
    public int? CampaignItemId { get; set; }
    public decimal Amount { get; set; }
    public string? Name { get; set; }
    public string? Mobile { get; set; }
    public string? Message { get; set; }
    public string? Description { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    public PaymentDto? Payment { get; set; }
}