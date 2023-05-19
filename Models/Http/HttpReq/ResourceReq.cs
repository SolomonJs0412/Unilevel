using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class ResourceReq
    {
        public int TaskId { get; set; }
        public string? TypeOfResource { get; set; }
        public string? SourceFile { get; set; }
    }
}