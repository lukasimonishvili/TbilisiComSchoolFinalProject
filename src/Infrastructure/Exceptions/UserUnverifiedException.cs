using System;

namespace Infrastructure.Exceptions
{
    public class UserUnverifiedException : Exception
    {
        public UserUnverifiedException() : base("Please confirm email to finish registration")
        {

        }
    }
}
