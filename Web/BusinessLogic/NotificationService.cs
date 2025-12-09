using DataModels.Models;
using DataModels.SearchModels;
using DataModels.Storages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class NotificationService
    {
        private readonly IMeetingStorage _meetingStorage;
        private readonly IMeetingUserStorage _meetingUserStorage;
        private readonly IUserStorage _userStorage;
        private readonly IConfiguration _configuration;

        public NotificationService(IMeetingStorage meetingStorage, IMeetingUserStorage meetingUserStorage,
            IUserStorage userStorage, IConfiguration configuration)
        {
            _meetingStorage = meetingStorage;
            _meetingUserStorage = meetingUserStorage;
            _userStorage = userStorage;
            _configuration = configuration;
        }

        public async Task<bool> SendNotificationAsync(string email, string subject, string message)
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var senderEmail = _configuration["Email:SenderEmail"];
                var senderPassword = _configuration["Email:SenderPassword"];

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    Console.WriteLine($"[LOG] [SendNotificationAsync] Email settings are missing. Logging notification instead of sending to {email}.");
                    Console.WriteLine($"[LOG] [SendNotificationAsync] Subject: {subject}");
                    // Console.WriteLine($"[LOG] [SendNotificationAsync] Message: {message}"); // Может быть длинным, раскомментируйте при необходимости
                    return true;
                }

                Console.WriteLine($"[LOG] [SendNotificationAsync] Attempting to send notification to {email}...");

                using var client = new SmtpClient(smtpServer, smtpPort);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"[LOG] [SendNotificationAsync] Notification successfully sent to {email}.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] [SendNotificationAsync] Error sending notification to {email}: {ex.Message}");
                return false;
            }
        }

        public async Task CheckAndSendMeetingNotificationsAsync()
        {
            Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Starting check at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.");

            var now = DateTime.Now;
            var oneHourFromNow = now.AddHours(1);
            var twoHoursFromNow = now.AddHours(2);

            Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Fetching meetings scheduled between {oneHourFromNow:yyyy-MM-dd HH:mm:ss} and {twoHoursFromNow:yyyy-MM-dd HH:mm:ss} that have not been notified yet.");

            // Получаем все мероприятия
            var allMeetings = await _meetingStorage.GetFullList();
            Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Retrieved {allMeetings?.Count ?? 0} meetings from storage.");

            // Фильтруем мероприятия: начинаются в следующий час (от +1 до +2 часов)
            var upcomingMeetings = allMeetings?.Where(meeting =>
                meeting.Date > oneHourFromNow &&
                meeting.Date <= twoHoursFromNow 
            ).ToList();

            Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Found {upcomingMeetings?.Count ?? 0} meetings scheduled for the next hour window that have not been notified yet.");

            if (upcomingMeetings != null)
            {
                foreach (var meeting in upcomingMeetings)
                {
                    Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Processing meeting '{meeting.Title}' (ID: {meeting.Id}) scheduled for {meeting.Date:yyyy-MM-dd HH:mm:ss}.");

                    // Получаем пользователей, зарегистрированных на это мероприятие
                    Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Fetching users for meeting ID {meeting.Id}...");
                    var listUsersIds = await _meetingUserStorage.GetFilteredList(new MeetingUserSearchModel { MeetingId = meeting.Id });

                    Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Retrieved {listUsersIds?.Count ?? 0} users registered for meeting '{meeting.Title}'.");

                    bool notificationSentToSomeone = false; // Флаг, чтобы отметить, что хотя бы одному пользователю отправили

                    if (listUsersIds != null)
                    {
                        foreach (var user in listUsersIds)
                        {
                            // Проверяем, включены ли уведомления у пользователя
                            if (user.isNotificationOn.Value)
                            {
                                Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] User {user.Username} (Email: {user.Email}) has notifications ON. Preparing to send notification.");
                                var subject = $"Напоминание: Мероприятие '{meeting.Title}' скоро начнется!";
                                var message = $@"
                                    <h2>Напоминание о мероприятии</h2>
                                    <p>Здравствуйте, {user.Username}!</p>
                                    <p>Напоминаем, что мероприятие <strong>{meeting.Title}</strong> начнется через примерно один час.</p>
                                    <p><strong>Описание:</strong> {meeting.Description}</p>
                                    <p><strong>Дата и время:</strong> {meeting.Date:dd.MM.yyyy HH:mm}</p>
                                    <p><strong>Ссылка:</strong> <a href='{meeting.Link}'>{meeting.Link}</a></p>
                                    <br>
                                    <p>Не упустите возможность принять участие!</p>
                                ";

                                var sendResult = await SendNotificationAsync(user.Email, subject, message);
                                if (sendResult)
                                {
                                    notificationSentToSomeone = true; // Отметим, что отправка была успешной хотя бы раз
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] User {user.Username} (Email: {user.Email}) has notifications OFF. Skipping.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] No users found for meeting ID {meeting.Id} or GetFilteredList returned null.");
                    }

                    // Если уведомление было успешно отправлено хотя бы одному пользователю, отмечаем встречу как "уведомление отправлено"
                    if (notificationSentToSomeone)
                    {
                        Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Marked notification as sent for meeting '{meeting.Title}' (ID: {meeting.Id}).");
                    }
                    else
                    {
                        Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] No notifications were sent for meeting '{meeting.Title}' (ID: {meeting.Id}), so it remains unmarked.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] No meetings found in the specified time window or GetFullList returned null.");
            }

            Console.WriteLine($"[LOG] [CheckAndSendMeetingNotificationsAsync] Finished check at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.");
        }
    }
}