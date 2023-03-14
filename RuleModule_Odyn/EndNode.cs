using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuleModule_Odyn
{
    public class EndNode : ComputationalNode
    {
        string restart = "";
        string conditionalRestart = "";
        List<string> reqChange = new List<string>();
        public string[] ReqChange { get { return reqChange.ToArray(); } }
        public EndNode() : base()
        {
        }
        public EndNode(Dictionary<string, double> variables) : base(variables)
        {
        }
        public override string ToString()
        {
            return "End";
        }

        public override List<string> GetLabels()
        {
            List<string> labels = new List<string>();
            if (reqChange.Count > 0)
            {
                foreach (var item in reqChange)
                    labels.Add("REQ_CHANGE: " + item);
            }
            if (conditionalRestart.Length > 0)
                labels.Add("CONDITIONAL RESTART: " + conditionalRestart);
            if (restart.Length > 0)
                labels.Add("RESTART_TIME: " + restart);

            if (labels.Count == 0)
                labels.Add("END");

            return labels;
        }
        protected override void AddExpression(List<string> expList)
        {
            string next = "";
            expression.Clear();
            foreach (var item in expList)
            {
                string tx = Regex.Replace(item, @"\s+", " ");
                string[] tmp = tx.Split(new char[] { ':' },2);
                string t = Regex.Replace(tmp[1], @"\s+", "");
                if (tmp.Length != 2)
                    throw new Exception("ERROR in EndNode:" + this.NodeName);
                if (item.Contains("RUN"))
                    expression.Add(t);

                if (item.Contains("WAIT_TIME"))
                {
                    double x;
                    double.TryParse(t, out x);
                    restart = x.ToString();
                }

                if (item.Contains("RESTART_TIME"))
                {
                    restart = t;
                    double x;
                    double.TryParse(t, out x);
                    sugestions = new List<Tuple<string, double>>();
                    sugestions.Add(new Tuple<string, double>("TIME", x));
                }
                if (item.Contains("CONDITIONAL_RESTART"))
                    conditionalRestart = tmp[1];
                if (item.Contains("REQ_CHANGE"))
                    reqChange.Add(tmp[1]);
                if (item.Contains("NEXT"))
                    next = t;

            }
            if (next.Length > 0)
                nextNode.Add(OutType.NEXT, next);
            else
                nextNode.Add(OutType.END, null);
        }
       
        public List<string> GetSchemasToRun()
        {
            List<string> l = new List<string>();
            if (expression.Count == 0)
                return null;

            l.AddRange(expression);

            return l;
        }
        public override string Run()
        {

            return null;
        }
    }

}
