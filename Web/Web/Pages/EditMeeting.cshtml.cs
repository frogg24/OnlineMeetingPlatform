using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataModels.Models;
using System;

namespace Web.Pages
{
    public class EditMeetingModel : PageModel
    {
        [BindProperty]
        public MeetingViewModel Meeting { get; set; } = new MeetingViewModel();

        [TempData]
        public string StatusMessage { get; set; }

        [FromRoute]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Получаем информацию о мероприятии по ID
                var meetingsList = await APIClient.GetAsync<List<MeetingViewModel>>($"api/meeting/getlist?Id={Id}") ?? new List<MeetingViewModel>();

                if (meetingsList == null || meetingsList.Count == 0)
                {
                    StatusMessage = "Мероприятие не найдено";
                    return RedirectToPage("/Account");
                }

                var meeting = meetingsList[0];

                // Проверяем, является ли текущий пользователь владельцем мероприятия
                var userIdCookie = Request.Cookies["UserId"];
                if (!int.TryParse(userIdCookie, out int userId))
                {
                    StatusMessage = "Ошибка: Пользователь не авторизован";
                    return RedirectToPage("/Account");
                }

                if (meeting.ManagerId != userId)
                {
                    StatusMessage = "У вас нет прав на редактирование этого мероприятия";
                    return RedirectToPage("/Account");
                }

                // Устанавливаем данные для отображения в форме
                Meeting = meeting;
                return Page();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при загрузке данных: {ex.Message}";
                return RedirectToPage("/Account");
            }
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
                return Page(); // Остаемся на странице с ошибками валидации
            }

            try
            {
                // Проверяем, является ли текущий пользователь владельцем мероприятия
                var userIdCookie = Request.Cookies["UserId"];
                if (!int.TryParse(userIdCookie, out int userId))
                {
                    StatusMessage = "Ошибка: Пользователь не авторизован";
                    return RedirectToPage("/Account");
                }

                // Получаем текущие данные мероприятия, чтобы проверить владельца
                // Используем тот же endpoint, что и в OnGetAsync
                var currentMeetingsList = await APIClient.GetAsync<List<MeetingViewModel>>($"api/meeting/getlist?Id={Id}");
                if (currentMeetingsList == null || currentMeetingsList.Count == 0)
                {
                    StatusMessage = "Мероприятие не найдено";
                    return RedirectToPage("/Account");
                }

                var currentMeeting = currentMeetingsList[0];

                if (currentMeeting.ManagerId != userId)
                {
                    StatusMessage = "У вас нет прав на редактирование этого мероприятия";
                    return RedirectToPage("/Account");
                }

                // Устанавливаем ID из маршрута, чтобы быть уверенным
                Meeting.Id = Id;

                // Устанавливаем ManagerId из куки
                Meeting.ManagerId = userId;

                // Форматируем дату в ISO 8601 (UTC)
                // Если дата приходит в формате DateTime, она автоматически сериализуется в ISO 8601
                // Но убедимся, что она в UTC
                Meeting.Date = Meeting.Date.ToUniversalTime();

                // Отладочный вывод перед отправкой
                Console.WriteLine($"Отправляемые данные:");
                Console.WriteLine($"ID: {Meeting.Id}");
                Console.WriteLine($"Title: {Meeting.Title}");
                Console.WriteLine($"Description: {Meeting.Description}");
                Console.WriteLine($"Link: {Meeting.Link}");
                Console.WriteLine($"Date (ISO): {Meeting.Date:o}");
                Console.WriteLine($"ManagerId: {Meeting.ManagerId}");

                // Отправляем запрос на обновление мероприятия
                var result = await APIClient.PutAsync<MeetingViewModel, dynamic>("api/meeting/update", Meeting);

                if (result != null)
                {
                    StatusMessage = "Мероприятие успешно обновлено!";
                    return RedirectToPage("/Account");
                }
                else
                {
                    StatusMessage = "Ошибка при обновлении мероприятия";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в OnPostAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                StatusMessage = $"Ошибка: {ex.Message}";
                return Page();
            }
        }
    }
}