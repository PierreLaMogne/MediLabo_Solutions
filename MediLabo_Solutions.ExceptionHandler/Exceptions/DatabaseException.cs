using System;
using System.Collections.Generic;
using System.Text;

namespace MediLabo_Solutions.ExceptionHandler.Exceptions
{
    internal class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
