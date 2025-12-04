using Database.Models;
using DataModels.Models;
using DataModels.SearchModels;
using DataModels.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Implements
{
    public class MeetingUserStorage: IMeetingUserStorage
    {
        public async Task<List<MeetingUserViewModel>> GetFullList()
        {
            using var context = new Database();
            var result = await context.MeetingUsers
                .Include(d => d.Meeting)
                .Include(t => t.User)
                .Select(x => x.GetViewModel)
                .ToListAsync();
            return result;
        }

        public async Task<List<MeetingUserViewModel>> GetFilteredList(MeetingUserSearchModel model)
        {
            using var context = new Database();

            var query = context.MeetingUsers
                .Include(d => d.Meeting)
                .Include(t => t.User)
                .AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }
            else
            {
                if (model.MeetingId.HasValue)
                {
                    query = query.Where(x => x.MeetingId == model.MeetingId.Value);
                }
                if (model.UserId.HasValue)
                {
                    query = query.Where(x => x.UserId == model.UserId.Value);
                }
            }

            var result = await query.Select(x => x.GetViewModel).ToListAsync();
            return result;
        }

        public async Task<MeetingUserViewModel?> GetElement(MeetingUserSearchModel model)
        {
            using var context = new Database();

            // Поиск по Id (высший приоритет)
            if (model.Id.HasValue)
            {
                var lesgr = await context.MeetingUsers
                    .Include(d => d.Meeting)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(x => x.Id == model.Id.Value);

                return lesgr?.GetViewModel;
            }

            // Поиск по связке MeetingId + UserId (если оба указаны)
            if (model.MeetingId.HasValue && model.UserId.HasValue)
            {
                var lesgr = await context.MeetingUsers
                    .Include(d => d.Meeting)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(x =>
                        x.MeetingId == model.MeetingId.Value &&
                        x.UserId == model.UserId.Value);

                return lesgr?.GetViewModel;
            }

            // Поиск только по MeetingId
            if (model.MeetingId.HasValue)
            {
                var lesgr = await context.MeetingUsers
                    .Include(d => d.Meeting)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(x => x.MeetingId == model.MeetingId.Value);

                return lesgr?.GetViewModel;
            }

            // Поиск только по UserId
            if (model.UserId.HasValue)
            {
                var lesgr = await context.MeetingUsers
                    .Include(d => d.Meeting)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(x => x.UserId == model.UserId.Value);

                return lesgr?.GetViewModel;
            }

            return null;
        }

        public async Task<MeetingUserViewModel?> Insert(MeetingUserViewModel model)
        {
            using var context = new Database();

            var newLesUser = MeetingUser.Create(context, model);
            if (newLesUser == null)
            {
                return null;
            }

            await context.MeetingUsers.AddAsync(newLesUser);
            await context.SaveChangesAsync();
            return newLesUser.GetViewModel;
        }

        public async Task<MeetingUserViewModel?> Update(MeetingUserViewModel model)
        {
            using var context = new Database();
            var lesUser = await context.MeetingUsers
                .Include(d => d.Meeting)
                .Include(t => t.User)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (lesUser == null)
            {
                return null;
            }

            lesUser.Update(context, model);
            await context.SaveChangesAsync();
            return lesUser.GetViewModel;
        }

        public async Task<MeetingUserViewModel?> Delete(MeetingUserViewModel model)
        {
            using var context = new Database();
            var lesUser = await context.MeetingUsers
                .Include(d => d.Meeting)
                .Include(t => t.User)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (lesUser != null)
            {
                context.MeetingUsers.Remove(lesUser);
                await context.SaveChangesAsync();
                return lesUser.GetViewModel;
            }

            return null;
        }
    }
}
