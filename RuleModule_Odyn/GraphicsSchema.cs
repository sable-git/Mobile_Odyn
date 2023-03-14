using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuleModule_Odyn
{
    enum Direction
    {
        LEFT,
        RIGHT
    };
    enum Shape
    {
        Rectangle,
        Decision
    }
    class GNode
    {
        public bool active = true;
        public int x=0;
        public int y=0;
        public int w=0;
        public int h=0;
        public Rectangle r;
        public Point connectionIN;
        public Dictionary<OutType,Point> connectionOUT;

        Font f = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
        public Node node;
        /*public GNode(Shape s)
        {
            shape = s;
        }*/
        public void SetupNode(Graphics g)
        {            
            float maxV = int.MinValue;
            float hV = 0;
            for (int i = 0; i < node.expression.Count; i++)
            {
                var size=  g.MeasureString(node.expression[i], f);
                if (size.Width > maxV)
                    maxV = size.Width;
                hV = size.Height;
            }
            if(node.input!=null)
                for (int i = 0; i < node.input.Count; i++)
                {
                    var size = g.MeasureString(node.input[i], f);
                    if (size.Width > maxV)
                        maxV = size.Width;
                    hV = size.Height;
                }
            int count = node.expression.Count;
            if (node.input != null)
                count += node.input.Count;
           

            w = (int)(maxV + 10);
            h = (int)((hV + 5) * count);
            if(h==0 || w==0)
            {
                throw new Exception("Something wrong with node: " + node.nodeName);
            }
            if (node is ConditionalNode)
            {
                w += (int)(w * 0.1);
                h += (int)(h * 0.3);                   
            }
            SetupConnectionPoints();
            r = new Rectangle(x, y, w, h);
        }
        void SetupConnectionPoints()
        {

            connectionOUT = new Dictionary<OutType, Point>();

            connectionIN=new Point(x + w / 2, y);
            connectionOUT.Add(OutType.NEXT, new Point(x + w / 2, y + h));
            connectionOUT.Add(OutType.NEXT_LEFT, new Point(x, y + h / 2));
            connectionOUT.Add(OutType.NEXT_RIGHT, new Point(x+w, y + h / 2));
            connectionOUT.Add(OutType.TRUE, new Point(x + w, y + h / 2));
            connectionOUT.Add(OutType.FALSE, new Point(x, y + h / 2));            
        }
        public void UpdatePosition(int newX,int newY)
        {
            x = newX;
            y = newY;
            SetupConnectionPoints();
            r = new Rectangle(x, y, w, h);
        }
        public void DrawBox(Graphics g)
        {
            if (active == false)
                return;
          
            Pen p = new Pen(Color.Black);
           
            if (node is ComputationalNode)// && test)
            {
                switch(node)
                {
                    case AlarmNode alarmNode:
                        p = new Pen(Color.Yellow);
                         break;
                    case SugestionNode sugestionNode:
                        p = new Pen(Color.Blue);
                        break;
                    default:
                        p = new Pen(Color.Black);
                        break;
                }                
                g.DrawRectangle(p,r);                              
            }
            if(node is ConditionalNode)
            {
                g.DrawLine(p, x + w / 2, y, x, y + h / 2);
                g.DrawLine(p, x + w / 2, y, x+w, y + h / 2);
                g.DrawLine(p, x + w, y + h / 2,x+w/2,y+h);
                g.DrawLine(p, x, y+h/2, x + w/2, y + h);
            }
            g.DrawString(node.nodeName, f, new SolidBrush(Color.Black), x, y - 20);

            for (int i = 0; i < node.expression.Count; i++)
            {
                g.DrawString(node.expression[i], f, new SolidBrush(Color.Black), x + 5, y + i * 20 + 5);                
            }
            if (node.input != null)
                for (int i = 0; i < node.input.Count; i++)
                {
                    g.DrawString(node.input[i], f, new SolidBrush(Color.Green), x + 5, y + (i + node.expression.Count) * 20 + 5);
                }

        }

    }
   
    class GraphicsSchema
    {
        string startNode;
        Bitmap bitmap = new Bitmap(2000, 2000);
        int maxXLeft = int.MaxValue;
        int maxXRight = int.MinValue;
        Dictionary<string, Node> rules = null;
        Dictionary<string, GNode> gn = new Dictionary<string, GNode>();
        Font f = new Font(FontFamily.GenericSerif, 12, FontStyle.Regular);
        Graphics g;
        public GraphicsSchema(Dictionary<string, Node> rules,string startNode)
        {
            this.rules = rules;
            this.startNode = startNode;
        }
        public void AddSchema(Dictionary<string, Node> rules)
        {
            foreach (var item in rules)
                this.rules.Add(item.Key, item.Value);
        }
        void DrawArrow(Graphics g,Pen p,Point x1,Point x2,Direction d=Direction.RIGHT)
        {
            Point p1 = new Point();
            Point p2 = new Point();
            Point p3 = new Point();
            if (x2.Y != x1.Y)
            {
                p1.Y = (x2.Y + x1.Y) / 2;
                p1.X = x2.X;
                if (d == Direction.RIGHT)
                {
                    p2.X = p1.X - 5;
                    p2.Y = p1.Y - 10;
                    p3.X = p1.X + 5;
                    p3.Y = p1.Y - 10;
                }
                else
                {
                    p2.X = p1.X - 5;
                    p2.Y = p1.Y + 10;
                    p3.X = p1.X + 5;
                    p3.Y = p1.Y + 10;
                }
            }
            else
            {
                p1.X = (x2.X + x1.X) / 2;
                p1.Y = x2.Y;
                if (d == Direction.RIGHT)
                {
                    p2.X = p1.X - 10;
                    p2.Y = p1.Y + 5;
                    p3.X = p1.X - 10;
                    p3.Y = p1.Y - 5;
                }
                else
                {
                    p2.X = p1.X + 10;
                    p2.Y = p1.Y + 5;
                    p3.X = p1.X + 10;
                    p3.Y = p1.Y - 5;
                }

            }



            g.DrawLine(p, p1, p2);
            g.DrawLine(p, p1, p3);
        }
        List<Point> MakeListPoints(Point inPoint,Point outPoint,OutType type)
        {
            List<Point> points = new List<Point>();
            points.Add(outPoint);
            switch (type)
            {
                case OutType.NEXT:
                    if (outPoint.Y < inPoint.Y)
                    {
                        points.Add(new Point(outPoint.X, outPoint.Y +(inPoint.Y-outPoint.Y)/ 6));
                        points.Add(new Point(inPoint.X, points[points.Count() - 1].Y));
                    }
                    else
                    {
                        points.Add(new Point(outPoint.X, outPoint.Y + 10));
                        points.Add(new Point(outPoint.X + (inPoint.X-outPoint.X) / 6, points[points.Count() - 1].Y));
                        points.Add(new Point(points[points.Count() - 1].X, inPoint.Y - 10));
                        points.Add(new Point(inPoint.X, points[points.Count() - 1].Y));
                    }
                    break;
                case OutType.NEXT_RIGHT:
                    if (inPoint.X < outPoint.X)                    
                        points.Add(new Point(outPoint.X +20, outPoint.Y));                   
                    else                    
                        points.Add(new Point(outPoint.X + (inPoint.X - outPoint.X) / 6, outPoint.Y));
                    points.Add(new Point(points[points.Count() - 1].X, inPoint.Y - 10));
                    points.Add(new Point(inPoint.X, points[points.Count() - 1].Y));
                    
                    break;
                case OutType.NEXT_LEFT:
                    if (inPoint.X > outPoint.X)
                        points.Add(new Point(outPoint.X - 20, outPoint.Y));
                    else
                        points.Add(new Point(outPoint.X + (inPoint.X - outPoint.X) / 6, outPoint.Y));
                   
                    points.Add(new Point(points[points.Count() - 1].X, inPoint.Y-10));
                    points.Add(new Point(inPoint.X, points[points.Count() - 1].Y));
                    

                        break;
                case OutType.TRUE:
                    if (inPoint.X < outPoint.X)                    
                        points.Add(new Point(points[points.Count() - 1].X+10, points[points.Count() - 1].Y));                                           
                    else                    
                        points.Add(new Point(outPoint.X + (inPoint.X - outPoint.X) / 6, outPoint.Y));                                          
                    points.Add(new Point(points[points.Count() - 1].X, inPoint.Y - 10));
                    points.Add(new Point(inPoint.X, points[points.Count() - 1].Y));
                    break;
                case OutType.FALSE:
                    if (inPoint.X > outPoint.X)
                        points.Add(new Point(points[points.Count() - 1].X - 10, points[points.Count() - 1].Y));
                    else
                        points.Add(new Point(outPoint.X + (inPoint.X - outPoint.X) / 6, outPoint.Y));
                    points.Add(new Point(points[points.Count() - 1].X, inPoint.Y - 10));
                    points.Add(new Point(inPoint.X, points[points.Count() - 1].Y));
                    break;
            }
            points.Add(inPoint);


            return points;
        }
        void DrawLines(Graphics g,List<Point> points,Direction d)
        {
            Pen p = new Pen(Color.Red);
            for (int i = 0; i < points.Count - 1; i++)
            {
                g.DrawLine(p, points[i], points[i + 1]);
                if (i == 0)
                    DrawArrow(g, p, points[i], points[i + 1], d);
            }
        }
        KeyValuePair<Point,OutType> GetConnectionPoint(GNode actual, GNode next)
        {
            double distance = double.MaxValue;
            OutType index = 0;

            if (Math.Abs(actual.y - next.y) > 800)
                if (actual.x < 500)
                    return new KeyValuePair<Point, OutType>(actual.connectionOUT[OutType.NEXT_LEFT], OutType.NEXT_LEFT);
                else
                    return new KeyValuePair<Point, OutType>(actual.connectionOUT[OutType.NEXT_RIGHT], OutType.NEXT_RIGHT);

            foreach (var item in actual.connectionOUT.Keys)
            {
                if (item.ToString().Contains("NEXT"))
                {
                    double dist = Math.Pow(actual.connectionOUT[item].X - next.connectionIN.X, 2) +
                        Math.Pow(actual.connectionOUT[item].Y - next.connectionIN.Y, 2);
                    if (dist < distance)
                    {
                        distance = dist;
                        index = item;
                    }
                }
            }
            return new KeyValuePair<Point, OutType>(actual.connectionOUT[index],index);
        }
        void DrawConnections(Graphics g, Dictionary<string, GNode> gn)
        {
            foreach (var item in gn)
                item.Value.active = true;
            GNode actual = gn[startNode];
            actual.active = false;
            Queue<GNode> qList = new Queue<GNode>();
            qList.Enqueue(actual);
            Pen p = new Pen(Color.Red);

            while (qList.Count > 0)
            {
                actual = qList.Dequeue();

                Dictionary<OutType, string> listNext = actual.node.nextNode;
                foreach (var item in listNext)
                {
                    if (item.Key == OutType.END || !gn.ContainsKey(item.Value))
                        continue;

                    GNode gw = gn[item.Value];
                    if (gn[item.Value].active)
                        qList.Enqueue(gw);
                    gn[item.Value].active = false;
                    List<Point> points = new List<Point>();
                    Direction d = Direction.RIGHT;
                    switch (item.Key)
                    {
                        case OutType.NEXT:
                            KeyValuePair<Point,OutType> pi=GetConnectionPoint(actual, gn[item.Value]);
                            points = MakeListPoints(gn[item.Value].connectionIN, pi.Key, pi.Value);
                            if(pi.Value==OutType.NEXT_LEFT)
                                d = Direction.LEFT;
                            break;
                        default:
                            if (item.Key == OutType.FALSE)
                            {
                                d = Direction.LEFT;
                                g.DrawString("N", f, new SolidBrush(Color.Black), actual.x - 20, actual.y + actual.h / 2 - 20);
                            }
                            else
                                g.DrawString("Y", f, new SolidBrush(Color.Black), actual.x + actual.w + 5, actual.y + actual.h / 2 - 20);
                            points = MakeListPoints(gn[item.Value].connectionIN, actual.connectionOUT[item.Key], item.Key);
                            break;


                    }
                    DrawLines(g, points, d);
                }
            }
                
        }
        void DrawConnectionsNN(Graphics g, Dictionary<string, GNode> gn)
        {
            foreach (var item in gn)
                item.Value.active = true;
            GNode actual = gn[startNode];               
            actual.active = false;
            Queue<GNode> qList = new Queue<GNode>();
            qList.Enqueue(actual);
            Pen p = new Pen(Color.Red);

            while (qList.Count > 0)
            {
                actual = qList.Dequeue();
               
                Dictionary<OutType, string> listNext = actual.node.nextNode;
                foreach (var item in listNext)
                {              
                    if (item.Key == OutType.END || !gn.ContainsKey(item.Value))
                        continue;
                                            
                    GNode gw = gn[item.Value];
                    if (gn[item.Value].active)
                        qList.Enqueue(gw);
                    gn[item.Value].active = false;
                    List<Point> points = new List<Point>();
                    Direction d = Direction.RIGHT;    
                    if (item.Key == OutType.NEXT)
                    {
                        
                        if (actual.y < gw.y)
                        {
                            if(actual.y+actual.h<gw.y)
                            {
                                int halfY = actual.y + actual.h + (gw.y - (actual.y + actual.h)) / 2;
                                points.Add(new Point(actual.x + actual.w / 2, actual.y + actual.h));
                                points.Add(new Point(points[points.Count-1].X, halfY));
                                points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y));
                               
                            }
                            else
                            {
                                points.Add(new Point(actual.x + actual.w / 2, actual.y + actual.h));
                                points.Add(new Point(points[points.Count - 1].X, actual.y + actual.h + 10));
                                points.Add(new Point(actual.x - (actual.x - (gw.x + gw.w)) / 2, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y-10));
                                points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y));
                                
                                
                            }
                            
                            
                        }
                        else
                        {
                            if(actual.x>gw.x && actual.y-gw.y<300)
                            {
                                points.Add(new Point(actual.x, actual.y + actual.h / 2));
                                points.Add(new Point((actual.x+gw.x)/2, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y - 5));
                                points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                                d = Direction.LEFT;
                            }
                            else
                            if (Math.Abs(gw.x - maxXRight) > Math.Abs(gw.x - maxXLeft))
                            {
                                if (actual.y - gw.y > 600)
                                {
                                    points.Add(new Point(actual.x + actual.w, actual.y + actual.h / 2));
                                    points.Add(new Point(maxXRight + 10, points[points.Count - 1].Y));
                                    points.Add(new Point(points[points.Count - 1].X, gw.y - 5));
                                    points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                                    maxXRight += 10;
                                }
                                else
                                {
                                    points.Add(new Point(actual.x + actual.w, actual.y + actual.h / 2));
                                    points.Add(new Point((actual.x + gw.x) / 2 + actual.w, points[points.Count - 1].Y));
                                    points.Add(new Point(points[points.Count - 1].X, gw.y - 5));
                                    points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                                    //maxXRight += 10;
                                }
                            }
                            else
                                {
                                points.Add(new Point(actual.x + actual.w/2, actual.y + actual.h));
                                points.Add(new Point(points[points.Count - 1].X, points[points.Count - 1].Y+10));
                                points.Add(new Point(actual.x/2+gw.x/2, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y-10));
                                points.Add(new Point(gw.x+gw.w/2, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y));
                            }
                        }
                        
                    }                
                    if (item.Key == OutType.TRUE)
                    {
                        g.DrawString("Y", f, new SolidBrush(Color.Black), actual.x + actual.w + 5, actual.y + actual.h / 2 - 20);
                        if (actual.y < gw.y)
                        {
                            points.Add(actual.connectionOUT[OutType.TRUE]);
                            points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                            points.Add(gw.connectionIN);
                        }
                        else
                        {
                            points.Add(new Point(actual.x + actual.w, actual.y + actual.h / 2));
                            points.Add(new Point(points[points.Count - 1].X+20, points[points.Count - 1].Y));
                            points.Add(new Point(points[points.Count - 1].X, gw.y - 5));
                            points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                            points.Add(new Point(points[points.Count - 1].X,gw.y)); 
                        }
                       

                    }
                    if (item.Key == OutType.FALSE)
                    {
                        d = Direction.LEFT;
                        g.DrawString("N", f, new SolidBrush(Color.Black), actual.x - 20, actual.y + actual.h / 2 - 20);
                        if (actual.y < gw.y)
                        {                            
                            if (actual.x <= gw.x)
                            {
                                if (gw.node is ConditionalNode)
                                {
                                    /*points.Add(actual.connectionOUT[OutType.FALSE]);
                                    points.Add(new Point(actual.x - 20, points[points.Count - 1].Y));
                                    points.Add(new Point(points[points.Count - 1].X, actual.y + actual.h + 5));
                                    points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));*/
                                   
                                }
                                else
                                {
                                    points.Add(new Point(actual.x, actual.y + actual.h / 2));
                                    points.Add(new Point(actual.x - 20, points[points.Count - 1].Y));
                                    points.Add(new Point(points[points.Count - 1].X, gw.y + gw.h / 2));
                                    points.Add(new Point(gw.x, points[points.Count - 1].Y));
                                   
                                }
                            }
                            else                           
                            {
                                points.Add(actual.connectionOUT[OutType.FALSE]);
                                points.Add(new Point(gw.x + gw.w / 2, points[points.Count - 1].Y));
                                points.Add(gw.connectionIN);
                                
                               
                            }                            

                        }
                        else
                        {
                            if (gw.node is ComputationalNode)
                            {
                                points.Add(actual.connectionOUT[OutType.FALSE]);
                                points.Add(new Point(Math.Min(actual.x, gw.x) - 10, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y + gw.h / 2));
                                points.Add(gw.connectionIN);
                                
                                
                            }
                            else
                            {
                                points.Add(actual.connectionOUT[OutType.FALSE]);
                                int v = points[points.Count - 1].X - Math.Abs(actual.x - gw.x) / 2;
                                if (v < 0)
                                    v = 10;
                                points.Add(new Point(v, points[points.Count - 1].Y));
                                points.Add(new Point(points[points.Count - 1].X, gw.y -5));
                                points.Add(new Point(gw.connectionIN.X, points[points.Count - 1].Y));
                                points.Add(gw.connectionIN);
                                
                            }

                        }
                        
                    }
                    DrawLines(g, points,d);
                }
            }
        }
        public Size ActiveBox(string nodeName)
        {
            if (nodeName != null && gn.ContainsKey(nodeName))
            {
                GNode gNode = gn[nodeName];
                Pen p = new Pen(Color.DarkBlue,3);
                g.DrawRectangle(p, new Rectangle(gNode.x - 5, gNode.y - 5, gNode.w + 10, gNode.h + 10));
                return new Size(gNode.x, gNode.y);
            }
            return new Size(0,0);
        }
        void CheckOverlap()
        {
                        
            List<string> s = new List<string>(gn.Keys);
            for(int i=0;i<s.Count();i++)
            {
                for (int j = i + 1; j < s.Count; j++)
                    if (gn[s[i]].r.IntersectsWith(gn[s[j]].r))
                    {
                        int v = Math.Abs(gn[s[i]].x+ gn[s[i]].w - gn[s[j]].x);
                        if (gn[s[i]].x < gn[s[j]].x)
                        {
                            gn[s[i]].UpdatePosition(gn[s[i]].x - v / 2 - 5, gn[s[i]].y);
                            gn[s[j]].UpdatePosition(gn[s[j]].x + v / 2 + 5, gn[s[j]].y);
                        }
                        else
                        {
                            v = gn[s[j]].x + gn[s[j]].w - gn[s[i]].x;
                            gn[s[i]].UpdatePosition(gn[s[i]].x + v / 2 + 5, gn[s[i]].y);
                            gn[s[j]].UpdatePosition(gn[s[j]].x - v / 2 - 5, gn[s[j]].y);
                        }

                    }
                
            }
        }
        public void PrepareSchemaDraw()
        {
            foreach (var item in rules)
            {
                GNode gNode = new GNode();
                gNode.node = item.Value;
                gn.Add(item.Key, gNode);

            }
            g = Graphics.FromImage(bitmap);
            GNode actual = gn[startNode];
            actual.SetupNode(g);
            actual.active = false;
            Queue<GNode> qList = new Queue<GNode>();
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
                    GNode gw = gn[item.Value];
                    if (!gw.active)
                        continue;

                    gw.active = false;
                    gw.SetupNode(g);
                    qList.Enqueue(gw);
                    int newX=0, newY=0;
                    if (item.Key == OutType.NEXT)
                    {
                        newY = actual.y + actual.h + 80;
                        newX = actual.x;
                    }
                    if (item.Key == OutType.TRUE)
                    {
                        newY = actual.y + actual.h + 90;
                        newX = actual.x + actual.w + 60;

                    }
                    if (item.Key == OutType.FALSE)
                    {
                        newX = actual.x - gw.w - 60;//+Math.Abs(gw.w-actual.w)/2;
                        newY = actual.y + actual.h + 30;

                    }
                    gw.UpdatePosition(newX, newY);
                }
            }
            CheckOverlap();
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            foreach (var item in gn)
            {
                if (minY > item.Value.y)
                    minY = item.Value.y;
                if (maxY < item.Value.y)
                    maxY = item.Value.y;
                if (maxX < item.Value.x)
                    maxX = item.Value.x;
                if (minX > item.Value.x)
                    minX = item.Value.x;

            }
            foreach (var item in gn)
            {
                item.Value.x += Math.Abs(minX) + 100;
                if (maxXLeft > item.Value.x)
                    maxXLeft = item.Value.x;
                if (maxXRight < item.Value.x)
                    maxXRight = item.Value.x;
                item.Value.y += Math.Abs(minY) + 100;

                item.Value.UpdatePosition(item.Value.x, item.Value.y);
            }
            bitmap = new Bitmap(maxX - minX + 1000, maxY - minY + 200);
            g = Graphics.FromImage(bitmap);
        }
            public void DrawSchema(PictureBox pic)
        {

            g.Clear(pic.BackColor);
            foreach (var item in gn)
            {

                if (item.Value.active == false)
                    Console.WriteLine("Not active label: " + item.Value.node.nodeName);
                item.Value.active = true;
                item.Value.DrawBox(g);
            }
            pic.Size=new Size(bitmap.Width,bitmap.Height);
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            DrawConnections(g,gn);           
            pic.Image = bitmap;
        }
    }
}
