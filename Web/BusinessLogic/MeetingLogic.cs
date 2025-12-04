using Database.Implements;
using DataModels.Models;
using DataModels.Storages;
using DataModels.SearchModels;
using DataModels.Services;

namespace BusinessLogic
{
    public class MeetingLogic : IMeetingService
    {
        private readonly IMeetingStorage _meetingStorage;

        public MeetingLogic(IMeetingStorage meetingStorage)
        {
            _meetingStorage = meetingStorage;
        }

        public async Task<List<MeetingViewModel>?> ReadList(MeetingSearchModel? model)
        {
            var list = model == null
                ? await _meetingStorage.GetFullList()
                : await _meetingStorage.GetFilteredList(model);

            if (list == null)
            {
                return null;
            }

            return list;
        }

        public async Task<MeetingViewModel?> ReadElement(MeetingSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var element = await _meetingStorage.GetElement(model);

            if (element == null)
            {
                return null;
            }

            return element;
        }

        public async Task<bool> Create(MeetingViewModel model)
        {
            CheckModel(model);
            var result = await _meetingStorage.Insert(model);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Update(MeetingViewModel model)
        {
            CheckModel(model);
            var result = await _meetingStorage.Update(model);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Delete(MeetingViewModel model)
        {
            CheckModel(model, false);
            var result = await _meetingStorage.Delete(model);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        private void CheckModel(MeetingViewModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ArgumentException("Название встречи не может быть пустым", nameof(model.Title));
            }
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                throw new ArgumentException("Описание не может быть пустым", nameof(model.Description));
            }
            if (string.IsNullOrWhiteSpace(model.Link))
            {
                throw new ArgumentException("Ссылка не может быть пустой", nameof(model.Link));
            }
            if (model.Date == default)
            {
                throw new ArgumentException("Дата должна быть указана", nameof(model.Date));
            }
        }
    }
}