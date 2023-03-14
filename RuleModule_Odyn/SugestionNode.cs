using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleModule_Odyn
{
    [Serializable]
    public class SugestionNode:ComputationalNode
    {
        public SugestionNode() : base()
        {
        }

        public SugestionNode(Dictionary<string, double> variables) : base(variables)
        {
        }
        public override string ToString()
        {
            return "Sugestion";
        }
        public override string Run()
        {
            variablesNew.Clear();
            string res = base.Run();
            
            sugestions = new List<Tuple<string, double>>();
            foreach(var item in expression)
            {
                string key = item;
                if(item.Contains("="))
                {
                    string[] aux = item.Split('=');
                    key = aux[0];
                }
                lock (variables)
                {
                    sugestions.Add(new Tuple<string, double>(key,variables[key]));
                }
            }
            return res;
        }
    }
}
