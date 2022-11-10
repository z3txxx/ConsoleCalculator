using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    class Calculator
    {
        private Expression expression;

        public Calculator() { }

        public Calculator(Expression someExpression)
        {
            expression = someExpression;
        }

        public double Run()
        {
            return expression.Calculate();
        }
    }
}
