using System;

namespace Infrastructure.Exceptions
{
    public class UserIsAlreadyVerifiedException : Exception
    {
        public UserIsAlreadyVerifiedException() : base("Your account is already verified")
        {

        }
    }
}
