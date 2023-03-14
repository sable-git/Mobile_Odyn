using System;
using System.Collections.Generic;
using System.Text;
using RuleModule_Odyn;
using SkiaSharp;

//using System.Drawing;

namespace GUI_mobile4
{
    public class GaphicsSchemaAndroid:GraphicsSchemaN
    {
        public SKCanvas canvas;
        static bool test = false;
        readonly SKBitmap bitmap = new SKBitmap(2000,2000);
        public GaphicsSchemaAndroid(Dictionary<string, Node> rules, string startNode) : base(rules, startNode)
        {
        }
        public override void DrawSchema(object o,int posX=0,int posY=0)
        {
            if (!test)
            {
                test = true;
                canvas = (SKCanvas)o;
                GraphNodeAndroid.canvas = canvas;
                double minX=int.MaxValue, maxX=0, minY=int.MaxValue, maxY=0;
                foreach (var item in gn)
                {
                    if (item.Value.x.x<minX)
                        minX = item.Value.x.x;
                    if (item.Value.x.x > maxX)
                        maxX = item.Value.x.x;
                    if (item.Value.x.y < minY)
                        minY = item.Value.x.y;
                    if (item.Value.x.y > maxY)
                        maxY = item.Value.x.y;
                }


                ClearGraphics();
                if(minX +posX< App.ScreenWidth - 100 && maxX+posX>100 && minY+posY < App.ScreenHeight - 100 && maxY+posY> 100)
                    foreach (var item in gn)
                        item.Value.TransShape(posX, posY);
                foreach (var item in gn)
                {
                    item.Value.active = true;                
                    item.Value.DrawShape();
                }
                test = false;
            }
        }

        public override GraphNode CreateNode(Node n)
        {
            ColorOdyn r = NodeColor(n);
            return new GraphNodeAndroid(0, 0, r, n);
        }
        public override void SetGraphNodeGraphics()
        {
            canvas = new SKCanvas(bitmap);
            GraphNodeAndroid.canvas = canvas;
        }
        public override void ClearGraphics()
        {
            canvas.Clear();
        }

    }
}
