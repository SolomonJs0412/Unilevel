using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Common.MapperProfile
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<UserDto, User>();
        }
    }
}