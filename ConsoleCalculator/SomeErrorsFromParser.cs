using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    class SomeErrorsFromParser : ApplicationException
    {
        private string messageDetails = string.Empty;

        public SomeErrorsFromParser()
        {
            messageDetails = "Error! Parsing error.";
        }

        public SomeErrorsFromParser(string message)
        {
            messageDetails = message;
        }

        public override string Message => messageDetails;
    }
}
