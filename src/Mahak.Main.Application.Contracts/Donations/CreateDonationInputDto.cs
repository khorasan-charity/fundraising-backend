namespace Mahak.Main.Donations;

public class CreateDonationInputDto
{
    public int CampaignId { get; set; }
    public int? CampaignItemId { get; set; }
    public decimal Amount { get; set; }
    public string? Name { get; set; }
    public string? Mobile { get; set; }
    public string? Message { get; set; }
    public string? Description { get; set; }
}