using Database.Implements;
using DataModels.Models;
using DataModels.Storages;
using DataModels.SearchModels;
using DataModels.Services;

namespace BusinessLogic
{
    public class MeetingUserLogic : IMeetingUserService
    {
        private readonly IMeetingUserStorage _meetingUserStorage;

        public MeetingUserLogic(IMeetingUserStorage meetingUserStorage)
        {
            _meetingUserStorage = meetingUserStorage;
        }

        public async Task<List<MeetingUserViewModel>?> ReadList(MeetingUserSearchModel? model)
        {
            var list = model == null
                ? await _meetingUserStorage.GetFullList()
                : await _meetingUserStorage.GetFilteredList(model);

            return list;
        }

        public async Task<MeetingUserViewModel?> ReadElement(MeetingUserSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var element = await _meetingUserStorage.GetElement(model);

            return element;
        }

        public async Task<bool> Create(MeetingUserViewModel model)
        {
            CheckModel(model);
            var result = await _meetingUserStorage.Insert(model);

            return result != null;
        }

        public async Task<bool> Update(MeetingUserViewModel model)
        {
            CheckModel(model);
            var result = await _meetingUserStorage.Update(model);

            return result != null;
        }

        public async Task<bool> Delete(MeetingUserViewModel model)
        {
            CheckModel(model, false);
            var result = await _meetingUserStorage.Delete(model);

            return result != null;
        }

        private void CheckModel(MeetingUserViewModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.UserId <= 0)
            {
                throw new ArgumentException("UserId должен быть положительным числом", nameof(model.UserId));
            }
            if (model.MeetingId <= 0)
            {
                throw new ArgumentException("MeetingId должен быть положительным числом", nameof(model.MeetingId));
            }
        }
    }
}