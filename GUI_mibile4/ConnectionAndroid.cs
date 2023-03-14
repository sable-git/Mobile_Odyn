using System;
using System.Collections.Generic;
using System.Text;
using RuleModule_Odyn;
using System.Drawing;
using SkiaSharp;

namespace GUI_mobile4
{
    class ConnectionAndroid:Connection
    {
        public ConnectionAndroid(GraphNode s, GraphNode n, PointC sPoint,ColorOdyn c,double l) : base(s, n, sPoint,c,l)
        {

        }
        public override void DrawConnection(object graph,double scale)
        {
            SKCanvas canvas = (SKCanvas)graph;            
            PrepareConnectionPoints();

            var paint = new SKPaint { };

            paint.IsAntialias = true;

            paint.Color = new SKColor(color.red, color.green, color.blue);
            //paint.Color = SKColors.Green;
            for (int i = 0; i < conPoints.Count - 1; i++)
                canvas.DrawLine((float)conPoints[i].x,(float) conPoints[i].y, (float)conPoints[i + 1].x, (float)conPoints[i + 1].y,paint);

            for (int i = 0; i < arrowPoints.Count; i += 2)
                canvas.DrawLine((float)arrowPoints[i].x, (float)arrowPoints[i].y, (float)arrowPoints[i + 1].x, (float)arrowPoints[i + 1].y,paint);

        }

    }
}
