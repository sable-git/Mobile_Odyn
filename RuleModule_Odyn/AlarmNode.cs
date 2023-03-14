using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RuleModule_Odyn
{
    [Serializable]
    public class AlarmNode : ComputationalNode
    {

        public AlarmNode() : base()
        {
        }
        public AlarmNode(Dictionary<string, double> variables) : base(variables)
        {            
        }
        public override string ToString()
        {
            return "Alarm";
        }
        protected override void AddExpression(List<string> expList)
        {
            expression.Clear();
            foreach (var item in expList)
            {
                if (item.Contains("ALARM:"))
                {
                    string[] tmp = item.Split(':');
                    if (tmp.Length != 2)
                        throw new Exception("Alarm incorrect format :" + this.NodeName);
                    expression.Add(tmp[1]);
                }
                if (item.Contains("NEXT:"))
                {
                    string[] tmp = item.Split(':');
                    if (tmp.Length != 2)
                        throw new Exception("NEXT incorrect format :" + this.NodeName);
                    string t = Regex.Replace(tmp[1], @"\s+", "");
                    nextNode.Add(OutType.NEXT, t);
                }

            }

        }
        public override string Run()
        {
            return null;
        }
    }
}
