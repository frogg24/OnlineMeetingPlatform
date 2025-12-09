using DataModels.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Web.Models;

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
                    var userMeetingsUrl = $"api/meeting/getusermeetings?userId={userId}";
                    var userMeetingsData = await APIClient.GetAsync<UserMeetings>(userMeetingsUrl);

                    if (userMeetingsData != null)
                    {
                        UserMeetings = userMeetingsData.UserMeetingsAsOrganizer;
                        ParticipantMeetings = userMeetingsData.UserMeetingsAsParticipant;
                    }
                    else
                    {
                        UserMeetings = new List<MeetingViewModel>();
                        ParticipantMeetings = new List<MeetingViewModel>();
                        UserMeetingParticipations = new List<MeetingUserViewModel>();
                    }
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

        public async Task<IActionResult> OnPostChangeNotificationAsync()
        {
            try
            {
                var userIdCookie = Request.Cookies["UserId"];
                int userid = Convert.ToInt32(userIdCookie);

                var user = await APIClient.GetAsync<UserViewModel>($"api/User/users/{userid}");
                user.isNotificationOn = !user.isNotificationOn;

                UpdateUserRequest updUser = new UpdateUserRequest()
                {
                    isNotificationOn = user.isNotificationOn,
                };

                var response = await APIClient.PutAsync<UpdateUserRequest, dynamic>($"api/user/users/{userid}", updUser);

                Response.Cookies.Append("isNotificationOn", user.isNotificationOn.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                return Page();
            }
            return RedirectToPage();
        }
    }
}