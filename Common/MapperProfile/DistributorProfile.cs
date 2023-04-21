using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Unilever.v1.Models.DistributorConf;

namespace Unilever.v1.Common.MapperProfile
{
    public class DistributorProfile : Profile
    {
        public DistributorProfile() {
            CreateMap<DistributorDto, Distributor>();
        }
    }
}