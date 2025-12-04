using Database.Models;
using DataModels.Models;
using DataModels.SearchModels;
using DataModels.Services;
using DataModels.Storages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Database.Implements
{
    public class MeetingStorage : IMeetingStorage
    {
        public async Task<List<MeetingViewModel>> GetFullList()
        {
            using var context = new Database();
            return await context.Meetings.Select(x => x.GetViewModel).ToListAsync();
        }

        public async Task<List<MeetingViewModel>> GetFilteredList(MeetingSearchModel model)
        {
            using var context = new Database();

            var query = context.Meetings.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }
            if (!string.IsNullOrEmpty(model.Title))
            {
                var titleLower = model.Title.ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(titleLower));
            }
            if (!string.IsNullOrEmpty(model.Description))
            {
                var descriptionLower = model.Description.ToLower();
                query = query.Where(x => x.Description.ToLower().Contains(descriptionLower));
            }
            if (!string.IsNullOrEmpty(model.Link))
            {
                query = query.Where(x => x.Link.Contains(model.Link));
            }
            if (model.Date.HasValue)
            {
                query = query.Where(x => x.Date == model.Date.Value);
            }
            if (model.ManagerId.HasValue)
            {
                query = query.Where(x => x.ManagerId == model.ManagerId.Value);
            }

            var result = await query.Select(x => x.GetViewModel).ToListAsync();
            return result;
        }

        public async Task<MeetingViewModel?> GetElement(MeetingSearchModel model)
        {
            using var context = new Database();

            if (model.Id.HasValue)
            {
                var meeting = await context.Meetings.FirstOrDefaultAsync(x => x.Id == model.Id.Value);
                return meeting?.GetViewModel;
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                var meeting = await context.Meetings.FirstOrDefaultAsync(x => x.Title.Equals(model.Title));
                return meeting?.GetViewModel;
            }

            if (!string.IsNullOrEmpty(model.Link))
            {
                var meeting = await context.Meetings.FirstOrDefaultAsync(x => x.Link.Equals(model.Link));
                return meeting?.GetViewModel;
            }

            if (model.Date.HasValue)
            {
                var meeting = await context.Meetings.FirstOrDefaultAsync(x => x.Date == model.Date.Value);
                return meeting?.GetViewModel;
            }
            if (model.ManagerId.HasValue)
            {
                var meeting = await context.Meetings.FirstOrDefaultAsync(x => x.ManagerId == model.ManagerId.Value);
                return meeting?.GetViewModel;
            }

            return null;
        }

        public async Task<MeetingViewModel?> Insert(MeetingViewModel model)
        {
            using var context = new Database();
            var meeting = Meeting.Create(context, model);
            if (meeting == null)
            {
                return null;
            }
            await context.Meetings.AddAsync(meeting);
            await context.SaveChangesAsync();
            return meeting.GetViewModel;
        }

        public async Task<MeetingViewModel?> Update(MeetingViewModel model)
        {
            using var context = new Database();
            var meeting = await context.Meetings.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (meeting == null)    
            {
                return null;
            }
            meeting.Update(context, model);
            await context.SaveChangesAsync();
            return meeting.GetViewModel;
        }

        public async Task<MeetingViewModel?> Delete(MeetingViewModel model)
        {
            using var context = new Database();
            var meeting = await context.Meetings.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (meeting == null)
            {
                return null;
            }
            context.Meetings.Remove(meeting);
            await context.SaveChangesAsync();
            return meeting.GetViewModel;
        }
    }
}