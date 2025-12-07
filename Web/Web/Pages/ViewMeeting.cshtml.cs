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

        [BindProperty]
        public int ParticipantId { get; set; }
        [BindProperty]
        public int MeetingId { get; set; }

        public List<MeetingUserViewModel> MeetingParticipants { get; set; } = new List<MeetingUserViewModel>();
        public int ParticipantCount { get; set; }

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

                // Получаем список участников мероприятия
                MeetingParticipants = await APIClient.GetAsync<List<MeetingUserViewModel>>($"api/meeting/getlistusers?MeetingId={Id}") ?? new List<MeetingUserViewModel>();
                ParticipantCount = MeetingParticipants.Count;

                return Page();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при загрузке данных: {ex.Message}";
                return RedirectToPage("/Account");
            }
        }

        public async Task<IActionResult> OnPostRemoveParticipantAsync()
        {
            try
            {
                var deleteResponse = await APIClient.DeleteAsync($"api/meetinguser/RemoveUserByUserAndMeeting?userId={ParticipantId}&meetingId={MeetingId}");

                if (deleteResponse)
                {
                    StatusMessage = "Участник успешно удален из мероприятия";
                }
                else
                {
                    StatusMessage = "Ошибка при удалении участника";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при удалении участника: {ex.Message}";
            }

            return RedirectToPage(new { id = Meeting.Id });
        }
    }
}