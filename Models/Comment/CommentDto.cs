using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Comment
{
    public class CommentDto
    {
        public string? CommentContent { get; set; }
        public int UserCommentId { get; set; }
        public int Rate { get; set; }
        public int TaskCd { get; set; }
    }
}