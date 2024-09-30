using AutoMapper;
using Mahak.Main.Campaigns;
using Mahak.Main.Donations;
using Mahak.Main.Files;
using Mahak.Main.Payments;
using Mahak.Main.Transactions;

namespace Mahak.Main;

public class MainApplicationAutoMapperProfile : Profile
{
    public MainApplicationAutoMapperProfile()
    {
        CreateMap<File, FileDto>();
        CreateMap<Campaign, CampaignDto>();
        CreateMap<CampaignItem, CampaignItemDto>();
        CreateMap<Donation, DonationDto>();
        CreateMap<Payment, Parbad.Storage.Abstractions.Models.Payment>(MemberList.Destination)
            .ReverseMap();
        CreateMap<Transaction, Parbad.Storage.Abstractions.Models.Transaction>(MemberList.Destination)
            .ReverseMap();
    }
}