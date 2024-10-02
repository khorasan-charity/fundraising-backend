using System.Threading.Tasks;
using Mahak.Main.Campaigns;
using Volo.Abp.Application.Services;

namespace Mahak.Main.Donations;

public interface IDonationAppService : IApplicationService
{
    Task<DonationDto> GetAsync(long donationId);
    Task<long> GetCountAsync();
    Task<decimal> GetAmountAsync(CampaignType type = CampaignType.Money);
    Task<DonationDto> CreateAsync(CreateDonationInputDto input);
    Task<DonationDto> ConfirmAsync(ConfirmDonationInputDto input);
}