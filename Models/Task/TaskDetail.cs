using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Task
{
    public class TaskDetail
    {
        [Key]
        public int TaskDetailId { get; set; }
        public int TaskId { get; set; }
        public string Status { get; set; } = "Start";
        public int Rating { get; set; }
    }
}