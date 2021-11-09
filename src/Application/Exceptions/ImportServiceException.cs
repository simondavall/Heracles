using System;

namespace Heracles.Application.Exceptions
{
    public class ImportServiceException : Exception
    {
        public ImportServiceException()
        {
        }

        public ImportServiceException(string message)
            : base(message)
        {
        }

        public ImportServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
