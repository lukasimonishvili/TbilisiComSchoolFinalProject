using System;

namespace Infrastructure.Exceptions
{
    public class UserExistsException : Exception
    {
        public UserExistsException(string Message) : base(Message)
        {

        }
    }
}
