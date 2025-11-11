using Database.Implements;
using DataModels.Models;
using DataModels.SearchModels;
using DataModels.Services;

namespace BusinessLogic
{
    public class UserLogic: IUserService
    {   
        private readonly UserStorage _UserStorage;

        public UserLogic(UserStorage UserStorage)
        {
            _UserStorage = UserStorage;
        }

        public async Task<List<UserViewModel>?> ReadList(UserSearchModel? model)
        {
            var list = model == null
                ? await _UserStorage.GetFullList()
                : await _UserStorage.GetFilteredList(model);

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

            var element = await _UserStorage.GetElement(model);

            if (element == null)
            {
                return null;
            }

            return element;
        }

        public async Task<bool> Create(UserViewModel model)
        {
            CheckModel(model);
            var result = await _UserStorage.Insert(model);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Update(UserViewModel model)
        {
            CheckModel(model);
            var result = await _UserStorage.Update(model);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Delete(UserViewModel model)
        {
            CheckModel(model, false);
            var result = await _UserStorage.Delete(model);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        private void CheckModel(UserViewModel model, bool withParams = true)
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
                throw new ArgumentException("Почта не может быть пустым", nameof(model.Email));
            }
            if (string.IsNullOrWhiteSpace(model.PasswordHash))
            {
                throw new ArgumentException("Пароль не может быть пустым", nameof(model.PasswordHash));
            }

            var existingUser = _UserStorage.GetElement(new UserSearchModel
            {
                Email = model.Email,
            }).Result;

            if (existingUser != null && existingUser.Id != model.Id &&
                string.Equals(existingUser.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Пользователь с почтой '{model.Email}' уже существует"
                );
            }
        }
    }
}
