using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Fraction class to represent and simplify fractions
public class Fraction
{
    public long Numerator { get; private set; }
    public long Denominator { get; private set; }

    public Fraction(long numerator, long denominator = 1)
    {
        if (denominator == 0)
            throw new ArgumentException("Denominator cannot be zero.");

        // Handle negative signs
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        // Simplify the fraction
        long gcd = GCD(Math.Abs(numerator), Math.Abs(denominator));
        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
    }

    // Arithmetic operations
    public static Fraction operator +(Fraction a, Fraction b)
    {
        return new Fraction(
            a.Numerator * b.Denominator + b.Numerator * a.Denominator,
            a.Denominator * b.Denominator
        );
    }

    public static Fraction operator -(Fraction a, Fraction b)
    {
        return new Fraction(
            a.Numerator * b.Denominator - b.Numerator * a.Denominator,
            a.Denominator * b.Denominator
        );
    }

    public static Fraction operator *(Fraction a, Fraction b)
    {
        return new Fraction(
            a.Numerator * b.Numerator,
            a.Denominator * b.Denominator
        );
    }

    public static Fraction operator /(Fraction a, Fraction b)
    {
        if (b.Numerator == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        return new Fraction(
            a.Numerator * b.Denominator,
            a.Denominator * b.Numerator
        );
    }

    public static bool operator <(Fraction a, Fraction b)
    {
        return (a.Numerator / a.Denominator) < (b.Numerator / b.Denominator);
    }

    public static bool operator >(Fraction a, Fraction b)
    {
        return (a.Numerator / a.Denominator) > (b.Numerator / b.Denominator);
    }

    public Fraction Negate()
    {
        return new Fraction(-Numerator, Denominator);
    }

    // Greatest Common Divisor (GCD) using Euclidean algorithm
    private static long GCD(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    // Convert decimal to fraction
    public static Fraction FromDecimal(double value, int maxDenominator = 1000)
    {
        bool isNegative = value < 0;
        value = Math.Abs(value);

        long numerator = (long)Math.Round(value * maxDenominator);
        long denominator = maxDenominator;

        long gcd = GCD(numerator, denominator);
        numerator /= gcd;
        denominator /= gcd;

        return isNegative ? new Fraction(-numerator, denominator) : new Fraction(numerator, denominator);
    }

    public override string ToString()
    {
        // If denominator is 1, just return the numerator
        if (Denominator == 1)
            return Numerator.ToString();

        return $"{Numerator}/{Denominator}";
    }
}

public class Node
{
    public string Value { get; set; }
    public Node Left { get; set; }
    public Node Right { get; set; }

    public Node(string value)
    {
        Value = value;
        Left = null;
        Right = null;
    }

    public bool IsOperator()
    {
        return Value == "+" || Value == "-" || Value == "*" || Value == "/";
    }
}

public class ExpressionTree
{
    public Node Root { get; set; }

    public ExpressionTree()
    {
        Root = null;
    }

    // Helper method to determine operator precedence
    private int GetPrecedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            _ => 0,
        };
    }

    // Method to build tree from infix expression
    public void BuildFromInfix(string expression)
    {
        // First handle unary minus and tokenize the expression
        expression = PreprocessExpression(expression);
        List<string> tokens = TokenizeExpression(expression);
        
        // Convert infix to postfix
        List<string> postfix = InfixToPostfix(tokens);
        
        // Build tree from postfix
        Root = BuildFromPostfix(postfix);
    }

    // Preprocess the expression to handle unary minus
    private string PreprocessExpression(string expression)
    {
        // Replace leading negative with 0-
        expression = Regex.Replace(expression, @"^-", "0-");
        
        // Replace ( with (0- for cases like (-5)
        expression = Regex.Replace(expression, @"\(-", "(0-");
        
        // Replace operators followed by minus with the operator and 0-
        expression = Regex.Replace(expression, @"([+\-*/])\s*-", "$1 0-");

        return expression;
    }

    private List<string> TokenizeExpression(string expression)
    {
        List<string> tokens = new List<string>();
        string currentNumber = "";

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];
            if (char.IsDigit(c) || (c == '.' && currentNumber.Length > 0))
            {
                currentNumber += c;
            }
            else if (c == '-' && (i == 0 || IsOperator(expression[i-1].ToString()) || expression[i-1] == '('))
            {
                // Handling negative numbers
                currentNumber += c;
            }
            else
            {
                if (currentNumber != "")
                {
                    tokens.Add(currentNumber);
                    currentNumber = "";
                }
                if (!char.IsWhiteSpace(c))
                {
                    tokens.Add(c.ToString());
                }
            }
        }
        if (currentNumber != "")
            tokens.Add(currentNumber);

        return tokens;
    }

    private List<string> InfixToPostfix(List<string> tokens)
    {
        List<string> postfix = new List<string>();
        Stack<string> operators = new Stack<string>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                postfix.Add(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    postfix.Add(operators.Pop());
                }
                if (operators.Count > 0 && operators.Peek() == "(")
                {
                    operators.Pop(); // Remove "("
                }
            }
            else // Operator
            {
                while (operators.Count > 0 && operators.Peek() != "(" &&
                       GetPrecedence(operators.Peek()) >= GetPrecedence(token))
                {
                    postfix.Add(operators.Pop());
                }
                operators.Push(token);
            }
        }

        while (operators.Count > 0)
        {
            postfix.Add(operators.Pop());
        }

        return postfix;
    }

    private Node BuildFromPostfix(List<string> postfix)
    {
        if (postfix.Count == 0) return null;

        Stack<Node> stack = new Stack<Node>();

        foreach (string token in postfix)
        {
            Node node = new Node(token);

            if (IsOperator(token))
            {
                node.Right = stack.Pop();
                node.Left = stack.Pop();
                stack.Push(node);
            }
            else
            {
                stack.Push(node);
            }
        }

        return stack.Pop();
    }

    private bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    // Method to evaluate the expression tree and return a Fraction
    public Fraction Evaluate()
    {
        return EvaluateRecursive(Root);
    }

    private Fraction EvaluateRecursive(Node node)
    {
        if (node == null)
            return new Fraction(0);

        if (!node.IsOperator())
            return Fraction.FromDecimal(double.Parse(node.Value));

        Fraction leftValue = EvaluateRecursive(node.Left);
        Fraction rightValue = EvaluateRecursive(node.Right);

        return node.Value switch
        {
            "+" => leftValue + rightValue,
            "-" => leftValue - rightValue,
            "*" => leftValue * rightValue,
            "/" => leftValue / rightValue,
            _ => throw new ArgumentException("Invalid operator")
        };
    }

    // Traversal methods
    public void InorderTraversal()
    {
        InorderTraversalRecursive(Root);
        Console.WriteLine();
    }

    private void InorderTraversalRecursive(Node node)
    {
        if (node == null)
            return;

        if (node.IsOperator())
            Console.Write("(");
            
        InorderTraversalRecursive(node.Left);
        Console.Write(node.Value);
        InorderTraversalRecursive(node.Right);

        if (node.IsOperator())
            Console.Write(")");
    }
}