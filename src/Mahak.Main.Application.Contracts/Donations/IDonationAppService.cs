using System;
using System.Threading.Tasks;
using Mahak.Main.Campaigns;
using Volo.Abp.Application.Services;

namespace Mahak.Main.Donations;

public interface IDonationAppService : IApplicationService
{
    Task<DonationDetailsDto> GetAsync(Guid id);
    Task<long> GetCountAsync();
    Task<decimal> GetAmountAsync(CampaignType type = CampaignType.Money);
    Task<DonationDetailsDto> CreateAsync(CreateDonationInputDto input);
    Task<DonationDetailsDto> ConfirmAsync(ConfirmDonationInputDto input);
    Task<DonationDetailsDto> RejectAsync(RejectDonationInputDto input);
}