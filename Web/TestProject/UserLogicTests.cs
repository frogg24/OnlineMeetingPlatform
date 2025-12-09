using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BusinessLogic;
using DataModels.Models;
using DataModels.Storages;
using DataModels.SearchModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public class UserLogicTests
    {
        private Mock<IUserStorage> _mockUserStorage;
        private UserLogic _userLogic;

        [TestInitialize]
        public void Setup()
        {
            _mockUserStorage = new Mock<IUserStorage>();
            _userLogic = new UserLogic(_mockUserStorage.Object);
        }

        [TestMethod]
        public async Task Create_ValidUser_CreatesSuccessfully()
        {
            var user = new UserViewModel
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            _mockUserStorage.Setup(x => x.Insert(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            var result = await _userLogic.Create(user);

            Assert.IsTrue(result);
            _mockUserStorage.Verify(x => x.Insert(user), Times.Once);
        }

        [TestMethod]
        public async Task Create_UserWithExistingEmail_ThrowsException()
        {
            var existingUser = new UserViewModel
            {
                Id = 1,
                Email = "test@example.com"
            };

            var newUser = new UserViewModel
            {
                Id = 2,
                Username = "newuser",
                Email = "test@example.com", // Совпадает с существующим
                PasswordHash = "hashedpassword"
            };

            _mockUserStorage.Setup(x => x.GetElement(It.IsAny<UserSearchModel>())).ReturnsAsync(existingUser);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _userLogic.Create(newUser));
        }

        [TestMethod]
        public async Task Create_UserWithNullUsername_ThrowsException()
        {

            var user = new UserViewModel
            {
                Id = 1,
                Username = null,
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            _mockUserStorage.Setup(x => x.Insert(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _userLogic.Create(user));
        }

        [TestMethod]
        public async Task Create_UserWithEmptyUsermane_ThrowsException()
        {
            var user = new UserViewModel
            {
                Id = 1,
                Username = string.Empty,
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            _mockUserStorage.Setup(x => x.Insert(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _userLogic.Create(user));
        }

        [TestMethod]
        public async Task Create_UserWithNullEmail_ThrowsException()
        {

            var user = new UserViewModel
            {
                Id = 1,
                Username = "username",
                Email = null,
                PasswordHash = "hashedpassword"
            };

            _mockUserStorage.Setup(x => x.Insert(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _userLogic.Create(user));
        }

        [TestMethod]
        public async Task Create_UserWithEmptyEmail_ThrowsException()
        {
            var user = new UserViewModel
            {
                Id = 1,
                Username = "username",
                Email = string.Empty,
                PasswordHash = "hashedpassword"
            };

            _mockUserStorage.Setup(x => x.Insert(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _userLogic.Create(user));
        }

        [TestMethod]
        public async Task Create_UserWithNullPassword_ThrowsException()
        {

            var user = new UserViewModel
            {
                Id = 1,
                Username = "username",
                Email = "test@example.com",
                PasswordHash = null
            };

            _mockUserStorage.Setup(x => x.Insert(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _userLogic.Create(user));
        }

        [TestMethod]
        public async Task Create_UserWithEmptyPassword_ThrowsException()
        {
            var user = new UserViewModel
            {
                Id = 1,
                Username = "username",
                Email = "test@example.com",
                PasswordHash = string.Empty
            };

            _mockUserStorage.Setup(x => x.Insert(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _userLogic.Create(user));
        }

        [TestMethod]
        public async Task ReadElement_ValidModel_ReturnsUser()
        {
            var searchModel = new UserSearchModel { Email = "test@example.com" };
            var user = new UserViewModel { Id = 1, Email = "test@example.com" };

            _mockUserStorage.Setup(x => x.GetElement(searchModel)).ReturnsAsync(user);

            var result = await _userLogic.ReadElement(searchModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
        }

        [TestMethod]
        public async Task ReadElement_NullModel_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _userLogic.ReadElement(null));
        }

        [TestMethod]
        public async Task ReadElement_UserNotFound_ReturnsNull()
        {
            var searchModel = new UserSearchModel { Email = "nonexistent@example.com" };

            _mockUserStorage.Setup(x => x.GetElement(searchModel)).ReturnsAsync((UserViewModel)null);

            var result = await _userLogic.ReadElement(searchModel);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Update_ValidUser_UpdatesSuccessfully()
        {
            var user = new UserViewModel
            {
                Id = 1,
                Username = "updateduser",
                Email = "updated@example.com",
                PasswordHash = "newhashedpassword"
            };

            _mockUserStorage.Setup(x => x.Update(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            var result = await _userLogic.Update(user);

            Assert.IsTrue(result);
            _mockUserStorage.Verify(x => x.Update(user), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ValidUser_DeletesSuccessfully()
        {
            var user = new UserViewModel
            {
                Id = 1,
                Username = "todelete",
                Email = "delete@example.com",
                PasswordHash = "somepassword"
            };

            _mockUserStorage.Setup(x => x.Delete(It.IsAny<UserViewModel>())).ReturnsAsync(user);

            var result = await _userLogic.Delete(user);

            Assert.IsTrue(result);
            _mockUserStorage.Verify(x => x.Delete(user), Times.Once);
        }

        [TestMethod]
        public async Task ReadList_GetFullListReturnsNull_ReturnsNull()
        {
            _mockUserStorage.Setup(x => x.GetFullList()).ReturnsAsync((List<UserViewModel>)null);

            var result = await _userLogic.ReadList(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ReadList_GetFullListReturnsData_ReturnsList()
        {
            var userList = new List<UserViewModel>
            {
                new UserViewModel { Id = 1, Email = "user1@example.com" },
                new UserViewModel { Id = 2, Email = "user2@example.com" }
            };

            _mockUserStorage.Setup(x => x.GetFullList()).ReturnsAsync(userList);

            var result = await _userLogic.ReadList(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}