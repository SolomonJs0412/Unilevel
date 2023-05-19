using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class QuestionCreateReq
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Answer { get; set; } = new List<string>();
        public List<string> CorrectAnswer { get; set; } = new List<string>();
        public int SurveyCd { get; set; }
        public int isHasMoreCorrectAnswer { get; set; } = 0;
    };
}
