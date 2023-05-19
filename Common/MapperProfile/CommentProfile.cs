using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Unilever.v1.Models.Comment;

namespace Unilever.v1.Common.MapperProfile
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CommentDto, Comment>();
        }
    }
}