using System;
using System.Runtime.Serialization;

namespace Xmu.Crms.Shared.Exceptions
{
    [Serializable]
    public class PhoneAlreadyExistsException : ArgumentException
    {
        private string mes;
        public PhoneAlreadyExistsException()
        {
            mes = "找用户名已存在!";
        }
        public override string ToString()
        {
            return mes;
        }
        public PhoneAlreadyExistsException(string message) : base(message)
        {
        }

        public PhoneAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PhoneAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public PhoneAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PhoneAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}