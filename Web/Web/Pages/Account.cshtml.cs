using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataModels.Models;
using System.Collections.Generic;
using System.Linq;

namespace Web.Pages
{
    public class AccountModel : PageModel
    {
        public List<MeetingViewModel> UserMeetings { get; set; } = new();
        public List<MeetingViewModel> ParticipantMeetings { get; set; } = new();
        public List<MeetingUserViewModel> UserMeetingParticipations { get; set; } = new();
        public bool UserIsNotificationOn { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet()
        {
            var userIdCookie = Request.Cookies["UserId"];
            UserIsNotificationOn = Convert.ToBoolean(Request.Cookies["isNotificationOn"]);

            if (int.TryParse(userIdCookie, out int userId))
            {
                try
                {
                    //TODO: оптимизировать этот ужас

                    // Получаем мероприятия, где пользователь является организатором
                    var userMeetingsUrl = $"api/meeting/getlist?managerId={userId}";
                    UserMeetings = await APIClient.GetAsync<List<MeetingViewModel>>(userMeetingsUrl) ?? new List<MeetingViewModel>();

                    // Получаем мероприятия, в которых участвует пользователь
                    var participantMeetingsUrl = $"api/meeting/getlistusers?userId={userId}";
                    UserMeetingParticipations = await APIClient.GetAsync<List<MeetingUserViewModel>>(participantMeetingsUrl) ?? new List<MeetingUserViewModel>();

                    // Для каждого участия получаем полную информацию о мероприятии
                    foreach (var meetingUser in UserMeetingParticipations)
                    {
                        var meetingUrl = $"api/meeting/getlist?id={meetingUser.MeetingId}";
                        var meetings = await APIClient.GetAsync<List<MeetingViewModel>>(meetingUrl);
                        if (meetings != null && meetings.Any())
                        {
                            ParticipantMeetings.AddRange(meetings);
                        }
                    }

                    // Убираем дубликаты мероприятий
                    ParticipantMeetings = ParticipantMeetings
                        .GroupBy(m => m.Id)
                        .Select(g => g.First())
                        .ToList();
                }
                catch (System.Exception ex)
                {
                    StatusMessage = $"Ошибка при загрузке данных: {ex.Message}";
                    UserMeetings = new List<MeetingViewModel>();
                    ParticipantMeetings = new List<MeetingViewModel>();
                    UserMeetingParticipations = new List<MeetingUserViewModel>();
                }
            }
            else
            {
                StatusMessage = "Пользователь не авторизован. Пожалуйста, войдите в систему.";
            }
        }
    }
}