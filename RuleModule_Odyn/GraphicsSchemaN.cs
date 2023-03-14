using System;
using System.Collections.Generic;
using System.IO;

namespace RuleModule_Odyn
{
   
    public abstract class GraphicsSchemaN
    {
        public string startNode;
        double maxXLeft = int.MaxValue;
        double maxXRight = int.MinValue;
        double scale = 1.0;
        readonly Dictionary<string, Node> rules = null;
        public Dictionary<string, GraphNode> gn = new Dictionary<string, GraphNode>();        
        public GraphicsSchemaN(Dictionary<string, Node> rules, string startNode)
        {
            this.rules = rules;
            this.startNode = startNode;
        }
        public void NodeToScreen(string name,int width,int height)
        {
            GraphNode start = gn[name];

            double moveX = width/2 - start.x.x;
            double moveY = height/2-start.x.y;

            foreach (var item in gn)
                item.Value.TransShape(moveX,moveY);
        }
        public void AddSchema(Dictionary<string, Node> rules)
        {
            foreach (var item in rules)
                this.rules.Add(item.Key, item.Value);
        }
        public void Scale(double s, int x = -1,int y=-1)
        {
            scale = s;

            foreach(var item in gn)            
                item.Value.Scale(s);

            GraphNode actual = gn[startNode];
            PointC origin = actual.x;
            if (x > -1)
                origin = new PointC(x, y);

            foreach (var item in gn)
            {
                GraphNode gw = item.Value;
                double newPosX = origin.x - (origin.x - gw.x.x) * scale;
                double newPosY = origin.y - (origin.y - gw.x.y) * scale;
                gw.MoveShape(newPosX, newPosY);
            }

            foreach (var item in gn)
                item.Value.active = true;

           

            actual.active = false;
            Queue<GraphNode> qList = new Queue<GraphNode>();
            qList.Enqueue(actual);
            while (qList.Count > 0)
            {
                actual = qList.Dequeue();
                actual.ClearConnections();
                Dictionary<OutType, string> listNext = actual.node.nextNode;
                if (listNext!=null)
                foreach(var item in listNext)
                {
                    GraphNode gw = gn[item.Value];
                    if (gw.active)
                        qList.Enqueue(gw);

                    PointC connectionPos = null;
                    if (item.Key == OutType.NEXT)
                        connectionPos = new PointC(actual.MaxX / 2, actual.MaxY);
                    if (item.Key == OutType.TRUE)
                        connectionPos = new PointC(actual.MaxX, actual.MaxY / 2);
                    if (item.Key == OutType.FALSE)
                        connectionPos = new PointC(0, actual.MaxY / 2);
                    if (actual.x.y > gw.x.y && item.Key == OutType.NEXT)
                        if (actual.x.x< gw.x.x)
                            connectionPos = new PointC(actual.MaxX, actual.MaxY / 2);
                        else
                            connectionPos = new PointC(0, actual.MaxY / 2);
                    ColorOdyn c = new ColorOdyn(0, 255, 0);
                    actual.AddConnection(gw, connectionPos, c);                    

                    gw.active = false;
                }

            }

             //  MakeConnections();
//            DetermineNodePositions();

        }
        public void FindStartNode()
        {
            Dictionary<string, int> xx = new Dictionary<string, int>();
            foreach(var item in gn)
            {
                if (xx.ContainsKey(item.Key))
                    xx[item.Key]++;
                else
                    xx.Add(item.Key, 0);
            }

            foreach (var item in xx)
                if (item.Value == 0)
                {
                    startNode = item.Key;
                    return;
                }
        }
        public void ClearPath()
        {
            foreach (var item in gn)
                item.Value.OffPath();
        }
        public SizeM ActiveBox(string nodeName)
        {
            if (nodeName != null && gn.ContainsKey(nodeName))
            {
                GraphNode gNode = gn[nodeName];
                gNode.DrawRectangle();
                return new SizeM(gNode.x.x, gNode.x.y);
            }
            return new SizeM(0, 0);
        }
        public ColorOdyn NodeColor(Node n)
        {
            ColorOdyn r = new ColorOdyn(255, 0, 0);
            switch (n)
            {
                case AlarmNode alarmNode:
                    r.red = r.green = 255;
                    r.blue = 0;
                    break;
                case SugestionNode sugestionNode:
                    r.red = r.green = 0;
                    r.blue = 255;
                    break;
                case EndNode endNode:
                    r.red = 255;
                    r.green = r.blue = 0;
                    break;
                default:
                    r.red = r.green = r.blue;
                    break;
            }
            return r;
        }

