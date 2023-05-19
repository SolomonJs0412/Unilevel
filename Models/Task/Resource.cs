using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Unilever.v1.Models.Task
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }
        public int TaskId { get; set; }
        public string? TypeOfResource { get; set; }
        public string? SourceFile { get; set; }

        public Resource() { }
    }
}