using DataModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class HomeController:Controller
    {
        [HttpPost]
        public void Register(string username, string password, string email, string confirmPassword)
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
            Response.Redirect("/Login");
            return;
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


                APIClient.PostRequest("api/User/login", loginRequest);
                //HttpContext.Session.SetString("UserEmail", email);


                return Redirect("/Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка входа: " + ex.Message;
                return View();
            }
        }
    }
}
