using System;
using System.Collections.Generic;
using System.Linq;


namespace ConsoleApp1
{
    class Program
    {
        static string[] signs = { "+", "-", "*", "/", "^", "ln", "sin", "cos", "log" };
        static void Main()
        {
            float output;
            string function;
            function = Console.ReadLine();
            while (function != "")
            {
                output = SolveParenthesis(function);
                function = Console.ReadLine();
            }
            //output tiene la respuesta
        }
        static float SolveFunction(string function)
        {
            float aux;
            List<string>[] terms = new List<string>[signs.Length];
            GetTerms(function, ref terms);
            aux = SolveTerms(terms);
            return aux;
        }
        static void GetTerms(string function, ref List<string>[] terms)
        {
            int i, lastModified = 0;

            terms = new List<string>[signs.Length];
            if (function[0] == '+')
                function = function.Substring(1);

            foreach (string sign in signs)
                function = function.Replace(sign + "-", sign + "s");
            if (function[0] == '-')
                function = "s" + function.Substring(1);

            for (i = 0; i < terms.Length; i++)
                terms[i] = new List<string>();
            for (i = 0; i < signs.Length; i++)
            {
                switch (signs[i])
                {
                    case "+":
                        foreach (string str in function.Split(new string[] { signs[0] }, 0))
                            terms[0].Add(str);
                        break;
                    default:
                        if (function.Contains(signs[i]))
                        {
                            foreach (string arrList in terms[lastModified])
                                foreach (string str in arrList.Split(new string[] { signs[i] }, 0))
                                    if (str != "")
                                        terms[i].Add(str);
                            lastModified = i;
                        }
                        break;

                }

            }
            for (i = 0; i < terms.Length; i++)
                for (int j = 0; j < terms[i].Count; j++)
                    terms[i][j] = terms[i][j].Replace('s', '-');
        }
        static float SolveTerms(List<string>[] terms)
        {
            int i, lastModified, j;
            string[] arg, signs = { "+", "-", "*", "/", "^", "ln", "sin", "cos", "log" };
            int[] amountArguments = { 1, 2, 2, 2, 2, 1, 1, 1, 2 };
            float buffer = 0;
            float[] operators = new float[2];

            for (i = signs.Length - 1; i >= 0; i--)
            {
                if (terms[i].Count == 0)
                    continue;
                if (i != 0)
                {
                    for (lastModified = i - 1; lastModified > 0; lastModified--)
                        if (terms[lastModified].Count == 0)
                            continue;
                        else
                            break;
                    j = 0;
                    bool found;
                    while (!terms[i].SequenceEqual(terms[lastModified]))
                    {
                        if (terms[lastModified][j] != terms[i][j])
                        {
                            found = false;
                            arg = new string[] { terms[i][j], terms[i][j + amountArguments[i] - 1] };
                            operators[0] = float.Parse(arg[0]);
                            operators[1] = float.Parse(arg[1]);
                            switch (signs[i])
                            {
                                case "-":
                                    buffer = operators[0] - operators[1];
                                    break;
                                case "*":
                                    buffer = operators[0] * operators[1];
                                    break;
                                case "/":
                                    buffer = operators[0] / operators[1];
                                    break;
                                case "^":
                                    buffer = (float)Math.Pow(operators[0], operators[1]);
                                    break;
                                case "ln":
                                    buffer = (float)Math.Log(operators[0]);
                                    break;
                                case "sin":
                                    buffer = (float)Math.Sin(operators[0]);
                                    break;
                                case "cos":
                                    buffer = (float)Math.Cos(operators[0]);
                                    break;
                                case "log":
                                    buffer = (float)Math.Log(operators[1]) / (float)Math.Log(operators[0]);
                                    break;
                            }
                            for (int k = 0; k <= lastModified; k++)
                            {
                                for (int m = 0; m < terms[k].Count; m++)
                                    if (terms[k][m].Contains(arg[0] + signs[i] + arg[1]) || terms[k][m].Contains(signs[i] + arg[0]))
                                    {
                                        if (amountArguments[i] == 2)
                                            terms[k][m] = ReplaceFirst(terms[k][m], arg[0] + signs[i] + arg[1], buffer.ToString());
                                        else
                                            terms[k][m] = ReplaceFirst(terms[k][m], signs[i] + arg[0], buffer.ToString());
                                        found = true;
                                        break;
                                    }
                                if (!found)
                                    break;
                            }
                            terms[i][j] = buffer.ToString();
                            if (amountArguments[i] != 1)
                                terms[i].RemoveAt(j + 1);
                        }
                        else
                            j++;
                    }
                }
                else
                {
                    buffer = 0;
                    foreach (string str in terms[0])
                        buffer += float.Parse(str);
                }
            }//Se resuelven los terms
            return buffer;
        }
        static float SolveParenthesis(string function)//Tiene que tener número igual de paréntesis abiertos que cerrados.
        {
            int aux;
            List<int> OParenthesis = new List<int>(), CParenthesis = new List<int>();
            string subFunction;

            function = function.Replace('.', ',');
            function = function.Replace(" ", "");
            function = function.Replace(':', '/');
            function = function.Replace("E+", "*10^");
            function = function.Replace("E", "*10^");
            for (int j = 0; j < function.Length; j++)
                if (function[j] == '(')
                    OParenthesis.Add(j);
                else
                    if (function[j] == ')')
                    CParenthesis.Add(j);
            if (OParenthesis.Count != CParenthesis.Count)
                return 0;
            while (OParenthesis.Count != 0)
            {
                subFunction = "";
                aux = OParenthesis.Count - 1;
                while (OParenthesis[aux] > CParenthesis[0])
                    aux--;
                for (int j = OParenthesis[aux] + 1; j < CParenthesis[0]; j++)
                    subFunction += function[j];
                function = function.Replace('(' + subFunction + ')', SolveFunction(subFunction).ToString());
                OParenthesis.Clear();
                CParenthesis.Clear();
                for (int j = 0; j < function.Length; j++)
                    if (function[j] == '(')
                        OParenthesis.Add(j);
                    else
                        if (function[j] == ')')
                        CParenthesis.Add(j);
            }
            return SolveFunction(function);
        }
        static public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}