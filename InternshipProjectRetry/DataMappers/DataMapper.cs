using InternshipProjectRetry.Models;
using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataLibrary.BussinessLogic;

namespace InternshipProjectRetry.DataMappers
{
    public class DataMapper
    {

        public static void dataMapper()
        {
            // Finish setting up this custom mapper

            var config = new MapperConfiguration(cfg =>
                 cfg.CreateMap<SalesForceAccountModel, PartnerAccount>()
                 .ForMember(dest => dest.PartnerID, act => act.MapFrom(src => src.AccountNumber))
                 .ForMember(dest => dest.CompanyName, act => act.MapFrom(src => src.AccountName))
                 .ForMember(dest => dest.WebsiteURL, act => act.MapFrom(src => src.Website))
                 .ForMember(dest => dest.Street, act => act.MapFrom(src => src.Street))
                 .ForMember(dest => dest.City, act => act.MapFrom(src => src.City))
                 .ForMember(dest => dest.State, act => act.MapFrom(src => src.State))
                 .ForMember(dest => dest.Country, act => act.MapFrom(src => src.Country))
                 .ForMember(dest => dest.AccountManagerName, act => act.MapFrom(src => src.OwnerName))
                 .ForMember(dest => dest.AccountManagerEmail, act => act.MapFrom(src => src.OwnerEmail))

             );

            // Pull each Salesforce account object - 

            var salesForceData = SalesForceProcessor.LoadAccounts();

            // then insert it here
            var mapper = new Mapper(config);

            foreach (var account in salesForceData)
            {

              PartnerAccount data = mapper.Map<PartnerAccount>(account);
              SalesForceProcessor.CreateCMRecord(data);
              
            }

        }


    }
}