using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Unilever.v1.Models.CMS;

namespace Unilever.v1.Common.MapperProfile
{
    public class CMSProfile : Profile
    {
        public CMSProfile()
        {
            CreateMap<CMSDto, CMSModel>();
        }
    }
}