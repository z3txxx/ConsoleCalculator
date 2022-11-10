using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                try
                {
                    Console.WriteLine("***** Добро пожаловать в строковый калькулятор *****");
                    Console.WriteLine("Доступные операции: +, -, *, /, ^");
                    Console.WriteLine("Доступные функции: cos, sin, exp");
                    Console.WriteLine("Пример: 2+cos(2^-2)*sin(-3)");
                    Console.Write("Ваше выражение: ");

                    Lexer someExpression = new Lexer(Console.ReadLine());
                    someExpression.Run();
                    Parser RPNExpression = new Parser(someExpression.OurLexemes);
                    RPNExpression.ToRPN();
                    Calculator calc = new Calculator(RPNExpression.BuildAbstractTree());
           
                    Console.WriteLine("Результат: {0}", calc.Run());
                }
                catch
                {
                    Console.WriteLine("Ошибка!!! Попробуйте снова.");
                }

                Console.WriteLine();
                Console.WriteLine("Нажмите любую кнопку для продолжения...");
                Console.ReadKey();
            }
        }
    }
}
