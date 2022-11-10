using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    abstract class Expression
    {
        public abstract double Calculate();
    }

    class Number : Expression
    {
        private double doubleNumber = 0;

        public Number() { }
        public Number(double num)
        {
            doubleNumber = num;
        }

        public override double Calculate()
        {
            return doubleNumber;
        }
    }

    abstract class Function : Expression
    {
        protected Expression expression;

        public Function() { }

        public Function(Expression someExp)
        {
            expression = someExp;
        }
    }

    abstract class BinaryOp : Expression
    {
        protected Expression expression1, expression2;

        public BinaryOp() { }

        public BinaryOp(Expression firstArg, Expression secondArg)
        {
            expression1 = firstArg;
            expression2 = secondArg;
        }
    }

    abstract class UnaryOp : Expression
    {
        protected Expression expression;

        public UnaryOp() { }

        public UnaryOp(Expression someExpression)
        {
            expression = someExpression;
        }
    }

    class Sin : Function
    {
        public Sin() { }

        public Sin(Expression sin) : base(sin) { }

        public override double Calculate()
        {
            return Math.Sin(expression.Calculate());
        }
    }

    class Cos : Function
    {
        public Cos() { }

        public Cos(Expression cos) : base(cos) { }

        public override double Calculate()
        {
            return Math.Cos(expression.Calculate());
        }
    }

    class Exp : Function
    {
        public Exp() { }

        public Exp(Expression exp) : base(exp) { }

        public override double Calculate()
        {
            return Math.Exp(expression.Calculate());
        }
    }

    class UnaryMinus : UnaryOp
    {
        public UnaryMinus() { }

        public UnaryMinus(Expression unMinus) : base(unMinus) { }

        public override double Calculate()
        {
            return -expression.Calculate();
        }
    }

    class UnaryPlus : UnaryOp
    {
        public UnaryPlus() { }

        public UnaryPlus(Expression unPlus) : base(unPlus) { }

        public override double Calculate()
        {
            return expression.Calculate();
        }
    }

    class Multiply : BinaryOp
    {
        public Multiply() { }

        public Multiply(Expression firstExpression, Expression secondExpression) : base(firstExpression, secondExpression) { }

        public override double Calculate()
        {
            return expression1.Calculate() * expression2.Calculate(); 
        }
    }

    class Divide : BinaryOp
    {
        public Divide() { }

        public Divide(Expression firstExpression, Expression secondExpression) : base(firstExpression, secondExpression) { }

        public override double Calculate()
        {
            return expression1.Calculate() / expression2.Calculate();
        }
    }

    class Minus : BinaryOp
    {
        public Minus() { }

        public Minus(Expression firstExpression, Expression secondExpression) : base(firstExpression, secondExpression) { }

        public override double Calculate()
        {
            return expression1.Calculate() - expression2.Calculate();
        }
    }

    class Plus : BinaryOp
    {
        public Plus() { }

        public Plus(Expression firstExpression, Expression secondExpression) : base(firstExpression, secondExpression) { }

        public override double Calculate()
        {
            return expression1.Calculate() + expression2.Calculate();
        }
    }

    class PowerOf : BinaryOp
    {
        public PowerOf() { }

        public PowerOf(Expression firstExpression, Expression secondExpression) : base(firstExpression, secondExpression) { }

        public override double Calculate()
        {
            return Math.Pow(expression1.Calculate(),  expression2.Calculate());
        }
    }
}
