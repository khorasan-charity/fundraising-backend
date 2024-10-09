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
        CreateMap<Payment, PaymentDto>();
        CreateMap<Donation, DonationDto>();
        CreateMap<Donation, DonationDetailsDto>();
        CreateMap<CampaignItemAttribute, CampaignItemAttributeDto>()
            .ForMember(x => x.Title, x => x.MapFrom(y => y.Attribute.Title))
            .ForMember(x => x.ValueType, x => x.MapFrom(y => y.Attribute.ValueType))
            .ForMember(x => x.ValueTypeTitle, x => x.MapFrom(y => y.Attribute.ValueTypeTitle))
            .ForMember(x => x.Description, x => x.MapFrom(y => y.Attribute.Description));
        
        CreateMap<Payment, Parbad.Storage.Abstractions.Models.Payment>(MemberList.Destination)
            .ReverseMap();
        CreateMap<Transaction, Parbad.Storage.Abstractions.Models.Transaction>(MemberList.Destination)
            .ReverseMap();
    }
}