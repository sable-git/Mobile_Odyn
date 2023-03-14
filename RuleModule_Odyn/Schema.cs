using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RuleModule_Odyn
{
    public enum SchemaType
    {
        ALARM,
        SUGESTION
    }
    public enum STATUS
    {
        RUN,
        RESET,
        ALARM
    }
    public class InputDataObject
    {
        public Node node;
        public EventWaitHandle hl=new AutoResetEvent(false);
    }
    [Serializable]
    public class Schema : ICloneable
    {
        public SchemaType TypeS { get; set; }
        public string Name { get; set; }
        readonly object obj = new object();
        public Dictionary<string, Node> schema = new Dictionary<string, Node>();
        public string startNode = "START";
        public string nextNode;
        protected char remark = '%';
        readonly static List<Node> nodeTypes;
        public List<string> path = new List<string>();
        [NonSerialized]
       // public EventWaitHandle hl = new AutoResetEvent(false);
        public STATUS schemaStatus = STATUS.RUN;

        [NonSerialized]
        public ConcurrentQueue<InputDataObject> sugestions = new ConcurrentQueue<InputDataObject>();
        [NonSerialized]
        public ConcurrentQueue<InputDataObject> inputData = new ConcurrentQueue<InputDataObject>();
        [NonSerialized]
        public ConcurrentQueue<InputDataObject> alarmMessages = new ConcurrentQueue<InputDataObject>();

        public Dictionary<string, double> variables = new Dictionary<string, double>();

        static Schema()
        {

            nodeTypes = new List<Node>();
            nodeTypes.Add(new AlarmNode());
            nodeTypes.Add(new SugestionNode());
            nodeTypes.Add(new ComputationalNode());
            nodeTypes.Add(new ConditionalNode());
            nodeTypes.Add(new WaitNode());
            nodeTypes.Add(new EndNode());



            //            nodeTypes = aux.ToList<Node>();
        }
        public void Reset()
        {
            //globalCurrentNode = schema[startNode];
        }
        public Node GetCurrentNode()
        {
            if (path.Count > 0)
                return schema[path[path.Count - 1]];
            else
                if(startNode.Length>0)
                    return schema[startNode];
            return null;
            //return globalCurrentNode;
        }
        public Schema()
        { }
        public Schema(string fileName, SchemaType _type, string name)
        {
            this.Name = name;
            ReadSchema(fileName);
            TypeS = _type;
        }
        public Schema(Stream st, SchemaType _type, string name)
        {
            this.Name = name;
            ReadSchema(st);
            TypeS = _type;
        }
        public object Clone()
        {
            Schema other = (Schema)this.MemberwiseClone();

            other.variables = new Dictionary<string, double>();
            foreach (var item in variables)
                other.variables.Add(item.Key, item.Value);

            other.schema = new Dictionary<string, Node>();
            foreach (var item in schema)
            {
                var n = (Node)item.Value.Clone();
                n.variables = other.variables;
                other.schema.Add(item.Key, n);
            }

            other.sugestions = new ConcurrentQueue<InputDataObject>();
            other.inputData = new ConcurrentQueue<InputDataObject>();
            other.alarmMessages = new ConcurrentQueue<InputDataObject>();
            other.path = new List<string>();
            //other.globalCurrentNode = null;
            return other;


        }
        static List<string> GetMessage(ConcurrentQueue<List<string>> queue)
        {
            List<string> ans = null;
            lock (queue)
            {
                if (queue.Count > 0)
                    queue.TryDequeue(out ans);

            }
            return ans;
        }



        public void Start()
        {
            nextNode = startNode;
            //globalCurrentNode = schema[startNode];
        }
        public Dictionary<string, double> GetVariables()
        {
            return variables;
        }

        public void ClearVariables()
        {
            lock (variables)
            {
                variables.Clear();
            }
        }
        public void SetVariables(Dictionary<string, double> varS)
        {
            lock (variables)
            {
                foreach (var item in varS)
                {
                    if (variables.ContainsKey(item.Key))
                        variables[item.Key] = item.Value;
                    else
                        variables.Add(item.Key, item.Value);
                }
            }
        }
        public void ClearPath()
        {
            lock (path)
            {
                path.Clear();
            }
        }
        public void RunSchema()
        {
            Start();
            do
            {
                if (TypeS != SchemaType.ALARM)
                    if (schemaStatus == STATUS.ALARM)
                    {
                        Start();
                        schemaStatus = STATUS.RUN;
                    }
                bool nodeAlarm = false;
                lock (path)
                {
                    path.Add(nextNode);
                }
                if (schema[nextNode] is AlarmNode)
                    nodeAlarm = true;
                Node node = RunNext();
                if (node.sugestions != null)
                {

                    if (TypeS != SchemaType.ALARM && !nodeAlarm)
                    {
                        InputDataObject data = new InputDataObject { };
                        data.node = node;
                        lock (obj)
                        {
                            sugestions.Enqueue(data);
                        }
                        data.hl.WaitOne();
                    }
                }
                if(nodeAlarm)
                {
                    lock (obj)
                    {
                        InputDataObject data = new InputDataObject { };
                        data.node = node;
                        lock (alarmMessages)
                        {
                            alarmMessages.Enqueue(data);
                        }
                        schemaStatus = STATUS.ALARM;
                        data.hl.WaitOne();
                    }

                }
                

            }
            while (nextNode != null);
            schemaStatus = STATUS.RUN;
        }
       
        public Node RunNext()
        {

            Node currentNode = null;
            if (nextNode != null && schema.ContainsKey(nextNode))
            { 
                currentNode = schema[nextNode];                
                if (currentNode.input != null  && currentNode.input.Count>0)
                {
                    InputDataObject inputObj = new InputDataObject { };
                    inputObj.node = currentNode;
                    inputObj.hl = new AutoResetEvent(false);
                    inputData.Enqueue(inputObj);
                    inputObj.hl.WaitOne();
                }
            }
            else
                throw new Exception("Node with label " + nextNode + " does not exists");
            
            if(currentNode!=null)
                nextNode = currentNode.Run();

            return currentNode;
        }
       
        string FindStartNode()
        {           
            Dictionary<string, int> check = new Dictionary<string, int>();
            foreach(var item in schema)
            {
                if (item.Value.nextNode != null)
                    foreach (var it in item.Value.nextNode)
                        if (it.Value != null)
                            if (schema.ContainsKey(it.Value))
                            {
                                if (!check.ContainsKey(schema[it.Value].NodeName))
                                    check.Add(schema[it.Value].NodeName, 1);
                            }
                            else
                                throw new Exception("Brakuje węzła: " + it.Value);
            }
            foreach (var item in schema)
            {
                if (!check.ContainsKey(item.Key))
                    return item.Key;
            }
            return "";
        }
        public void ReadSchema(StreamReader st)
        {
            using (st)
            {
                string line = st.ReadLine();
                while (line != null)
                {
                    line = line.Trim();
                    if (line.Length > 0 && line[0] != remark )
                    {
                        if (line.Contains("TYPE"))
                        {
                            string[] aux = line.Split(new char[] { ':' },2);                            
                            Node n = null;

                            foreach (var item in nodeTypes)
                            {
                                if (aux[1].Contains(item.ToString()))
                                {
                                    n = (Node)Activator.CreateInstance(item.GetType(),variables);
                                    break;
                                }
                            }
                            if (n == null)
                                throw new Exception("Unknown node type " + aux[1]);

                            if (n is EndNode)
                                Console.Write("");
                            line = n.ReadNode(st);
                            if (schema.ContainsKey(n.NodeName))
                                throw new Exception("Label " + n.NodeName + " already exists");

                            schema.Add(n.NodeName, n);
                        }
                    }                    
                    line = st.ReadLine();
                    
                }
                st.Close();
            }
            startNode = FindStartNode();
            if(startNode.Length==0)
                throw new Exception("START node is not defined correctly");
        }

        public void ReadSchema(Stream wS)
        {
            StreamReader st = new StreamReader(wS);
            ReadSchema(st);
        }

        public void ReadSchema(string fileName)
        {
            StreamReader st = new StreamReader(fileName);
            ReadSchema(st);

        }
    }
}
