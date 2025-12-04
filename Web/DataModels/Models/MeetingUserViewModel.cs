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
    }
}
