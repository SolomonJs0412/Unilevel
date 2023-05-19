using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Comment
{
    public class Comment
    {
        [Key]
        public int CommentCd { get; set; }
        public string? CommentContent { get; set; }
        public int UserCommentId { get; set; }
        public int Rate { get; set; }
        public int TaskCd { get; set; }
    }
}