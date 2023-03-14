using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuleModule_Odyn;
using System.Drawing;
namespace GUI
{
    class GraphNodeFrame:GraphNode
    {
        public static Graphics g;
       // protected Color shapeColor;
        

        protected Font f = new Font(FontFamily.GenericSansSerif, (float)(12), FontStyle.Regular);

        public GraphNodeFrame(int posX, int posY, ColorOdyn c, Node n): base(posX,posY,c,n)
        {         
            CreateFont();
        }
        public override void CreateFont()
        {
            f = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Regular);
        }
        public override SizeM MeasureString(string s)
        {
            var sf= g.MeasureString(s, f);

            SizeM res = new SizeM();
            res.Height = sf.Height;
            res.Width = sf.Width;
            return res;
        }
        public override void DrawRectangle()
        {
            Pen p = new Pen(Color.DarkBlue, 3);
            g.DrawRectangle(p, new Rectangle((int)x.x - 5, (int)x.y - 5, (int)MaxX + 10, (int)MaxY + 10));
        }        
        public override void DrawString(string s, float x, float y, ColorOdyn c)
        {
            g.DrawString(s, f, new SolidBrush(Color.FromArgb(c.red,c.green,c.blue)), x-5, y-15);
        }
        public override void DrawLine(float x1, float x2, float x3, float x4, ColorOdyn c)
        {
            Pen p = new Pen(Color.FromArgb(c.red,c.green,c.blue));
            g.DrawLine(p, x1, x2,x3,x4);
        }
         
        public override void DrawCircle(float x, float y, float r, ColorOdyn c)
        {
            SolidBrush b = new SolidBrush(Color.FromArgb(c.red,c.green,c.blue));
            g.FillEllipse(b, x,y, r, r);

        }

        public override void DrawConnections(double scale)
        {
            foreach (var item in connectionOut)
            {
                item.DrawConnection(g,scale);
            }
        }
        public override void SetGraphics(object o)
        {
            g = (Graphics)o;
        }
        public override void AddConnection( GraphNode dest, PointC start,ColorOdyn c)
        {
            
            connectionOut.Add(new ConnectionFrame(this, dest, start, c,arrowLength));
        }
        public override void SetConnectionColor(GraphNode dest, ColorOdyn color)
        {
            foreach (var item in connectionOut)
            {
                if (item.destination.Equals(dest))
                {
                    item.color = color;
                    return;
                }

            }
        }
        }
}
