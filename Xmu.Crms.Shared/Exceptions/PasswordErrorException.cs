using System;
using System.Runtime.Serialization;

namespace Xmu.Crms.Shared.Exceptions
{
    [Serializable]
    public class PasswordErrorException : ArgumentException
    {
        private string mes;
        public PasswordErrorException()
        {
            mes = "密码错误!";
        }
        public override string ToString()
        {
            return mes;
        }
        public PasswordErrorException(string message) : base(message)
        {
        }

        public PasswordErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PasswordErrorException(string message, string paramName) : base(message, paramName)
        {
        }

        public PasswordErrorException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PasswordErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}