using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Surveys
{
    public class SurveyDto
    {
        public string SurveyTitle { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
    }
}