using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Surveys
{
    public class Survey
    {
        [Key]
        public int SurveyCd { get; set; }
        public string SurveyTitle { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
    }
}