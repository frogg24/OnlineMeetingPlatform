using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Models
{
    public class MeetingUserViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MeetingId { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public bool? isNotificationOn { get; set; }
    }
}
