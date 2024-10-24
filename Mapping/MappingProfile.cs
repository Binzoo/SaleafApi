using AutoMapper;
using SaleafApi.Models.DTO;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;


namespace SeleafAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map DTOs to Models
            CreateMap<BursaryApplicationFileUploadDto, BursaryApplicationDataDto>()
                .ForMember(dest => dest.TertiarySubjectsAndResultsUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Grade12SubjectsAndResultsUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Grade11SubjectsAndResultsUrl, opt => opt.Ignore());




            CreateMap<BursaryApplicationDataDto, BursaryApplication>()
            .ForMember(dest => dest.OtherLiabilities, opt => opt.MapFrom(src => src.OtherLiabilities))
            .ForMember(dest => dest.OtherAssets, opt => opt.MapFrom(src => src.OtherAssets))
            .ForMember(dest => dest.Dependents, opt => opt.MapFrom(src => src.Dependents))
            .ForMember(dest => dest.FinancialDetailsList, opt => opt.MapFrom(src => src.FinancialDetailsList))
            .ForMember(dest => dest.FixedProperties, opt => opt.MapFrom(src => src.FixedProperties))
            .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.Vehicles))
            .ForMember(dest => dest.LifeAssurancePolicies, opt => opt.MapFrom(src => src.LifeAssurancePolicies))
            .ForMember(dest => dest.Investments, opt => opt.MapFrom(src => src.Investments))
            // Map other properties as needed
            ;
            CreateMap<BursaryApplication, BursaryApplicationDataDto>()
           .ForMember(dest => dest.OtherLiabilities, opt => opt.MapFrom(src => src.OtherLiabilities))
           .ForMember(dest => dest.OtherAssets, opt => opt.MapFrom(src => src.OtherAssets))
           .ForMember(dest => dest.Dependents, opt => opt.MapFrom(src => src.Dependents))
           .ForMember(dest => dest.FinancialDetailsList, opt => opt.MapFrom(src => src.FinancialDetailsList))
           .ForMember(dest => dest.FixedProperties, opt => opt.MapFrom(src => src.FixedProperties))
           .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.Vehicles))
           .ForMember(dest => dest.LifeAssurancePolicies, opt => opt.MapFrom(src => src.LifeAssurancePolicies))
           .ForMember(dest => dest.Investments, opt => opt.MapFrom(src => src.Investments))
           // Map other properties as needed
           ;


            CreateMap<FinancialDetailsDto, FinancialDetails>();
            CreateMap<DependentInfoDto, DependentInfo>();
            CreateMap<PropertyDetailsDto, PropertyDetails>();
            CreateMap<VehicleDetailsDto, VehicleDetails>();
            CreateMap<LifeAssurancePolicyDto, LifeAssurancePolicy>();
            CreateMap<InvestmentDetailsDto, InvestmentDetails>();
            CreateMap<OtherAssetDto, OtherAsset>();
            CreateMap<OtherLiabilityDto, OtherLiability>();


            CreateMap<OtherLiability, OtherLiabilityDto>();
            CreateMap<OtherAsset, OtherAssetDto>();
            CreateMap<DependentInfo, DependentInfoDto>();
            CreateMap<FinancialDetails, FinancialDetailsDto>();
            CreateMap<PropertyDetails, PropertyDetailsDto>();
            CreateMap<VehicleDetails, VehicleDetailsDto>();
            CreateMap<LifeAssurancePolicy, LifeAssurancePolicyDto>();
            CreateMap<InvestmentDetails, InvestmentDetailsDto>();
        }
    }
}
