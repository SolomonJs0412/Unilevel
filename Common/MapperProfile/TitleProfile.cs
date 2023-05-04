using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Unilever.v1.Models.Title;

namespace Unilever.v1.Common.MapperProfile
{
    public class TitleProfile : Profile
    {
        public TitleProfile()
        {
            CreateMap<TitleDto, Title>();
        }
    }
}