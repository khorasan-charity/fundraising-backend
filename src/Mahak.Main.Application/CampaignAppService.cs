using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mahak.Main.Campaigns;
using Mahak.Main.Donations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mahak.Main;

public class CampaignAppService(IReadOnlyRepository<Campaign, int> readOnlyCampaignRepository,
    IReadOnlyRepository<Donation, long> readOnlyDonationRepository) : MainAppService
{
    public async Task<PagedResultDto<CampaignDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await readOnlyCampaignRepository.GetQueryableAsync();
        var now = Clock.Now;

        query = query.Where(x => x.IsActive && x.IsVisible)
            .Where(x => x.StartDateTime == null || x.StartDateTime <= now)
            .Where(x => x.EndDateTime == null || x.EndDateTime >= now);

        var totalCount = await AsyncExecuter.CountAsync(query);

        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            query = System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy(query, input.Sorting);
        }

        query = query.PageBy(input);

        var items = await AsyncExecuter.ToListAsync(query);
        return new PagedResultDto<CampaignDto>(totalCount, MapTo<List<CampaignDto>>(items)!);
    }

    public async Task<List<CampaignItemDto>> GetItemsAsync(int id)
    {
        var campaign = await readOnlyCampaignRepository.GetAsync(id);
        return MapTo<List<CampaignItemDto>>(campaign.CampaignItems)!;
    }

    public async Task<PagedResultDto<DonationDto>> GetDonationsAsync(int id, PagedResultRequestDto input)
    {
        var query = await readOnlyDonationRepository.GetQueryableAsync();
        
        query = query.Where(x => x.CampaignId == id);
        
        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query
            .OrderByDescending(x => x.CreationTime)
            .PageBy(input);
        
        var items = await AsyncExecuter.ToListAsync(query);
        return new PagedResultDto<DonationDto>(totalCount, MapTo<List<DonationDto>>(items)!);
    }

    public async Task<long> GetDonationCountAsync(int id)
    {
        var query = await readOnlyDonationRepository.GetQueryableAsync();
        
        query = query.Where(x => x.CampaignId == id);
        
        return await AsyncExecuter.CountAsync(query);
    }
}