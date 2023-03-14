using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace RuleModule_Odyn
{
    public enum CheckResult
    {
        NEXT,
        YES,
        NO
    }
   
    public class RuleModule:ICloneable
    {              
        Dictionary<string,Schema> rules = new Dictionary<string, Schema>();
        Dictionary<string,Schema> alarms = new Dictionary<string,Schema>();

        public ConcurrentQueue<InputDataObject> sugestions;
        public ConcurrentQueue<InputDataObject> inputData;
        public ConcurrentQueue<InputDataObject> alarmMessages;

        private string activeSchema;

        public string ActiveSchema { get { return activeSchema; }
            set
            {
                activeSchema = value;
                if (rules.ContainsKey(activeSchema))
                {
                    sugestions = rules[activeSchema].sugestions;
                    inputData = rules[activeSchema].inputData;
                    alarmMessages = rules[activeSchema].alarmMessages;
                }

            } }

        readonly List<Thread> alarmThread = new List<Thread>();
        List<Thread> schemaThread = new List<Thread>();
        public delegate CheckResult GetAnswer(string msg);
        public static GetAnswer CheckAnswer = null;
        public RuleModule()
        {
        }
        public Node GetCurrentNode()
        {            
            return rules[ActiveSchema].GetCurrentNode();
        }
        public Schema GetActiveSchema()
        {
            return rules[ActiveSchema];
        }
        public object Clone()
        {
            RuleModule x = (RuleModule)this.MemberwiseClone();
            x.rules = new Dictionary<string, Schema>();
            foreach (var item in rules)
            {
                x.rules.Add(item.Key, (Schema)item.Value.Clone());
            }

            
            x.alarms = new Dictionary<string, Schema>(alarms);

            SetList(rules[ActiveSchema]);
            schemaThread = new List<Thread>();
            return x;
        }
        public void SetList(Schema s)
        {
            inputData = s.inputData;           
            sugestions= s.sugestions;
            alarmMessages = s.alarmMessages;             
        }
        public void Reset()
        {
            ResetPath();
            ClearVariables();
            foreach (var item in rules)
                item.Value.Reset();
            AbortAllSchemas();
            StartActiveSchemaInThread();            
            Task.Delay(10);
        }
        public void ResetPath()
        {
                rules[ActiveSchema].ClearPath();
        }
        public List<string> GetPath(string s)
        {
            return rules[s].path;
        }
        public void SetVariables(Dictionary<string, double> varS)
        {
            rules[ActiveSchema].SetVariables(varS);
        }
        public Dictionary<string, double> GetVariables()
        {
            return rules[ActiveSchema].variables;
        }
        public void ClearVariables()
        {
            rules[ActiveSchema].ClearVariables();
        }
        public void ClearAll()
        {
            ClearRules();
            ClearVariables();
        }      
        public void RunAlarms()
        {
            List<string> al = new List<string>(alarms.Keys);
            for(int i=0;i<alarms.Count;i++)
            {
                Thread t = new Thread(alarms[al[i]].RunSchema);
                t.Start();
                alarmThread.Add(t);

            }
        }
        
       
        public void ClearRules()
        {
            rules.Clear();
        }       
        public void AddSchema(string schemaName,Stream st, SchemaType type = SchemaType.SUGESTION)
        {
            if (rules.ContainsKey(schemaName))
                rules[schemaName].ReadSchema(st);
            else
                rules.Add(schemaName, new Schema(st, type, schemaName));

            ActiveSchema = schemaName;

        }
        public void AddSchema(string schemaName,string fileName,SchemaType type=SchemaType.SUGESTION)
        {
            if (rules.ContainsKey(schemaName))
                rules[schemaName].ReadSchema(fileName);
            else
                rules.Add(schemaName, new Schema(fileName,type,schemaName));
            ActiveSchema = schemaName;

        }
        public void StartActiveSchemaInThread()
        {
            StartSchemaInThread(ActiveSchema);
        }
        public void StartSchemaInThread(string name)
        {
            ActiveSchema = name;
            Thread t = new Thread(rules[ActiveSchema].RunSchema)
            {
                Name = name
        };
            t.Start();
            schemaThread.Add(t);
        }
        public void AbortAllSchemas()
        {
            foreach (var item in schemaThread)
                item.Abort();
        }
        
        public void NewSchema(string schemaName,string fileName,SchemaType type=SchemaType.SUGESTION)
        {
            if (rules.ContainsKey(schemaName))
                rules.Remove(schemaName);

            rules.Add(schemaName,new Schema(fileName,type,schemaName));
        }
                                    
    }
    
}
