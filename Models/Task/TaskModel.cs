using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Task
{
    public class TaskModel
    {
        [Key]
        public int TaskCd { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public int UserCd { get; set; }
        public int PlanCd { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
        public string? Resources { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public int UserAssigned { get; set; }

        public TaskModel() { }

        public TaskModel(string TaskName, string PlanName, string TaskDescription, int UserCd, string Resources, DateTime StartDay, DateTime EndDay, int UserAssigned, int PlanCd)
        {
            this.TaskName = TaskName;
            this.PlanName = PlanName;
            this.TaskDescription = TaskDescription;
            this.UserCd = UserCd;
            this.Resources = Resources;
            this.StartDay = StartDay;
            this.EndDay = EndDay;
            this.UserAssigned = UserAssigned;
            this.PlanCd = PlanCd;
        }
    }
}