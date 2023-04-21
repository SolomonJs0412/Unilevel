using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Unilever.v1.Models.AreaConf;

namespace Unilever.v1.Common.MapperProfile
{
    public class AreaProfile : Profile
    {
        public AreaProfile() {
            CreateMap<AreaDto, Area>();
        }
    }
}