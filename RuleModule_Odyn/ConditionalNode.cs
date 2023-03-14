using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    [Serializable]
    public class ConditionalNode : Node
    {        
        Argument[] arg = null;
        public ConditionalNode() : base()
        {
        }
        public ConditionalNode(Dictionary<string, double> variables) : base(variables)
        {
        }
        public override string ToString()
        {
            return "Conditional";
        }
        protected override void AddExpression(List<string> expList)
        {
            expression.Clear();
            for (int i = 0; i < expList.Count; i++)
            {
                string exp = expList[i];
                if (exp.Contains("if"))
                {
                    string[] sep = new string[] { "if " };
                    string[] aux = exp.Split(sep, StringSplitOptions.None);
                    string t = aux[1];
                    if (!aux[1].Contains("then"))
                    {
                        throw new Exception("There is no 'then' in rule definition " + NodeName);
                    }
                    sep = new string[] { "then" };
                    aux = t.Split(sep, StringSplitOptions.None);
                    expression.Add(aux[0]);
                    t = Regex.Replace(aux[1], @"\s+", "");
                    nextNode.Add(OutType.TRUE, t);
                }
                if (exp.Contains("else") && !exp.Contains("null"))
                {
                    string t = Regex.Replace(exp, @"\s+", " ");
                    t = t.Trim();
                    string[] aux = t.Split(' ');
                    if (aux.Length == 3)
                    {
                        nextNode.Add(OutType.FALSE, aux[2]);
                    }
                    else
                        throw new Exception("After else label is missing : nodeName=" + NodeName);
                }
            }
        }
        public override string Run()
        {
            arg = GetArguments(expression[0]);
            expression[0] = expression[0].Replace("or", "||");
            expression[0] = expression[0].Replace("and", "&&");

            Expression e = new Expression(expression[0], arg);

            if (e.calculate() == 1)
            {
                return nextNode[OutType.TRUE];
            }
            return nextNode[OutType.FALSE];
        }
    }
}