        void MakeConnections()
        {
            GraphNode actual = gn[startNode];
            
            actual.active = false;
            Queue<GraphNode> qList = new Queue<GraphNode>();
            qList.Enqueue(actual);
            while (qList.Count > 0)
            {
                actual = qList.Dequeue();
                Dictionary<OutType, string> listNext = actual.node.nextNode;

                foreach (var item in listNext)
                {

                    if (item.Key == OutType.END)
                        continue;
                    if (!gn.ContainsKey(item.Value))
                    {
                        continue;
                        //throw new Exception("Node : " + item.Value + " not defined");
                    }
                    GraphNode gw = gn[item.Value];
                    if (gw.active)
                        qList.Enqueue(gw);



                    // gw.SetupNode();

                    double newX = 0, newY = 0;
                    PointC connectionPos = null;
                    if (item.Key == OutType.NEXT)
                    {
                        newY = actual.x.y + actual.MaxY + 80;
                        newX = actual.x.x;
                        connectionPos = new PointC(actual.MaxX / 2, actual.MaxY);

                    }
                    if (item.Key == OutType.TRUE)
                    {
                        newY = actual.x.y + actual.MaxY + 90;
                        newX = actual.x.x + actual.MaxX + 60;
                        connectionPos = new PointC(actual.MaxX, actual.MaxY / 2);
                    }
                    if (item.Key == OutType.FALSE)
                    {
                        newX = actual.x.x - gw.MaxX - 60;//+Math.Abs(gw.w-actual.w)/2;
                        newY = actual.x.y + actual.MaxY + 30;
                        connectionPos = new PointC(0, actual.MaxY / 2);
                    }
                    if (gw.active && !gw.node.freezPos)
                        gw.MoveShape(newX, newY);
                    if (actual.x.y > gw.x.y && item.Key == OutType.NEXT)
                        if (actual.x.x
                            < gw.x.x)
                            connectionPos = new PointC(actual.MaxX, actual.MaxY / 2);
                        else
                            connectionPos = new PointC(0, actual.MaxY / 2);
                    ColorOdyn c = new ColorOdyn(0, 255, 0);
                    actual.AddConnection(gw, connectionPos, c);
                    gw.active = false;

                }
            }
        }
        void DetermineNodePositions()
        {
            double minY = int.MaxValue;
            double maxY = int.MinValue;
            double minX = int.MaxValue;
            double maxX = int.MinValue;
            foreach (var item in gn)
            {
                if (minY > item.Value.x.y)
                    minY = item.Value.x.y;
                if (maxY < item.Value.x.y)
                    maxY = item.Value.x.y;
                if (maxX < item.Value.x.x)
                    maxX = item.Value.x.x;
                if (minX > item.Value.x.x)
                    minX = item.Value.x.x;

            }

            foreach (var item in gn)
            {
                if (!item.Value.node.freezPos)
                {
                    item.Value.x.x += Math.Abs(minX) + 100 * scale;
                    if (maxXLeft > item.Value.x.x)
                        maxXLeft = item.Value.x.x;
                    if (maxXRight < item.Value.x.x)
                        maxXRight = item.Value.x.x;
                    item.Value.x.y += Math.Abs(minY) + 100 * scale;

                    item.Value.MoveShape(item.Value.x.x, item.Value.x.y);
                }
                else
                    item.Value.node.freezPos = false;
            }

        }
        public SizeM GetResolution()
        {
            SizeM resMax = new SizeM(int.MinValue,int.MinValue);
           
            foreach (var item in gn)
            {
                if (resMax.Width < item.Value.x.x + item.Value.MaxX)
                    resMax.Width = item.Value.x.x + item.Value.MaxX;
                if (resMax.Height < item.Value.x.y + item.Value.MaxY)
                    resMax.Height = item.Value.x.y + item.Value.MaxY;
               

            }
            return new SizeM(resMax.Width +200, resMax.Height+200);
        }
        public void PrepareSchemaDraw()
        {
            SetGraphNodeGraphics();
            if(gn.Count==0)
            foreach (var item in rules)
            {
                GraphNode gNode = CreateNode(item.Value);
                gNode.node = item.Value;
                gn.Add(item.Key, gNode);

            }
            MakeConnections();
            FindStartNode();
            DetermineNodePositions();
            SetGraphNodeGraphics();
        }
        public GraphNode GetPointedNode(int x,int y)
        {
            foreach(var item in gn)
            {
                if (item.Value.CheckIfIn(x, y))
                    return item.Value;
            }
            return null;
        }
        public void ReadPositions(string fileName)
        {
            StreamReader r = new StreamReader(fileName);
            string[] tmp;
            string line = r.ReadLine();
            string name = "";
            while (line != null)
            {
                if(line.Contains("LABEL"))
                {
                    tmp = line.Split(' ');
                    name = tmp[1];
                }
                if (line.Contains("POSITION:"))
                {
                    if (line[0] != '%')
                    {
                        tmp = line.Split(':');
                        if (tmp.Length == 2)
                        {
                            if (gn.ContainsKey(name))
                            {
                                GraphNode n = gn[name];
                                tmp = tmp[1].Split(',');
                                string[] aux = tmp[0].Split('[');
                                n.x.x = Convert.ToInt32(aux[1]);
                                aux = tmp[1].Split(']');
                                n.x.y = Convert.ToInt32(aux[0]);
                            }
                        }
                    }
                }                
                line = r.ReadLine();
            }
            r.Close();
        }
        public void SavePositions(string fileName, string outFile)
        {
            StreamReader r = new StreamReader(fileName);
            string[] tmp;
            if (outFile == fileName)
            {
                string[] aux= fileName.Split('.');
                outFile = aux[0] + "_new" + "." + aux[1];
              }      
            StreamWriter w = new StreamWriter(outFile);
            string line = r.ReadLine();            
            string name = "";
            List<string> node = new List<string>();
            while(line!=null)
            {

                if(line.Contains("[NODE]"))
                {                   
                    node.Clear();
                }
                if(line.Contains("LABEL"))
                {
                    tmp = line.Split(' ');
                    name = tmp[1];
                }
                if(line.Contains("[END]"))
                {
                    bool pos = false;
                    foreach(var item in node)
                    {
                        if (item.Contains("POSITION:"))
                        {
                            GraphNode n = gn[name];
                            w.WriteLine("POSITION" + ": [" + n.x.x + "," + n.x.y + "]");
                            pos = true;
                        }
                        else
                            w.WriteLine(item);

                    }
                    if(!pos)
                    {
                        GraphNode n = gn[name];                        
                        w.WriteLine("POSITION" + ": [" + n.x.x + "," + n.x.y + "]");
                    }
                    w.WriteLine("[END]");
                }
                node.Add(line);                
                line = r.ReadLine();
            }


            r.Close();
            w.Close();
        }
        public abstract GraphNode CreateNode(Node n);
        public abstract void SetGraphNodeGraphics();
        public abstract void ClearGraphics();
        public abstract void DrawSchema(object o, int posX, int posY);


    }
}
