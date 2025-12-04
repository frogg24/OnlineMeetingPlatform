using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.SearchModels
{
    public class MeetingSearchModel
    {
        public int? Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Link { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerUsername { get; set; } = string.Empty;
    }
}
