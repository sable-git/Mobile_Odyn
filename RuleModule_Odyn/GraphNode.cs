using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleModule_Odyn
{
    public struct SizeM
    {
        public SizeM(double x,double y)
        {
            Width = x;
            Height = y;
        }
        public double Width;
        public double Height;
    }
   
    public abstract class GraphNode
    {
        public bool active = true;
        public double scale = 1;
        public float fontSize = 12;
        public double arrowLength=5;        
        public PointC connectionIn;
        protected List<Connection> connectionOut = new List<Connection>();
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public PointC x { get { return node.x; } set { node.x = value; } }
        protected List<PointC> shape;
        protected bool inPath=false;
        readonly ColorOdyn cLine;
        public Node node;
        public GraphNode(int posX, int posY, ColorOdyn c,Node n)
        {
            cLine = c;
            MaxX = int.MinValue;
            MaxY = int.MinValue;
            node = n;
            if(x==null)
                x = new PointC(posX, posY);
            SetupNode();
            
        }
        public void InPath()
        {
            inPath = true;
        }
        public void OffPath()
        {
            inPath = false;
            ColorOdyn c = new ColorOdyn(0, 255, 0);
            foreach(var item in connectionOut)
            {
                item.SetColor(c);
            }
        }
        public void SetupNode()
        {
            
            double maxV = int.MinValue;
            double hV = 0;

            List<string> vv = node.GetLabels();
            if (vv != null)
                foreach (var item in vv)
                {
                    var size = MeasureString(item);//g.MeasureString(node.expression[i], f);                

                    if (size.Width > maxV)
                        maxV = size.Width;
                    hV = size.Height;
                }

            for (int i = 0; i < node.expression.Count; i++)
            {                
                var size = MeasureString(node.expression[i]);//g.MeasureString(node.expression[i], f);                
                
                if (size.Width > maxV)
                    maxV = size.Width;
                hV = size.Height;
            }
            if (node.input != null)
                for (int i = 0; i < node.input.Count; i++)
                {
                    var size = MeasureString(node.input[i]);//g.MeasureString(node.input[i], f);
                    if (size.Width > maxV)
                        maxV = size.Width;
                    hV = size.Height;
                }
            int count = node.expression.Count;
            if (node.input != null)
                count += node.input.Count;
            List<string> zz = node.GetLabels();
            if (zz != null)
                count += zz.Count;

            int w = (int)(maxV + 10);
            int h = (int)((hV + 5) * count);
            if (h == 0 || w == 0)
            {
                throw new Exception("Something wrong with node: " + node.NodeName);
            }
            shape = new List<PointC>();

            if (node is ConditionalNode)
            {
                w += (int)(w * 0.1);
                h += (int)(h * 0.3);
                shape.Add(new PointC(w/2, 0));
                shape.Add(new PointC(0, h/2));
                shape.Add(new PointC(w/2, h));
                shape.Add(new PointC(w, h/2));

            }
            else
            if (node is EndNode)
            {
                h += (int)(h * 0.3);
                double xx = w / 3;
                double yy = h / 3;

                shape.Add(new PointC((int)xx, 0));
                shape.Add(new PointC((int)(2 * xx), 0));
                shape.Add(new PointC(w, (int)yy));
                shape.Add(new PointC(w, (int)(2 * yy)));
                shape.Add(new PointC((int)(2 * xx), h));
                shape.Add(new PointC((int)(xx), h));
                shape.Add(new PointC(0, (int)(2 * yy)));
                shape.Add(new PointC(0, (int)(yy)));


            }
            else
            {
                shape.Add(new PointC(0, 0));
                shape.Add(new PointC(w, 0));
                shape.Add(new PointC(w, h));
                shape.Add(new PointC(0, h));
            }

            MaxX = w;
            MaxY = h;
            connectionIn = new PointC(MaxX / 2, 0);
        }
                        
        public bool CheckIfIn(int xPos, int yPos)
        {
            if (xPos >= x.x && xPos <= MaxX + x.x)
                if (yPos >= x.y && yPos <= MaxY + x.y)
                    return true;

            return false;
        }
        public abstract void CreateFont();
        public virtual void Scale(double s)
        {
            scale = s;
            fontSize *= (float)scale;
            arrowLength *= scale;
            CreateFont();
            SetupNode();
        }
        public void TransShape(double transVecX, double transVecY)
        {
            x.x += transVecX;
            x.y += transVecY;

        }
        public void MoveShape(double transVecX, double transVecY)
        {
            x.x = transVecX;
            x.y = transVecY;
        }
        public void ClearConnections()
        {
            connectionOut.Clear();
        }
       
        public void DrawShape()
        {
            ColorOdyn c;
            SizeM r = MeasureString("N");
            for (int i = 0; i < shape.Count - 1; i++)
                DrawLine((float)(shape[i].x + x.x), (float)(shape[i].y + x.y), (float)(shape[i + 1].x + x.x), (float)(shape[i + 1].y + x.y), cLine);

            DrawLine((float)(shape[shape.Count - 1].x + x.x), (float)(shape[shape.Count - 1].y + x.y), (float)(shape[0].x + x.x), (float)(shape[0].y + x.y), cLine);

            DrawConnections(scale);
            if (inPath)
                DrawRectangle();


            DrawString(node.NodeName, (float)x.x, (float)(x.y - 10), new ColorOdyn(0,0,0) );
            int s = 0;
            if (node.input != null)
            {
                s = node.input.Count;
                c = new ColorOdyn(0, 255, 0);
                for (int i = 0; i < node.input.Count; i++)
                    DrawString(node.input[i], (float)(x.x + 10), (float)(x.y + i * r.Height + r.Height),c);
            }
           
           
            c = new ColorOdyn(0, 0, 0);
            for (int i = 0; i < node.expression.Count; i++)
            {
                DrawString(node.expression[i], (float)(x.x + r.Height), (float)(x.y + (i+s) * r.Height + r.Height),c );
            }
            List<string> nn = node.GetLabels();
            if(nn!=null)
            for (int i = 0; i < nn.Count; i++)
            {
                DrawString(nn[i], (float)(x.x + r.Height), (float)(x.y + i * r.Height + r.Height), c);
            }

            if (node is ConditionalNode)
            {
                DrawString("Y", (float)(x.x + MaxX + 5), (float)(x.y + MaxY / 2 - 10), c);
                DrawString("N", (float)(x.x - r.Width), (float)(x.y + MaxY / 2 - 10), c);
            }
            DrawCircle((float)(x.x + connectionIn.x - 3), (float)(x.y + connectionIn.y - 3), 5, new ColorOdyn(255,0,0));
            foreach (var item in connectionOut)
            {
                DrawCircle((float)(x.x + item.sPoint.x - 3), (float)(x.y + item.sPoint.y - 3), 5, new ColorOdyn(255, 0, 0));
            }

        }
        public abstract void DrawConnections(double scale);
        public abstract void DrawLine(float x1,float x2,float x3,float x4,ColorOdyn c);

        public abstract void DrawCircle(float x,float y,float r,ColorOdyn c);

        public abstract void DrawString(string s, float x, float y,ColorOdyn c);
        public abstract void AddConnection(GraphNode dest, PointC start,ColorOdyn c);
        public abstract void SetConnectionColor(GraphNode dest,ColorOdyn color);
        public abstract void DrawRectangle();
        public abstract void SetGraphics(object o);
        public abstract SizeM MeasureString(string s);


    }
}
