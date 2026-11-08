using System;
using System.Globalization;

namespace Nuton {
  public class FormulaEvaluator {
    private const char decimalSeparator = '.';
    private const char openBracket = '(';
    private const char closeBracket = ')';
    private const char plusSign = '+';
    private const char minusSign = '-';
    private const char multiplySign = '*';
    private const char divideSign = '/';
    private const char powerSign = '^';
    private const char variableName = 'x';

    private readonly string formula;
    private string text;
    private int position;
    private double variableValue;

    public FormulaEvaluator(string formula) {
      if (string.IsNullOrWhiteSpace(formula)) {
        throw new ArgumentException("Формула не должна быть пустой.");
      }

      this.formula = formula;
      text = string.Empty;
    }

    public double Evaluate(double xValue) {
      text = formula.ToLower().Replace(" ", string.Empty);
      position = 0;
      variableValue = xValue;

      double result;
      result = ParseExpression();

      if (position < text.Length) {
        throw new FormatException("Не удалось разобрать формулу около символа '" + text[position] + "'.");
      }

      if (double.IsNaN(result) || double.IsInfinity(result)) {
        throw new ArithmeticException("Функция не определена в точке x = " +
            xValue.ToString("F6", CultureInfo.InvariantCulture) + ".");
      }

      return result;
    }

    private double ParseExpression() {
      double value;
      value = ParseTerm();

      while (position < text.Length) {
        char symbol;
        symbol = text[position];

        if (symbol == plusSign) {
          position++;
          value = value + ParseTerm();
        } else if (symbol == minusSign) {
          position++;
          value = value - ParseTerm();
        } else {
          break;
        }
      }

      return value;
    }

    private double ParseTerm() {
      double value;
      value = ParsePower();

      while (position < text.Length) {
        char symbol;
        symbol = text[position];

        if (symbol == multiplySign) {
          position++;
          value = value * ParsePower();
        } else if (symbol == divideSign) {
          position++;
          value = value / ParsePower();
        } else {
          break;
        }
      }

      return value;
    }

    private double ParsePower() {
      double value;
      value = ParseUnary();

      if (position < text.Length && text[position] == powerSign) {
        position++;
        double exponent;
        exponent = ParsePower();
        value = Math.Pow(value, exponent);
      }

      return value;
    }

    private double ParseUnary() {
      if (position < text.Length) {
        char symbol;
        symbol = text[position];

        if (symbol == minusSign) {
          position++;
          double value;
          value = ParseUnary();
          return -value;
        }

        if (symbol == plusSign) {
          position++;
          return ParseUnary();
        }
      }

      return ParsePrimary();
    }

    private double ParsePrimary() {
      if (position >= text.Length) {
        throw new FormatException("Формула завершилась неожиданно.");
      }

      char symbol;
      symbol = text[position];

      if (symbol == openBracket) {
        position++;
        double value;
        value = ParseExpression();
        Expect(closeBracket);
        return value;
      }

      if (char.IsDigit(symbol) || symbol == decimalSeparator) {
        return ParseNumber();
      }

      if (symbol == variableName) {
        position++;
        return variableValue;
      }

      if (char.IsLetter(symbol)) {
        return ParseFunction();
      }

      throw new FormatException("Недопустимый символ в формуле: '" + symbol + "'.");
    }

    private double ParseNumber() {
      int start;
      start = position;

      while (position < text.Length) {
        char symbol;
        symbol = text[position];

        if (char.IsDigit(symbol) || symbol == decimalSeparator) {
          position++;
        } else {
          break;
        }
      }

      string numberText;
      numberText = text.Substring(start, position - start);

      double value;
      if (!double.TryParse(numberText, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
        throw new FormatException("Не удалось прочитать число '" + numberText + "'.");
      }

      return value;
    }

    private double ParseFunction() {
      int start;
      start = position;

      while (position < text.Length && char.IsLetter(text[position])) {
        position++;
      }

      string name;
      name = text.Substring(start, position - start);

      Expect(openBracket);
      double argument;
      argument = ParseExpression();
      Expect(closeBracket);

      switch (name) {
        case "sin":
          return Math.Sin(argument);
        case "cos":
          return Math.Cos(argument);
        case "tg":
          return Math.Tan(argument);
        case "tan":
          return Math.Tan(argument);
        case "exp":
          return Math.Exp(argument);
        case "ln":
          return Math.Log(argument);
        case "log":
          return Math.Log10(argument);
        case "sqrt":
          return Math.Sqrt(argument);
        case "abs":
          return Math.Abs(argument);
        default:
          throw new FormatException("Неизвестная функция: " + name);
      }
    }

    private void Expect(char expected) {
      if (position >= text.Length || text[position] != expected) {
        throw new FormatException("Ожидался символ '" + expected + "'.");
      }

      position++;
    }
  }
}