using DataModels.Models;
using DataModels.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Storages
{
    public interface IUserStorage
    {
        Task<List<UserViewModel>> GetFullList();
        Task<List<UserViewModel>> GetFilteredList(UserSearchModel model);
        Task<UserViewModel?> GetElement(UserSearchModel model);
        Task<UserViewModel?> Insert(UserViewModel model);
        Task<UserViewModel?> Update(UserViewModel model);
        Task<UserViewModel?> Delete(UserViewModel model);
    }
}
