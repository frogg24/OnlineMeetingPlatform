using BusinessLogic;
using DataModels.Models;
using DataModels.SearchModels;
using DataModels.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest(new { message = "Пароли не совпадают" });
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var userModel = new UserViewModel
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    isNotificationOn = request.isNotificationOn
                };

                var result = await _userService.Create(userModel);

                if (result)
                {
                    return Ok(new { message = "Регистрация успешна" });
                }

                return BadRequest(new { message = "Ошибка при регистрации" });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message + ex.InnerException?.Message + ex.InnerException?.StackTrace });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var searchModel = new UserSearchModel { Email = request.Email };
                var user = await _userService.ReadElement(searchModel);

                if (user == null)
                {
                    return Unauthorized(new { message = "Пользователь не найден" });
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Неверный пароль" });
                }

                return Ok(new
                {
                    message = "Вход выполнен успешно",
                    user = new { user.Id, user.Username, user.Email, user.isNotificationOn }
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.ReadList(null);

                // Не возвращаем пароли
                var safeUsers = users?.Select(u => new {
                    u.Id,
                    u.Username,
                    u.Email
                }).ToList();

                return Ok(safeUsers);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var searchModel = new UserSearchModel { Id = id };
                var user = await _userService.ReadElement(searchModel);

                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                // Не возвращаем пароль
                var safeUser = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.isNotificationOn
                };

                return Ok(safeUser);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                // Получаем текущего пользователя
                var searchModel = new UserSearchModel { Id = id };
                var existingUser = await _userService.ReadElement(searchModel);

                if (existingUser == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                // Обновляем данные
                existingUser.Username = request.Username ?? existingUser.Username;
                existingUser.Email = request.Email ?? existingUser.Email;
                if (request.isNotificationOn.HasValue)
                {
                    existingUser.isNotificationOn = request.isNotificationOn.Value;
                }


                // Если пришел новый пароль - хешируем его
                if (!string.IsNullOrEmpty(request.NewPassword))
                {
                    if (string.IsNullOrEmpty(request.CurrentPassword))
                    {
                        return BadRequest(new { message = "Для смены пароля требуется текущий пароль" });
                    }

                    // Проверяем текущий пароль
                    if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, existingUser.PasswordHash))
                    {
                        return Unauthorized(new { message = "Текущий пароль неверен" });
                    }

                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                }

                var result = await _userService.Update(existingUser);

                if (result)
                {
                    return Ok(new { message = "Данные пользователя обновлены" });
                }

                return BadRequest(new { message = "Ошибка при обновлении" });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var searchModel = new UserSearchModel { Id = id };
                var user = await _userService.ReadElement(searchModel);

                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                var result = await _userService.Delete(user);

                if (result)
                {
                    return Ok(new { message = "Пользователь удален" });
                }

                return BadRequest(new { message = "Ошибка при удалении" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string email = null, [FromQuery] string username = null)
        {
            try
            {
                var searchModel = new UserSearchModel
                {
                    Email = email,
                    Username = username
                };

                var users = await _userService.ReadList(searchModel);

                // Не возвращаем пароли
                var safeUsers = users?.Select(u => new {
                    u.Id,
                    u.Username,
                    u.Email
                }).ToList();

                return Ok(safeUsers);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }
    }

    public class UpdateUserRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public bool? isNotificationOn { get; set; }

    }
}