using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.SearchModels
{
    public class MeetingUserSearchModel
    {
        public int? Id { get; set; }
        public int? MeetingId { get; set; }
        public int? UserId { get; set; }
    }
}
