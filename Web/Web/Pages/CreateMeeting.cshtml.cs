using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataModels.Models;
using System;

namespace Web.Pages
{
    public class CreateMeetingModel : PageModel
    {
        [BindProperty]
        public MeetingViewModel Meeting { get; set; } = new MeetingViewModel();

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            // Устанавливаем дату по умолчанию (через 2 дня в 19:00)
            Meeting.Date = DateTime.Now.AddDays(2).Date.AddHours(19);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"Model State Valid: {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: '{error.Key}', Errors: [{string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}]");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Получаем ID текущего пользователя из куки
                var userIdCookie = Request.Cookies["UserId"];
                if (!int.TryParse(userIdCookie, out int userId))
                {
                    StatusMessage = "Ошибка: Пользователь не авторизован";
                    return Page();
                }

                // Устанавливаем ManagerId из куки
                Meeting.ManagerId = userId;

                // Отправляем запрос на создание мероприятия
                var result = await APIClient.PostAsync<MeetingViewModel, dynamic>("api/meeting/create", Meeting);

                if (result != null)
                {
                    StatusMessage = "Мероприятие успешно создано!";
                    return RedirectToPage("/Account");
                }
                else
                {
                    StatusMessage = "Ошибка при создании мероприятия";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                return Page();
            }
        }
    }
}