using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Utilidades para los operadores y logica especial.
/// </summary>
public class LogicalLineUtils
{
    public static readonly string STRING_ID = "\"";
    public static class Encapsulation
    {
        private const char ENCAPSULATION_START = '{';
        private const char ENCAPSULATION_END = '}';

        public struct EncapsulationData
        {
            public bool IsNull => lines == null;
            public List<string> lines;
            public int startingIndex;
            //Le dice a la conversación donde debe continuar.
            public int endingIndex;
        }

        public static EncapsulationData RipEncapsulationData(Conversation conv, int startingIndex, bool ripHeaderAndEncapsulators = false, int parentStartingIndex=0)
        {
            int encapsulationDepth = 0;

            EncapsulationData data = new EncapsulationData { lines = new List<string>(), startingIndex = (startingIndex + parentStartingIndex), endingIndex = 0 };
                
            for (int i = startingIndex; i < conv.Count; i++)
            {
                string line = conv.GetLines()[i];

                if(ripHeaderAndEncapsulators || encapsulationDepth > 0 && !IsEncapsulationEnd(line))
                    data.lines.Add(line);

                if (IsEncapsulationStart(line))
                {
                    encapsulationDepth++;
                    continue;
                }

                if (IsEncapsulationEnd(line))
                {
                    encapsulationDepth--;
                    if (encapsulationDepth == 0)
                    {
                        data.endingIndex = (i + parentStartingIndex);
                        break;
                    }

                }
            }

            return data;
        }

        public static bool IsEncapsulationStart(string line) => line.Trim().StartsWith(ENCAPSULATION_START);
        public static bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(ENCAPSULATION_END);
    }
    public static class Expressions
    {
        public static HashSet<string> OPERATORS = new HashSet<string>() { "-", "+", "/", "-=", "+=", "*=", "/=", "*", "=" };
        public static readonly string REGEX_ARITHMETIC = @"([-+*/=]=?)";
        //Variables que empiezan son $ y son precedidas por += Variable por ejemplo.
        public static readonly string REGEX_OPERATOR_LINE = @"^\$\w+\s*(=|\+=|-=|\*=|/=|)\s*";
        public static readonly string REGEX_VARIABLE_IDS = @"[!]?\$[a-zA-Z0-9_.]+";

        public static readonly char NEGATED_ID = '!';
        public static readonly char VARIABLE_ID = '$';
        //public static readonly char STRING_ID = '\"';
        public static object CalculateValue(string[] expressionParts)
        {
            List<string> operandStrings = new List<string>();
            //Suma y resta, etc.
            List<string> operatorStrings = new List<string>();
            //Nombre variable por ejemplo.
            List<object> operands = new List<object>();

            for (int i = 0; i < expressionParts.Length; i++)
            {
                string part = expressionParts[i].Trim();
                if (part == string.Empty)
                    continue;

                if (OPERATORS.Contains(part))
                    operatorStrings.Add(part);
                else
                    operandStrings.Add(part);
            }

            foreach (string operandStr in operandStrings)
            {
                operands.Add(ExtractValue(operandStr));
            }

            //PAPOMUDAS para la gente.
            CalculateValue_DivAndMul(operatorStrings, operands);
            CalculateValue_AddAndSub(operatorStrings, operands);

            return operands[0];
        }

