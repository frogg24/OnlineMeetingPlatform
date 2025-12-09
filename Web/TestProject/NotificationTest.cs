using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BusinessLogic;
using DataModels.Models;
using DataModels.Storages;
using DataModels.SearchModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public class NotificationServiceTests
    {
        private Mock<IMeetingStorage> _mockMeetingStorage;
        private Mock<IMeetingUserStorage> _mockMeetingUserStorage;
        private Mock<IUserStorage> _mockUserStorage;
        private Mock<IConfiguration> _mockConfiguration;
        private NotificationService _notificationService;

        [TestInitialize]
        public void Setup()
        {
            _mockMeetingStorage = new Mock<IMeetingStorage>();
            _mockMeetingUserStorage = new Mock<IMeetingUserStorage>();
            _mockUserStorage = new Mock<IUserStorage>();
            _mockConfiguration = new Mock<IConfiguration>();
            _notificationService = new NotificationService(
                _mockMeetingStorage.Object,
                _mockMeetingUserStorage.Object,
                _mockUserStorage.Object,
                _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task SendNotificationAsync_WithMissingEmailSettings_ReturnsFalse()
        {
            var email = "test@example.com";
            var subject = "Test Subject";
            var message = "Test Message";

            _mockConfiguration.Setup(x => x["Email:SmtpServer"]).Returns((string)null);

            var result = await _notificationService.SendNotificationAsync(email, subject, message);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CheckAndSendMeetingNotificationsAsync_WithUpcomingMeetings_SendsNotifications()
        {
            var now = DateTime.Now;
            var meetingTime = now.AddHours(1).AddMinutes(30);

            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Upcoming Meeting",
                Description = "Test Description",
                Link = "https://example.com",
                Date = meetingTime
            };

            var user = new MeetingUserViewModel
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                isNotificationOn = true
            };

            var meetings = new List<MeetingViewModel> { meeting };
            var users = new List<MeetingUserViewModel> { user };

            _mockMeetingStorage.Setup(x => x.GetFullList()).ReturnsAsync(meetings);
            _mockMeetingUserStorage.Setup(x => x.GetFilteredList(It.Is<MeetingUserSearchModel>(m => m.MeetingId == meeting.Id))).ReturnsAsync(users);

            await _notificationService.CheckAndSendMeetingNotificationsAsync();

            _mockMeetingStorage.Verify(x => x.GetFullList(), Times.Once);
            _mockMeetingUserStorage.Verify(x => x.GetFilteredList(It.Is<MeetingUserSearchModel>(m => m.MeetingId == meeting.Id)), Times.Once);
        }

        [TestMethod]
        public async Task CheckAndSendMeetingNotificationsAsync_WithNoMeetings_DoesNotSendNotifications()
        {
            var meetings = new List<MeetingViewModel>();

            _mockMeetingStorage.Setup(x => x.GetFullList()).ReturnsAsync(meetings);

            await _notificationService.CheckAndSendMeetingNotificationsAsync();

            _mockMeetingStorage.Verify(x => x.GetFullList(), Times.Once);
            _mockMeetingUserStorage.Verify(x => x.GetFilteredList(It.IsAny<MeetingUserSearchModel>()), Times.Never);
        }

        [TestMethod]
        public async Task CheckAndSendMeetingNotificationsAsync_WithNoUpcomingMeetings_DoesNotSendNotifications()
        {
            var now = DateTime.Now;
            var meetingTime = now.AddHours(3);

            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Future Meeting",
                Description = "Test Description",
                Link = "https://example.com",
                Date = meetingTime
            };

            var meetings = new List<MeetingViewModel> { meeting };

            _mockMeetingStorage.Setup(x => x.GetFullList()).ReturnsAsync(meetings);

            await _notificationService.CheckAndSendMeetingNotificationsAsync();

            _mockMeetingStorage.Verify(x => x.GetFullList(), Times.Once);
            _mockMeetingUserStorage.Verify(x => x.GetFilteredList(It.IsAny<MeetingUserSearchModel>()), Times.Never);
        }

        [TestMethod]
        public async Task CheckAndSendMeetingNotificationsAsync_WithNotificationOff_DoesNotSendNotification()
        {
            var now = DateTime.Now;
            var meetingTime = now.AddHours(1).AddMinutes(30);

            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Upcoming Meeting",
                Description = "Test Description",
                Link = "https://example.com",
                Date = meetingTime
            };

            var user = new MeetingUserViewModel
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                isNotificationOn = false
            };

            var meetings = new List<MeetingViewModel> { meeting };
            var users = new List<MeetingUserViewModel> { user };

            _mockMeetingStorage.Setup(x => x.GetFullList()).ReturnsAsync(meetings);
            _mockMeetingUserStorage.Setup(x => x.GetFilteredList(It.Is<MeetingUserSearchModel>(m => m.MeetingId == meeting.Id))).ReturnsAsync(users);

            await _notificationService.CheckAndSendMeetingNotificationsAsync();

            _mockMeetingStorage.Verify(x => x.GetFullList(), Times.Once);
            _mockMeetingUserStorage.Verify(x => x.GetFilteredList(It.Is<MeetingUserSearchModel>(m => m.MeetingId == meeting.Id)), Times.Once);
        }
    }
}