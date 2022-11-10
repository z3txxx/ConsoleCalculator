using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    enum TokenForParser
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
        ParensClose,
        UnMinus,
        UnPlus
    }

    struct LexemeForParser
    {
        private string element;
        private TokenForParser typeOfElement;
        public string Element
        {
            get
            {
                return element;
            }
        }

        public TokenForParser TypeOfElement
        {
            get
            {
                return typeOfElement;
            }
        }

        public LexemeForParser(string str, TokenForParser token)
        {
            element = str;
            typeOfElement = token;
        }
    }

    class Parser
    {
        private List<LexemeForParser> RPNLexemes;
        private List<LexemeForParser> normalLexemes;
        private List<Lexeme> lexemesFromLexer;

        public Expression OutputExpression { get; set; }

        public Parser() { }

        public Parser(List<Lexeme> someLexemes)
        {
            lexemesFromLexer = someLexemes;
            normalLexemes = new List<LexemeForParser>(lexemesFromLexer.Count);
        }

        private void LexemesToLexemesForParser()
        {
            for (int i = 0; i < lexemesFromLexer.Count; i++)
            {
                switch (lexemesFromLexer[i].TypeOfElement)
                {
                    case Token.Divide:
                        normalLexemes.Add(new LexemeForParser("/", TokenForParser.Divide));
                        break;
                    case Token.Multiply:
                        normalLexemes.Add(new LexemeForParser("*", TokenForParser.Multiply));
                        break;
                    case Token.PowerOf:
                        normalLexemes.Add(new LexemeForParser("/", TokenForParser.PowerOf));
                        break;
                    case Token.Number:
                        normalLexemes.Add(new LexemeForParser(lexemesFromLexer[i].Element, TokenForParser.Number));
                        break;
                    case Token.Exp:
                        normalLexemes.Add(new LexemeForParser("exp", TokenForParser.Exp));
                        break;
                    case Token.Cos:
                        normalLexemes.Add(new LexemeForParser("cos", TokenForParser.Cos));
                        break;
                    case Token.Sin:
                        normalLexemes.Add(new LexemeForParser("sin", TokenForParser.Sin));
                        break;
                    case Token.ParensOpen:
                        normalLexemes.Add(new LexemeForParser("(", TokenForParser.ParensOpen));
                        break;
                    case Token.ParensClose:
                        normalLexemes.Add(new LexemeForParser(")", TokenForParser.ParensClose));
                        break;
                    case Token.Plus:
                        if (i > 0 && (lexemesFromLexer[i - 1].TypeOfElement == Token.Number || lexemesFromLexer[i - 1].TypeOfElement == Token.ParensClose))
                            normalLexemes.Add(new LexemeForParser("+", TokenForParser.Plus));
                        else
                            normalLexemes.Add(new LexemeForParser("+", TokenForParser.UnPlus));
                        break;
                    case Token.Minus:
                        if (i > 0 && (lexemesFromLexer[i - 1].TypeOfElement == Token.Number || lexemesFromLexer[i - 1].TypeOfElement == Token.ParensClose))
                            normalLexemes.Add(new LexemeForParser("-", TokenForParser.Minus));
                        else
                            normalLexemes.Add(new LexemeForParser("-", TokenForParser.UnMinus));
                        break;
                    default:
                        throw new SomeErrorsFromParser();
                }
            }
        }

        public void ToRPN()
        {
            LexemesToLexemesForParser();
            RPNLexemes = new List<LexemeForParser>(normalLexemes.Count);
            Stack<LexemeForParser> ourStack = new Stack<LexemeForParser>();

            for (int i = 0; i < normalLexemes.Count; i++)
            {
                if (IsNumber(i))
                {
                    ValidationNumber(i);
                    RPNLexemes.Add(normalLexemes[i]);
                }
                else if (IsFunction(i))
                {
                    ValidationFunction(i);
                    ourStack.Push(normalLexemes[i]);
                }
                else if (IsOperator(i))
                {
                    if (IsBinaryOperator(i))
                    {
                        ValidationBinaryOp(i);
                        while (ourStack.Count !=0 && (IsFunction(ourStack.Peek()) || (IsOperator(ourStack.Peek()) && Priority(ourStack.Peek()) >= Priority(normalLexemes[i]))))
                        {
                            RPNLexemes.Add(ourStack.Pop());
                        }
                        ourStack.Push(normalLexemes[i]);
                    }
                    else if (IsUnaryOperator(i))
                    {
                        ValidationUnaryOp(i);
                        ourStack.Push(normalLexemes[i]);
                    }
                    else
                        throw new SomeErrorsFromParser();
                }
                else if (IsParensOpen(i))
                {
                    ValidationParensOpen(i);
                    ourStack.Push(normalLexemes[i]);
                }
                else if (IsParensClose(i))
                {
                    ValidationParensClose(i);
                    if (ourStack.Count == 0)
                        throw new SomeErrorsFromParser();
                    while(ourStack.Count != 0 && ourStack.Peek().TypeOfElement != TokenForParser.ParensOpen)
                    {
                        RPNLexemes.Add(ourStack.Pop());
                    }
                    if (ourStack.Count == 0)
                        throw new SomeErrorsFromParser();

                    ourStack.Pop();
                }
                else
                    throw new SomeErrorsFromParser();
            }
            
            while(ourStack.Count != 0)
            {
                RPNLexemes.Add(ourStack.Pop());
            }
        }

        public Expression BuildAbstractTree()
        {
            Stack<Expression> ourExpression = new Stack<Expression>();

            for(int i = 0; i < RPNLexemes.Count; i++)
            {
                if (IsNumber(RPNLexemes[i]))
                    ourExpression.Push(new Number(double.Parse(RPNLexemes[i].Element)));
                else if (IsFunction(RPNLexemes[i]))
                {
                    if (ourExpression.Count != 0)
                    {
                        if (RPNLexemes[i].TypeOfElement == TokenForParser.Cos)
                            ourExpression.Push(new Cos(ourExpression.Pop()));
                        else if (RPNLexemes[i].TypeOfElement == TokenForParser.Sin)
                            ourExpression.Push(new Sin(ourExpression.Pop()));
                        else if (RPNLexemes[i].TypeOfElement == TokenForParser.Exp)
                            ourExpression.Push(new Exp(ourExpression.Pop()));
                    }
                    else
                        throw new SomeErrorsFromParser();
                }
                else if (IsUnaryOperator(RPNLexemes[i]))
                {
                    if (ourExpression.Count != 0)
                    {
                        if (RPNLexemes[i].TypeOfElement == TokenForParser.UnMinus)
                            ourExpression.Push(new UnaryMinus(ourExpression.Pop()));
                        else if (RPNLexemes[i].TypeOfElement == TokenForParser.UnPlus)
                            ourExpression.Push(new UnaryPlus(ourExpression.Pop()));
                    }
                    else
                        throw new SomeErrorsFromParser();
                }
                else if (IsBinaryOperator(RPNLexemes[i]))
                {
                    if (ourExpression.Count >= 2)
                    {
                        if (RPNLexemes[i].TypeOfElement == TokenForParser.Plus)
                        {
                            Expression rightArg = ourExpression.Pop();
                            Expression leftArg = ourExpression.Pop();
                            ourExpression.Push(new Plus(leftArg, rightArg));
                        }
                        else if (RPNLexemes[i].TypeOfElement == TokenForParser.Minus)
                        {
                            Expression rightArg = ourExpression.Pop();
                            Expression leftArg = ourExpression.Pop();
                            ourExpression.Push(new Minus(leftArg, rightArg));
                        }
                        else if (RPNLexemes[i].TypeOfElement == TokenForParser.Multiply)
                        {
                            Expression rightArg = ourExpression.Pop();
                            Expression leftArg = ourExpression.Pop();
                            ourExpression.Push(new Multiply(leftArg, rightArg));
                        }
                        else if (RPNLexemes[i].TypeOfElement == TokenForParser.Divide)
                        {
                            Expression rightArg = ourExpression.Pop();
                            Expression leftArg = ourExpression.Pop();
                            ourExpression.Push(new Divide(leftArg, rightArg));
                        }
                        else if (RPNLexemes[i].TypeOfElement == TokenForParser.PowerOf)
                        {
                            Expression rightArg = ourExpression.Pop();
                            Expression leftArg = ourExpression.Pop();
                            ourExpression.Push(new PowerOf(leftArg, rightArg));
                        }
                    }
                    else
                        throw new SomeErrorsFromParser();
                }
                else
                    throw new SomeErrorsFromParser();
            }

            if (ourExpression.Count == 1)
                return ourExpression.Pop();
            else
                throw new SomeErrorsFromParser();
        }

        public void Display()
        {
            for(int i = 0; i < RPNLexemes.Count; i++)
                Console.WriteLine("Lexeme: {0}, Token: {1}", RPNLexemes[i].Element, RPNLexemes[i].TypeOfElement);
        }

        private bool IsNumber(int index) => normalLexemes[index].TypeOfElement == TokenForParser.Number;

        private bool IsNumber(LexemeForParser someLexeme) => someLexeme.TypeOfElement == TokenForParser.Number;

        private void ValidationNumber(int index)
        {
            if (index < normalLexemes.Count - 1 && (IsParensOpen(index + 1) || IsFunction(index + 1) || IsUnaryOperator(index + 1)))
                throw new SomeErrorsFromParser("0");
            if (index > 0 && (IsParensClose(index - 1) || IsFunction(index - 1)))
                throw new SomeErrorsFromParser("1");
        }

        private bool IsParensOpen(int index) => normalLexemes[index].TypeOfElement == TokenForParser.ParensOpen;

        private void ValidationParensOpen(int index)
        {
            if(index < normalLexemes.Count - 1 && (IsParensClose(index + 1) || IsBinaryOperator(index + 1)))
                throw new SomeErrorsFromParser("2");
            if (index > 0 && (IsNumber(index - 1) || IsParensClose(index - 1)))
                throw new SomeErrorsFromParser("3");
        }

        private bool IsParensClose(int index) => normalLexemes[index].TypeOfElement == TokenForParser.ParensClose;

        private void ValidationParensClose(int index)
        {
            if (index < normalLexemes.Count - 1 && (IsNumber(index + 1) || IsParensOpen(index + 1) || IsFunction(index + 1) || IsUnaryOperator(index + 1)))
                throw new SomeErrorsFromParser("4");
            if (index > 0 && (IsFunction(index - 1) || IsParensOpen(index - 1) || IsOperator(index - 1)))
                throw new SomeErrorsFromParser("5");
        }

        private bool IsFunction(int index) => normalLexemes[index].TypeOfElement == TokenForParser.Cos
            || normalLexemes[index].TypeOfElement == TokenForParser.Sin || normalLexemes[index].TypeOfElement == TokenForParser.Exp;

        private bool IsFunction(LexemeForParser someLexeme) => someLexeme.TypeOfElement == TokenForParser.Cos
            || someLexeme.TypeOfElement == TokenForParser.Sin || someLexeme.TypeOfElement == TokenForParser.Exp;

        private void ValidationFunction(int index)
        {
            if (index < normalLexemes.Count - 1 && !IsParensOpen(index + 1))
                throw new SomeErrorsFromParser("6");
            if (index > 0 && (IsNumber(index - 1) || IsParensClose(index - 1) || IsFunction(index - 1)))
                throw new SomeErrorsFromParser("7");
        }

        private bool IsBinaryOperator(int index) => normalLexemes[index].TypeOfElement == TokenForParser.Plus || normalLexemes[index].TypeOfElement == TokenForParser.Minus || normalLexemes[index].TypeOfElement == TokenForParser.Multiply
            || normalLexemes[index].TypeOfElement == TokenForParser.Divide || normalLexemes[index].TypeOfElement == TokenForParser.PowerOf;

        private void ValidationBinaryOp(int index)
        {
            if (index < normalLexemes.Count - 1 && (IsParensClose(index + 1) || IsBinaryOperator(index + 1)))
                throw new SomeErrorsFromParser("8");
            if (index > 0 && (IsParensOpen(index - 1) || IsFunction(index - 1) || IsOperator(index - 1)))
                throw new SomeErrorsFromParser("9");
        }

        private bool IsUnaryOperator(int index) => normalLexemes[index].TypeOfElement == TokenForParser.UnMinus || normalLexemes[index].TypeOfElement == TokenForParser.UnPlus;

        private void ValidationUnaryOp(int index)
        {
            if (index < normalLexemes.Count - 1 && (IsParensClose(index + 1) || IsBinaryOperator(index + 1) || IsUnaryOperator(index + 1)))
                throw new SomeErrorsFromParser("10");
            if (index > 0 && (IsParensClose(index - 1) || IsFunction(index - 1) || IsNumber(index - 1) || IsUnaryOperator(index - 1)))
                throw new SomeErrorsFromParser("11");
        }

        private bool IsOperator(int index) => IsBinaryOperator(index) || IsUnaryOperator(index);

        private bool IsBinaryOperator(LexemeForParser someLexeme) => someLexeme.TypeOfElement == TokenForParser.Plus || someLexeme.TypeOfElement == TokenForParser.Minus || someLexeme.TypeOfElement == TokenForParser.Multiply
            || someLexeme.TypeOfElement == TokenForParser.Divide || someLexeme.TypeOfElement == TokenForParser.PowerOf;

        private bool IsUnaryOperator(LexemeForParser someLexeme) => someLexeme.TypeOfElement == TokenForParser.UnMinus || someLexeme.TypeOfElement == TokenForParser.UnPlus;

        private bool IsOperator(LexemeForParser someLexeme) => IsBinaryOperator(someLexeme) || IsUnaryOperator(someLexeme);

        private int Priority(LexemeForParser currLexeme)
        {
            switch (currLexeme.TypeOfElement)
            {
                case TokenForParser.Plus:
                    return 1;
                case TokenForParser.Minus:
                    return 1;
                case TokenForParser.Multiply:
                    return 2;
                case TokenForParser.Divide:
                    return 2;
                case TokenForParser.UnMinus:
                    return 3;
                case TokenForParser.UnPlus:
                    return 3;
                case TokenForParser.PowerOf:
                    return 4;
                default:
                    throw new SomeErrorsFromParser();
            }
        }
    }
}
