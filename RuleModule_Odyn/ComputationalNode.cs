using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace RuleModule_Odyn
{
    [Serializable]
    public class ComputationalNode:Node,ICloneable
    {
        public List<Function> functions = new List<Function>();
        readonly Expression e = new Expression();
        //public static string label = Schema.ruleLabel+"EX";
        public ComputationalNode() : base()
        {
            CreateFunctions();
        }
        public new object Clone()
        {
            ComputationalNode other =(ComputationalNode) base.Clone();
            CreateFunctions();
            return other;
        }

        public ComputationalNode(Dictionary<string, double> variables) :base(variables)
        {            
            CreateFunctions();
        }
        public override string ToString()
        {
            return "Computational";
        }
        void CreateFunctions()
        {
            functions.Add(new Function("RBVtInit", new RBVtInitF()));
            functions.Add(new Function("RBPaO2FiO2PEEPminus", new RBPaO2FiO2PEEPminusF()));
            functions.Add(new Function("RBPaO2FiO2PEEPplus", new RBPaO2FiO2PEEPplusF()));
            functions.Add(new Function("RBPpeak", new RBPpeekF()));
            functions.Add(new Function("RBSpO2FiO2procSugeno", new RBSpO2FiO2procSugenoF()));
            functions.Add(new Function("RBSpO2PEEPSugeno", new RBSpO2PEEPSugenoF()));
            functions.Add(new Function("RBTi", new RBTiF()));
            functions.Add(new Function("RBVtminus", new RBVtminusF()));
            functions.Add(new Function("RBVtVt1kg", new RBVtVt1kgF()));
            functions.Add(new Function("RBRRTi", new RBRRTiF()));
            functions.Add(new Function("RBVt1kgVt", new RBVt1kgVF()));

            AddFunnctionToExpresssion(e);


        }
        void AddFunnctionToExpresssion(Expression e)
        {
            foreach (var item in functions)
                e.addDefinitions(item);
        }
        protected override void AddExpression(List<string> expList)
        {
            if (input == null)
                input = new List<string>();
            input.Clear();
            expression.Clear();
            foreach (var exp in expList)
            {
                string tx = Regex.Replace(exp, @"\s+", " ");
                string[] aux = tx.Split(new char[] { ':' }, 2);
                string t = Regex.Replace(aux[1], @"\s+", "");
                switch(exp)
                {
                    case var s when exp.Contains("INPUT"):
                        input.Add(t);
                        break;
                    case var s when exp.Contains("SET"):
                        expression.Add(t);
                        break;
                    case var s when exp.Contains("STOP_CONTINUE"):
                        runContinue = false;
                        break;
                    case var s when exp.Contains("NEXT"):
                        if (!t.Contains(endNode))
                            nextNode.Add(OutType.NEXT, t);
                        else
                            nextNode.Add(OutType.END, null);

                        break;

                }
            }
            if (nextNode.Count == 0)
                throw new Exception("In node "+NodeName+" Missing next node");
        }
        protected double CalculateExpression(string exp)
        {
            string[] aux;
            string name = "anonymous";
            string remExp = exp;
            if (exp.Contains("="))
            {
                aux = exp.Split('=');
                name = aux[0];
                name = Regex.Replace(name, @"\s+", "");
                exp = Regex.Replace(aux[1], @"\s+", "");
            }
            Argument[] arg = GetArguments(exp);
            e.clearExpressionString();
            e.removeAllArguments();
            e.setExpressionString(exp);
            e.addArguments(arg);
            double res = e.calculate();
            Debug.WriteLine(remExp + " " + res);
            if (variablesNew.ContainsKey(name))
                variablesNew.Remove(name);
            variablesNew.Add(name, res);
            return res;
        }
        public override string Run()
        {
            lock (variables)
            {
                foreach (var item in expression)
                    CalculateExpression(item);
            }


            foreach(var item in variablesNew)
            {
                lock(variables)
                {
                    if (variables.ContainsKey(item.Key))
                            variables[item.Key] = item.Value;
                    else
                            variables.Add(item.Key, item.Value);
                }

            }
            variablesNew.Clear();
            if(nextNode.ContainsKey(OutType.NEXT))
                return nextNode[OutType.NEXT];
            return nextNode[OutType.END] ;               
        }
        
    }
}
