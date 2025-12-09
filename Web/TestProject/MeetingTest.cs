using BusinessLogic;
using DataModels.Models;
using DataModels.SearchModels;
using DataModels.Services;
using DataModels.Storages;
using Moq;
using TestProject;

namespace TestProject
{
    [TestClass]
    public class MeetingLogicTest
    {
        private Mock<IMeetingStorage> _mockMeetingStorage;
        private Mock<IMeetingUserService> _mockMeetingUserService;
        private MeetingLogic _meetingLogic;

        [TestInitialize]
        public void Setup()
        {
            _mockMeetingStorage = new Mock<IMeetingStorage>();
            _mockMeetingUserService = new Mock<IMeetingUserService>();
            _meetingLogic = new MeetingLogic(_mockMeetingStorage.Object, _mockMeetingUserService.Object);
        }

        [TestMethod]
        public async Task Create_ValidMeeting_CreatesSuccessfully()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Test Meeting",
                Description = "Test Description",
                Link = "https://example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            var result = await _meetingLogic.Create(meeting);

            Assert.IsTrue(result);
            _mockMeetingStorage.Verify(x => x.Insert(meeting), Times.Once);
        }

        [TestMethod]
        public async Task Create_MeetingWithNullTitle_ThrowsException()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = null,
                Description = "Test Description",
                Link = "https://example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingLogic.Create(meeting));
        }

        [TestMethod]
        public async Task Create_MeetingWithEmptyTitle_ThrowsException()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = string.Empty,
                Description = "Test Description",
                Link = "https://example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingLogic.Create(meeting));
        }

        [TestMethod]
        public async Task Create_MeetingWithNullDescription_ThrowsException()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Test Meeting",
                Description = null,
                Link = "https://example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingLogic.Create(meeting));
        }

        [TestMethod]
        public async Task Create_MeetingWithEmptyDescription_ThrowsException()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Test Meeting",
                Description = string.Empty,
                Link = "https://example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingLogic.Create(meeting));
        }

        [TestMethod]
        public async Task Create_MeetingWithNullLink_ThrowsException()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Test Meeting",
                Description = "Test Description",
                Link = null,
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingLogic.Create(meeting));
        }

        [TestMethod]
        public async Task Create_MeetingWithEmptyLink_ThrowsException()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Test Meeting",
                Description = "Test Description",
                Link = string.Empty,
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingLogic.Create(meeting));
        }

        [TestMethod]
        public async Task Create_MeetingWithDefaultDate_ThrowsException()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Test Meeting",
                Description = "Test Description",
                Link = "https://example.com",
                Date = default
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingLogic.Create(meeting));
        }

        [TestMethod]
        public async Task Create_InsertReturnsNull_ReturnsFalse()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Test Meeting",
                Description = "Test Description",
                Link = "https://example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Insert(It.IsAny<MeetingViewModel>())).ReturnsAsync((MeetingViewModel)null);

            var result = await _meetingLogic.Create(meeting);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ReadElement_ValidModel_ReturnsMeeting()
        {
            var searchModel = new MeetingSearchModel { Id = 1 };
            var meeting = new MeetingViewModel { Id = 1, Title = "Test Meeting" };

            _mockMeetingStorage.Setup(x => x.GetElement(searchModel)).ReturnsAsync(meeting);

            var result = await _meetingLogic.ReadElement(searchModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(meeting.Title, result.Title);
        }

        [TestMethod]
        public async Task ReadElement_NullModel_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _meetingLogic.ReadElement(null));
        }

        [TestMethod]
        public async Task ReadElement_MeetingNotFound_ReturnsNull()
        {
            var searchModel = new MeetingSearchModel { Id = 999 };

            _mockMeetingStorage.Setup(x => x.GetElement(searchModel)).ReturnsAsync((MeetingViewModel)null);

            var result = await _meetingLogic.ReadElement(searchModel);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Update_ValidMeeting_UpdatesSuccessfully()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Updated Meeting",
                Description = "Updated Description",
                Link = "https://updated-example.com",
                Date = DateTime.Now.AddDays(2)
            };

            _mockMeetingStorage.Setup(x => x.Update(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            var result = await _meetingLogic.Update(meeting);

            Assert.IsTrue(result);
            _mockMeetingStorage.Verify(x => x.Update(meeting), Times.Once);
        }

        [TestMethod]
        public async Task Update_UpdateReturnsNull_ReturnsFalse()
        {
            // Arrange
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "Updated Meeting",
                Description = "Updated Description",
                Link = "https://updated-example.com",
                Date = DateTime.Now.AddDays(2)
            };

            _mockMeetingStorage.Setup(x => x.Update(It.IsAny<MeetingViewModel>())).ReturnsAsync((MeetingViewModel)null);

            var result = await _meetingLogic.Update(meeting);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Delete_ValidMeeting_DeletesSuccessfully()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "To Delete",
                Description = "To Delete Description",
                Link = "https://delete-example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Delete(It.IsAny<MeetingViewModel>())).ReturnsAsync(meeting);

            var result = await _meetingLogic.Delete(meeting);

            Assert.IsTrue(result);
            _mockMeetingStorage.Verify(x => x.Delete(meeting), Times.Once);
        }

        [TestMethod]
        public async Task Delete_DeleteReturnsNull_ReturnsFalse()
        {
            var meeting = new MeetingViewModel
            {
                Id = 1,
                Title = "To Delete",
                Description = "To Delete Description",
                Link = "https://delete-example.com",
                Date = DateTime.Now.AddDays(1)
            };

            _mockMeetingStorage.Setup(x => x.Delete(It.IsAny<MeetingViewModel>())).ReturnsAsync((MeetingViewModel)null);

            var result = await _meetingLogic.Delete(meeting);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ReadList_GetFullListReturnsNull_ReturnsNull()
        {
            _mockMeetingStorage.Setup(x => x.GetFullList()).ReturnsAsync((List<MeetingViewModel>)null);

            var result = await _meetingLogic.ReadList(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ReadList_GetFullListReturnsData_ReturnsList()
        {
            var meetingList = new List<MeetingViewModel>
            {
                new MeetingViewModel { Id = 1, Title = "Meeting 1" },
                new MeetingViewModel { Id = 2, Title = "Meeting 2" }
            };

            _mockMeetingStorage.Setup(x => x.GetFullList()).ReturnsAsync(meetingList);

            var result = await _meetingLogic.ReadList(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
        
        [TestMethod]
        public async Task ReadList_GetFilteredListReturnsData_ReturnsList()
        {
            var searchModel = new MeetingSearchModel { Title = "Test Meeting" };
            var meetingList = new List<MeetingViewModel>
            {
                new MeetingViewModel { Id = 1, Title = "Test Meeting" }
            };

            _mockMeetingStorage.Setup(x => x.GetFilteredList(searchModel)).ReturnsAsync(meetingList);

            var result = await _meetingLogic.ReadList(searchModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Meeting", result[0].Title);
        }

        [TestMethod]
        public async Task ReadUserMeetings_ValidUserId_ReturnsUserMeetings()
        {
            var userId = 1;
            var userMeetings = new UserMeetings
            {
                UserMeetingsAsOrganizer = new List<MeetingViewModel>
                {
                    new MeetingViewModel { Id = 1, Title = "Organizer Meeting 1" }
                },
                UserMeetingsAsParticipant = new List<MeetingViewModel>
                {
                    new MeetingViewModel { Id = 2, Title = "Participant Meeting 1" }
                }
            };

            _mockMeetingStorage.Setup(x => x.GetUserMeetings(userId)).ReturnsAsync(userMeetings);

            var result = await _meetingLogic.ReadUserMeetings(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.UserMeetingsAsOrganizer.Count);
            Assert.AreEqual(1, result.UserMeetingsAsParticipant.Count);
            Assert.AreEqual("Organizer Meeting 1", result.UserMeetingsAsOrganizer[0].Title);
            Assert.AreEqual("Participant Meeting 1", result.UserMeetingsAsParticipant[0].Title);
        }
    }

    [TestClass]
    public class MeetingUserLogicTests
    {
        private Mock<IMeetingUserStorage> _mockMeetingUserStorage;
        private MeetingUserLogic _meetingUserLogic;

        [TestInitialize]
        public void Setup()
        {
            _mockMeetingUserStorage = new Mock<IMeetingUserStorage>();
            _meetingUserLogic = new MeetingUserLogic(_mockMeetingUserStorage.Object);
        }

        [TestMethod]
        public async Task Create_ValidMeetingUser_CreatesSuccessfully()
        {
            // Arrange
            var meetingUser = new MeetingUserViewModel
            {
                Id = 1,
                UserId = 1,
                MeetingId = 1,
                Username = "testuser",
                Email = "test@example.com",
                isNotificationOn = true
            };

            _mockMeetingUserStorage.Setup(x => x.Insert(It.IsAny<MeetingUserViewModel>())).ReturnsAsync(meetingUser);

            var result = await _meetingUserLogic.Create(meetingUser);

            Assert.IsTrue(result);
            _mockMeetingUserStorage.Verify(x => x.Insert(meetingUser), Times.Once);
        }

        [TestMethod]
        public async Task Create_MeetingUserWithInvalidUserId_ThrowsException()
        {
            var meetingUser = new MeetingUserViewModel
            {
                Id = 1,
                UserId = 0, // Невалидный ID
                MeetingId = 1,
                Username = "testuser",
                Email = "test@example.com",
                isNotificationOn = true
            };

            _mockMeetingUserStorage.Setup(x => x.Insert(It.IsAny<MeetingUserViewModel>())).ReturnsAsync(meetingUser);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingUserLogic.Create(meetingUser));
        }

        [TestMethod]
        public async Task Create_MeetingUserWithInvalidMeetingId_ThrowsException()
        {
            var meetingUser = new MeetingUserViewModel
            {
                Id = 1,
                UserId = 1,
                MeetingId = 0, // Невалидный ID
                Username = "testuser",
                Email = "test@example.com",
                isNotificationOn = true
            };

            _mockMeetingUserStorage.Setup(x => x.Insert(It.IsAny<MeetingUserViewModel>())).ReturnsAsync(meetingUser);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingUserLogic.Create(meetingUser));
        }

        [TestMethod]
        public async Task Create_MeetingUserWithNegativeUserId_ThrowsException()
        {
            var meetingUser = new MeetingUserViewModel
            {
                Id = 1,
                UserId = -1, // Невалидный ID
                MeetingId = 1,
                Username = "testuser",
                Email = "test@example.com",
                isNotificationOn = true
            };

            _mockMeetingUserStorage.Setup(x => x.Insert(It.IsAny<MeetingUserViewModel>())).ReturnsAsync(meetingUser);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingUserLogic.Create(meetingUser));
        }

        [TestMethod]
        public async Task Create_MeetingUserWithNegativeMeetingId_ThrowsException()
        {
            var meetingUser = new MeetingUserViewModel
            {
                Id = 1,
                UserId = 1,
                MeetingId = -1, // Невалидный ID
                Username = "testuser",
                Email = "test@example.com",
                isNotificationOn = true
            };

            _mockMeetingUserStorage.Setup(x => x.Insert(It.IsAny<MeetingUserViewModel>())).ReturnsAsync(meetingUser);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _meetingUserLogic.Create(meetingUser));
        }

        [TestMethod]
        public async Task ReadElement_ValidModel_ReturnsMeetingUser()
        {
            var searchModel = new MeetingUserSearchModel { UserId = 1, MeetingId = 1 };
            var meetingUser = new MeetingUserViewModel { Id = 1, UserId = 1, MeetingId = 1 };

            _mockMeetingUserStorage.Setup(x => x.GetElement(searchModel)).ReturnsAsync(meetingUser);

            var result = await _meetingUserLogic.ReadElement(searchModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(meetingUser.UserId, result.UserId);
        }

        [TestMethod]
        public async Task ReadElement_NullModel_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _meetingUserLogic.ReadElement(null));
        }

        [TestMethod]
        public async Task ReadElement_MeetingUserNotFound_ReturnsNull()
        {
            var searchModel = new MeetingUserSearchModel { UserId = 999, MeetingId = 999 };

            _mockMeetingUserStorage.Setup(x => x.GetElement(searchModel)).ReturnsAsync((MeetingUserViewModel)null);

            var result = await _meetingUserLogic.ReadElement(searchModel);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Update_ValidMeetingUser_UpdatesSuccessfully()
        {
            var meetingUser = new MeetingUserViewModel
            {
                Id = 1,
                UserId = 1,
                MeetingId = 1,
                Username = "updateduser",
                Email = "updated@example.com",
                isNotificationOn = false
            };

            _mockMeetingUserStorage.Setup(x => x.Update(It.IsAny<MeetingUserViewModel>())).ReturnsAsync(meetingUser);

            var result = await _meetingUserLogic.Update(meetingUser);

            Assert.IsTrue(result);
            _mockMeetingUserStorage.Verify(x => x.Update(meetingUser), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ValidMeetingUser_DeletesSuccessfully()
        {
            var meetingUser = new MeetingUserViewModel
            {
                Id = 1,
                UserId = 1,
                MeetingId = 1,
                Username = "todelete",
                Email = "delete@example.com",
                isNotificationOn = true
            };

            _mockMeetingUserStorage.Setup(x => x.Delete(It.IsAny<MeetingUserViewModel>())).ReturnsAsync(meetingUser);

            var result = await _meetingUserLogic.Delete(meetingUser);

            Assert.IsTrue(result);
            _mockMeetingUserStorage.Verify(x => x.Delete(meetingUser), Times.Once);
        }

        [TestMethod]
        public async Task ReadList_GetFullListReturnsNull_ReturnsNull()
        {
            _mockMeetingUserStorage.Setup(x => x.GetFullList()).ReturnsAsync((List<MeetingUserViewModel>)null);

            var result = await _meetingUserLogic.ReadList(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ReadList_GetFullListReturnsData_ReturnsList()
        {
            var meetingUserList = new List<MeetingUserViewModel>
            {
                new MeetingUserViewModel { Id = 1, UserId = 1, MeetingId = 1 },
                new MeetingUserViewModel { Id = 2, UserId = 2, MeetingId = 1 }
            };

            _mockMeetingUserStorage.Setup(x => x.GetFullList()).ReturnsAsync(meetingUserList);

            var result = await _meetingUserLogic.ReadList(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}