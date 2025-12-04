using DataModels.Models;
using DataModels.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Services
{
    public interface IMeetingUserService
    {
        Task<List<MeetingUserViewModel>?> ReadList(MeetingUserSearchModel? model);
        Task<MeetingUserViewModel?> ReadElement(MeetingUserSearchModel model);
        Task<bool> Create(MeetingUserViewModel model);
        Task<bool> Update(MeetingUserViewModel model);
        Task<bool> Delete(MeetingUserViewModel model);
    }
}
