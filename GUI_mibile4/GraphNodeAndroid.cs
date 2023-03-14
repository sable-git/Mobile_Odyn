using System;
using System.Collections.Generic;
using System.Text;
using RuleModule_Odyn;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
//using System.Drawing;

namespace GUI_mobile4
{
    class GraphNodeAndroid:GraphNode
    {
        public static SKCanvas canvas;
        readonly SKPaint lineStroke;
        SKPaint paint;
        public GraphNodeAndroid(int posX, int posY, ColorOdyn c, Node n) : base(posX, posY, c, n)
        {
            lineStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = new SKColor(c.red,c.green,c.blue),
                StrokeWidth = 1,
                //PathEffect = SKPathEffect.CreateDash(new float[] { 7, 7 }, 0)
            };
            CreateFont();
        }
        public override void CreateFont()
        {
            paint = new SKPaint()
            {
                TextSize = (float)(fontSize * 1.7),
                IsAntialias = true,
                Color = new SKColor(0x00, 0x00, 0x00),
                StrokeWidth = 1,
                TextAlign = SKTextAlign.Left
        };                        
        }
        public override SizeM MeasureString(string s)
        {
            var pp = new SKPaint()
            {
                TextSize = (float)(fontSize * 2),
                IsAntialias = true,
                Color = new SKColor(0x9C, 0xAF, 0xB7),
                StrokeWidth = 1,
                TextAlign = SKTextAlign.Left
        };                                                           

            float w =pp.MeasureText(s);           
            return new SizeM((int)w,(int) pp.TextSize);
            
        }
        public override void DrawRectangle()
        {
            var path = new SKPath();
            var paint = new SKPaint { };

            path.MoveTo(new SKPoint((float)(x.x+shape[0].x), (float)(x.y+shape[0].y)));
            for(int i=1;i<shape.Count;i++)
            {
                path.LineTo(new SKPoint((float)(x.x+shape[i].x), (float)(x.y+shape[i].y)));
            }
            
            paint.IsAntialias = true;
            paint.Color = new SKColor(0xFF, 0x00, 0x00);
            //paint.IsStroke = true;
            paint.StrokeWidth = 3;
//            canvas.DrawRect((float)x.x - 5, (float)x.y - 5, (float)MaxX + 10, (float)MaxY + 10,paint);
            canvas.DrawPath(path, paint);

        }
        public override void DrawString(string s, float x, float y, ColorOdyn c)
        {
            paint.Color = new SKColor(c.red, c.green, c.blue);
            canvas.DrawText(s,x, y, paint);
        }
        public override void DrawLine(float x1, float x2, float x3, float x4, ColorOdyn c)
        {
            lineStroke.Color = new SKColor(c.red, c.green, c.blue);
            canvas.DrawLine(x1,x2,x3,x4, lineStroke);
        }
        public override void DrawConnections(double scale)
        {
            foreach (var item in connectionOut)
            {
                item.DrawConnection(canvas,scale);
            }
        }
        public override void DrawCircle(float x, float y, float r, ColorOdyn c)
        {
            paint.Color = new SKColor(c.red, c.green, c.blue);
            canvas.DrawCircle(x,y, r, paint);
        }
        public override void SetGraphics(object o)
        {
            canvas = (SKCanvas)o;
        }
        public override void AddConnection( GraphNode dest, PointC start,ColorOdyn c)
        {
           
            connectionOut.Add(new ConnectionAndroid(this, dest, start,c,arrowLength));
        }
        public override void SetConnectionColor(GraphNode dest,ColorOdyn color)
        {
            foreach(var item in connectionOut)
            {
                if (item.destination.Equals(dest))
                {
                    item.SetColor(color);
                    return;
                }

            }
        }
    }
}
