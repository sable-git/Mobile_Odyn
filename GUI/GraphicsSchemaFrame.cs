using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RuleModule_Odyn;
using System.IO;

namespace GUI
{
   
    class GraphicsSchemaFrame:GraphicsSchemaN
    {
        public Bitmap bitmap = new Bitmap(3000, 3000);
        //        string startNode;
        PictureBox pic;
        Graphics g=null;
        public GraphicsSchemaFrame(Dictionary<string, Node> rules, string startNode):base(rules,startNode)
        {
        }
        public override void DrawSchema(object o,int x=0,int y=0)
        {
            pic = (PictureBox)o;
            ClearGraphics();
            foreach (var item in gn)
            {
                item.Value.active = true;
                //item.Value.TransShape(x, y);
                item.Value.DrawShape();
            }
//            pic.Size = new Size(bitmap.Width, bitmap.Height);
            //pic.SizeMode = PictureBoxSizeMode.AutoSize;
            //pic.Image = bitmap;
        }

        public override GraphNode CreateNode(Node n)
        {
            ColorOdyn r = NodeColor(n);
            return new GraphNodeFrame(0, 0, r, n);
        }
        public override void SetGraphNodeGraphics()
        {
            SizeM r = GetResolution();
            if(r.Width<0)
            {
                bitmap = new Bitmap(3000, 3000);
            }
            else
                bitmap = new Bitmap(6000,6000);
            g = Graphics.FromImage(bitmap);
            GraphNodeFrame.g = g;
        }
        public override void ClearGraphics()
        {
            g.Clear(pic.BackColor);
        }
        
    }
}
