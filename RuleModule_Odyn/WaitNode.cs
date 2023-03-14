using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleModule_Odyn
{
    [Serializable]
    class WaitNode:ComputationalNode
    {
        public WaitNode() : base()
        {
        }

        public WaitNode(Dictionary<string, double> variables) :base(variables)
        {
        }
        public override string ToString()
        {
            return "Wait";
        }
        public override string Run()
        {
            double sleepTime;
            lock (variables)
            {
                foreach (var item in expression)
                    CalculateExpression(item);

                sleepTime = variablesNew["Wait"];
            }
            if (sugestions == null)
                sugestions = new List<Tuple<string, double>>();
            
            sugestions.Clear();
            sugestions.Add(new Tuple<string, double>("sleepTime", variablesNew["Wait"]));
            return nextNode[OutType.NEXT];
        }
    }
}
