using DataModels.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController:Controller
    {
        [HttpPost]
        public IActionResult Register(string username, string password, string email, string confirmPassword)
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
                ConfirmPassword = confirmPassword
            };
            APIClient.PostRequest("api/User/register", regReq);
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


                var response = APIClient.PostRequest("api/User/login", loginRequest);

                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response);

                if (loginResponse?.User == null)
                {
                    throw new Exception("Неверный ответ от сервера: информация о пользователе не получена");
                }

                string temporaryToken = loginResponse.User.Id.ToString();

                Response.Cookies.Append("AuthToken", temporaryToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                Response.Cookies.Append("UserEmail", loginResponse.User.Email, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                Response.Cookies.Append("UserId", loginResponse.User.Id.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                Response.Cookies.Append("Username", loginResponse.User.Username.ToString(), new CookieOptions
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

            APIClient.Client = null;

            return Redirect("/Login");
        }
    }
}
