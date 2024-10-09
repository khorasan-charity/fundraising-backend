using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Mahak.Main.Campaigns;
using Mahak.Main.Payments;
using Parbad.Storage.Abstractions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

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
    public async Task<DonationDetailsDto> GetAsync(Guid id)
    {
        var q = await readOnlyDonationRepository.WithDetailsAsync(x => x.Payment);
        q = q.Where(x => x.Hash == id);

        var item = await AsyncExecuter.FirstOrDefaultAsync(q)
            ?? throw new EntityNotFoundException(typeof(Donation));

        return MapTo<DonationDetailsDto>(item)!;
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

    public async Task<DonationDetailsDto> CreateAsync(CreateDonationInputDto input)
    {
        var campaign = await readOnlyCampaignRepository.GetAsync(input.CampaignId);
        var donation = new Donation
        {
            Hash = GuidGenerator.Create(),
            CampaignId = input.CampaignId,
            Type = campaign.Type,
            CampaignItemId = input.CampaignItemId,
            Amount = input.Amount,
            PaymentId = null,
            Name = input.Name,
            Mobile = input.Mobile,
            Message = input.Message,
            Description = input.Description,
            IsConfirmed = false
        };

        await donationRepository.InsertAsync(donation, true);

        return MapTo<DonationDetailsDto>(donation)!;
    }

    // [Authorize(Roles = "admin")]
    public async Task<DonationDetailsDto> ConfirmAsync(ConfirmDonationInputDto input)
    {
        var donation = await donationRepository.GetAsync(x => x.Hash == input.Hash);
        if (donation.IsConfirmed)
        {
            throw new UserFriendlyException("Donation is already confirmed");
        }
        var campaign = await campaignRepository.GetAsync(donation.CampaignId);
        donation.IsConfirmed = true;
        campaign.RaiseCount++;
        campaign.RaisedAmount += donation.Amount;
        
        for (var i = 0; i < campaign.CampaignItems.Count; i++)
        {
            var item = campaign.CampaignItems.ElementAt(i);
            
            if (item.TargetAmount.HasValue && item.RaisedAmount < item.TargetAmount.Value)
            {
                if (item.RaisedAmount + donation.Amount > item.TargetAmount.Value)
                {
                    var extraAmount = (item.RaisedAmount + donation.Amount) - item.TargetAmount.Value;
                    item.RaiseCount++;
                    item.RaisedAmount = item.TargetAmount.Value;
                    
                    for (var j = i + 1; j < campaign.CampaignItems.Count; j++)
                    {
                        if (extraAmount <= 0)
                        {
                            break;
                        }
                        
                        var nextItem = campaign.CampaignItems.ElementAt(j);
                        nextItem.RaiseCount++;
                        nextItem.RaisedAmount += extraAmount;
                        
                        if (nextItem.TargetAmount.HasValue && nextItem.RaisedAmount > nextItem.TargetAmount.Value)
                        {
                            extraAmount = nextItem.RaisedAmount - nextItem.TargetAmount.Value;
                            nextItem.RaisedAmount = nextItem.TargetAmount.Value;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    item.RaiseCount++;
                    item.RaisedAmount += donation.Amount;
                }

                break;
            }
        }
        
        var trackingNumber = 0;
        do
        {
            trackingNumber = RandomNumberGenerator.GetInt32(10000000, int.MaxValue);
        } while (await storageManager.DoesPaymentExistAsync(trackingNumber));

        var payment = new Payment
        {
            TrackingNumber = trackingNumber,
            Amount = donation.Amount,
            Token = input.Token,
            TransactionCode = input.TransactionCode,
            GatewayName = "SamanKish",
            GatewayAccountName = null,
            IsCompleted = true,
            IsPaid = true
        };

        if (input.ExtraProperties is not null)
        {
            foreach (var kv in input.ExtraProperties)
            {
                payment.SetProperty(kv.Key, kv.Value);
            }
        }

        await campaignRepository.UpdateAsync(campaign);
        await paymentRepository.InsertAsync(payment);
        
        donation.Payment = payment;
        await donationRepository.UpdateAsync(donation, true);

        return MapTo<DonationDetailsDto>(donation)!;
    }

    public async Task<DonationDetailsDto> RejectAsync(RejectDonationInputDto input)
    {
        var donation = await donationRepository.GetAsync(x => x.Hash == input.Hash);
        if (donation.IsConfirmed)
        {
            throw new UserFriendlyException("Donation is already confirmed");
        }

        var trackingNumber = 0;
        do
        {
            trackingNumber = RandomNumberGenerator.GetInt32(1000000000, int.MaxValue);
        } while (await storageManager.DoesPaymentExistAsync(trackingNumber));

        var payment = new Payment
        {
            TrackingNumber = trackingNumber,
            Amount = donation.Amount,
            Token = null,
            TransactionCode = null,
            GatewayName = "SamanKish",
            GatewayAccountName = null,
            IsCompleted = true,
            IsPaid = false
        };
        
        if (input.ExtraProperties is not null)
        {
            foreach (var kv in input.ExtraProperties)
            {
                payment.SetProperty(kv.Key, kv.Value);
            }
        }
        
        await paymentRepository.InsertAsync(payment);
        
        donation.Payment = payment;
        await donationRepository.UpdateAsync(donation, true);

        return MapTo<DonationDetailsDto>(donation)!;
    }
}