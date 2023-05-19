using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Notification
{
    public class NotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public int UserCd { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}