using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Models
{
    public class UserMeetings
    {
        public List<MeetingViewModel> UserMeetingsAsOrganizer { get; set; } = new List<MeetingViewModel>();
        public List<MeetingViewModel> UserMeetingsAsParticipant { get; set; } = new List<MeetingViewModel>();
    }
}
