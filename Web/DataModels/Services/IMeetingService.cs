using DataModels.Models;
using DataModels.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Services
{
    public interface IMeetingService
    {
        Task<List<MeetingViewModel>?> ReadList(MeetingSearchModel? model);
        Task<MeetingViewModel?> ReadElement(MeetingSearchModel model);
        Task<bool> Create(MeetingViewModel model);
        Task<bool> Update(MeetingViewModel model);
        Task<bool> Delete(MeetingViewModel model);
    }
}
