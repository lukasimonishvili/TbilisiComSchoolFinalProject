using System;

namespace Infrastructure.Exceptions
{
    public class UserBlockedEcxeption : Exception
    {
        public UserBlockedEcxeption() : base("Your account is temporary blocked for new loan requests. please contact your accountant")
        {

        }
    }
}
