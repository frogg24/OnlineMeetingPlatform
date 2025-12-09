using DataModels.Models;
using DataModels.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Storages
{
    public interface IMeetingStorage
    {
        Task<List<MeetingViewModel>> GetFullList();
        Task<List<MeetingViewModel>> GetFilteredList(MeetingSearchModel model);
        Task<MeetingViewModel?> GetElement(MeetingSearchModel model);
        Task<MeetingViewModel?> Insert(MeetingViewModel model);
        Task<MeetingViewModel?> Update(MeetingViewModel model);
        Task<MeetingViewModel?> Delete(MeetingViewModel model);
        Task<UserMeetings> GetUserMeetings(int userId);
    }
}
