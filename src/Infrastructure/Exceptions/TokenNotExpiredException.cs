using System;

namespace Infrastructure.Exceptions
{
    public class TokenNotExpiredException : Exception
    {
        public TokenNotExpiredException() : base("Token is not expired.")
        {

        }
    }
}
