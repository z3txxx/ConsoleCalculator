using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    enum Token
    {
        Number,
        Plus,
        Minus,
        Multiply,
        Divide,
        PowerOf,
        Sin,
        Cos,
        Exp,
        ParensOpen,
        ParensClose
    }

    struct Lexeme
    {
        private string element;
        private Token typeOfElement;
        public string Element
        {
            get
            {
                return element;
            }
        }

        public Token TypeOfElement
        {
            get
            {
                return typeOfElement;
            }
        }

        public Lexeme(string str, Token token)
        {
            element = str;
            typeOfElement = token;
        }
    }

    class Lexer
    {
        public List<Lexeme> OurLexemes { get; set; }

        private string Str { get; set; } = string.Empty;

        public Lexer() { }

        public Lexer(string str)
        {
            Str = str;
        }

        public void Run()
        {
            PrepapingOurString();

            OurLexemes = new List<Lexeme>(Str.Length);
            string digit = string.Empty;

            InputIsNotValidException e = new InputIsNotValidException();
            for (int i = 0; i < Str.Length; i++)
            {
                if (IsCos(i))
                {
                    AddToOurLexemes(Str.Substring(i, 3), Token.Cos);
                    i += 2;
                }
                else if (IsSin(i))
                {
                    AddToOurLexemes(Str.Substring(i, 3), Token.Sin);
                    i += 2;
                }
                else if (IsExp(i))
                {
                    AddToOurLexemes(Str.Substring(i, 3), Token.Exp);
                    i += 2;
                }
                else if (IsParensOpen(i))
                    AddToOurLexemes("(", Token.ParensOpen);
                else if (IsParensClose(i))
                    AddToOurLexemes(")", Token.ParensClose);
                else if (IsPlus(i))
                    AddToOurLexemes("+", Token.Plus);
                else if (IsMinus(i))
                    AddToOurLexemes("-", Token.Minus);
                else if (IsMultiply(i))
                    AddToOurLexemes("*", Token.Multiply);
                else if (IsDivide(i))
                    AddToOurLexemes("/", Token.Divide);
                else if (IsPowerOf(i))
                    AddToOurLexemes("^", Token.PowerOf);
                else if (IsNumber(i))
                {
                    int lastIndex = FindLastIndexOfNumber(i);
                    AddToOurLexemes(Str.Substring(i, (lastIndex - i)), Token.Number);
                    i = lastIndex - 1;
                }
                else
                    throw e;
            }
        }

        private void AddToOurLexemes(string str, Token token)
        {
            OurLexemes.Add(new Lexeme(str, token));
        }

        private void DeleteWhiteSpace()
        {
            for (int i = 0; i < Str.Length; i++)
            {
                if (Char.IsWhiteSpace(Str[i]))
                    Str = Str.Remove(i--, 1);
            }
        }

        private void PrepapingOurString()
        {
            DeleteWhiteSpace();
            Str = Str.Replace(".", ",");
        }

        private bool IsCos(int index) => Str[index] == 'c' && index <= Str.Length - 3 && Str[index + 1] == 'o' && Str[index + 2] == 's';

        private bool IsSin(int index) => Str[index] == 's' && index <= Str.Length - 3 && Str[index + 1] == 'i' && Str[index + 2] == 'n';

        private bool IsExp(int index) => Str[index] == 'e' && index <= Str.Length - 3 && Str[index + 1] == 'x' && Str[index + 2] == 'p';

        private bool IsNumber(int index) => FindLastIndexOfNumber(index) != -1;

        private bool IsParensOpen(int index) => Str[index] == '(';

        private bool IsParensClose(int index) => Str[index] == ')';
        
        private bool IsPlus(int index) => Str[index] == '+';
        
        private bool IsMinus(int index) => Str[index] == '-';

        private bool IsMultiply(int index) => Str[index] == '*';

        private bool IsDivide(int index) => Str[index] == '/';

        private bool IsPowerOf(int index) => Str[index] == '^';

        private int FindLastIndexOfNumber(int index)
        {
            if (!Char.IsDigit(Str[index]))
                return -1;
            int inputIndex = index;
            while (index < Str.Length && (Char.IsDigit(Str[index]) || Str[index] == ','))
            {
                index++;
            }

            double number;
            if (double.TryParse(Str.Substring(inputIndex, (index - inputIndex)), out number))
                return index;
            else
                return -1;
        }

        public void Display()
        {
            for(int i = 0; i < OurLexemes.Count; i++)
                Console.WriteLine("My lexeme: {0} and his token: {1}", OurLexemes[i].Element, OurLexemes[i].TypeOfElement);
        }
    }
}