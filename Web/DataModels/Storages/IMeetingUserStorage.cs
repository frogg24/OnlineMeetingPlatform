using DataModels.Models;
using DataModels.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Storages
{
    public interface IMeetingUserStorage
    {
        Task<List<MeetingUserViewModel>> GetFullList();
        Task<List<MeetingUserViewModel>> GetFilteredList(MeetingUserSearchModel model);
        Task<MeetingUserViewModel?> GetElement(MeetingUserSearchModel model);
        Task<MeetingUserViewModel?> Insert(MeetingUserViewModel model);
        Task<MeetingUserViewModel?> Update(MeetingUserViewModel model);
        Task<MeetingUserViewModel?> Delete(MeetingUserViewModel model);
    }
}
