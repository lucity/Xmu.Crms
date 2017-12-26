using System;
using System.Runtime.Serialization;

namespace Xmu.Crms.Shared.Exceptions
{
    [Serializable]
    public class UserNotFoundException : ArgumentOutOfRangeException
    {
        private string mes;
        public UserNotFoundException()
        {
            mes = "用户名不存在!";
        }
        public override string ToString()
        {
            return mes;
        }
        public UserNotFoundException(string paramName) : base(paramName)
        {
        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserNotFoundException(string paramName, string message) : base(paramName, message)
        {
        }

        public UserNotFoundException(string paramName, object actualValue, string message) : base(paramName, actualValue, message)
        {
        }

        protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}