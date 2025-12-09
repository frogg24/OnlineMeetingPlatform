using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages
{
    public class LoginModel : PageModel
    {
        [TempData]
        public string? ErrorMessage { get; set; }
        public void OnGet()
        {
            // Redirect if already logged in
            if (Request.Cookies["UserId"] != null)
            {
                Response.Redirect("/Account");
            }

            // Получаем ошибку из TempData для отображения
            if (TempData["Error"] != null)
            {
                ErrorMessage = TempData["Error"] as string;
            }
        }
    }
}