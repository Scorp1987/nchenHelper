using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.Bot.Exceptions
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException() { }
        public InvalidInputException(string message) : base(message) { }
    }
}
