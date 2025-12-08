using DataModels.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string email, string confirmPassword, bool isNotificationOn)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(confirmPassword))
            {
                throw new Exception("Введите логин, пароль и почту");
            }
            if (!string.Equals(confirmPassword, password))
            {
                throw new Exception("Пароли не совпадают");
            }

            var regReq = new
            {
                Username = username,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword,
                isNotificationOn = isNotificationOn
            };
            await APIClient.PostAsync<object, dynamic>("api/User/register", regReq);
            return Redirect("/Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    throw new Exception("Введите логин и пароль");
                }

                var loginRequest = new
                {
                    Email = email,
                    Password = password
                };

                var response = await APIClient.PostAsync<object, LoginResponse>("api/User/login", loginRequest);

                if (response?.User == null)
                {
                    throw new Exception("Неверный ответ от сервера: информация о пользователе не получена");
                }

                string temporaryToken = response.User.Id.ToString();

                Response.Cookies.Append("AuthToken", temporaryToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                Response.Cookies.Append("UserEmail", response.User.Email, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                Response.Cookies.Append("UserId", response.User.Id.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                Response.Cookies.Append("Username", response.User.Username.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                return Redirect("/Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка входа: " + ex.Message;
                return View();
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("UserEmail");
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("Username");

            APIClient.CurrentUser = null;

            return Redirect("/Login");
        }
    }
}