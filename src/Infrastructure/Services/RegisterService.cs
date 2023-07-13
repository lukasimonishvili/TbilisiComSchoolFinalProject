using BCryptNet = BCrypt.Net.BCrypt;
using Domain.DTO.Authentication;
using Domain.Entities;
using Mapster;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Infrastructure.Exceptions;

namespace Infrastructure.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IEmailService _emailInterface;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<IRegisterService> _logger;
        public RegisterService(IEmailService emailInterface, IUserRepository userRepository, ILogger<IRegisterService> logger)
        {
            _emailInterface = emailInterface;
            _userRepository = userRepository;
            _logger = logger;
        }



        public async Task<string> RegisterUser(RegisterDTO registerDto, string url)
        {
            var userWithEmail = _userRepository.GetUserByEmail(registerDto.Email);
            if (userWithEmail != null)
            {
                throw new UserExistsException($"user with email {registerDto.Email} already exists");
            }

            var userWithUsername = _userRepository.GetUserByUsername(registerDto.Username);
            if (userWithUsername != null)
            {
                throw new UserExistsException($"user with username {registerDto.Username} already exists");
            }

            var emailSendUrl = url + registerDto.Email;
            var emailSent = await _emailInterface.SenEmail(registerDto.Email, registerDto.Username, emailSendUrl);

            var AdapdetUser = registerDto.Adapt<User>();
            AdapdetUser.Password = BCryptNet.HashPassword(AdapdetUser.Password);
            AdapdetUser.IsBlocked = false;
            AdapdetUser.Verified = false;
            AdapdetUser.Role = Roles.User;
            _userRepository.AddUserToDataBase(AdapdetUser);
            _logger.LogInformation($"New user with email address: {registerDto.Email} added to system");
            return emailSent;
        }

        public string ActivateUser(string Email)
        {
            var user = _userRepository.GetUserByEmail(Email);
            if (user == null)
            {
                throw new DataNotFoundException($"user with email {Email} not found in system");
            }

            if (user.Verified)
            {
                throw new UserIsAlreadyVerifiedException();
            }

            user.Verified = true;
            _userRepository.UpdateUser(user);
            return "success";
        }
    }
}
