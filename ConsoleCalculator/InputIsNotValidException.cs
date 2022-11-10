using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    public class InputIsNotValidException : ApplicationException
    {
        private string messageDetails = string.Empty;

        public InputIsNotValidException()
        {
            messageDetails = "Error! Incorrect expression, try again";
        }

        public InputIsNotValidException(string message)
        {
            messageDetails = message;
        }

        public override string Message => messageDetails;
    }
}
