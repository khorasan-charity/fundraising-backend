using System.Linq;
using System.Threading.Tasks;
using Mahak.Main.Campaigns;
using Mahak.Main.Donations;
using Mahak.Main.Utils;
using Volo.Abp.Domain.Repositories;

namespace Mahak.Main;

public class DonationAppService(IReadOnlyRepository<Donation, long> readOnlyDonationRepository) : MainAppService
{
    public async Task<long> GetCountAsync()
    {
        return await readOnlyDonationRepository.GetCountAsync();
    }

    public async Task<decimal> GetAmountAsync(CampaignType type = CampaignType.Money)
    {
        var query = await readOnlyDonationRepository.GetQueryableAsync();

        query = query.Where(x => x.Type == type);

        return await AsyncExecuter.LongCountAsync(query);
    }
}