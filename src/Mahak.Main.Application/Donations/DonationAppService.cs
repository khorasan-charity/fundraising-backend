using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Mahak.Main.Campaigns;
using Mahak.Main.Payments;
using Parbad.Storage.Abstractions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Mahak.Main.Donations;

public class DonationAppService(
    IReadOnlyRepository<Donation, long> readOnlyDonationRepository,
    IRepository<Donation, long> donationRepository,
    IReadOnlyRepository<Campaign, int> readOnlyCampaignRepository,
    IRepository<Campaign, int> campaignRepository,
    IRepository<Payment, long> paymentRepository,
    IStorageManager storageManager)
    : MainAppService, IDonationAppService
{
    public async Task<DonationDto> GetAsync(long id)
    {
        var item = await readOnlyDonationRepository.GetAsync(id);

        return MapTo<DonationDto>(item)!;
    }

    public async Task<long> GetCountAsync()
    {
        var query = await readOnlyDonationRepository.GetQueryableAsync();

        return await AsyncExecuter.LongCountAsync(query, x => x.IsConfirmed);
    }

    public async Task<decimal> GetAmountAsync(CampaignType type = CampaignType.Money)
    {
        var query = await readOnlyDonationRepository.GetQueryableAsync();

        query = query.Where(x => x.Type == type && x.IsConfirmed);

        return await AsyncExecuter.SumAsync(query, x => x.Amount);
    }

    public async Task<DonationDto> CreateAsync(CreateDonationInputDto input)
    {
        var campaign = await readOnlyCampaignRepository.GetAsync(input.CampaignId);
        var donation = new Donation
        {
            CampaignId = input.CampaignId,
            Type = campaign.Type,
            CampaignItemId = input.CampaignItemId,
            Amount = input.Amount,
            PaymentId = null,
            Name = input.Name,
            Message = input.Message,
            Description = input.Description,
            IsConfirmed = false
        };

        await donationRepository.InsertAsync(donation, true);

        return MapTo<DonationDto>(donation)!;
    }

    // [Authorize(Roles = "admin")]
    public async Task<DonationDto> ConfirmAsync(ConfirmDonationInputDto input)
    {
        var donation = await donationRepository.GetAsync(input.DonationId);
        if (donation.IsConfirmed)
        {
            throw new UserFriendlyException("Donation is already confirmed");
        }
        var campaign = await campaignRepository.GetAsync(donation.CampaignId);
        donation.IsConfirmed = true;
        campaign.RaiseCount++;
        campaign.RaisedAmount += donation.Amount;

        var trackingNumber = 0;
        do
        {
            trackingNumber = RandomNumberGenerator.GetInt32(10000000, int.MaxValue);
        } while (await storageManager.DoesPaymentExistAsync(trackingNumber));

        var payment = new Payment
        {
            TrackingNumber = trackingNumber,
            Amount = donation.Amount,
            Token = null,
            TransactionCode = input.TransactionCode,
            GatewayName = "SamanKish",
            GatewayAccountName = null,
            IsCompleted = true,
            IsPaid = true
        };

        await campaignRepository.UpdateAsync(campaign);
        await paymentRepository.InsertAsync(payment);
        await donationRepository.UpdateAsync(donation, true);

        return MapTo<DonationDto>(donation)!;
    }
}