        private static void CalculateValue_AddAndSub(List<string> operatorStrings, List<object> operands)
        {
            for (int i = 0; i < operatorStrings.Count; i++)
            {
                string opStr = operatorStrings[i];
                if (opStr == "+" || opStr == "-")
                {
                    //OPERANDOS...
                    double leftOp = Convert.ToDouble(operands[i]);
                    double rightOp = Convert.ToDouble(operands[i + 1]);

                    if (opStr == "+")
                        operands[i] = leftOp + rightOp;
                    else
                        operands[i] = leftOp - rightOp;

                    operands.RemoveAt(i + 1);
                    operatorStrings.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void CalculateValue_DivAndMul(List<string> operatorStrings, List<object> operands)
        {
            for (int i = 0; i < operatorStrings.Count; i++)
            {
                string opStr = operatorStrings[i];
                if (opStr == "*" || opStr == "/")
                {
                    //OPERANDOS...
                    double leftOp = Convert.ToDouble(operands[i]);
                    double rightOp = Convert.ToDouble(operands[i + 1]);

                    if (opStr == "*")
                    {
                        operands[i] = leftOp * rightOp;
                    }
                    else
                    {
                        if (rightOp == 0)
                        {
                            Debug.LogError("Cannot divide by zero.");
                            return;
                        }

                        operands[i] = leftOp / rightOp;
                    }

                    operands.RemoveAt(i + 1);
                    operatorStrings.RemoveAt(i);
                    i--;
                }
            }
        }

        //Extrae variables y sus valores.
        private static object ExtractValue(string value)
        {
            bool negate = false;

            if (value.StartsWith(NEGATED_ID))
            {
                negate = true;
                value = value.Substring(1);
            }

            if (value.StartsWith(VARIABLE_ID))
            {
                string variableName = value.TrimStart(VARIABLE_ID);
                if (!VariableStore.HasVariable(variableName))
                {
                    Debug.LogError($"Variable {variableName} no existe.");
                    return null;
                }

                VariableStore.TryGetValue(variableName, out object val);
                if (val is bool boolVal && negate)
                    return !boolVal;

                return val;
            }
            else if (value.StartsWith(STRING_ID) && value.EndsWith(STRING_ID))
            {
                value = TagController.Inject(value, true, true);
                return value.Trim('"');
            }
            else
            {
                if (int.TryParse(value, out int intVal))
                    return intVal;
                else if (float.TryParse(value, out float floatVal))
                    return floatVal;
                else if (bool.TryParse(value, out bool boolVal))
                    return negate ? !boolVal : boolVal;
                else
                {
                    value = TagController.Inject(value, true, true);
                    return value;
                }
            }
        }
    }
    public static class Conditions
    {
        private static readonly string AND_OPERATOR = "and";
        private static readonly string OR_OPERATOR = "or";
        public static readonly string REGEX_CONDITIONAL_OP = $@"(==|!=|<=|=>|<|>|{AND_OPERATOR}|{OR_OPERATOR})"; //\|\|)";
        public static bool EvaluateCondition(string condition)
        {
            condition = TagController.Inject(condition, true, true);
            string[] parts = Regex.Split(condition, REGEX_CONDITIONAL_OP).
                Select(p => p.Trim()).ToArray();

            for(int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith(STRING_ID) && parts[i].EndsWith(STRING_ID))
                    parts[i] = parts[i].Substring(1, parts[i].Length - 2);
            }

            if (parts.Length == 1)
                if (bool.TryParse(parts[0], out bool boolVal))
                    return boolVal;
                else
                {
                    Debug.LogError($"No se pudo parsear la condicion {condition}");
                    return false;
                }
            else if (parts.Length == 3) {
                return EvaluateExpression(parts[0], parts[1], parts[2]);
            }

            Debug.LogError($"Condicion no soportada por el juego {condition}");
            return false;
        }

        //Callback para poder raelizar operaciones booleanas dependiendo del tipo de dato.
        private delegate bool OperatorFunc<T>(T left, T right);
        //Para booleanos.
        private static Dictionary<string, OperatorFunc<bool>> boolOperators = new Dictionary<string, OperatorFunc<bool>>() {
            {AND_OPERATOR, (left, right) => left && right },
            {OR_OPERATOR, (left, right) => left || right},
            {"==", (left, right) => left == right },
            {"!=", (left, right) => left != right },
        };

        private static Dictionary<string, OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>() {
            {"==", (left, right) => left == right },
            {"!=", (left, right) => left != right },
            {"<=", (left, right) => left <= right },
            {">=", (left, right) => left >= right },
            {"<", (left, right) => left < right },
            {">", (left, right) => left > right },
        };

        private static Dictionary<string, OperatorFunc<int>> intOperators = new Dictionary<string, OperatorFunc<int>>() {
            {"==", (left, right) => left == right },
            {"!=", (left, right) => left != right },
            {"<=", (left, right) => left <= right },
            {">=", (left, right) => left >= right },
            {"<", (left, right) => left < right },
            {">", (left, right) => left > right },
        };

        /// <summary>
        /// Evaluates an expression based on float, bool, or integers.
        /// Left and Right should be the same type!!!!
        /// </summary>
        /// <param name="left">The left side of the condition.</param>
        /// <param name="op">The operation.</param>
        /// <param name="right">The right side of the condition.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static bool EvaluateExpression(string left, string op, string right)
        {
            if (bool.TryParse(left, out bool leftBool) && bool.TryParse(right, out bool rightBool))
                return boolOperators[op](leftBool, rightBool);

            if (float.TryParse(left, out float leftFloat) && float.TryParse(right, out float rightFloat))
                return floatOperators[op](leftFloat, rightFloat);

            if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
                return intOperators[op](leftInt, rightInt);

            switch (op)
            {
                case "==":
                    return left == right;
                case "!=": 
                    return left != right;
                default:
                    throw new InvalidOperationException($"Operacion no soportada {op}");
            }
        }
    }
}
