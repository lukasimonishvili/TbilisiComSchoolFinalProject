using System;

namespace Infrastructure.Exceptions
{
    public class WrongCredentialsException : Exception
    {
        public WrongCredentialsException() : base("Wron username or password")
        {

        }
    }
}
