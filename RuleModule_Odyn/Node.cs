using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    public enum OutType
    {
        TRUE,
        FALSE,
        NEXT,
        NEXT_LEFT,
        NEXT_RIGHT,
        END
    }
    [Serializable]
    public class PointC
    {
        public double x;
        public double y;

        public PointC(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
    }
    [Serializable]
    public abstract class Node:ICloneable
    {
        public Dictionary<OutType, string> nextNode = new Dictionary<OutType, string>();
        
        static protected readonly string endNode = "stop";
        protected readonly char note = '%';
        readonly string remark="//";
        public bool runContinue=true;
        public string NodeName { get; protected set; } = "";
        public List<string> expression;
        public List<string> remarks;
        public List<string> notes;
        public List<Tuple<string,double>> sugestions;
        public List<string> input;
        protected Dictionary<string, double> variablesNew = new Dictionary<string, double>();
        public Dictionary<string, double> variables;
        public PointC x;
        public bool freezPos = false;
        public Node()
        {
            input = null;
            expression = new List<string>();
        }

        public virtual List<string> GetLabels()
        {
            return null;
        }
        public object Clone()
        {
            Node other = (Node)this.MemberwiseClone();
            //other.variables = variables;
            return other;


        }
        public Node(Dictionary<string, double> variables) :this()
        {
            this.variables = variables;            
        }
        public Argument[] GetArguments(string exp)
        {
            Expression e = new Expression(exp);
            var k = e.getCopyOfInitialTokens();
            HashSet<string> varNames = new HashSet<string>();
            List<Argument> arg = new List<Argument>();
            for (int i = 0; i < k.Count(); i++)
                if (k[i].looksLike == "argument")
                {
                    if (k[i].tokenStr.Contains("_new"))
                    {
                        string[] aux = k[i].tokenStr.Split('_');
                        if (variablesNew.ContainsKey(aux[0]))
                        {
                            arg.Add(new Argument(k[i].tokenStr, variablesNew[aux[0]]));
                        }
                        else
                            throw new Exception("Variable " + k[i].tokenStr + " unknown");
                    }
                    else
                        lock (variables)
                        {                                                      
                            if (variablesNew.ContainsKey(k[i].tokenStr))
                            {
                                if (!varNames.Contains(k[i].tokenStr))
                                {
                                    arg.Add(new Argument(k[i].tokenStr, variablesNew[k[i].tokenStr]));
                                    varNames.Add(k[i].tokenStr);
                                }
                            }
                            else
                            if (variables.ContainsKey(k[i].tokenStr))
                            {
                                if (!varNames.Contains(k[i].tokenStr))
                                {
                                    arg.Add(new Argument(k[i].tokenStr, variables[k[i].tokenStr]));
                                    varNames.Add(k[i].tokenStr);
                                }
                            }
                            else
                                throw new Exception("Variable " + k[i].tokenStr + " unknown");
                        }
                }

            return arg.ToArray();
        }
        public string ReadNode(StreamReader r)
        {
            List<string> exp = new List<string>();            
            string line = r.ReadLine();
            line = line.Trim();
            while (line != null && !(line=="[END]"))
            {
                if (line.Length > 0 && line[0] != note && !line.Contains(remark))
                {
                    if (line.Contains(note))
                    {
                        string[] aux = line.Split(note);
                        line = aux[0];
                    }
                    if (line.Contains("LABEL"))
                    {
                        string[] aux = line.Split(new char[] { ':' },2);
                        line = aux[1];
                        NodeName = line.Trim();
                    }
                    else
                    if (line.Contains("POSITION"))
                    {
                        string[] tmp = line.Split(new char[] { ':' }, 2);
                        if (tmp.Length == 2)
                        {
                            tmp = tmp[1].Split(',');
                            string[] aux = tmp[0].Split('[');
                            int xp = Convert.ToInt32(aux[1]);
                            aux = tmp[1].Split(']');
                            int yp = Convert.ToInt32(aux[0]);
                            x = new PointC(xp, yp);
                            freezPos = true;
                        }

                    }
                    else
                        if (line.Length > 2)
                            exp.Add(line.Trim());
                }
                else
                    if (line.Length > 0 && line[0] == note)
                {
                    if (notes == null)
                        notes = new List<string>();
                    notes.Add(line);
                }
                else
                    if (line.Length > 0 && line.Contains(remark))
                {
                    if (remarks == null)
                        remarks = new List<string>();
                    line = line.Remove(0, 2);
                    remarks.Add(line);
                }
                
                line = r.ReadLine();
                line = line.Trim();
            }
            AddExpression(exp);
            
            return line;      
        }

        public abstract string Run();
        protected abstract void AddExpression(List<string> w);

    } 

}
