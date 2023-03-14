using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;

namespace RuleModule_Odyn
{
    public class ColorOdyn
    {
        public byte red;
        public byte green;
        public byte blue;

        public ColorOdyn(byte x,byte y,byte z)
        {
            red = x;
            green = y;
            blue = z;
        }
    }
    public abstract class Connection
    {
        protected GraphNode source;
        public GraphNode destination;
        public PointC sPoint;
        public ColorOdyn color;
        readonly double arrowLength;
        protected List<PointC> conPoints = new List<PointC>();
        protected List<PointC> arrowPoints = new List<PointC>();
        public Connection(GraphNode s, GraphNode n, PointC sPoint, ColorOdyn c, double arrowL)
        {
            source = s;
            arrowLength = arrowL;

            destination = n;
            this.sPoint = sPoint;
            color = c;
        }
        public void SetColor(ColorOdyn c)
        {
            color = c;
        }
        void MakeLongConnection(PointC startPoint,PointC endPoint)
        {
            PointC c;
            PointC lastPoint;
            if (startPoint.x == source.x.x)
                c = new PointC(startPoint.x - 10, startPoint.y);
            else
                if ((source.x.x + source.MaxX) == startPoint.x)
                c = new PointC(startPoint.x + 10, startPoint.y);
            else
                c = new PointC(startPoint.x, startPoint.y + 10);
            conPoints.Add(startPoint);
            conPoints.Add(c);
            if (startPoint.y > endPoint.y)
            {
                if (startPoint.x <= source.x.x)
                {
                    conPoints.Add(new PointC(source.x.x - 10, c.y));
                    conPoints.Add(new PointC(source.x.x - 10, endPoint.y + 10));
                }
                else
                {
                    conPoints.Add(new PointC(source.x.x + source.MaxX + 10, c.y));
                    conPoints.Add(new PointC(source.x.x + source.MaxX + 10, endPoint.y + 10));

                }
            }
            lastPoint = conPoints[conPoints.Count - 1];
            conPoints.Add(new PointC(lastPoint.x, endPoint.y - 10));
            conPoints.Add(new PointC(endPoint.x, endPoint.y - 10));

            conPoints.Add(endPoint);

        }
        public void PrepareConnectionPoints()
        {   
            conPoints.Clear();
            arrowPoints.Clear();            
            PointC startPoint = new PointC(source.x.x + sPoint.x, source.x.y + sPoint.y);
            PointC endPoint = new PointC(destination.connectionIn.x + destination.x.x, destination.connectionIn.y + destination.x.y);
            if (startPoint.y < endPoint.y)
            {
                PointC mid;

                if (startPoint.x == source.x.x)
                {
                    if (startPoint.x > endPoint.x)
                    {
                        conPoints.Add(startPoint);
                        mid = new PointC(endPoint.x, startPoint.y);
                        conPoints.Add(mid);
                        conPoints.Add(endPoint);
                    }
                    else
                        MakeLongConnection(startPoint, endPoint);

                }
                else
                    if (startPoint.x == source.x.x + source.MaxX)
                {
                    if (startPoint.x < endPoint.x)
                    {
                        conPoints.Add(startPoint);
                        mid = new PointC(endPoint.x, startPoint.y);
                        conPoints.Add(mid);
                        conPoints.Add(endPoint);
                    }
                    else
                        MakeLongConnection(startPoint,endPoint);
                }
                else
                    MakeLongConnection(startPoint,endPoint);

            }
            else
            {
                MakeLongConnection(startPoint,endPoint);
            }

            for (int i = 0; i < conPoints.Count - 1; i++)
            {
                if (conPoints[i].x == conPoints[i + 1].x)
                {
                    if (conPoints[i].y == conPoints[i + 1].y)
                        continue;
                    double y = (conPoints[i].y + conPoints[i + 1].y) / 2;

                    double step =arrowLength;   
                    if (conPoints[i].y > conPoints[i + 1].y)
                        step = -arrowLength;

                    arrowPoints.Add(new PointC(conPoints[i].x, y));
                    arrowPoints.Add(new PointC(conPoints[i].x+step, y-step));
                    arrowPoints.Add(new PointC(conPoints[i].x, y));
                    arrowPoints.Add(new PointC(conPoints[i].x-step, y-step));                    
                }
                else
                {
                    double x = (conPoints[i].x + conPoints[i + 1].x) / 2;
                    double step = arrowLength;
                    if (conPoints[i].x > conPoints[i + 1].x)
                        step = -arrowLength;

                    arrowPoints.Add(new PointC(x,conPoints[i].y));
                    arrowPoints.Add(new PointC(x-step, conPoints[i].y+step));
                    arrowPoints.Add(new PointC(x, conPoints[i].y));
                    arrowPoints.Add(new PointC(x - step, conPoints[i].y - step));
                }

            }


        }
        public abstract void DrawConnection(object graph,double scale);
       
    }
}
