using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Unilever.v1.Models.Surveys;

namespace Unilever.v1.Common.MapperProfile
{
    public class SurveyProfile : Profile
    {
        public SurveyProfile()
        {
            CreateMap<SurveyDto, Survey>();
        }
    }
}