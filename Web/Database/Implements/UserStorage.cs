using Database.Models;
using DataModels.Models;
using DataModels.Storages;
using DataModels.SearchModels;
using DataModels.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Implements
{
    public class UserStorage: IUserStorage
    {
        public async Task<List<UserViewModel>> GetFullList()
        {
            using var context = new Database();
            return await context.Users.Select(x => x.GetViewModel).ToListAsync();
        }

        //Будет использоваться для подгрузки участников мероприятия
        public async Task<List<UserViewModel>> GetFilteredList(UserSearchModel model)
        {
            using var context = new Database();

            var query = context.Users.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }
            if (!string.IsNullOrEmpty(model.Username))
            {
                query = query.Where(x => x.Username.Equals(model.Username));
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                query = query.Where(x => x.Email.Equals(model.Email));
            }
            if (!string.IsNullOrEmpty(model.PasswordHash))
            {
                var hash = model.PasswordHash;
                query = query.Where(x => x.PasswordHash.Contains(hash));
            }

            var result = await query.Select(x => x.GetViewModel).ToListAsync();
            return result;
        }
        public async Task<UserViewModel?> GetElement(UserSearchModel model)
        {
            using var context = new Database();

            if (model.Id.HasValue)
            {
                var tec = await context.Users.FirstOrDefaultAsync(x => x.Id == model.Id.Value);
                return tec?.GetViewModel;
            }

            if (!string.IsNullOrEmpty(model.Username))
            {
                var tec = await context.Users.FirstOrDefaultAsync(x => x.Username.Equals(model.Username));
                return tec?.GetViewModel;
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                var tec = await context.Users.FirstOrDefaultAsync(x => x.Email.Equals(model.Email));
                return tec?.GetViewModel;
            }
            return null;
        }

        public async Task<UserViewModel?> Insert(UserViewModel model)
        {
            var user = User.Create(model);
            if (user == null)
            {
                return null;
            }
            using var context = new Database();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user.GetViewModel;
        }

        public async Task<UserViewModel?> Update(UserViewModel model)
        {
            using var context = new Database();
            var User = await context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (User == null)
            {
                return null;
            }
            User.Update(model);
            await context.SaveChangesAsync();
            return User.GetViewModel;
        }

        public async Task<UserViewModel?> Delete(UserViewModel model)
        {
            using var context = new Database();
            var User = await context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (User == null)
            {
                return null;
            }
            context.Users.Remove(User);
            await context.SaveChangesAsync();
            return User.GetViewModel;
        }
    }
}