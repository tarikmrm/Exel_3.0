using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Exel_3._0
{
    class ParserException : ApplicationException
    {
        public ParserException(string str) : base(str) { }
        public override string ToString()
        { return Message;
        }
    }
    public class Parser
    {
        public bool[,] control;
        private const int ABC = 26; //літер в англійському алфавіті
        enum Types { NONE, DELIMITER, VARIABLE, NUMBER }; //лексеми.
        enum Errors { SYNTAX, UNBALPARENS, NOEXP, DIVBYZERO, CYCLE }; // помилки.
        string exp; // рядок виразу
        int expIdx; // поточний індекс у виразі
        string token; // поточна лексема
        Types tokType; // тип лексеми
        int ColumnCount;
        int RowCount;
        public string[,] massFormulas;             // Масив для змінних

        public Parser()
        {

        }
        public Parser(string[,] mass, int ColumnCount_, int RowCount_)
        {
            control = new bool[ColumnCount_, RowCount_];
            massFormulas = new string[ColumnCount_, RowCount_];
            ColumnCount = ColumnCount_;
            RowCount = RowCount_;
            for (int i = 0; i < ColumnCount; i++)
            {
                for  (int j=0; j<RowCount; j++)
                {
                    control[i, j] = false;
                    if (mass[i, j] != null)
                        massFormulas[i, j] = mass[i, j];
                    else massFormulas[i, j] = "0";
                }
            }
            massFormulas= mass;
        }

        public Parser(string[,] mass, int ColumnCount_, int RowCount_, bool[,] control_)
        {
            control = new bool[ColumnCount_, RowCount_];
            massFormulas = new string[ColumnCount_, RowCount_];
            ColumnCount = ColumnCount_;
            RowCount = RowCount_;
            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    control[i, j] = control_[i, j];
                    if (mass[i, j] != null)
                        massFormulas[i, j] = mass[i, j];
                    else massFormulas[i, j] = "0";
                }
            }
            massFormulas = mass;

        }
        //Метод повертає значення true,
        // якщо с - роздільник
        bool IsDelim(char c)
        {
            if (("+-/*%^=()".IndexOf(c) != -1))
                return true;
            return false;
        }
        // отримуємо наступну лексему
        void GetToken()
        {
            tokType = Types.NONE;
            token = "";
            if (expIdx == exp.Length) return; /*кінець виразу*/
                                              // пропускаємо пробіл
            while (expIdx < exp.Length && Char.IsWhiteSpace(exp[expIdx])) ++expIdx;
            // Хвостовий пробіл 
            if (expIdx == exp.Length) return;
            if (IsDelim(exp[expIdx]))
            {
                token += exp[expIdx];
                expIdx++;
                tokType = Types.DELIMITER;
            }
            else if (Char.IsLetter(exp[expIdx]))
            {
                // Це змінна?
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length) break;
                }
               /* if (token == "div" || token == "mod")
                    tokType = Types.DELIMITER;*/
                tokType = Types.VARIABLE;
            }
            else if (Char.IsDigit(exp[expIdx]))
            {
                // Це число?
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length) break;
                }
                tokType = Types.NUMBER;
            }
        }
        void PutBack()
        {
            for (int i = 0; i < token.Length; i++) expIdx--;
        }
        /* Точка входу аналізатора. */
        public double StartEvaluate(string expstr)
        {
            double result;
            try
            {
                result = Evaluate(expstr);
                return result;
            }
            catch
            {
                return 0.0;
            }
        }
        public double Evaluate(string expstr)
        {
            double result;
            exp = expstr;
            expIdx = 0;

            GetToken();
            if (token == "")
            {
                SyntaxErr(Errors.NOEXP); // Вираз відсутній
                return 0.0;
            }
            EvalExp2(out result);
            if (token != "") /* остання лексема повинна бути нульова */
                SyntaxErr(Errors.SYNTAX);
            return result;
        }

        // Повертаємо значення змінної
        double FindVar(string vname)
        {
            double result;
            int tokencolumn = 0;
            int tokenrow = 0;
            if (!Char.IsLetter(vname[0]))
            {
                Show a = new Show();
                a.INCORRECT();                
                SyntaxErr(Errors.SYNTAX);
                return 0.0;
            }
            int i = 0;
            string letters = "";
            string numbers = "";

            while ((i < vname.Length) && (Char.IsLetter(vname[i]))) //вибираємо всі літери
            {
                letters += Char.ToUpper(vname[i]);
                i++;
            }
            while ((i < vname.Length) && (Char.IsNumber(vname[i]))) //вибираємо всі цифри
            {
                numbers += vname[i];
                i++;
            }
            if ((i < vname.Length) && (Char.IsLetter(vname[i]))) 
            {
                Show a = new Show();
                a.INCORRECT();
                SyntaxErr(Errors.SYNTAX);
                return 0.0;
            }
            if (numbers == "")
            {
                Show a = new Show();
                a.INCORRECT();
                SyntaxErr(Errors.SYNTAX);
                return 0.0;
            }
            tokencolumn = Convert.ToInt32(numbers);
            if ((tokencolumn)>ColumnCount)
            {
                Show a = new Show();
                a.INCORRECT();
                SyntaxErr(Errors.SYNTAX);
                return 0.0;
            }
            i = letters.Length;
            int j = 0;
            while (j < letters.Length)
            {
                tokenrow += (letters[j] + 1 - 'A') * (int)(Math.Pow(ABC, (i - 1)));
                i--;
                j++;
            }
            if ((tokenrow)>RowCount)
            {
                Show a = new Show();
                a.INCORRECT();
                SyntaxErr(Errors.SYNTAX);
                return 0.0;
            }
            if (control[tokencolumn - 1, tokenrow - 1])
            {
                Show b = new Show();
                b.CYCLE();
                SyntaxErr(Errors.CYCLE);
                return 0.0;
            }
            else
            {
                control[tokencolumn - 1, tokenrow - 1] = true;
                Parser a = new Parser(massFormulas, ColumnCount, RowCount, control);
                result = a.Evaluate(massFormulas[tokencolumn - 1, tokenrow - 1]);
                control[tokencolumn - 1, tokenrow - 1] = false;
                return result;
            }
        }

        // Обробка присвоювання
        void EvalExp1(out double result)
        {
            int varIdx;
            Types ttokType;
            string temptoken;
            if (tokType == Types.VARIABLE)
            {
                // зберегти стару лексему 
                temptoken = String.Copy(token);
                ttokType = tokType;
                // обчислити індекс змінної 
                varIdx = Char.ToUpper(token[0]) - 'A';
                GetToken();
                if (token != "=")
                {
                    PutBack();// повернути поточну лексему  в поток
                    //відновити стару лексему - це не присвоювання
                    token = String.Copy(temptoken);
                    tokType = ttokType;
                }
                else
                {
                    GetToken();// одержати наступну частину виразу 
                    EvalExp2(out result);
                    return;
                }
            }//if (tokType == Types.VARIABLE)
            EvalExp2(out result);
        }
        // Додавання або віднімання двох доданків
        void EvalExp2(out double result)
        {
            string op;
            double partialResult;

            EvalExp3(out result);
            while ((op = token) == "+" || op == "-")
            {
                GetToken();
                EvalExp3(out partialResult);
                switch (op)
                {
                    case "-":
                        result = result - partialResult;
                        break;
                    case "+":
                        result = result + partialResult;
                        break;
                }
            }
        }
        // Выполняем умножение или деление двух множителей.
        void EvalExp3(out double result)
        {
            string op;
            double partialResult = 0.0;
            EvalExp4(out result);
            while ((op = token) == "*" || op == "/" || op == "%")
            {
                GetToken();
                EvalExp4(out partialResult);
                switch (op)
                {
                    case "*":
                        result = result * partialResult;
                        break;
                    case "/":
                        if (partialResult == 0.0)
                        {
                            Show a = new Show();
                            a.DIVBYZERO();
                            result = 0;
                            SyntaxErr(Errors.DIVBYZERO);
                            break;
                        }
                        result = result/partialResult;
                        break;
                    case "%":
                        if (partialResult == 0.0)
                        {
                            result = 0;
                            Show b = new Show();
                            b.DIVBYZERO();
                            SyntaxErr(Errors.DIVBYZERO);
                            break;
                        }
                        result = result % partialResult;
                        break;
                }
            }
        }

        // Піднесення до степені 
        void EvalExp4(out double result)
        {
            double partialResult, ex;
            int t;
            EvalExp5(out result);
            if (token == "^")
            {
                GetToken();
                EvalExp4(out partialResult);
                ex = result;
                if (partialResult == 0.0)
                {
                    result = 1.0;
                    return;
                }
                for (t = (int)partialResult - 1; t > 0; t--)
                    result = result * (double)ex;
            }
        }

        // Множення унарних операторів + й -. 
        void EvalExp5(out double result)
        {
            string op;
            op = "";
            if ((tokType == Types.DELIMITER) && token == "+" || token == "-")
            {
                op = token;
                GetToken();
            }
            EvalExp6(out result);
            if (op == "-") result = -result;
        }
        // Обчислення виразів в дужках
        void EvalExp6(out double result)
        {
            if ((token == "("))
            {
                GetToken();
                EvalExp2(out result);
                if (token != ")")
                    SyntaxErr(Errors.UNBALPARENS);
                GetToken();
            }
            else Atom(out result);
        }
        // Одержання значення числа, або змінної 
        void Atom(out double result)
        {

            switch (tokType)
            {
                case Types.NUMBER:
                    try
                    {
                        result = Double.Parse(token);
                    }
                    catch (FormatException)
                    {
                        result = 0.0;
                        SyntaxErr(Errors.SYNTAX);
                    }
                    GetToken();
                    return;
                case Types.VARIABLE:
                    result = FindVar(token);
                    GetToken();
                    return;
                default:
                    result = 0.0;
                    SyntaxErr(Errors.SYNTAX);
                    break;
            }
        }//Atom
        void SyntaxErr(Errors e)
        {
            throw new Exception(e.ToString());
        }
    }
}