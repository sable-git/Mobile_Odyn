using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuleModule_Odyn;
using System.Drawing;
namespace GUI
{
    class ConnectionFrame:Connection
    {        
        public ConnectionFrame(GraphNode s, GraphNode n, PointC sPoint, ColorOdyn c,double l):base(s,n,sPoint,c,l)
        {
            color = c;
        }
        public override void DrawConnection(object graph,double scale)
        {
            Graphics g = (Graphics)graph;
            Pen p = new Pen(Color.FromArgb(color.red,color.green,color.blue));
            PrepareConnectionPoints();
            for (int i = 0; i < conPoints.Count - 1; i++)
                g.DrawLine(p, (float)conPoints[i].x, (float)conPoints[i].y, (float)conPoints[i + 1].x, (float)conPoints[i + 1].y);

            for (int i = 0; i < arrowPoints.Count; i += 2)
                g.DrawLine(p, (float)arrowPoints[i].x, (float)arrowPoints[i].y, (float)arrowPoints[i + 1].x, (float)arrowPoints[i + 1].y);

        }

    }
}
