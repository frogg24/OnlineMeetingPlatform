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
            try
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
                    Email = email.ToLower(),
                    Password = password,
                    ConfirmPassword = confirmPassword,
                    isNotificationOn = isNotificationOn
                };
                await APIClient.PostAsync<object, dynamic>("api/User/register", regReq);
                return Redirect("/Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка регистрации: {ex.Message}";
                return Redirect("/Register");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    TempData["Error"] = "Введите email и пароль";
                    return Redirect("/Login");
                }

                var loginRequest = new
                {
                    Email = email.ToLower(),
                    Password = password
                };

                var response = await APIClient.PostAsync<object, LoginResponse>("api/User/login", loginRequest);

                if (response?.User == null)
                {
                    TempData["Error"] = "Неверный email или пароль";
                    return Redirect("/Login");
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

                Response.Cookies.Append("isNotificationOn", response.User.isNotificationOn.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                return Redirect("/Account");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка входа: " + ex.Message;
                return Redirect("/Login");
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("UserEmail");
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("isNotificationOn");

            APIClient.CurrentUser = null;

            return Redirect("/Login");
        }
    }
}