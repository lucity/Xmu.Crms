using System;
using System.Runtime.Serialization;

namespace Xmu.Crms.Shared.Exceptions
{
    [Serializable]
    public class SeminarNotFoundException : ArgumentOutOfRangeException
    {
        private string mes;
        public SeminarNotFoundException()
        {
            mes = "找不到该讨论课!";
        }
        public override string ToString()
        {
            return mes;
        }
        public SeminarNotFoundException(string paramName) : base(paramName)
        {
        }

        public SeminarNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SeminarNotFoundException(string paramName, string message) : base(paramName, message)
        {
        }

        public SeminarNotFoundException(string paramName, object actualValue, string message) : base(paramName, actualValue, message)
        {
        }

        protected SeminarNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}