using Domain.Entities;
using Domain.Interface;
using Domain.DTO.Authentication;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Infrastructure.Exceptions;

namespace UnitTests
{
    [TestClass]
    public class RegisterServiceTest
    {
        private Mock<IEmailService> _mockEmailInterface = new Mock<IEmailService>();
        private Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
        private Mock<ILogger<IRegisterService>> _mockLogger = new Mock<ILogger<IRegisterService>>();

        [TestMethod]
        public async Task RegisterUser_Success()
        {
            _mockUserRepository.Setup(mock => mock.GetUserByEmail(It.IsAny<string>())).Returns((User)null);
            _mockUserRepository.Setup(mock => mock.GetUserByUsername(It.IsAny<string>())).Returns((User)null);
            _mockUserRepository.Setup(mock => mock.AddUserToDataBase(It.IsAny<User>())).Returns(new User());
            _mockEmailInterface.Setup(mock => mock.SenEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("success");

            var registerService = new RegisterService(_mockEmailInterface.Object, _mockUserRepository.Object, _mockLogger.Object);
            var result = await registerService.RegisterUser(
                new RegisterDTO()
                {
                    Username = "testuser",
                    Email = "test@gmail.com",
                    Password = "password",
                },
                "https://randomurl.com"
            );

            Assert.AreEqual("success", result);
        }

        [TestMethod]
        [ExpectedException(typeof(UserExistsException))]
        public async Task RegisterUser_MailConflict()
        {
            _mockUserRepository.Setup(mock => mock.GetUserByEmail(It.IsAny<string>())).Returns(new User() { Email = "test@gmail.com" });
            _mockUserRepository.Setup(mock => mock.GetUserByUsername(It.IsAny<string>())).Returns((User)null);
            _mockUserRepository.Setup(mock => mock.AddUserToDataBase(It.IsAny<User>())).Returns(new User());
            _mockEmailInterface.Setup(mock => mock.SenEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("success");

            var registerService = new RegisterService(_mockEmailInterface.Object, _mockUserRepository.Object, _mockLogger.Object);
            await registerService.RegisterUser(
                new RegisterDTO()
                {
                    Username = "testuser",
                    Email = "test@gmail.com",
                    Password = "password",
                },
                "https://randomurl.com"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UserExistsException))]
        public async Task RegisterUser_UsernameConflict()
        {
            _mockUserRepository.Setup(mock => mock.GetUserByEmail(It.IsAny<string>())).Returns((User)null);
            _mockUserRepository.Setup(mock => mock.GetUserByUsername(It.IsAny<string>())).Returns(new User() { Username = "testuser" });
            _mockUserRepository.Setup(mock => mock.AddUserToDataBase(It.IsAny<User>())).Returns(new User());
            _mockEmailInterface.Setup(mock => mock.SenEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("success");

            var registerService = new RegisterService(_mockEmailInterface.Object, _mockUserRepository.Object, _mockLogger.Object);
            await registerService.RegisterUser(
                new RegisterDTO()
                {
                    Username = "testuser",
                    Email = "test@gmail.com",
                    Password = "password",
                },
                "https://randomurl.com"
            );
        }

        [TestMethod]
        public void ActivateUser_Success()
        {
            var registerService = new RegisterService(_mockEmailInterface.Object, _mockUserRepository.Object, _mockLogger.Object);
            _mockUserRepository.Setup(mock => mock.GetUserByEmail(It.IsAny<string>())).Returns(new User() { Email = "test@gmail.com" });
            _mockUserRepository.Setup(mock => mock.UpdateUser(It.IsAny<User>())).Returns("success");

            var result = registerService.ActivateUser("test@gmail.com");

            Assert.AreEqual("success", result);
        }

        [TestMethod]
        [ExpectedException(typeof(DataNotFoundException))]
        public void ActivateUser_NotFound()
        {
            var registerService = new RegisterService(_mockEmailInterface.Object, _mockUserRepository.Object, _mockLogger.Object);
            _mockUserRepository.Setup(mock => mock.GetUserByEmail(It.IsAny<string>())).Returns((User)null);
            _mockUserRepository.Setup(mock => mock.UpdateUser(It.IsAny<User>())).Returns("success");

            registerService.ActivateUser("test@gmail.com");
        }

        [TestMethod]
        [ExpectedException(typeof(UserIsAlreadyVerifiedException))]
        public void ActivateUser_AlreadyVerified()
        {
            var registerService = new RegisterService(_mockEmailInterface.Object, _mockUserRepository.Object, _mockLogger.Object);
            _mockUserRepository.Setup(mock => mock.GetUserByEmail(It.IsAny<string>())).Returns(new User() { Email = "test@gmail.com", Verified = true });
            _mockUserRepository.Setup(mock => mock.UpdateUser(It.IsAny<User>())).Returns("success");

            registerService.ActivateUser("test@gmail.com");
        }
    }
}
