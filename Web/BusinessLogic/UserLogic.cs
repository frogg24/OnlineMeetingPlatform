using Database.Implements;
using DataModels.Models;
using DataModels.Storages;
using DataModels.SearchModels;
using DataModels.Services;

namespace BusinessLogic
{
    public class UserLogic: IUserService
    {   
        private readonly IUserStorage _userStorage;

        public UserLogic(IUserStorage UserStorage)
        {
            _userStorage = UserStorage;
        }

        public async Task<List<UserViewModel>?> ReadList(UserSearchModel? model)
        {
            var list = model == null
                ? await _userStorage.GetFullList()
                : await _userStorage.GetFilteredList(model);

            if (list == null)
            {
                return null;
            }

            return list;
        }

        public async Task<UserViewModel?> ReadElement(UserSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var element = await _userStorage.GetElement(model);

            if (element == null)
            {
                return null;
            }

            return element;
        }

        public async Task<bool> Create(UserViewModel model)
        {
            await CheckModelAsync(model);
            var result = await _userStorage.Insert(model);
            return result != null;
        }

        public async Task<bool> Update(UserViewModel model)
        {
            await CheckModelAsync(model);
            var result = await _userStorage.Update(model);
            return result != null;
        }

        public async Task<bool> Delete(UserViewModel model)
        {
            await CheckModelAsync(model, false);
            var result = await _userStorage.Delete(model);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        private async Task CheckModelAsync(UserViewModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(model.Username))
            {
                throw new ArgumentException("Логин не может быть пустым", nameof(model.Username));
            }
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentException("Почта не может быть пустой", nameof(model.Email));
            }
            if (string.IsNullOrWhiteSpace(model.PasswordHash))
            {
                throw new ArgumentException("Пароль не может быть пустым", nameof(model.PasswordHash));
            }

            var existingUser = await _userStorage.GetElement(new UserSearchModel { Email = model.Email });

            if (existingUser != null && existingUser.Id != model.Id &&
                string.Equals(existingUser.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Пользователь с почтой '{model.Email}' уже существует");
            }
        }
    }
}
