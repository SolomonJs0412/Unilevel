using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Plan
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        public string TimeOfPlan { get; set; } = string.Empty;
        public int DistributorCd { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Invited { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public int UserReq { get; set; }

        public Plan() { }
        public Plan(string TimeOfPlan, int DistributorCd, string Description, string Invited, string Status, DateTime StartDay, DateTime EndDay, int UserReq)
        {
            this.TimeOfPlan = TimeOfPlan;
            this.DistributorCd = DistributorCd;
            this.Description = Description;
            this.Invited = Invited;
            this.Status = Status;
            this.StartDay = StartDay;
            this.EndDay = EndDay;
            this.UserReq = UserReq;
        }
    }
}