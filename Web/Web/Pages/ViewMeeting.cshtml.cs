using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataModels.Models;
using System;

namespace Web.Pages
{
    public class ViewMeetingModel : PageModel
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

                // Устанавливаем данные для отображения
                Meeting = meetingsList[0];
                return Page();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при загрузке данных: {ex.Message}";
                return RedirectToPage("/Account");
            }
        }
    }
}