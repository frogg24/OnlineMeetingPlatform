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
        public bool isManager { get; set; }
        public bool isParticipant { get; set; }

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

                var userIdCookie = Request.Cookies["UserId"];
                if (int.TryParse(userIdCookie, out int userId))
                {
                    isManager = Meeting.ManagerId == userId;
                }
                else
                {
                    isManager = false;
                }

                // Получаем список участников мероприятия
                MeetingParticipants = await APIClient.GetAsync<List<MeetingUserViewModel>>($"api/meeting/getlistusers?MeetingId={Id}") ?? new List<MeetingUserViewModel>();
                ParticipantCount = MeetingParticipants.Count;
                foreach ( var meeting in MeetingParticipants)
                {
                    if (meeting.UserId == userId)
                    {
                        isParticipant = true;
                        break;
                    }
                }

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
                var deleteResponse = await APIClient.DeleteAsync($"api/Meeting/RemoveUserByUserAndMeeting?userId={ParticipantId}&meetingId={MeetingId}");

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

            return RedirectToPage(new { id = MeetingId });
        }

        public async Task<IActionResult> OnPostJoinMeetingAsync()
        {
            try
            {
                MeetingUserViewModel addmodel = new MeetingUserViewModel()
                {
                    MeetingId = MeetingId,
                };

                var userIdCookie = Request.Cookies["UserId"];
                try
                {
                    addmodel.UserId = int.Parse(userIdCookie);
                }
                catch
                {
                    return RedirectToPage(new { id = MeetingId });
                }

                var addResponse = await APIClient.PostAsync<MeetingUserViewModel, dynamic>("api/meeting/AddUser", addmodel);

                if (addResponse)
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
                StatusMessage = $"Ошибка при добавлении участника: {ex.Message}";
            }

            return RedirectToPage(new { id = MeetingId });
        }

        public async Task<IActionResult> OnPostRefuseMeetingAsync()
        {
            try
            {

                var userIdCookie = Request.Cookies["UserId"];
                int userid;
                try
                {
                    userid = int.Parse(userIdCookie);
                }
                catch
                {
                    return RedirectToPage(new { id = MeetingId });
                }

                var deleteResponse = await APIClient.DeleteAsync($"api/Meeting/RemoveUserByUserAndMeeting?userId={userid}&meetingId={MeetingId}");

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

            return RedirectToPage(new { id = MeetingId });
        }
    }
}