using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Question
{
    public class QuestionModel
    {
        [Key]
        public int QuestionCd { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string Answers { get; set; } = string.Empty;
        public int SurveyCd { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public int isHasMoreCorrectAnswer { get; set; } = 0;
    }
}